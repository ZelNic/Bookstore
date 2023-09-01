using Bookstore.Models.Models;

namespace Bookstore.Models
{
    public class StockVM
    {
        public Stock? Stock { get; set; }
        public List<RecordStock>? WarehouseJournal { get; set; }
    }
}
