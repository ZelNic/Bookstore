using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IOfficesRepository : IRepository<Office>
    {
        void Update(Office office);
        void UpdateRange(Office[] offices);
    }
}