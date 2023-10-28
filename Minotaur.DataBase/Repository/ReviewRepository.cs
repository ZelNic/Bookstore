using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models.ModelReview;

namespace Minotaur.DataAccess.Repository
{
    public class ReviewRepository : Repository<Review>, IReviewsRepository
    {
        private readonly ApplicationDbContext _db;
        public ReviewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Review review)
        {
            //_db.Reviews.Update(review);
        }

        public void UpdateRange(Review[] reviews)
        {
            //_db.Reviews.UpdateRange(reviews);
        }
    }
}