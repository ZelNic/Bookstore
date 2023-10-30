using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models.ModelReview;

namespace Minotaur.DataAccess.Repository
{
    internal class ProductReviewsRepository : Repository<ProductReview>, IProductReviewsRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductReviewsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ProductReview review)
        {
            _db.ProductReviews.Update(review);
        }

        public void UpdateRange(ProductReview[] reviews)
        {
            _db.ProductReviews.UpdateRange(reviews);
        }
    }
}