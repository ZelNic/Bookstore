using Minotaur.Models.Models;

namespace Minotaur.Models
{
    public class StockVM
    {
        public Office? Office { get; set; }
        public RecordStock[]? WarehouseJournal { get; set; }
    }
}
