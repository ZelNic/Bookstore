using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models.ModelReview;

namespace Minotaur.DataAccess.Repository
{
    public class WorkerReviewRepository : Repository<WorkerReview>, IWorkerReviewRepository
    {
        private readonly ApplicationDbContext _db;
        public WorkerReviewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(WorkerReview workerReview)
        {
            _db.WorkerReviews.Update(workerReview);
        }

        public void UpdateRange(WorkerReview[] workerReviews)
        {
            _db.WorkerReviews.UpdateRange(workerReviews);
        }
    }
}