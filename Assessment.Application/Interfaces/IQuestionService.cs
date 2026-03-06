using Assessment.Application.DTOs.Questions;

namespace Assessment.Application.Interfaces
{
    public interface IQuestionService
    {
        Task<List<QuestionResponseDto>> GetQuestionsAsync();
        Task<List<QuestionResponseDto>> GetQuestionsForTestAsync(Guid testId);
    }
}