using Assessment.Application.Interfaces.Repositories;

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

        if (string.Equals(test.Status, "Submitted", StringComparison.OrdinalIgnoreCase))
            return;

        var (answered, correct) = await _hrRepo.GetScoreCountsAsync(testId);

        test.AnsweredCount = answered;
        test.CorrectCount = correct;
        test.Status = "Submitted";
        test.SubmittedAtUtc = DateTime.UtcNow;

        await _hrRepo.SaveChangesAsync();
    }
}