using Assessment.Application.DTOs.Answers;
using Assessment.Application.Interfaces;
using Assessment.Application.Interfaces.Repositories;
using Assessment.Domain.Entities;

namespace Assessment.Infrastructure.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository _repo;

        public AnswerService(IAnswerRepository repo)
        {
            _repo = repo;
        }

        public async Task<SubmitAnswerResponseDto> SubmitAnswerAsync(SubmitAnswerDto dto)
        {
            if (dto.TestId == Guid.Empty) throw new ArgumentException("Invalid TestId.");
            if (dto.ApplicantId == Guid.Empty) throw new ArgumentException("Invalid ApplicantId.");
            if (dto.QuestionId == Guid.Empty) throw new ArgumentException("Invalid QuestionId.");

            var test = await _repo.GetTestByIdAsync(dto.TestId, asNoTracking: true);
            if (test == null) throw new KeyNotFoundException("Test not found.");

            if (test.ExpiresAtUtc <= DateTime.UtcNow)
                throw new InvalidOperationException("This test link has expired.");

            var question = await _repo.GetQuestionAsync(dto.QuestionId);
            if (question == null) throw new KeyNotFoundException("Question not found.");

            bool? isCorrect = null;
            if (dto.SelectedOptionId.HasValue)
            {
                var opt = await _repo.GetOptionForQuestionAsync(dto.SelectedOptionId.Value, dto.QuestionId);
                if (opt == null) throw new ArgumentException("Selected option is invalid.");
                isCorrect = opt.IsCorrect;
            }

            var existing = await _repo.GetExistingAnswerAsync(dto.ApplicantId, dto.TestId, dto.QuestionId);

            if (existing == null)
            {
                await _repo.AddAsync(new UserAnswer
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

            await _repo.SaveChangesAsync();

            return new SubmitAnswerResponseDto { Ok = true, IsCorrect = isCorrect };
        }
    }
}
