using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models.ModelReview;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IDeliveryReviewRepository : IRepository<DeliveryReview>
    {
        void Update(DeliveryReview deliveryReview);
        void UpdateRange(DeliveryReview[] deliveryReviews);
    }
}