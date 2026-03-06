using System;
using System.Collections.Generic;

namespace Assessment.Domain.Entities
{
    public class Question
    {
        public Guid Id { get; set; }

        // Order is only for display if needed; for test we randomize
        public int Order { get; set; }

        public string Text { get; set; } = "";

        // Kept for DB compatibility (frontend ignores this now)
        public int TimeLimitSeconds { get; set; } = 60;

        public QuestionLevel Level { get; set; }

        public bool IsActive { get; set; } = true;

        // ✅ NEW: link question to tech stack
        public Guid TechStackId { get; set; }
        public TechStack TechStack { get; set; } = null!;

        public List<AnswerOption> Options { get; set; } = new();
    }
}