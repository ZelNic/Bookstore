namespace Minotaur.Models
{
    public class ShoppingBasketClient
    {
        public Guid Id { get; set; }
        public Dictionary<int, int>? ProductIdAndCount { get; set; }
    }
}
