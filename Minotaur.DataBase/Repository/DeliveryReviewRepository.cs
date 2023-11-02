using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models.ModelReview;

namespace Minotaur.DataAccess.Repository
{
    public class DeliveryReviewRepository : Repository<DeliveryReview>, IDeliveryReviewRepository
    {
        private readonly ApplicationDbContext _db;
        public DeliveryReviewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(DeliveryReview deliveryReview)
        {
            _db.DeliveryReviews.Update(deliveryReview);
        }

        public void UpdateRange(DeliveryReview[] deliveryReviews)
        {
            _db.DeliveryReviews.UpdateRange(deliveryReviews);
        }
    }
}