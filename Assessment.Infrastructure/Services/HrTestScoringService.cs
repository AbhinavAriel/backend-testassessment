using Assessment.Application.Constants;
using Assessment.Application.Interfaces.Repositories;

namespace Assessment.Infrastructure.Services
{
   
    public class HrTestScoringService
    {
        private readonly IHrTestRepository _hrRepo;

        public HrTestScoringService(IHrTestRepository hrRepo)
        {
            _hrRepo = hrRepo;
        }

        public async Task SubmitTestAsync(Guid testId)
        {
            if (testId == Guid.Empty)
                throw new ArgumentException("Invalid TestId.");

            var test = await _hrRepo.GetTestByIdForUpdateAsync(testId);
            if (test == null)
                throw new KeyNotFoundException("Test not found.");

            if (string.Equals(test.Status, TestStatus.Submitted, StringComparison.OrdinalIgnoreCase))
                return;

            var (answered, correct) = await _hrRepo.GetScoreCountsAsync(testId);

            var scorePercentage = test.TotalQuestions > 0
                ? (decimal)Math.Round((double)correct / test.TotalQuestions * 100, 2)
                : 0m;

            test.AnsweredCount = answered;
            test.CorrectCount = correct;
            test.ScorePercentage = scorePercentage;
            test.IsPassed = scorePercentage >= ScoringConstants.PassThresholdPercent;
            test.Status = TestStatus.Submitted;
            test.SubmittedAtUtc = DateTime.UtcNow;

            await _hrRepo.SaveChangesAsync();
        }
    }
}
