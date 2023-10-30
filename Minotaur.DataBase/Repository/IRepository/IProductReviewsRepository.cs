using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models.ModelReview;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IProductReviewsRepository : IRepository<ProductReview>
    {
        void Update(ProductReview review);
        void UpdateRange(ProductReview[] reviews);
    }
}