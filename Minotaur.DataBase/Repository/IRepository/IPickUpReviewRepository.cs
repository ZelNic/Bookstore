using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models.Models.ModelReview;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IPickUpReviewRepository: IRepository<PickUpReview>
    {
        void Update(PickUpReview pickUpReview);
        void UpdateRange(PickUpReview[] pickUpReviews);
    }
}