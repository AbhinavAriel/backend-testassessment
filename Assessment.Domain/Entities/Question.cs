using System;
using System.Collections.Generic;

namespace Assessment.Domain.Entities
{
    public class Question
    {
        public Guid Id { get; set; }

        public int Order { get; set; }

        public string Text { get; set; } = "";
        public QuestionLevel Level { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid TechStackId { get; set; }
        public TechStack TechStack { get; set; } = null!;

        public List<AnswerOption> Options { get; set; } = new();
    }
}