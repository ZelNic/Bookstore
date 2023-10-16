using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;

namespace Minotaur.DataAccess.Repository
{
    public class MinotaurUserRepository : Repository<MinotaurUser>, IMinotaurUsersRepository
    {
        private readonly ApplicationDbContext _db;

        public MinotaurUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(MinotaurUser minotaurUser)
        {
            MinotaurUser? user = _db.MinotaurUsers.FirstOrDefault(i => i.Id == minotaurUser.Id);
            if (minotaurUser != null)
            {
                _db.MinotaurUsers.Update(minotaurUser);
            }
        }

        public void UpdateRange(MinotaurUser[] minotaurUsers)
        {
            _db.UpdateRange(minotaurUsers);
        }
    }
}