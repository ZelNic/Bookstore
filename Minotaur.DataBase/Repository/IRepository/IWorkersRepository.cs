using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IWorkersRepository : IRepository<Worker>
    {
        void Update(Worker worker);
        void UpdateRange(Worker[] workers);
    }
}