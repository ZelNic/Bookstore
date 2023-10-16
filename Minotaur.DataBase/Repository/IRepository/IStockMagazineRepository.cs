using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IStockMagazineRepository : IRepository<RecordStock>
    {
        void Update(RecordStock recordStock);
        void UpdateRange(RecordStock[] recordStocks);
    }
}