using Assessment.Application.Constants;
using Assessment.Application.DTOs.Hr;
using Assessment.Application.DTOs.Hr.Meta;
using Assessment.Application.DTOs.Hr.Test;
using Assessment.Application.DTOs.Hr.Requests;
using Assessment.Application.DTOs.Hr.Applicant;
using Assessment.Application.DTOs.Questions;
using Assessment.Application.Helpers;
using Assessment.Application.Interfaces;
using Assessment.Application.Interfaces.Repositories;
using Assessment.Domain.Entities;
using Assessment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Infrastructure.Services
{
    public class HrTestService : IHrTestService
    {
        private readonly IHrTestRepository _repo;
        private readonly IQuestionRepository _questionRepo;
        private readonly ApplicationDbContext _db;

        public HrTestService(
            IHrTestRepository repo,
            IQuestionRepository questionRepo,
            ApplicationDbContext db)
        {
            _repo = repo;
            _questionRepo = questionRepo;
            _db = db;
        }

        // ──────────────────────────── Meta ────────────────────────────

        public async Task<HrMetaDto> GetMetaAsync()
        {
            var all = await _repo.GetAllTechStacksAsync();
            return new HrMetaDto
            {
                TechStacks = all.Select(x => new HrTechStackDto { Id = x.Id, Name = x.Name }).ToList(),
                Levels = QuestionLevelLabels.All.ToList()
            };
        }

        // ──────────────────────────── List ────────────────────────────

        public async Task<PagedResultDto<HrTestRowDto>> GetListPagedAsync(int page, int pageSize)
        {
            if (page < PaginationDefaults.DefaultPage) page = PaginationDefaults.DefaultPage;
            if (pageSize <= 0) pageSize = PaginationDefaults.DefaultPageSize;
            if (pageSize > PaginationDefaults.MaxPageSize) pageSize = PaginationDefaults.MaxPageSize;

            var skip = (page - 1) * pageSize;
            var total = await _repo.CountTestsAsync();
            var rows = await _repo.GetTestsPagedAsync(skip, pageSize);

            var items = await MapTestsToRowDtosAsync(rows, skip);

            return new PagedResultDto<HrTestRowDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        // ──────────────────────────── Create ────────────────────────────

        public async Task<HrTestRowDto> CreateAsync(CreateHrTestRequestDto dto)
        {
            if (dto == null) throw new ArgumentException("Invalid payload.");
            if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("Email is required.");

            var techEntries = (dto.TechStacks ?? new())
                .Where(x => x.TechStackId != Guid.Empty)
                .GroupBy(x => x.TechStackId)
                .Select(g => g.First())
                .ToList();

            if (techEntries.Count == 0) throw new ArgumentException("Select at least one tech stack.");

            if (dto.TotalQuestions <= 0) throw new ArgumentException("TotalQuestions must be greater than 0.");
            if (dto.DurationMinutes <= 0) throw new ArgumentException("DurationMinutes must be greater than 0.");

            var techIds = techEntries.Select(x => x.TechStackId).ToList();

            var validCount = await _repo.CountTechStacksByIdsAsync(techIds);
            if (validCount != techIds.Count)
                throw new ArgumentException("One or more TechStackIds are invalid.");

            // Validate per-tech question availability
            foreach (var entry in techEntries)
            {
                var lvl = LevelParser.Parse(entry.Level);
                var perTechCount = dto.TotalQuestions / techEntries.Count
                                   + (techEntries.IndexOf(entry) < dto.TotalQuestions % techEntries.Count ? 1 : 0);
                var avail = await _db.Questions.AsNoTracking()
                    .CountAsync(q => q.IsActive && q.TechStackId == entry.TechStackId && q.Level == lvl);
                if (avail < Math.Max(1, perTechCount))
                    throw new ArgumentException(
                        $"Not enough questions for one of the selected tech stacks at the chosen level. Available: {avail}.");
            }

            var normalizedEmail = dto.Email.Trim().ToLowerInvariant();
            var applicant = await _repo.GetApplicantByEmailAsync(normalizedEmail);

            if (applicant == null)
            {
                var (first, last) = SplitName(dto.FullName);
                applicant = new HrApplicant
                {
                    Id = Guid.NewGuid(),
                    Email = normalizedEmail,
                    FirstName = first,
                    LastName = last,
                    PhoneNumber = (dto.PhoneNumber ?? "").Trim(),
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                };
                await _repo.AddApplicantAsync(applicant);
                await _repo.SaveChangesAsync();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(dto.FullName))
                {
                    var (first, last) = SplitName(dto.FullName);
                    applicant.FirstName = first;
                    applicant.LastName = last;
                }
                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    applicant.PhoneNumber = dto.PhoneNumber.Trim();

                applicant.UpdatedAtUtc = DateTime.UtcNow;
                await _repo.UpdateApplicantAsync(applicant);
                await _repo.SaveChangesAsync();
            }

            var test = new HrTest
            {
                Id = Guid.NewGuid(),
                ApplicantId = applicant.Id,
                TotalQuestions = dto.TotalQuestions,
                DurationMinutes = dto.DurationMinutes,
                Level = string.Join(", ", techEntries.Select(e => $"{e.TechStackId}:{e.Level}")), // kept for any legacy readers
                Status = TestStatus.Created,
                CreatedAtUtc = DateTime.UtcNow,
                AnsweredCount = 0,
                CorrectCount = 0,
                SubmittedAtUtc = null,
                TestToken = Guid.NewGuid().ToString("N"),
                ExpiresAtUtc = DateTime.UtcNow.AddDays(ScoringConstants.TokenExpiryDays)
            };

            await _repo.AddTestAsync(test);

            var links = techEntries.Select(entry => new HrTestTechStack
            {
                Id = Guid.NewGuid(),
                TestId = test.Id,
                TechStackId = entry.TechStackId,
                Level = LevelParser.Parse(entry.Level)
            }).ToList();

            await _repo.AddTechStacksAsync(links);
            await _repo.SaveChangesAsync();

            var picked = await PickQuestionsPerTechAsync(techEntries, dto.TotalQuestions);
            var testQuestions = picked.Select((q, idx) => new HrTestQuestion
            {
                TestId = test.Id,
                QuestionId = q.Id,
                Order = idx + 1
            }).ToList();

            await _db.HrTestQuestions.AddRangeAsync(testQuestions);
            await _repo.SaveChangesAsync();

            var techNames = await _repo.GetTechStackNamesByIdsAsync(techIds);

            return new HrTestRowDto
            {
                TestId = test.Id,
                ApplicantId = applicant.Id,
                ApplicantName = BuildApplicantName(applicant),
                Email = applicant.Email,
                PhoneNumber = applicant.PhoneNumber ?? "",
                TotalQuestions = test.TotalQuestions,
                DurationMinutes = test.DurationMinutes,
                Status = test.Status,
                CreatedAtUtc = test.CreatedAtUtc,
                SubmittedAtUtc = test.SubmittedAtUtc,
                AnsweredCount = 0,
                CorrectCount = 0,
                TechStacks = techNames,
                TechStackLevels = links.Select(l => new HrTechStackWithLevelDto
                {
                    Id = l.TechStackId,
                    Name = techNames.ElementAtOrDefault(links.IndexOf(l)) ?? "",
                    Level = l.Level.ToString()
                }).ToList(),
                TestToken = test.TestToken ?? "",
                ExpiresAtUtc = test.ExpiresAtUtc
            };
        }

        // ──────────────────────────── Get by ID ────────────────────────────

        public async Task<HrTestDetailDto> GetByIdAsync(Guid testId)
        {
            if (testId == Guid.Empty) throw new ArgumentException("Invalid testId.");

            var test = await _repo.GetTestByIdAsync(testId, asNoTracking: true);
            if (test == null) throw new KeyNotFoundException("Test not found.");

            if (test.ExpiresAtUtc <= DateTime.UtcNow)
                throw new InvalidOperationException("This test link has expired.");

            return new HrTestDetailDto
            {
                TestId = test.Id,
                ApplicantId = test.ApplicantId,
                Applicant = new HrApplicantDto
                {
                    Id = test.ApplicantId,
                    FullName = BuildApplicantName(test.Applicant),
                    Email = test.Applicant?.Email ?? "",
                    PhoneNumber = test.Applicant?.PhoneNumber ?? ""
                },
                TechStacks = test.TechStacks.Select(x => x.TechStack.Name).ToList(),
                Test = new HrTestDetailDto.HrTestInfoDto
                {
                    TotalQuestions = test.TotalQuestions,
                    DurationMinutes = test.DurationMinutes,
                    Status = test.Status,
                    AnsweredCount = test.AnsweredCount,
                    CorrectCount = test.CorrectCount,
                    CreatedAtUtc = test.CreatedAtUtc,
                    SubmittedAtUtc = test.SubmittedAtUtc
                }
            };
        }

        // ──────────────────────────── Get by Token (proper DTO) ────────────────────────────

        public async Task<HrTestTokenResponseDto> GetByTokenAsync(string token)
        {
            token = (token ?? "").Trim();
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Invalid test link.");

            var test = await _db.HrTests.AsNoTracking()
                .Include(t => t.Applicant)
                .Include(t => t.TechStacks)
                    .ThenInclude(ts => ts.TechStack)
                .FirstOrDefaultAsync(t => t.TestToken == token);

            if (test == null)
                throw new KeyNotFoundException("Invalid or expired test link.");

            if (test.ExpiresAtUtc <= DateTime.UtcNow)
                throw new InvalidOperationException("This test link has expired.");

            return new HrTestTokenResponseDto
            {
                TestId = test.Id,
                ApplicantId = test.ApplicantId,
                Applicant = new HrApplicantDto
                {
                    Id = test.ApplicantId,
                    FullName = BuildApplicantName(test.Applicant),
                    Email = test.Applicant?.Email ?? "",
                    PhoneNumber = test.Applicant?.PhoneNumber ?? ""
                },
                TechStacks = test.TechStacks
                    .Where(x => x.TechStack != null)
                    .Select(x => x.TechStack.Name)
                    .ToList(),
                Test = new HrTestTokenResponseDto.HrTestInfoDto
                {
                    TotalQuestions = test.TotalQuestions,
                    DurationMinutes = test.DurationMinutes,
                    Status = test.Status,
                    AnsweredCount = test.AnsweredCount,
                    CorrectCount = test.CorrectCount,
                    CreatedAtUtc = test.CreatedAtUtc,
                    SubmittedAtUtc = test.SubmittedAtUtc,
                    ExpiresAtUtc = test.ExpiresAtUtc
                }
            };
        }

        // ──────────────────────────── Applicant ────────────────────────────

        public async Task<HrApplicantDto> GetApplicantByIdAsync(Guid applicantId)
        {
            if (applicantId == Guid.Empty) throw new ArgumentException("Invalid applicantId.");

            var applicant = await _repo.GetApplicantByIdAsync(applicantId);
            if (applicant == null) throw new KeyNotFoundException("Applicant not found.");

            return new HrApplicantDto
            {
                Id = applicant.Id,
                FullName = BuildApplicantName(applicant),
                Email = applicant.Email,
                PhoneNumber = applicant.PhoneNumber ?? ""
            };
        }

        // ──────────────────────────── Update ────────────────────────────

        public async Task<HrTestRowDto> UpdateAsync(Guid testId, UpdateHrTestRequestDto dto)
        {
            if (testId == Guid.Empty) throw new ArgumentException("Invalid testId.");
            if (dto == null) throw new ArgumentException("Invalid payload.");

            var test = await _db.HrTests
                .Include(t => t.Applicant)
                .Include(t => t.TechStacks)
                .FirstOrDefaultAsync(t => t.Id == testId);
            if (test == null) throw new KeyNotFoundException("Test not found.");

            var applicant = await _db.HrApplicants.FirstOrDefaultAsync(a => a.Id == test.ApplicantId);
            if (applicant == null) throw new KeyNotFoundException("Applicant not found.");

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                var (first, last) = SplitName(dto.FullName);
                applicant.FirstName = first;
                applicant.LastName = last;
            }
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                applicant.PhoneNumber = dto.PhoneNumber.Trim();

            applicant.UpdatedAtUtc = DateTime.UtcNow;

            if (dto.TotalQuestions > 0) test.TotalQuestions = dto.TotalQuestions;
            if (dto.DurationMinutes > 0) test.DurationMinutes = dto.DurationMinutes;

            var techEntries = (dto.TechStacks ?? new())
                .Where(x => x.TechStackId != Guid.Empty)
                .GroupBy(x => x.TechStackId)
                .Select(g => g.First())
                .ToList();

            if (techEntries.Count > 0)
            {
                var techIds2 = techEntries.Select(x => x.TechStackId).ToList();
                var validCount = await _repo.CountTechStacksByIdsAsync(techIds2);
                if (validCount != techIds2.Count)
                    throw new ArgumentException("One or more TechStackIds are invalid.");

                await _repo.RemoveTechStacksByTestIdAsync(testId);
                await _repo.SaveChangesAsync();

                var newLinks = techEntries.Select(entry => new HrTestTechStack
                {
                    Id = Guid.NewGuid(),
                    TestId = testId,
                    TechStackId = entry.TechStackId,
                    Level = LevelParser.Parse(entry.Level)
                }).ToList();
                await _repo.AddTechStacksAsync(newLinks);
            }

            await _repo.SaveChangesAsync();

            // Reload the test so TechStacks nav is current
            var reloadedTest = await _db.HrTests.AsNoTracking()
                .Include(t => t.TechStacks).ThenInclude(ts => ts.TechStack)
                .FirstOrDefaultAsync(t => t.Id == testId);

            var finalTechLinks = reloadedTest?.TechStacks?.ToList() ?? new();
            var techNames = await _repo.GetTechStackNamesByIdsAsync(
                finalTechLinks.Select(x => x.TechStackId).ToList());

            return new HrTestRowDto
            {
                TestId = test.Id,
                ApplicantId = test.ApplicantId,
                ApplicantName = BuildApplicantName(applicant),
                Email = applicant.Email,
                PhoneNumber = applicant.PhoneNumber ?? "",
                TotalQuestions = test.TotalQuestions,
                DurationMinutes = test.DurationMinutes,
                Status = test.Status,
                CreatedAtUtc = test.CreatedAtUtc,
                SubmittedAtUtc = test.SubmittedAtUtc,
                AnsweredCount = test.AnsweredCount,
                CorrectCount = test.CorrectCount,
                ScorePercentage = test.ScorePercentage,
                IsPassed = test.IsPassed && !test.IsRejected,
                IsRejected = test.IsRejected,
                TechStacks = techNames,
                TechStackLevels = finalTechLinks.Zip(techNames, (link, name) => new HrTechStackWithLevelDto
                {
                    Id = link.TechStackId,
                    Name = name,
                    Level = link.Level.ToString()
                }).ToList(),
                TestToken = test.TestToken ?? "",
                ExpiresAtUtc = test.ExpiresAtUtc
            };
        }

        // ──────────────────────────── Questions ────────────────────────────

        public async Task<List<QuestionResponseDto>> GetQuestionsForTestAsync(Guid testId)
        {
            if (testId == Guid.Empty) throw new ArgumentException("Invalid testId.");

            var test = await _db.HrTests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == testId);
            if (test == null) throw new KeyNotFoundException("Test not found.");

            if (test.ExpiresAtUtc <= DateTime.UtcNow)
                throw new InvalidOperationException("This test link has expired.");

            var questions = await _db.HrTestQuestions.AsNoTracking()
                .Where(x => x.TestId == testId)
                .OrderBy(x => x.Order)
                .Include(x => x.Question)
                    .ThenInclude(q => q.Options)
                .Select(x => x.Question)
                .ToListAsync();

            if (questions.Count == 0)
                throw new InvalidOperationException(
                    "No questions were generated for this test. Please regenerate or recreate the test.");

            return questions.Select((q, idx) => new QuestionResponseDto
            {
                Id = q.Id,
                Order = idx + 1,
                Text = q.Text,
                Level = q.Level.ToString(),
                Options = q.Options.Select(o => new AnswerOptionResponseDto
                {
                    Id = o.Id,
                    Text = o.Text
                }).ToList()
            }).ToList();
        }

        public async Task<BeginTestResultDto> BeginTestAsync(Guid testId)
        {
            if (testId == Guid.Empty) throw new ArgumentException("Invalid testId.");

            var test = await _db.HrTests.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == testId);

            if (test == null)
                throw new KeyNotFoundException("Test not found.");

            if (test.ExpiresAtUtc <= DateTime.UtcNow)
                throw new InvalidOperationException("This test link has expired.");

            if (string.Equals(test.Status, TestStatus.Submitted, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("This test has already been submitted.");

            return new BeginTestResultDto
            {
                TestId = test.Id,
                ApplicantId = test.ApplicantId,
                DurationMinutes = test.DurationMinutes,
            };
        }

        // ──────────────────────────── Submit ────────────────────────────

        public async Task SubmitTestAsync(Guid testId)
        {
            if (testId == Guid.Empty) throw new ArgumentException("Invalid testId.");

            var test = await _db.HrTests.FirstOrDefaultAsync(x => x.Id == testId);
            if (test == null) throw new KeyNotFoundException("Test not found.");

            if (test.ExpiresAtUtc <= DateTime.UtcNow)
                throw new InvalidOperationException("This test link has expired.");

            if (string.Equals(test.Status, TestStatus.Submitted, StringComparison.OrdinalIgnoreCase))
                return;

            var answered = await _db.UserAnswers.CountAsync(x =>
                x.TestId == testId && x.SelectedOptionId != null);

            var correct = await _db.UserAnswers.CountAsync(x =>
                x.TestId == testId && x.IsCorrect == true);

            test.AnsweredCount = answered;
            test.CorrectCount = correct;
            test.ScorePercentage = test.TotalQuestions > 0
                ? (decimal)Math.Round((double)correct / test.TotalQuestions * 100, 2)
                : 0m;
            test.IsPassed = test.ScorePercentage >= ScoringConstants.PassThresholdPercent;
            test.Status = TestStatus.Submitted;
            test.SubmittedAtUtc = DateTime.UtcNow;

            await _repo.SaveChangesAsync();
        }

        // ──────────────────────────── Delete ────────────────────────────

        public async Task DeleteTestAsync(Guid testId)
        {
            if (testId == Guid.Empty) throw new ArgumentException("Invalid testId.");

            var test = await _db.HrTests.FirstOrDefaultAsync(x => x.Id == testId);
            if (test == null) throw new KeyNotFoundException("Test not found.");

            var applicantId = test.ApplicantId;

            var answers = await _db.UserAnswers.Where(x => x.TestId == testId).ToListAsync();
            if (answers.Count > 0) _db.UserAnswers.RemoveRange(answers);

            var tq = await _db.HrTestQuestions.Where(x => x.TestId == testId).ToListAsync();
            if (tq.Count > 0) _db.HrTestQuestions.RemoveRange(tq);

            var ts = await _db.HrTestTechStacks.Where(x => x.TestId == testId).ToListAsync();
            if (ts.Count > 0) _db.HrTestTechStacks.RemoveRange(ts);

            _db.HrTests.Remove(test);
            await _repo.SaveChangesAsync();

            var hasRemaining = await _db.HrTests.AsNoTracking().AnyAsync(x => x.ApplicantId == applicantId);
            if (!hasRemaining)
            {
                var applicant = await _db.HrApplicants.FirstOrDefaultAsync(a => a.Id == applicantId);
                if (applicant != null)
                {
                    _db.HrApplicants.Remove(applicant);
                    await _repo.SaveChangesAsync();
                }
            }
        }

        // ──────────────────────────── Report ────────────────────────────

        public async Task<HrTestReportDto> GetReportAsync(Guid testId)
        {
            if (testId == Guid.Empty) throw new ArgumentException("Invalid testId.");

            var test = await _db.HrTests.AsNoTracking()
                .Include(t => t.Applicant)
                .Include(t => t.TechStacks)
                    .ThenInclude(ts => ts.TechStack)
                .FirstOrDefaultAsync(t => t.Id == testId);
            if (test == null) throw new KeyNotFoundException("Test not found.");

            var testQuestions = await _db.HrTestQuestions.AsNoTracking()
                .Where(x => x.TestId == testId)
                .OrderBy(x => x.Order)
                .Include(x => x.Question)
                    .ThenInclude(q => q.Options)
                .ToListAsync();

            var answers = await _db.UserAnswers.AsNoTracking()
                .Where(a => a.TestId == testId && a.ApplicantId == test.ApplicantId)
                .ToListAsync();

            var ansMap = answers
                .GroupBy(a => a.QuestionId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.AnsweredAt).First());

            var answeredCount = answers.Count(x => x.SelectedOptionId != null);
            var correctCount = answers.Count(x => x.IsCorrect == true);
            var scorePercentage = test.TotalQuestions > 0
                ? (decimal)Math.Round((double)correctCount / test.TotalQuestions * 100, 2)
                : 0m;

            var report = new HrTestReportDto
            {
                TestId = test.Id,
                ApplicantId = test.ApplicantId,
                ApplicantName = BuildApplicantName(test.Applicant),
                Email = test.Applicant?.Email ?? "",
                PhoneNumber = test.Applicant?.PhoneNumber ?? "",
                Level = test.Level ?? "",
                Status = test.Status ?? "",
                TotalQuestions = test.TotalQuestions,
                DurationMinutes = test.DurationMinutes,
                AnsweredCount = answeredCount,
                CorrectCount = correctCount,
                ScorePercentage = scorePercentage,
                IsPassed = scorePercentage >= ScoringConstants.PassThresholdPercent && !test.IsRejected,
                IsRejected = test.IsRejected,
                CancellationReason = test.CancellationReason,
                CreatedAtUtc = test.CreatedAtUtc,
                SubmittedAtUtc = test.SubmittedAtUtc,
                TechStacks = test.TechStacks
                    .Where(x => x.TechStack != null)
                    .Select(x => x.TechStack.Name)
                    .ToList()
            };

            foreach (var tq in testQuestions)
            {
                var q = tq.Question;
                if (q == null) continue;

                ansMap.TryGetValue(q.Id, out var a);
                var correctOpt = q.Options?.FirstOrDefault(o => o.IsCorrect);
                var selectedOpt = a?.SelectedOptionId.HasValue == true
                    ? q.Options?.FirstOrDefault(o => o.Id == a.SelectedOptionId!.Value)
                    : null;

                report.Questions.Add(new HrReportQuestionDto
                {
                    QuestionId = q.Id,
                    Order = tq.Order,
                    Text = q.Text ?? "",
                    SelectedOptionId = a?.SelectedOptionId,
                    SelectedOptionText = selectedOpt?.Text ?? "",
                    CorrectOptionId = correctOpt?.Id,
                    CorrectOptionText = correctOpt?.Text ?? "",
                    IsCorrect = a?.IsCorrect == true,
                    Options = (q.Options ?? new List<AnswerOption>())
                        .Select(o => new HrReportOptionDto { Id = o.Id, Text = o.Text ?? "" })
                        .ToList()
                });
            }

            return report;
        }

        // ──────────────────────────── Reject ────────────────────────────

        public async Task RejectTestAsync(Guid testId, string cancellationReason)
        {
            if (testId == Guid.Empty) throw new ArgumentException("Invalid testId.");

            var test = await _db.HrTests.FirstOrDefaultAsync(x => x.Id == testId);
            if (test == null) throw new KeyNotFoundException("Test not found.");

            if (!string.Equals(test.Status, TestStatus.Submitted, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only submitted tests can be rejected.");

            test.IsRejected = true;
            test.CancellationReason = cancellationReason?.Trim();
            await _repo.SaveChangesAsync();
        }

        // ──────────────────────────── Private helpers ────────────────────────────

        private async Task<List<Question>> PickQuestionsPerTechAsync(
            List<TechStackLevelDto> techEntries, int totalQuestions)
        {
            if (techEntries == null || techEntries.Count == 0)
                throw new ArgumentException("No tech stacks selected.");

            var perTech = totalQuestions / techEntries.Count;
            var remainder = totalQuestions % techEntries.Count;

            var picked = new List<Question>();
            var pickedIds = new HashSet<Guid>();

            for (int i = 0; i < techEntries.Count; i++)
            {
                var entry = techEntries[i];
                var level = LevelParser.Parse(entry.Level);
                var take = perTech + (i < remainder ? 1 : 0);
                if (take <= 0) continue;

                var chunk = await _db.Questions.AsNoTracking()
                    .Include(q => q.Options)
                    .Where(q => q.IsActive && q.Level == level && q.TechStackId == entry.TechStackId)
                    .OrderBy(q => Guid.NewGuid())
                    .Take(take)
                    .ToListAsync();

                foreach (var q in chunk)
                    if (pickedIds.Add(q.Id)) picked.Add(q);
            }

            // Fill any shortfall from any of the selected techs at their own level
            var remaining = totalQuestions - picked.Count;
            if (remaining > 0)
            {
                foreach (var entry in techEntries)
                {
                    if (remaining <= 0) break;
                    var level = LevelParser.Parse(entry.Level);
                    var fill = await _db.Questions.AsNoTracking()
                        .Include(q => q.Options)
                        .Where(q => q.IsActive && q.Level == level &&
                                    q.TechStackId == entry.TechStackId &&
                                    !pickedIds.Contains(q.Id))
                        .OrderBy(q => Guid.NewGuid())
                        .Take(remaining)
                        .ToListAsync();

                    foreach (var q in fill)
                    {
                        if (pickedIds.Add(q.Id)) { picked.Add(q); remaining--; }
                    }
                }
            }

            if (picked.Count < totalQuestions)
                throw new ArgumentException(
                    $"Not enough questions available. Needed {totalQuestions}, got {picked.Count}.");

            return picked.Take(totalQuestions).ToList();
        }

        private async Task<List<HrTestRowDto>> MapTestsToRowDtosAsync(List<HrTest> rows, int skip)
        {
            var testIds = rows.Select(r => r.Id).Distinct().ToList();

            var stats = await _db.UserAnswers.AsNoTracking()
                .Where(a => testIds.Contains(a.TestId))
                .GroupBy(a => a.TestId)
                .Select(g => new
                {
                    TestId = g.Key,
                    Answered = g.Count(x => x.SelectedOptionId != null),
                    Correct = g.Count(x => x.IsCorrect == true)
                })
                .ToListAsync();

            var statsMap = stats.ToDictionary(x => x.TestId, x => (x.Answered, x.Correct));

            return rows.Select((t, index) =>
            {
                statsMap.TryGetValue(t.Id, out var s);
                var isSubmitted = string.Equals(t.Status, TestStatus.Submitted, StringComparison.OrdinalIgnoreCase);

                var answeredFinal = isSubmitted ? t.AnsweredCount : s.Answered;
                var correctFinal = isSubmitted ? t.CorrectCount : s.Correct;
                var score = t.TotalQuestions > 0
                    ? (decimal)Math.Round((double)correctFinal / t.TotalQuestions * 100, 2)
                    : 0m;

                return new HrTestRowDto
                {
                    SerialNo = skip + index + 1,
                    TestId = t.Id,
                    ApplicantId = t.ApplicantId,
                    ApplicantName = BuildApplicantName(t.Applicant),
                    Email = t.Applicant?.Email ?? "",
                    PhoneNumber = t.Applicant?.PhoneNumber ?? "",
                    TotalQuestions = t.TotalQuestions,
                    DurationMinutes = t.DurationMinutes,
                    Status = t.Status,
                    CreatedAtUtc = t.CreatedAtUtc,
                    SubmittedAtUtc = t.SubmittedAtUtc,
                    AnsweredCount = answeredFinal,
                    CorrectCount = correctFinal,
                    ScorePercentage = score,
                    IsPassed = !t.IsRejected && t.TotalQuestions > 0 &&
                                        score >= ScoringConstants.PassThresholdPercent,
                    IsRejected = t.IsRejected,
                    CancellationReason = t.CancellationReason,
                    TechStacks = t.TechStacks.Select(x => x.TechStack.Name).ToList(),
                    TechStackLevels = t.TechStacks.Select(x => new HrTechStackWithLevelDto
                    {
                        Id = x.TechStackId,
                        Name = x.TechStack?.Name ?? "",
                        Level = x.Level.ToString()
                    }).ToList(),
                    TestToken = t.TestToken ?? "",
                    ExpiresAtUtc = t.ExpiresAtUtc
                };
            }).ToList();
        }

        private static string BuildApplicantName(HrApplicant? a)
        {
            if (a == null) return "Unknown";
            var full = $"{a.FirstName} {a.LastName}".Trim();
            return string.IsNullOrWhiteSpace(full) ? (a.Email ?? "Unknown") : full;
        }

        private static (string FirstName, string LastName) SplitName(string? fullName)
        {
            var name = (fullName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name)) return ("", "");
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 1
                ? (parts[0], "")
                : (parts[0], string.Join(" ", parts.Skip(1)));
        }
    }
}