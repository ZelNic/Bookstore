using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IOrderMovementHistoryRepository : IRepository<OrderMovementHistory>
    {
        void Update(OrderMovementHistory orderMovementHistory);
        void UpdateRange(OrderMovementHistory[] orderMovementHistory);
    }
}