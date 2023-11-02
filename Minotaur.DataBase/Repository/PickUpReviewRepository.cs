using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models.ModelReview;

namespace Minotaur.DataAccess.Repository
{
    public class PickUpReviewRepository : Repository<PickUpReview>, IPickUpReviewRepository
    {
        private readonly ApplicationDbContext _db;
        public PickUpReviewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(PickUpReview pickUpReview)
        {
            _db.PickUpReviews.Update(pickUpReview);
        }

        public void UpdateRange(PickUpReview[] pickUpReviews)
        {
            _db.PickUpReviews.UpdateRange(pickUpReviews);
        }
    }
}