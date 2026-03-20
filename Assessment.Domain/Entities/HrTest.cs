using System;
using System.Collections.Generic;

namespace Assessment.Domain.Entities
{
    public class HrTest
    {
        public Guid Id { get; set; }

        public Guid ApplicantId { get; set; }
        public HrApplicant Applicant { get; set; } = null!;

        public int TotalQuestions { get; set; }
        public int DurationMinutes { get; set; }

        public string Level { get; set; } = "Beginner";

        public string Status { get; set; } = "Created";

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public string? TestToken { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public int AnsweredCount { get; set; }
        public int CorrectCount { get; set; }

        public decimal ScorePercentage { get; set; }  
        public bool IsPassed { get; set; }
        public DateTime? SubmittedAtUtc { get; set; }

        public ICollection<HrTestTechStack> TechStacks { get; set; } = new List<HrTestTechStack>();
    }
}