using Azure.Core;
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

        public void Update(RequestTelegram request)
        {
            _db.TelegramRequests.Update(request);
        }

        public void UpdateRange(RequestTelegram[] request)
        {
            _db.TelegramRequests.UpdateRange(request);
        }
    }
}