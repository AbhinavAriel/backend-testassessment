using Assessment.Domain.Entities;

namespace Assessment.Application.Interfaces.Repositories
{
    public interface IAnswerRepository
    {
        Task<bool> ApplicantExistsAsync(Guid applicantId);
        Task<HrTest?> GetTestByIdAsync(Guid testId, bool asNoTracking = true);
        Task<Question?> GetQuestionAsync(Guid questionId);
        Task<AnswerOption?> GetOptionForQuestionAsync(Guid optionId, Guid questionId);

        // ✅ FIXED signature
        Task<UserAnswer?> GetExistingAnswerAsync(Guid applicantId, Guid testId, Guid questionId);

        Task AddAsync(UserAnswer answer);
        Task<(int AnsweredCount, int CorrectCount)> GetStatsForTestAsync(Guid testId);
        Task SaveChangesAsync();
    }
}