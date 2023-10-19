using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository
{
    public class WorkersRepository : Repository<Worker>, IWorkersRepository
    {
        private readonly ApplicationDbContext _db;
        public WorkersRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Worker worker)
        {
            _db.Workers.Update(worker);
        }

        public void UpdateRange(List<Worker> workers)
        {
            _db.Workers.UpdateRange(workers);
        }
    }
}