using System;
using System.Collections.Generic;

namespace Assessment.Application.DTOs.Questions
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public string Text { get; set; } = "";
        public List<AnswerOptionDto> Options { get; set; } = new();
    }

    public class AnswerOptionDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = "";
    }
}