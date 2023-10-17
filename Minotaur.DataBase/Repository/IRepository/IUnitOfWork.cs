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
        IReviewsRepository Reviews { get; }
        INotificationsRepository Notifications { get; }
        IStockMagazineRepository StockMagazine { get; }
        IOfficesRepository Offices { get; }
        IWorkersRepository Workers { get; }
        IOrganizationalOrderRepository OrganizationalOrders { get; }
        void SaveAsync();
    }
}
