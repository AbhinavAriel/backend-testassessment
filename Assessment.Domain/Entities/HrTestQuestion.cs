using System;

namespace Assessment.Domain.Entities
{
    public class HrTestQuestion
    {
        public Guid TestId { get; set; }
        public HrTest Test { get; set; } = null!;

        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public int Order { get; set; }
    }
}