namespace Minotaur.Models
{
    public class ShoppingBasketClient
    {
        public string Id { get; set; }
        public Dictionary<int, int>? ProductIdAndCount { get; set; }
    }
}
