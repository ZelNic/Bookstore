namespace Minotaur.Models
{
    public class ShoppingBasketClient
    {
        public int Id { get; set; }
        public Dictionary<int, int>? ProductIdAndCount { get; set; }
    }
}
