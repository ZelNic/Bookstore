using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models.Models.ModelReview;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IWorkerReviewRepository : IRepository<WorkerReview>
    {
        void Update(WorkerReview workerReview);
        void UpdateRange(WorkerReview[] workerReviews);
    }
}