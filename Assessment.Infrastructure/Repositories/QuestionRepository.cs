using Assessment.Application.Interfaces.Repositories;
using Assessment.Domain.Entities;
using Assessment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Infrastructure.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _db;

        public QuestionRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<int> CountAvailableAsync(List<Guid> techIds, QuestionLevel level)
        {
            return await _db.Questions.AsNoTracking()
                .Where(q => q.IsActive && techIds.Contains(q.TechStackId) && q.Level == level)
                .CountAsync();
        }

        public async Task<List<Question>> GetForTestAsync(List<Guid> techIds, QuestionLevel level, int take)
        {
            var list = await _db.Questions.AsNoTracking()
                .Include(q => q.Options)
                .Where(q => q.IsActive && techIds.Contains(q.TechStackId) && q.Level == level)
                .ToListAsync();

            var rnd = Random.Shared;
            return list.OrderBy(_ => rnd.Next()).Take(take).ToList();
        }

        public async Task<List<Question>> GetAllWithOptionsAsync()
        {
            return await _db.Questions.AsNoTracking()
                .Include(q => q.Options)
                .Where(q => q.IsActive)
                .OrderBy(q => q.Order)
                .ToListAsync();
        }
    }
}