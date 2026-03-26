using System;

namespace Assessment.Domain.Entities
{
   
    public class TestSnapshot
    {
        public Guid Id { get; set; }

        public Guid TestId { get; set; }
        public HrTest Test { get; set; } = null!;

        public Guid ApplicantId { get; set; }
        public HrApplicant Applicant { get; set; } = null!;
       
        public string ImageData { get; set; } = "";

        public DateTime CapturedAt { get; set; } = DateTime.UtcNow;

        public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    }
}