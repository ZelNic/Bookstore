using Microsoft.EntityFrameworkCore;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        IMinotaurUsersRepository MinotaurUsers { get; }
        ICategoryRepository Categories { get; }
        IShoppingBasketsRepository ShoppingBaskets { get; }
        IWishListRepository WishLists { get; }
        IOrdersRepository Orders { get; }
        INotificationsRepository Notifications { get; }
        IStockMagazineRepository StockMagazine { get; }
        IOfficesRepository Offices { get; }
        IWorkersRepository Workers { get; }
        IOrganizationalOrderRepository OrganizationalOrders { get; }
        IProductReviewsRepository ProductReviews { get; }
        ITelegramRequestsRepository TelegramRequests { get; }
        IPickUpReviewRepository PickUpReviews { get; }
        IWorkerReviewRepository WorkerReviews { get; }
        IDeliveryReviewRepository DeliveryReviews { get; }
        IOrderMovementHistoryRepository OrderMovementHistory { get; }
        IAccountingForOrdersRepository AccountingForOrders { get; }
        Task SaveAsync();
    }
}
