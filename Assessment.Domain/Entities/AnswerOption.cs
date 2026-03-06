using System;

namespace Assessment.Domain.Entities
{
    public class AnswerOption
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }

        public string Text { get; set; } = "";
        public bool IsCorrect { get; set; }

        public Question Question { get; set; } = null!;
    }
}