using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository
{
    public class WishListRepository : Repository<WishList>, IWishListRepository
    {
        private readonly ApplicationDbContext _db;
        public WishListRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(WishList wishList)
        {
            _db.WishLists.Update(wishList);
        }

        public void UpdateRange(WishList[] wishLists)
        {
           _db.WishLists.UpdateRange(wishLists);
        }
    }
}