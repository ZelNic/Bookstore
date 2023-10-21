using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface ITelegramRequestsRepository : IRepository<RequestTelegram>
    {
        void Update(RequestTelegram review);
        void UpdateRange(RequestTelegram[] reviews);
    }
}