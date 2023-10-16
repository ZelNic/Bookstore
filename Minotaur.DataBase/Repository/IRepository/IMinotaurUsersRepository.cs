using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IMinotaurUsersRepository : IRepository<MinotaurUser>
    {
        void Update(MinotaurUser minotaurUser);
        void UpdateRange(MinotaurUser[] minotaurUsers);
    }   
}