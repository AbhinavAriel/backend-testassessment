using Assessment.Application.Constants;
using Assessment.Application.DTOs.Hr.Applicant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assessment.Application.DTOs.Hr.Test
{
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
            public string Status { get; set; } = TestStatus.Created;
            public int AnsweredCount { get; set; }
            public int CorrectCount { get; set; }
            public DateTime CreatedAtUtc { get; set; }
            public DateTime? SubmittedAtUtc { get; set; }
        }
    }
}
