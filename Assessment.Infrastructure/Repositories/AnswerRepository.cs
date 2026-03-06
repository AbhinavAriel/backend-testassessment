using Assessment.Application.Interfaces.Repositories;
using Assessment.Domain.Entities;
using Assessment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Infrastructure.Repositories
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly ApplicationDbContext _db;

        public AnswerRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> ApplicantExistsAsync(Guid applicantId)
        {
            // ✅ option A: applicants stored in HrApplicants
            return await _db.HrApplicants.AnyAsync(a => a.Id == applicantId);
        }

        public async Task<HrTest?> GetTestByIdAsync(Guid testId, bool asNoTracking = true)
        {
            var q = _db.HrTests
                .Include(t => t.Applicant)
                .Where(t => t.Id == testId);

            if (asNoTracking) q = q.AsNoTracking();

            return await q.FirstOrDefaultAsync();
        }

        public async Task<Question?> GetQuestionAsync(Guid questionId)
        {
            return await _db.Questions.FirstOrDefaultAsync(q => q.Id == questionId);
        }

        public async Task<AnswerOption?> GetOptionForQuestionAsync(Guid optionId, Guid questionId)
        {
            return await _db.AnswerOptions
                .FirstOrDefaultAsync(o => o.Id == optionId && o.QuestionId == questionId);
        }

        // ✅ FIXED: parameter order should match service call (ApplicantId, TestId, QuestionId)
        public async Task<UserAnswer?> GetExistingAnswerAsync(Guid applicantId, Guid testId, Guid questionId)
        {
            return await _db.UserAnswers
                .FirstOrDefaultAsync(a =>
                    a.TestId == testId &&
                    a.ApplicantId == applicantId &&
                    a.QuestionId == questionId);
        }

        public async Task AddAsync(UserAnswer answer)
        {
            await _db.UserAnswers.AddAsync(answer);
        }

        public async Task<(int AnsweredCount, int CorrectCount)> GetStatsForTestAsync(Guid testId)
        {
            var baseQ = _db.UserAnswers.AsNoTracking().Where(x => x.TestId == testId);

            var answered = await baseQ.CountAsync(x => x.SelectedOptionId != null);
            var correct = await baseQ.CountAsync(x => x.IsCorrect == true);

            return (answered, correct);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}