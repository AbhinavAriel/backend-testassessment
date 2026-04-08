using Assessment.Application.DTOs.Answers;

namespace Assessment.Application.Interfaces
{
    public interface IAnswerService
    {
        Task<SubmitAnswerResponseDto> SubmitAnswerAsync(SubmitAnswerDto dto);
    }
}
