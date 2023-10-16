using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IWishListRepository : IRepository<WishList>
    {
        void Update(WishList wishList);
        void UpdateRange(WishList[] wishLists);
    }
}