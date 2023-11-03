using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository
{
    public class OrderMovementHistoryRepository : Repository<OrderMovementHistory>, IOrderMovementHistoryRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderMovementHistoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderMovementHistory orderMovementHistory)
        {
            _db.OrderMovementHistory.Update(orderMovementHistory);
        }
      
        public void UpdateRange(OrderMovementHistory[] orderMovementHistory)
        {
            _db.OrderMovementHistory.UpdateRange(orderMovementHistory);
        }
    }
}