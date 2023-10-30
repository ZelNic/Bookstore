using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository
{
    public class ShoppingBasketRepository : Repository<ShoppingBasket>, IShoppingBasketsRepository
    {
        private readonly ApplicationDbContext _db;
        public ShoppingBasketRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ShoppingBasket basket)
        {
            _db.ShoppingBaskets.Update(basket);
        }

        public void UpdateRange(ShoppingBasket[] baskets)
        {
            _db.ShoppingBaskets.UpdateRange(baskets);
        }
    }
}