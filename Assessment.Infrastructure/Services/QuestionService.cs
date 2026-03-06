using System;
using Assessment.Application.DTOs.Questions;
using Assessment.Application.Interfaces;
using Assessment.Application.Interfaces.Repositories;
using Assessment.Domain.Entities;

namespace Assessment.Infrastructure.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questions;
        private readonly IHrTestRepository _hr;

        public QuestionService(IQuestionRepository questions, IHrTestRepository hr)
        {
            _questions = questions;
            _hr = hr;
        }

        public async Task<List<QuestionResponseDto>> GetQuestionsAsync()
        {
            var list = await _questions.GetAllWithOptionsAsync();
            return list.Select(Map).ToList();
        }

        public async Task<List<QuestionResponseDto>> GetQuestionsForTestAsync(Guid testId)
        {
            if (testId == Guid.Empty) throw new ArgumentException("Invalid TestId.");

            var test = await _hr.GetTestByIdAsync(testId, asNoTracking: true);
            if (test == null) throw new KeyNotFoundException("Invalid TestId.");

            if (test.ExpiresAtUtc <= DateTime.UtcNow)
                throw new InvalidOperationException("This test link has expired.");
            var questions = await _hr.GetAssignedQuestionsForTestAsync(testId);

    if (questions == null || questions.Count == 0)
        throw new ArgumentException("No questions found for this test.");

            var techIds = await _hr.GetTechStackIdsByTestIdAsync(testId);
            if (techIds.Count == 0) throw new ArgumentException("No tech stacks found for this test.");

            if (!Enum.TryParse<QuestionLevel>(test.Level, ignoreCase: true, out var level))
                throw new ArgumentException("Invalid test level.");

            var total = test.TotalQuestions;
            if (total <= 0) throw new ArgumentException("Invalid total questions in test.");

            var available = await _questions.CountAvailableAsync(techIds, level);
            if (available < total)
                throw new ArgumentException($"Not enough questions for selected tech stacks/level. Needed {total}, available {available}.");

            var picked = await _questions.GetForTestAsync(techIds, level, total);
            return picked.Select(Map).ToList();
        }

        private static QuestionResponseDto Map(Question q) =>
            new QuestionResponseDto
            {
                Id = q.Id,
                Order = q.Order,
                Text = q.Text,
                TimeLimitSeconds = q.TimeLimitSeconds,
                Level = q.Level.ToString(),
                Options = q.Options.Select(o => new AnswerOptionResponseDto
                {
                    Id = o.Id,
                    Text = o.Text
                }).ToList()
            };
    }
}