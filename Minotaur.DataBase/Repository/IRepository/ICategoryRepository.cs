using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
        void UpdateRange(Category[] category);
    }
}