using System;
using Assessment.Application.DTOs.Answers;
using Assessment.Application.Interfaces;
using Assessment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Infrastructure.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly ApplicationDbContext _db;

        public AnswerService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<object> SubmitAnswerAsync(SubmitAnswerDto dto)
        {
            if (dto.TestId == Guid.Empty) throw new ArgumentException("Invalid TestId.");
            if (dto.ApplicantId == Guid.Empty) throw new ArgumentException("Invalid ApplicantId.");
            if (dto.QuestionId == Guid.Empty) throw new ArgumentException("Invalid QuestionId.");

            // ✅ Enforce expiry on answer submission
            var test = await _db.HrTests.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == dto.TestId);

            if (test == null) throw new KeyNotFoundException("Test not found.");

            if (test.ExpiresAtUtc <= DateTime.UtcNow)
                throw new InvalidOperationException("This test link has expired.");

            var question = await _db.Questions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == dto.QuestionId);

            if (question == null) throw new KeyNotFoundException("Question not found.");

            bool? isCorrect = null;

            if (dto.SelectedOptionId != null)
            {
                var opt = question.Options.FirstOrDefault(o => o.Id == dto.SelectedOptionId);
                if (opt == null) throw new ArgumentException("Selected option is invalid.");
                isCorrect = opt.IsCorrect;
            }

            var existing = await _db.UserAnswers
                .FirstOrDefaultAsync(a =>
                    a.TestId == dto.TestId &&
                    a.ApplicantId == dto.ApplicantId &&
                    a.QuestionId == dto.QuestionId);

            if (existing == null)
            {
                _db.UserAnswers.Add(new Domain.Entities.UserAnswer
                {
                    Id = Guid.NewGuid(),
                    TestId = dto.TestId,
                    ApplicantId = dto.ApplicantId,
                    QuestionId = dto.QuestionId,
                    SelectedOptionId = dto.SelectedOptionId,
                    IsCorrect = isCorrect,
                    ElapsedSeconds = dto.ElapsedSeconds,
                    AnsweredAt = DateTime.UtcNow
                });
            }
            else
            {
                existing.SelectedOptionId = dto.SelectedOptionId;
                existing.IsCorrect = isCorrect;
                existing.ElapsedSeconds = dto.ElapsedSeconds;
                existing.AnsweredAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            return new
            {
                ok = true,
                isCorrect = isCorrect
            };
        }
    }
}