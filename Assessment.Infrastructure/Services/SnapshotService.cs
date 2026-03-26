using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assessment.Application.DTOs.Snapshots;
using Assessment.Application.Interfaces;
using Assessment.Domain.Entities;
using Assessment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Infrastructure.Services
{
    public class SnapshotService : ISnapshotService
    {
        private readonly ApplicationDbContext _db;

        public SnapshotService(ApplicationDbContext db)
        {
            _db = db;
        }

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

            // Verify the test exists — do not require it to be non-submitted
            // because the final snapshot may arrive just after SubmitTestAsync runs.
            var testExists = await _db.HrTests.AsNoTracking()
                .AnyAsync(t => t.Id == dto.TestId);

            if (!testExists)
                throw new KeyNotFoundException("Test not found.");

            var applicantExists = await _db.HrApplicants.AsNoTracking()
                .AnyAsync(a => a.Id == dto.ApplicantId);

            if (!applicantExists)
                throw new KeyNotFoundException("Applicant not found.");

            var snapshot = new TestSnapshot
            {
                Id = Guid.NewGuid(),
                TestId = dto.TestId,
                ApplicantId = dto.ApplicantId,
                ImageData = dto.ImageData,
                CapturedAt = dto.CapturedAt == default ? DateTime.UtcNow : dto.CapturedAt,
                ReceivedAtUtc = DateTime.UtcNow
            };

            await _db.TestSnapshots.AddAsync(snapshot);
            await _db.SaveChangesAsync();

            return ToDto(snapshot);
        }

        public async Task<List<SnapshotResponseDto>> GetByTestIdAsync(Guid testId)
        {
            if (testId == Guid.Empty)
                throw new ArgumentException("Invalid testId.");

            var snapshots = await _db.TestSnapshots.AsNoTracking()
                .Where(s => s.TestId == testId)
                .OrderBy(s => s.CapturedAt)
                .ToListAsync();

            return snapshots.Select(ToDto).ToList();
        }

        // ── Mapping ──────────────────────────────────────────────────────────

        private static SnapshotResponseDto ToDto(TestSnapshot s) => new()
        {
            Id = s.Id,
            TestId = s.TestId,
            ApplicantId = s.ApplicantId,
            // Frontend expects the field to be called ImageUrl so it can drop
            // it directly into <img src>. We just return the stored data-URL.
            ImageUrl = s.ImageData,
            CapturedAt = s.CapturedAt
        };
    }
}