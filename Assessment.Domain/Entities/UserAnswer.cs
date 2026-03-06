using System;

namespace Assessment.Domain.Entities
{
    public class UserAnswer
    {
        public Guid Id { get; set; }

        // ✅ identifies which test this answer belongs to
        public Guid TestId { get; set; }
        public HrTest Test { get; set; } = null!;

        // ✅ applicant (not Identity user)
        public Guid ApplicantId { get; set; }
        public HrApplicant Applicant { get; set; } = null!;

        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        // ✅ can be NULL when skipped
        public Guid? SelectedOptionId { get; set; }
        public AnswerOption? SelectedOption { get; set; }

        // ✅ can be NULL when skipped
        public bool? IsCorrect { get; set; }

        // ✅ overall elapsed seconds when answer was saved
        public int ElapsedSeconds { get; set; }

        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
    }
}