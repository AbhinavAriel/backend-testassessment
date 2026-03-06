using Assessment.Application.Interfaces.Repositories;
using Assessment.Domain.Entities;
using Assessment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Infrastructure.Repositories
{
    public class HrTestRepository : IHrTestRepository
    {
        private readonly ApplicationDbContext _db;

        public HrTestRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<int> CountTestsAsync()
        {
            return await _db.HrTests.AsNoTracking().CountAsync();
        }

        public async Task<List<HrTest>> GetTestsPagedAsync(int skip, int take)
        {
            if (skip < 0) skip = 0;
            if (take <= 0) take = 10;

            return await _db.HrTests.AsNoTracking()
                .Include(t => t.Applicant)
                .Include(t => t.TechStacks)
                    .ThenInclude(x => x.TechStack)
                .OrderByDescending(t => t.CreatedAtUtc)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<HrTest>> GetLatestTestsAsync(int take)
        {
            if (take <= 0) take = 10;

            return await _db.HrTests.AsNoTracking()
                .Include(x => x.Applicant)
                .Include(x => x.TechStacks)
                    .ThenInclude(ts => ts.TechStack)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(take)
                .ToListAsync();
        }

        public async Task<HrTest?> GetTestByIdAsync(Guid testId, bool asNoTracking = true)
        {
            var q = _db.HrTests
                .Include(x => x.Applicant)
                .Include(x => x.TechStacks)
                    .ThenInclude(ts => ts.TechStack)
                .Where(x => x.Id == testId);

            if (asNoTracking) q = q.AsNoTracking();

            return await q.FirstOrDefaultAsync();
        }

        public async Task<HrTest?> GetTestByIdForUpdateAsync(Guid testId)
        {
            return await _db.HrTests
                .Include(x => x.Applicant)
                .Include(x => x.TechStacks)
                    .ThenInclude(ts => ts.TechStack)
                .FirstOrDefaultAsync(x => x.Id == testId);
        }

        public async Task<HrApplicant?> GetApplicantByEmailAsync(string normalizedEmail)
        {
            var email = (normalizedEmail ?? "").Trim().ToLower();
            return await _db.HrApplicants.FirstOrDefaultAsync(x => x.Email.ToLower() == email);
        }

        public async Task<HrApplicant?> GetApplicantByIdAsync(Guid applicantId)
        {
            return await _db.HrApplicants.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == applicantId);
        }

        public async Task AddApplicantAsync(HrApplicant applicant)
        {
            await _db.HrApplicants.AddAsync(applicant);
        }

        public Task UpdateApplicantAsync(HrApplicant applicant)
        {
            _db.HrApplicants.Update(applicant);
            return Task.CompletedTask;
        }

        public async Task AddTestAsync(HrTest test)
        {
            await _db.HrTests.AddAsync(test);
        }

        public Task DeleteTestAsync(HrTest test)
        {
            _db.HrTests.Remove(test);
            return Task.CompletedTask;
        }

        public Task DeleteApplicantAsync(HrApplicant applicant)
        {
            _db.HrApplicants.Remove(applicant);
            return Task.CompletedTask;
        }

        public async Task AddTechStacksAsync(IEnumerable<HrTestTechStack> techStacks)
        {
            await _db.HrTestTechStacks.AddRangeAsync(techStacks);
        }

        public async Task RemoveTechStacksByTestIdAsync(Guid testId)
        {
            var rows = await _db.HrTestTechStacks
                .Where(x => x.TestId == testId)
                .ToListAsync();

            if (rows.Count > 0)
                _db.HrTestTechStacks.RemoveRange(rows);
        }

        public async Task<List<TechStack>> GetAllTechStacksAsync()
        {
            return await _db.TechStacks.AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<int> CountTechStacksByIdsAsync(IEnumerable<Guid> ids)
        {
            var list = ids.Distinct().ToList();
            return await _db.TechStacks.CountAsync(x => list.Contains(x.Id));
        }

        public async Task<List<string>> GetTechStackNamesByIdsAsync(IEnumerable<Guid> ids)
        {
            var list = ids.Distinct().ToList();

            return await _db.TechStacks.AsNoTracking()
                .Where(x => list.Contains(x.Id))
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToListAsync();
        }

        public async Task<List<Guid>> GetTechStackIdsByTestIdAsync(Guid testId)
        {
            return await _db.HrTestTechStacks.AsNoTracking()
                .Where(x => x.TestId == testId)
                .Select(x => x.TechStackId)
                .ToListAsync();
        }

        public async Task<int> CountTestsByApplicantIdAsync(Guid applicantId)
        {
            return await _db.HrTests.CountAsync(x => x.ApplicantId == applicantId);
        }

        public async Task<(int answeredCount, int correctCount)> GetScoreCountsAsync(Guid testId)
        {
            var answeredCount = await _db.UserAnswers.AsNoTracking()
                .Where(x => x.TestId == testId && x.SelectedOptionId != null)
                .CountAsync();

            var correctCount = await _db.UserAnswers.AsNoTracking()
                .Where(x => x.TestId == testId && x.IsCorrect == true)
                .CountAsync();

            return (answeredCount, correctCount);
        }

        public Task SaveChangesAsync() => _db.SaveChangesAsync();

        public async Task<List<Question>> GetAssignedQuestionsForTestAsync(Guid testId)
        {
            return await _db.HrTestQuestions.AsNoTracking()
                .Where(x => x.TestId == testId)
                .OrderBy(x => x.Order)
                .Include(x => x.Question)
                    .ThenInclude(q => q.Options)
                .Select(x => x.Question)
                .ToListAsync();
        }
    }
}