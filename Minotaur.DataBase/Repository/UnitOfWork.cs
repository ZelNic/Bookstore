using Microsoft.EntityFrameworkCore;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public IProductRepository Products { get; }
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
        public IOrganizationalOrderRepository OrganizationalOrders { get; set; }


        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            Products = new ProductRepository(_db);
            MinotaurUsers = new MinotaurUserRepository(_db);
            Categories = new CategoryRepository(_db);
            ShoppingBaskets = new ShoppingBasketRepository(_db);
            WishLists = new WishListRepository(_db);
            Orders = new OrdersRepository(_db);
            Reviews = new ReviewRepository(_db);
            Notifications = new NotificationsRepository(_db);
            StockMagazine = new StockMagazineRepository(_db);
            Offices = new OfficesRepository(_db);
            Workers = new WorkersRepository(_db);
            OrganizationalOrders = new OrganizationalOrdersRepository(_db);
        }

        public async void Save()
        {
            _db.SaveChanges();
        }

    }
}
