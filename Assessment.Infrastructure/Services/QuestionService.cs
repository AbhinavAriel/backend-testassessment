using System;
using Assessment.Application.DTOs.Questions;
using Assessment.Application.Interfaces;
using Assessment.Application.Interfaces.Repositories;
using Assessment.Domain.Entities;
using Assessment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Infrastructure.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questions;
        private readonly IHrTestRepository _hr;
        private readonly ApplicationDbContext _db;

        public QuestionService(
            IQuestionRepository questions,
            IHrTestRepository hr,
            ApplicationDbContext db)
        {
            _questions = questions;
            _hr = hr;
            _db = db;
        }

        public async Task<List<QuestionResponseDto>> GetQuestionsAsync()
        {
            var list = await _questions.GetAllWithOptionsAsync();
            return list.Select(Map).ToList();
        }

        public async Task<QuestionResponseDto> CreateAsync(CreateQuestionRequestDto dto)
        {
            if (dto == null)
                throw new ArgumentException("Invalid payload.");

            var text = (dto.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Question text is required.");

            if (dto.TechStackId == Guid.Empty)
                throw new ArgumentException("Tech stack is required.");

            if (string.IsNullOrWhiteSpace(dto.Level))
                throw new ArgumentException("Level is required.");

            var techExists = await _db.TechStacks.AsNoTracking()
                .AnyAsync(x => x.Id == dto.TechStackId);

            if (!techExists)
                throw new KeyNotFoundException("Selected tech stack was not found.");

            var options = dto.Options ?? new List<CreateQuestionOptionDto>();
            if (options.Count != 4)
                throw new ArgumentException("Exactly 4 options are required.");

            var normalizedOptions = options
                .Select(x => new CreateQuestionOptionDto
                {
                    Text = (x.Text ?? "").Trim(),
                    IsCorrect = x.IsCorrect
                })
                .ToList();

            if (normalizedOptions.Any(x => string.IsNullOrWhiteSpace(x.Text)))
                throw new ArgumentException("All option texts are required.");

            if (normalizedOptions.Count(x => x.IsCorrect) != 1)
                throw new ArgumentException("Exactly one correct option must be selected.");

            var level = ParseLevel(dto.Level);

            var alreadyExists = await _db.Questions.AsNoTracking()
                .AnyAsync(q =>
                    q.Text.ToLower() == text.ToLower() &&
                    q.TechStackId == dto.TechStackId &&
                    q.Level == level);

            if (alreadyExists)
                throw new InvalidOperationException("This question already exists for the selected tech stack and level.");

            var maxOrder = await _db.Questions.AnyAsync()
                ? await _db.Questions.MaxAsync(q => q.Order)
                : 0;

            var question = new Question
            {
                Id = Guid.NewGuid(),
                Order = maxOrder + 1,
                Text = text,
                TechStackId = dto.TechStackId,
                Level = level,
                IsActive = true,
                Options = normalizedOptions.Select(o => new AnswerOption
                {
                    Id = Guid.NewGuid(),
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList()
            };

            _db.Questions.Add(question);
            await _db.SaveChangesAsync();

            return Map(question);
        }

        public async Task<List<QuestionResponseDto>> GetQuestionsForTestAsync(Guid testId)
        {
            if (testId == Guid.Empty)
                throw new ArgumentException("Invalid TestId.");

            var test = await _hr.GetTestByIdAsync(testId, asNoTracking: true);
            if (test == null)
                throw new KeyNotFoundException("Invalid TestId.");

            if (test.ExpiresAtUtc <= DateTime.UtcNow)
                throw new InvalidOperationException("This test link has expired.");

            var assignedQuestions = await _hr.GetAssignedQuestionsForTestAsync(testId);

            if (assignedQuestions == null || assignedQuestions.Count == 0)
                throw new ArgumentException("No questions found for this test.");

            return assignedQuestions
                .Where(x => x.Question != null)
                .Select(x => new QuestionResponseDto
                {
                    Id = x.Question.Id,
                    Order = x.Order, 
                    Text = x.Question.Text,
                    Level = x.Question.Level.ToString(),
                    Options = x.Question.Options.Select(o => new AnswerOptionResponseDto
                    {
                        Id = o.Id,
                        Text = o.Text
                    }).ToList()
                })
                .ToList();
        }

        private static QuestionResponseDto Map(Question q) =>
            new QuestionResponseDto
            {
                Id = q.Id,
                Order = q.Order,
                Text = q.Text,
                Level = q.Level.ToString(),
                Options = q.Options.Select(o => new AnswerOptionResponseDto
                {
                    Id = o.Id,
                    Text = o.Text
                }).ToList()
            };

        private static QuestionLevel ParseLevel(string? level)
        {
            return (level ?? "").Trim().ToLowerInvariant() switch
            {
                "beginner" => QuestionLevel.Beginner,
                "intermediate" => QuestionLevel.Intermediate,
                "professional" => QuestionLevel.Professional,
                _ => throw new ArgumentException("Invalid level. Use Beginner / Intermediate / Professional.")
            };
        }
    }
}