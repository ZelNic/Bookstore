using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository
{
    public class NotificationsRepository : Repository<Notification>, INotificationsRepository
    {
        private readonly ApplicationDbContext _db;
        public NotificationsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Notification notification)
        {
            _db.Notifications.Update(notification);
        }

        public void UpdateRange(Notification[] notifications)
        {
            _db.Notifications.UpdateRange(notifications);
        }
    }
}