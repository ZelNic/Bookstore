using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository
{
    public class AccountingForOrdersRepository : Repository<AccountingEntry>, IAccountingForOrdersRepository
    {
        private readonly ApplicationDbContext _db;
        public AccountingForOrdersRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(AccountingEntry accountingEntry)
        {
            _db.AccountingForOrders.Update(accountingEntry);
        }

        public void UpdateRange(AccountingEntry[] accountingEntrys)
        {
            _db.AccountingForOrders.UpdateRange(accountingEntrys);
        }
    }
}