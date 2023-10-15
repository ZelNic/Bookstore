using Microsoft.EntityFrameworkCore;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models;
using Minotaur.Models.OrganizationalDocumentation.HR;
using Minotaur.Models;

namespace Minotaur.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public IProductRepository Products { get; private set; }
        public IMinotaurUsersRepository MinotaurUsers { get; set; }

        public ICategoryRepository Categories { get; set; }
        public IShoppingBasketsRepository ShoppingBaskets { get; set; }
        public IWishListRepository WishLists { get; set; }
        public IOrdersRepository Orders { get; set; }
        public IReviewsRepository Reviews { get; set; }
        public INotificationsRepository Notifications { get; set; }
        public IStockMagazineRepository StockMagazine { get; set; }
        public IOfficesRepository Offices { get; set; }
        public IWorkersRepository Workers { get; set; }
        public DbSet<OrganizationalOrder> OrganizationalOrders { get; set; }


        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            Products = new ProductRepository(_db);

        }

        public async void SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
