using Minotaur.Models.Models;

namespace Minotaur.Models
{
    public class StockVM
    {
        public Office? Office { get; set; }
        public List<RecordStock>? WarehouseJournal { get; set; }
    }
}
