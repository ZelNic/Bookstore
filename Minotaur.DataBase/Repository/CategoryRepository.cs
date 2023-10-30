using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;

namespace Minotaur.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
            _db.Categories.Update(category);
        }

        public void UpdateRange(Category[] category)
        {
            _db.Categories.UpdateRange(category);
        }
    }
}