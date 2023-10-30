using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models.Models.ModelReview;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IReviewsRepository : IRepository<ProductReview>
    {
        void Update(ProductReview review);
        void UpdateRange(ProductReview[] reviews);
    }
}