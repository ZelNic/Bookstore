using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository
{
    public class OfficesRepository : Repository<Office>, IOfficesRepository
    {
        private readonly ApplicationDbContext _db;
        public OfficesRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Office office)
        {
            _db.Offices.Update(office);
        }

        public void UpdateRange(Office[] offices)
        {
            _db.Offices.UpdateRange(offices);
        }
    }
}