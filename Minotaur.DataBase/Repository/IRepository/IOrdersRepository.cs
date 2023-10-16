using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IOrdersRepository : IRepository<Order>
    {
        void Update(Order order);
        void UpdateRange(Order[] orders);
    }
}