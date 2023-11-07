using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models;
using Minotaur.Models.Models.ModelReview;

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
        public INotificationsRepository Notifications { get; set; }
        public IStockMagazineRepository StockMagazine { get; set; }
        public IOfficesRepository Offices { get; set; }
        public IWorkersRepository Workers { get; set; }
        public IOrganizationalOrderRepository OrganizationalOrders { get; set; }        
        public ITelegramRequestsRepository TelegramRequests { get; set; }
        public IProductReviewsRepository ProductReviews { get; set; }
        public IPickUpReviewRepository PickUpReviews { get; set; }
        public IWorkerReviewRepository WorkerReviews { get; set; }
        public IDeliveryReviewRepository DeliveryReviews { get; set; }
        public IOrderMovementHistoryRepository OrderMovementHistory { get; set; }

        public IAccountingForOrdersRepository AccountingForOrders { get; set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            Products = new ProductRepository(_db);
            MinotaurUsers = new MinotaurUserRepository(_db);
            Categories = new CategoryRepository(_db);
            ShoppingBaskets = new ShoppingBasketRepository(_db);
            WishLists = new WishListRepository(_db);
            Orders = new OrdersRepository(_db);
            Notifications = new NotificationsRepository(_db);
            StockMagazine = new StockMagazineRepository(_db);
            Offices = new OfficesRepository(_db);
            Workers = new WorkersRepository(_db);
            OrganizationalOrders = new OrganizationalOrdersRepository(_db);
            TelegramRequests = new TelegramRequestsRepository(_db);
            ProductReviews = new ProductReviewsRepository(_db);
            PickUpReviews = new PickUpReviewRepository(_db);
            WorkerReviews = new WorkerReviewRepository(_db);
            DeliveryReviews = new DeliveryReviewRepository(_db);
            OrderMovementHistory = new OrderMovementHistoryRepository(_db);
            AccountingForOrders = new AccountingForOrdersRepository(_db);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

    }
}
