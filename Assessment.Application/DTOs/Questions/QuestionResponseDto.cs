using System;
using System.Collections.Generic;

namespace Assessment.Application.DTOs.Questions
{
    public class QuestionResponseDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public string Text { get; set; } = "";
        public int TimeLimitSeconds { get; set; } // (even if frontend doesn't use it)
        public string Level { get; set; } = "";
        public List<AnswerOptionResponseDto> Options { get; set; } = new();
    }
}