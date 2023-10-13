using Minotaur.Models.Models;

namespace Minotaur.Models.ModelForWorkingControllers
{
    public class DataByStock
    {
        public MinotaurUser MinotaurUser { get; set; }
        public Worker StockKeeper { get; set; }
        public Office Stock { get; set; }
        public List<RecordStock>? Records { get; set; }
    }
}
