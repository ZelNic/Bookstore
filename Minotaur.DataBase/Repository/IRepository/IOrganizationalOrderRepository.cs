using BookStore.DataBase.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.OrganizationalDocumentation.HR;

namespace Minotaur.DataAccess.Repository.IRepository
{
    public interface IOrganizationalOrderRepository : IRepository<OrganizationalOrder>
    {
        void Update(OrganizationalOrder organizationalOrder);
        void UpdateRange(OrganizationalOrder[] organizationalOrders);
    }
}