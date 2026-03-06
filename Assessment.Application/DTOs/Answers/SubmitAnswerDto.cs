using System;

namespace Assessment.Application.DTOs.Answers
{
    public class SubmitAnswerDto
    {
        public Guid TestId { get; set; }
        public Guid ApplicantId { get; set; }
        public Guid QuestionId { get; set; }

        // ✅ nullable (skipped)
        public Guid? SelectedOptionId { get; set; }

        public int ElapsedSeconds { get; set; }
    }
}