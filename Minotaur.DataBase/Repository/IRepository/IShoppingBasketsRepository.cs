using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IShoppingBasketsRepository : IRepository<ShoppingBasket>
    {
        void Update(ShoppingBasket basket);
        void UpdateRange(ShoppingBasket[] baskets);
    }
}