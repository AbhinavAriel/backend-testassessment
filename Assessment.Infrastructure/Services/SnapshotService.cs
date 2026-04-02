using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Assessment.Application.DTOs.Snapshots;
using Assessment.Application.Interfaces;
using Assessment.Domain.Entities;
using Assessment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Assessment.Infrastructure.Services
{
    public class SnapshotService : ISnapshotService
    {
        private readonly ApplicationDbContext _db;
        private readonly HttpClient _http;

        private readonly string _projectUrl;
        private readonly string _serviceRoleKey;
        private readonly string _bucket;

        public SnapshotService(ApplicationDbContext db, HttpClient http, IConfiguration config)
        {
            _db = db;
            _http = http;
            _projectUrl = config["Supabase:ProjectUrl"] ?? throw new InvalidOperationException("Supabase:ProjectUrl is missing.");
            _serviceRoleKey = config["Supabase:ServiceRoleKey"] ?? throw new InvalidOperationException("Supabase:ServiceRoleKey is missing.");
            _bucket = config["Supabase:BucketName"] ?? "snapshots";
        }

        // ── Direct public URL — works because bucket is set to Public ─────────
        // Format: https://<project>.supabase.co/storage/v1/object/public/<bucket>/<path>
        // No token, no expiry, browser loads it directly.
        private string GetPublicUrl(string storagePath)
        {
            return $"{_projectUrl}/storage/v1/object/public/{_bucket}/{storagePath}";
        }

        // ── Upload ────────────────────────────────────────────────────────────

        public async Task<SnapshotResponseDto> UploadAsync(UploadSnapshotRequestDto dto)
        {
            if (dto == null)
                throw new ArgumentException("Invalid payload.");

            if (dto.TestId == Guid.Empty)
                throw new ArgumentException("TestId is required.");

            if (dto.ApplicantId == Guid.Empty)
                throw new ArgumentException("ApplicantId is required.");

            if (string.IsNullOrWhiteSpace(dto.ImageData))
                throw new ArgumentException("ImageData is required.");

            var testExists = await _db.HrTests.AsNoTracking()
                .AnyAsync(t => t.Id == dto.TestId);
            if (!testExists)
                throw new KeyNotFoundException("Test not found.");

            var applicantExists = await _db.HrApplicants.AsNoTracking()
                .AnyAsync(a => a.Id == dto.ApplicantId);
            if (!applicantExists)
                throw new KeyNotFoundException("Applicant not found.");

            // ── 1. Decode base64 data-URL to raw bytes ────────────────────────
            var base64 = dto.ImageData;
            if (base64.Contains(','))
                base64 = base64.Split(',')[1];

            var imageBytes = Convert.FromBase64String(base64);

            // ── 2. Build storage path ─────────────────────────────────────────
            var timestamp = dto.CapturedAt == default
                ? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                : new DateTimeOffset(dto.CapturedAt, TimeSpan.Zero).ToUnixTimeMilliseconds();

            var storagePath = $"{dto.TestId}/{dto.ApplicantId}/{timestamp}.jpg";

            // ── 3. PUT to Supabase Storage REST API ───────────────────────────
            var uploadUrl = $"{_projectUrl}/storage/v1/object/{_bucket}/{storagePath}";

            using var content = new ByteArrayContent(imageBytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            using var request = new HttpRequestMessage(HttpMethod.Put, uploadUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _serviceRoleKey);
            request.Headers.Add("x-upsert", "true");
            request.Content = content;

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Supabase upload failed ({response.StatusCode}): {body}");
            }

            // ── 4. Save path to DB ────────────────────────────────────────────
            var capturedAt = dto.CapturedAt == default ? DateTime.UtcNow : dto.CapturedAt;

            var snapshot = new TestSnapshot
            {
                Id = Guid.NewGuid(),
                TestId = dto.TestId,
                ApplicantId = dto.ApplicantId,
                ImagePath = storagePath,
                CapturedAt = capturedAt,
                ReceivedAtUtc = DateTime.UtcNow
            };

            await _db.TestSnapshots.AddAsync(snapshot);
            await _db.SaveChangesAsync();

            // ── 5. Return direct public URL — no token, no expiry ─────────────
            return new SnapshotResponseDto
            {
                Id = snapshot.Id,
                TestId = snapshot.TestId,
                ApplicantId = snapshot.ApplicantId,
                ImageUrl = GetPublicUrl(storagePath),
                CapturedAt = snapshot.CapturedAt,
            };
        }

        // ── Retrieve (HR admin) ───────────────────────────────────────────────

        public async Task<List<SnapshotResponseDto>> GetByTestIdAsync(Guid testId)
        {
            if (testId == Guid.Empty)
                throw new ArgumentException("Invalid testId.");

            var snapshots = await _db.TestSnapshots.AsNoTracking()
                .Where(s => s.TestId == testId)
                .OrderBy(s => s.CapturedAt)
                .ToListAsync();

            // Just construct the public URL from the stored path — no API call to Supabase needed
            return snapshots.Select(s => new SnapshotResponseDto
            {
                Id = s.Id,
                TestId = s.TestId,
                ApplicantId = s.ApplicantId,
                ImageUrl = GetPublicUrl(s.ImagePath),
                CapturedAt = s.CapturedAt,
            }).ToList();
        }

        // ── Proxy: stream image bytes (kept for backward compat) ──────────────
        public async Task<(byte[] Bytes, string ContentType)> GetImageAsync(Guid snapshotId)
        {
            var snapshot = await _db.TestSnapshots.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == snapshotId)
                ?? throw new KeyNotFoundException("Snapshot not found.");

            var fileUrl = $"{_projectUrl}/storage/v1/object/{_bucket}/{snapshot.ImagePath}";

            using var req = new HttpRequestMessage(HttpMethod.Get, fileUrl);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _serviceRoleKey);

            var res = await _http.SendAsync(req);
            if (!res.IsSuccessStatusCode)
                throw new InvalidOperationException($"Supabase fetch failed ({res.StatusCode})");

            var bytes = await res.Content.ReadAsByteArrayAsync();
            var ct = res.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
            return (bytes, ct);
        }
    }
}