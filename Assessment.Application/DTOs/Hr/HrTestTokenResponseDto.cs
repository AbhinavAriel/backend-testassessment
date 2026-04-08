namespace Assessment.Application.DTOs.Hr
{
    public class HrTestTokenResponseDto
    {
        public Guid TestId { get; init; }
        public Guid ApplicantId { get; init; }
        public HrApplicantDto Applicant { get; init; } = new();
        public List<string> TechStacks { get; init; } = new();
        public HrTestInfoDto Test { get; init; } = new();

        public class HrTestInfoDto
        {
            public int TotalQuestions { get; init; }
            public int DurationMinutes { get; init; }
            public string Level { get; init; } = "";
            public string Status { get; init; } = "";
            public int AnsweredCount { get; init; }
            public int CorrectCount { get; init; }
            public DateTime CreatedAtUtc { get; init; }
            public DateTime? SubmittedAtUtc { get; init; }
            public DateTime? ExpiresAtUtc { get; init; }
        }
    }
}
