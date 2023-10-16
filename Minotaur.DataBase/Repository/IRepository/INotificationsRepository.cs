using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface INotificationsRepository : IRepository<Notification>
    {
        void Update(Notification notification);
        void UpdateRange(Notification[] notifications);
    }
}