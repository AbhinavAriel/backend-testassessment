using System;
using System.Collections.Generic;

namespace Assessment.Application.DTOs.Hr
{
    public class HrTestReportDto
    {
        public Guid TestId { get; set; }
        public Guid ApplicantId { get; set; }

        public string ApplicantName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";

        public string Level { get; set; } = "";
        public string Status { get; set; } = "";

        public int TotalQuestions { get; set; }
        public int DurationMinutes { get; set; }
        public int AnsweredCount { get; set; }
        public int CorrectCount { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? SubmittedAtUtc { get; set; }
        public List<string> TechStacks { get; set; } = new();
        public List<HrReportQuestionDto> Questions { get; set; } = new();
    }

    public class HrReportQuestionDto
    {
        public Guid QuestionId { get; set; }
        public int Order { get; set; }
        public string Text { get; set; } = "";

        public Guid? SelectedOptionId { get; set; }
        public string SelectedOptionText { get; set; } = "";

        public Guid? CorrectOptionId { get; set; }
        public string CorrectOptionText { get; set; } = "";

        public bool IsCorrect { get; set; }
        public List<HrReportOptionDto> Options { get; set; } = new();
    }

    public class HrReportOptionDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = "";
    }
}