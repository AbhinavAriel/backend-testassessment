using Assessment.Domain.Entities;

namespace Assessment.Application.Interfaces.Repositories
{
    public interface IHrTestRepository
    {
        Task<HrApplicant?> GetApplicantByEmailAsync(string normalizedEmail);
        Task AddApplicantAsync(HrApplicant applicant);
        Task UpdateApplicantAsync(HrApplicant applicant);

        Task AddTestAsync(HrTest test);
        Task AddTechStacksAsync(IEnumerable<HrTestTechStack> techStacks);

        Task<List<TechStack>> GetAllTechStacksAsync();
        Task<int> CountTechStacksByIdsAsync(IEnumerable<Guid> ids);
        Task<List<string>> GetTechStackNamesByIdsAsync(IEnumerable<Guid> ids);

        Task SaveChangesAsync();
        Task<List<HrTest>> GetLatestTestsAsync(int take);

        Task<HrTest?> GetTestByIdAsync(Guid testId, bool asNoTracking = true);
        Task<List<Guid>> GetTechStackIdsByTestIdAsync(Guid testId);
        Task RemoveTechStacksByTestIdAsync(Guid testId);

        Task DeleteTestAsync(HrTest test);
        Task DeleteApplicantAsync(HrApplicant applicant);
        Task<int> CountTestsByApplicantIdAsync(Guid applicantId);
        Task<HrApplicant?> GetApplicantByIdAsync(Guid applicantId);

        Task<HrTest?> GetTestByIdForUpdateAsync(Guid testId);
        Task<(int answeredCount, int correctCount)> GetScoreCountsAsync(Guid testId);

        Task<int> CountTestsAsync();
        Task<List<HrTest>> GetTestsPagedAsync(int skip, int take);
        Task<List<HrTestQuestion>> GetAssignedQuestionsForTestAsync(Guid testId);
    }
}