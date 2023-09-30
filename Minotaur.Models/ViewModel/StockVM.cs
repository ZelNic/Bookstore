using Minotaur.Models.Models;

namespace Minotaur.Models
{
    public class StockVM
    {
        public Stock? Stock { get; set; }
        public List<RecordStock>? WarehouseJournal { get; set; }
    }
}
