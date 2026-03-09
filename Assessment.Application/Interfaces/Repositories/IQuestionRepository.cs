using Assessment.Domain.Entities;

namespace Assessment.Application.Interfaces.Repositories
{
    public interface IQuestionRepository
    {
        Task<int> CountAvailableAsync(List<Guid> techIds, QuestionLevel level);
        Task<List<Question>> GetForTestAsync(List<Guid> techIds, QuestionLevel level, int take);

        Task<List<Question>> GetAllWithOptionsAsync();
    }
}