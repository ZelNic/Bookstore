using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class RecordStock
    {
        [Key]
        public Guid Id { get; set; }
        public Guid StockId { get; set; }
        public Guid ResponsiblePersonId { get; set; }
        public DateTime Time { get; set; }
        [JsonProperty] public int ProductId { get; set; }
        [JsonProperty] public string? ProductName { get; set; }
        [JsonProperty] public int Count { get; set; }
        public int ShelfNumber { get; set; }
        public string? Operation { get; set; }
        public bool IsNeed { get; set; }
        public string? ProductDataOnPurchase { get; set; }
    }
}
