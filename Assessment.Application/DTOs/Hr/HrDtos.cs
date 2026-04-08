using Assessment.Application.Constants;

namespace Assessment.Application.DTOs.Hr
{
    // -------- Meta --------
    public class HrMetaDto
    {
        public List<HrTechStackDto> TechStacks { get; set; } = new();
        public List<string> Levels { get; set; } = QuestionLevelLabels.All.ToList();
    }

    public class HrTechStackDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
    }

    // -------- Applicant --------
    public class HrApplicantDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
    }

    // -------- Table Row --------
    public class HrTestRowDto
    {
        public int SerialNo { get; set; }
        public Guid TestId { get; set; }
        public Guid ApplicantId { get; set; }

        public string ApplicantName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";

        public int TotalQuestions { get; set; }
        public int DurationMinutes { get; set; }
        public string Level { get; set; } = QuestionLevelLabels.Beginner;
        public string Status { get; set; } = TestStatus.Created;

        public int AnsweredCount { get; set; }
        public int CorrectCount { get; set; }
        public decimal ScorePercentage { get; set; }
        public bool IsPassed { get; set; }
        public bool IsRejected { get; set; }
        public string? CancellationReason { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? SubmittedAtUtc { get; set; }

        public string TestToken { get; set; } = "";
        public DateTime? ExpiresAtUtc { get; set; }

        public List<string> TechStacks { get; set; } = new();
    }

    public class CreateHrTestRequestDto
    {
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public List<Guid> TechStackIds { get; set; } = new();
        public int TotalQuestions { get; set; }
        public int DurationMinutes { get; set; }
        public string Level { get; set; } = QuestionLevelLabels.Beginner;
    }

    // -------- Update --------
    public class UpdateHrTestRequestDto
    {
        public string FullName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public List<Guid> TechStackIds { get; set; } = new();
        public int TotalQuestions { get; set; }
        public int DurationMinutes { get; set; }
        public string Level { get; set; } = "";
    }

    // -------- Detail --------
    public class HrTestDetailDto
    {
        public Guid TestId { get; set; }
        public Guid ApplicantId { get; set; }
        public HrApplicantDto Applicant { get; set; } = new();
        public HrTestInfoDto Test { get; set; } = new();
        public List<string> TechStacks { get; set; } = new();

        public class HrTestInfoDto
        {
            public int TotalQuestions { get; set; }
            public int DurationMinutes { get; set; }
            public string Level { get; set; } = QuestionLevelLabels.Beginner;
            public string Status { get; set; } = TestStatus.Created;
            public int AnsweredCount { get; set; }
            public int CorrectCount { get; set; }
            public DateTime CreatedAtUtc { get; set; }
            public DateTime? SubmittedAtUtc { get; set; }
        }
    }
}
