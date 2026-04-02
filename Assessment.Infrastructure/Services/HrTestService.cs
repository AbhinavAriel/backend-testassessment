using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assessment.Application.DTOs.Hr;
using Assessment.Application.DTOs.Questions;
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

        public HrTestService(IHrTestRepository repo, IQuestionRepository questionRepo, ApplicationDbContext db)
        {
            _repo = repo;
            _questionRepo = questionRepo;
            _db = db;
        }

        public async Task<HrMetaDto> GetMetaAsync()
        {
            var all = await _repo.GetAllTechStacksAsync();
            return new HrMetaDto
            {
                TechStacks = all.Select(x => new HrTechStackDto { Id = x.Id, Name = x.Name }).ToList(),
                Levels = new List<string> { "Beginner", "Intermediate", "Professional" }
            };
        }

        public async Task<PagedResultDto<HrTestRowDto>> GetListPagedAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var skip = (page - 1) * pageSize;

            var total = await _repo.CountTestsAsync();
            var rows = await _repo.GetTestsPagedAsync(skip, pageSize);

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

            var items = rows.Select((t, index) =>
            {
                var applicantName = BuildApplicantName(t.Applicant);
                statsMap.TryGetValue(t.Id, out var s);

                var isSubmitted = string.Equals(t.Status, "Submitted", StringComparison.OrdinalIgnoreCase);

                var answeredCountFinal = isSubmitted ? t.AnsweredCount : s.Answered;
                var correctCountFinal = isSubmitted ? t.CorrectCount : s.Correct;

                return new HrTestRowDto
                {
                    SerialNo = skip + index + 1,
                    TestId = t.Id,
                    ApplicantId = t.ApplicantId,
                    ApplicantName = applicantName,
                    Email = t.Applicant?.Email ?? "",
                    PhoneNumber = t.Applicant?.PhoneNumber ?? "",
                    TotalQuestions = t.TotalQuestions,
                    DurationMinutes = t.DurationMinutes,
                    Level = t.Level,
                    Status = t.Status,
                    CreatedAtUtc = t.CreatedAtUtc,
                    SubmittedAtUtc = t.SubmittedAtUtc,

                    AnsweredCount = answeredCountFinal,
                    CorrectCount = correctCountFinal,
                    ScorePercentage = t.TotalQuestions > 0
                        ? (decimal)Math.Round((double)correctCountFinal / t.TotalQuestions * 100, 2)
                        : 0,
                    IsPassed = !t.IsRejected && t.TotalQuestions > 0 && (decimal)Math.Round((double)correctCountFinal / t.TotalQuestions * 100, 2) >= 75,
                    IsRejected = t.IsRejected,

                    TechStacks = t.TechStacks.Select(x => x.TechStack.Name).ToList(),

                    TestToken = t.TestToken ?? "",
                    ExpiresAtUtc = t.ExpiresAtUtc
                };
            }).ToList();

            return new PagedResultDto<HrTestRowDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<List<HrTestRowDto>> GetListAsync(int take)
        {
            if (take <= 0) take = 100;

            var rows = await _repo.GetLatestTestsAsync(take);
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

            return rows.Select(t =>
            {
                var applicantName = BuildApplicantName(t.Applicant);
                statsMap.TryGetValue(t.Id, out var s);

                var isSubmitted = string.Equals(t.Status, "Submitted", StringComparison.OrdinalIgnoreCase);

                var answeredCountFinal = isSubmitted ? t.AnsweredCount : s.Answered;
                var correctCountFinal = isSubmitted ? t.CorrectCount : s.Correct;

                return new HrTestRowDto
                {
                    TestId = t.Id,
                    ApplicantId = t.ApplicantId,
                    ApplicantName = applicantName,
                    Email = t.Applicant?.Email ?? "",
                    PhoneNumber = t.Applicant?.PhoneNumber ?? "",
                    TotalQuestions = t.TotalQuestions,
                    DurationMinutes = t.DurationMinutes,
                    Level = t.Level,
                    Status = t.Status,
                    CreatedAtUtc = t.CreatedAtUtc,
                    SubmittedAtUtc = t.SubmittedAtUtc,

                    AnsweredCount = answeredCountFinal,
                    CorrectCount = correctCountFinal,
                    ScorePercentage = t.TotalQuestions > 0
                        ? (decimal)Math.Round((double)correctCountFinal / t.TotalQuestions * 100, 2)
                        : 0,
                    IsPassed = !t.IsRejected && t.TotalQuestions > 0 && (decimal)Math.Round((double)correctCountFinal / t.TotalQuestions * 100, 2) >= 75,
                    IsRejected = t.IsRejected,

                    TechStacks = t.TechStacks.Select(x => x.TechStack.Name).ToList(),

                    TestToken = t.TestToken ?? "",
                    ExpiresAtUtc = t.ExpiresAtUtc
                };
            }).ToList();
        }

        public async Task<HrTestRowDto> CreateAsync(CreateHrTestRequestDto dto)
        {
            if (dto == null) throw new ArgumentException("Invalid payload.");
            if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("Email is required.");

            var techIds = dto.TechStackIds?.Where(x => x != Guid.Empty).Distinct().ToList() ?? new List<Guid>();
            if (techIds.Count == 0) throw new ArgumentException("Select at least one tech stack.");

            if (dto.TotalQuestions <= 0) throw new ArgumentException("TotalQuestions must be greater than 0.");
            if (dto.DurationMinutes <= 0) throw new ArgumentException("DurationMinutes must be greater than 0.");

            var levelEnum = ParseLevel(dto.Level);

            var validCount = await _repo.CountTechStacksByIdsAsync(techIds);
            if (validCount != techIds.Count) throw new ArgumentException("One or more TechStackIds are invalid.");

            var available = await _questionRepo.CountAvailableAsync(techIds, levelEnum);
            if (available < dto.TotalQuestions)
                throw new ArgumentException($"Not enough questions for selected TechStacks at {levelEnum}. Available: {available}.");

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
                Level = levelEnum.ToString(),
                Status = "Created",
                CreatedAtUtc = DateTime.UtcNow,
                AnsweredCount = 0,
                CorrectCount = 0,
                SubmittedAtUtc = null,

                // ✅ IMPORTANT
                TestToken = Guid.NewGuid().ToString("N"),
                ExpiresAtUtc = DateTime.UtcNow.AddDays(1)
            };

            await _repo.AddTestAsync(test);

            var links = techIds.Select(tid => new HrTestTechStack
            {
                Id = Guid.NewGuid(),
                TestId = test.Id,
                TechStackId = tid
            }).ToList();

            await _repo.AddTechStacksAsync(links);
            await _repo.SaveChangesAsync();

            var picked = await PickQuestionsBalancedAsync(techIds, levelEnum, dto.TotalQuestions);

            var testQuestions = picked.Select((q, idx) => new HrTestQuestion
            {
                TestId = test.Id,
                QuestionId = q.Id,
                Order = idx + 1
            }).ToList();

            _db.HrTestQuestions.AddRange(testQuestions);
            await _db.SaveChangesAsync();

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
                Level = test.Level,
                Status = test.Status,
                CreatedAtUtc = test.CreatedAtUtc,
                SubmittedAtUtc = test.SubmittedAtUtc,
                AnsweredCount = 0,
                CorrectCount = 0,
                TechStacks = techNames,

                TestToken = test.TestToken ?? "",
                ExpiresAtUtc = test.ExpiresAtUtc
            };
        }

        private async Task<List<Question>> PickQuestionsBalancedAsync(List<Guid> techIds, QuestionLevel level, int totalQuestions)
        {
            if (techIds == null || techIds.Count == 0)
                throw new ArgumentException("No tech stacks selected.");

            var perTech = totalQuestions / techIds.Count;
            var remainder = totalQuestions % techIds.Count;

            var picked = new List<Question>();
            var pickedIds = new HashSet<Guid>();

            foreach (var techId in techIds)
            {
                var take = perTech + (remainder > 0 ? 1 : 0);
                if (remainder > 0) remainder--;

                if (take <= 0) continue;

                var chunk = await _db.Questions.AsNoTracking()
                    .Include(q => q.Options)
                    .Where(q => q.IsActive && q.Level == level && q.TechStackId == techId)
                    .OrderBy(q => Guid.NewGuid())
                    .Take(take)
                    .ToListAsync();

                foreach (var q in chunk)
                {
                    if (pickedIds.Add(q.Id))
                        picked.Add(q);
                }
            }

            var remaining = totalQuestions - picked.Count;
            if (remaining > 0)
            {
                var fill = await _db.Questions.AsNoTracking()
                    .Include(q => q.Options)
                    .Where(q =>
                        q.IsActive &&
                        q.Level == level &&
                        techIds.Contains(q.TechStackId) &&
                        !pickedIds.Contains(q.Id))
                    .OrderBy(q => Guid.NewGuid())
                    .Take(remaining)
                    .ToListAsync();

                foreach (var q in fill)
                {
                    if (pickedIds.Add(q.Id))
                        picked.Add(q);
                }
            }

            if (picked.Count < totalQuestions)
                throw new ArgumentException($"Not enough questions available. Needed {totalQuestions}, got {picked.Count}.");

            return picked.Take(totalQuestions).ToList();
        }

        public async Task<HrTestDetailDto> GetByIdAsync(Guid testId)
        {
            if (testId == Guid.Empty) throw new ArgumentException("Invalid testId.");

            var test = await _repo.GetTestByIdAsync(testId, asNoTracking: true);
            if (test == null) throw new KeyNotFoundException("Test not found.");

            if (test.ExpiresAtUtc <= DateTime.UtcNow)
                throw new InvalidOperationException("This test link has expired.");

            var applicantName = BuildApplicantName(test.Applicant);

            return new HrTestDetailDto
            {
                TestId = test.Id,
                ApplicantId = test.ApplicantId,
                Applicant = new HrApplicantDto
                {
                    Id = test.ApplicantId,
                    FullName = applicantName,
                    Email = test.Applicant?.Email ?? "",
                    PhoneNumber = test.Applicant?.PhoneNumber ?? ""
                },
                TechStacks = test.TechStacks.Select(x => x.TechStack.Name).ToList(),
                Test = new HrTestDetailDto.HrTestInfoDto
                {
                    TotalQuestions = test.TotalQuestions,
                    DurationMinutes = test.DurationMinutes,
                    Level = test.Level,
                    Status = test.Status,
                    AnsweredCount = test.AnsweredCount,
                    CorrectCount = test.CorrectCount,
                    CreatedAtUtc = test.CreatedAtUtc,
                    SubmittedAtUtc = test.SubmittedAtUtc
                }
            };
        }

        public async Task<object> GetByTokenAsync(string token)
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

            var applicantName = BuildApplicantName(test.Applicant);

            return new
            {
                testId = test.Id,
                applicantId = test.ApplicantId,
                applicant = new
                {
                    id = test.ApplicantId,
                    fullName = applicantName,
                    email = test.Applicant?.Email ?? "",
                    phoneNumber = test.Applicant?.PhoneNumber ?? ""
                },
                techStacks = test.TechStacks
                    .Where(x => x.TechStack != null)
                    .Select(x => x.TechStack.Name)
                    .ToList(),
                test = new
                {
                    totalQuestions = test.TotalQuestions,
                    durationMinutes = test.DurationMinutes,
                    level = test.Level,
                    status = test.Status,
                    answeredCount = test.AnsweredCount,
                    correctCount = test.CorrectCount,
                    createdAtUtc = test.CreatedAtUtc,
                    submittedAtUtc = test.SubmittedAtUtc,
                    expiresAtUtc = test.ExpiresAtUtc
                }
            };
        }

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

            // Update applicant details
            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                var (first, last) = SplitName(dto.FullName);
                applicant.FirstName = first;
                applicant.LastName = last;
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                applicant.PhoneNumber = dto.PhoneNumber.Trim();

            applicant.UpdatedAtUtc = DateTime.UtcNow;

            // Update test fields
            if (dto.TotalQuestions > 0) test.TotalQuestions = dto.TotalQuestions;
            if (dto.DurationMinutes > 0) test.DurationMinutes = dto.DurationMinutes;
            if (!string.IsNullOrWhiteSpace(dto.Level)) test.Level = dto.Level;

            // Update tech stacks
            var techIds = dto.TechStackIds?.Where(x => x != Guid.Empty).Distinct().ToList() ?? new List<Guid>();

            if (techIds.Count > 0)
            {
                var validCount = await _repo.CountTechStacksByIdsAsync(techIds);
                if (validCount != techIds.Count)
                    throw new ArgumentException("One or more TechStackIds are invalid.");

                await _repo.RemoveTechStacksByTestIdAsync(testId);
                await _db.SaveChangesAsync();

                var newLinks = techIds.Select(tid => new HrTestTechStack
                {
                    Id = Guid.NewGuid(),
                    TestId = testId,
                    TechStackId = tid
                }).ToList();

                await _repo.AddTechStacksAsync(newLinks);
            }

            await _db.SaveChangesAsync();

            var finalTechIds = techIds.Count > 0
                ? techIds
                : test.TechStacks.Select(x => x.TechStackId).ToList();

            var techNames = await _repo.GetTechStackNamesByIdsAsync(finalTechIds);

            return new HrTestRowDto
            {
                TestId = test.Id,
                ApplicantId = test.ApplicantId,
                ApplicantName = BuildApplicantName(applicant),
                Email = applicant.Email,
                PhoneNumber = applicant.PhoneNumber ?? "",
                TotalQuestions = test.TotalQuestions,
                DurationMinutes = test.DurationMinutes,
                Level = test.Level,
                Status = test.Status,
                CreatedAtUtc = test.CreatedAtUtc,
                SubmittedAtUtc = test.SubmittedAtUtc,
                AnsweredCount = test.AnsweredCount,
                CorrectCount = test.CorrectCount,
                ScorePercentage = test.ScorePercentage,
                IsPassed = test.IsPassed && !test.IsRejected,
                IsRejected = test.IsRejected,
                TechStacks = techNames,
                TestToken = test.TestToken ?? "",
                ExpiresAtUtc = test.ExpiresAtUtc
            };
        }

        public async Task<List<QuestionResponseDto>> GetQuestionsForTestAsync(Guid testId)
        {
            if (testId == Guid.Empty) throw new ArgumentException("Invalid testId.");

            // ✅ Enforce expiry here too (otherwise someone can bypass token by calling this with a Guid)
            var test = await _db.HrTests.AsNoTracking()
    .FirstOrDefaultAsync(x => x.Id == testId);

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
                throw new InvalidOperationException("No questions were generated for this test. Please regenerate or recreate the test.");

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

        public async Task SubmitTestAsync(Guid testId)
        {
            if (testId == Guid.Empty) throw new ArgumentException("Invalid testId.");

            var test = await _db.HrTests.FirstOrDefaultAsync(x => x.Id == testId);
            if (test == null) throw new KeyNotFoundException("Test not found.");

            // ✅ Do not allow submit after link expiry
            if (test.ExpiresAtUtc <= DateTime.UtcNow)
                throw new InvalidOperationException("This test link has expired.");

            if (string.Equals(test.Status, "Submitted", StringComparison.OrdinalIgnoreCase))
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
            test.IsPassed = test.ScorePercentage >= 75m;
            test.Status = "Submitted";
            test.SubmittedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

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
            await _db.SaveChangesAsync();

            var hasRemaining = await _db.HrTests.AsNoTracking().AnyAsync(x => x.ApplicantId == applicantId);
            if (!hasRemaining)
            {
                var applicant = await _db.HrApplicants.FirstOrDefaultAsync(a => a.Id == applicantId);
                if (applicant != null)
                {
                    _db.HrApplicants.Remove(applicant);
                    await _db.SaveChangesAsync();
                }
            }
        }

        public async Task<HrTestReportDto> GetReportAsync(Guid testId)
        {
            if (testId == Guid.Empty) throw new ArgumentException("Invalid testId.");

            var test = await _db.HrTests.AsNoTracking()
                .Include(t => t.Applicant)
                .Include(t => t.TechStacks)
                    .ThenInclude(ts => ts.TechStack)
                .FirstOrDefaultAsync(t => t.Id == testId);

            if (test == null) throw new KeyNotFoundException("Test not found.");

            var applicantName = BuildApplicantName(test.Applicant);

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
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.AnsweredAt).FirstOrDefault());

            var answeredCount = answers.Count(x => x.SelectedOptionId != null);
            var correctCount = answers.Count(x => x.IsCorrect == true);

            var scorePercentage = test.TotalQuestions > 0
                ? (decimal)Math.Round((double)correctCount / test.TotalQuestions * 100, 2)
                : 0;

            var report = new HrTestReportDto
            {
                TestId = test.Id,
                ApplicantId = test.ApplicantId,

                ApplicantName = applicantName,
                Email = test.Applicant?.Email ?? "",
                PhoneNumber = test.Applicant?.PhoneNumber ?? "",

                Level = test.Level ?? "",
                Status = test.Status ?? "",

                TotalQuestions = test.TotalQuestions,
                DurationMinutes = test.DurationMinutes,
                AnsweredCount = answeredCount,
                CorrectCount = correctCount,
                ScorePercentage = scorePercentage,
                IsPassed = scorePercentage >= 75 && !test.IsRejected,
                IsRejected = test.IsRejected,

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

                var selectedOpt = a?.SelectedOptionId != null
                    ? q.Options?.FirstOrDefault(o => o.Id == a.SelectedOptionId)
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
                        .Select(o => new HrReportOptionDto
                        {
                            Id = o.Id,
                            Text = o.Text ?? ""
                        })
                        .ToList()
                });
            }

            return report;
        }

        public async Task RejectTestAsync(Guid testId)
        {
            if (testId == Guid.Empty) throw new ArgumentException("Invalid testId.");

            var test = await _db.HrTests.FirstOrDefaultAsync(x => x.Id == testId);
            if (test == null) throw new KeyNotFoundException("Test not found.");

            if (!string.Equals(test.Status, "Submitted", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only submitted tests can be rejected.");

            if (!test.IsPassed)
                throw new InvalidOperationException("Only passed tests can be rejected by admin.");

            test.IsRejected = true;
            await _db.SaveChangesAsync();
        }

        private static QuestionLevel ParseLevel(string? level)
        {
            return (level ?? "").Trim().ToLowerInvariant() switch
            {
                "beginner" => QuestionLevel.Beginner,
                "intermediate" => QuestionLevel.Intermediate,
                "professional" => QuestionLevel.Professional,
                _ => throw new ArgumentException("Invalid level. Use Beginner / Intermediate / Professional.")
            };
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
            if (parts.Length == 1) return (parts[0], "");

            var first = parts[0];
            var last = string.Join(" ", parts.Skip(1));
            return (first, last);
        }
    }
}