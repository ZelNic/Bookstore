using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository
{
    public class TelegramRequestsRepository : Repository<RequestTelegram>, ITelegramRequestsRepository
    {
        private readonly ApplicationDbContext _db;
        public TelegramRequestsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(RequestTelegram review)
        {
            _db.TelegramRequests.Update(review);
        }

        public void UpdateRange(RequestTelegram[] reviews)
        {
            _db.TelegramRequests.UpdateRange(reviews);
        }
    }
}