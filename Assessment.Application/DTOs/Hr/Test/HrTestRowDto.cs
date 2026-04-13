using Assessment.Application.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessment.Application.DTOs.Hr.Test
{
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

        public List<HrTechStackWithLevelDto> TechStackLevels { get; set; } = new();
    }
}
