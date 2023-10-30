using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
        void UpdateRange(Product[] products);
    }
}
