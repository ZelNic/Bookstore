using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IAccountingForOrdersRepository: IRepository<AccountingEntry>
    {
        void Update(AccountingEntry accountingEntry);
        void UpdateRange(AccountingEntry[] accountingEntrys);
    }
}