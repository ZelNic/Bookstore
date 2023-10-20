using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository
{
    public class BankRepository : Repository<Bank>, IBankRepository
    {
        private readonly ApplicationDbContext _db;

        public BankRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void CreateBank()
        {
            
            //_db.Bank.Add();
        }
    }
}