using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class RecordStock
    {
        [Key]
        public int Id { get; set; }
        [Required] public int StockId { get; set; }
        [Required] public string ResponsiblePersonId { get; set; }
        [Required] public DateTime Time { get; set; }
        [JsonProperty] public int ProductId { get; set; }
        [JsonProperty] public string? ProductName { get; set; }
        [JsonProperty] public int Count { get; set; }
        public int ShelfNumber { get; set; }
        [Required] public string Operation { get; set; }
        public bool IsOrder { get; set; }
        public string? ProductDataOnPurchase { get; set; }
    }
}
