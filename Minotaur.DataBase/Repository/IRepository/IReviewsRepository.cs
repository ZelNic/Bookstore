using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models.Models.ModelReview;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IReviewsRepository : IRepository<Review>
    {
        void Update(Review review);
        void UpdateRange(Review[] reviews);
    }
}