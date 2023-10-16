using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository
{
    public class StockMagazineRepository : Repository<RecordStock>, IStockMagazineRepository
    {
        private readonly ApplicationDbContext _db;
        public StockMagazineRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(RecordStock recordStock)
        {
            _db.StockMagazine.Update(recordStock);
        }

        public void UpdateRange(RecordStock[] recordStocks)
        {
            _db.StockMagazine.UpdateRange(recordStocks);
        }
    }
}