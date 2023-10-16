using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.OrganizationalDocumentation.HR;

namespace Minotaur.DataAccess.Repository
{
    public class OrganizationalOrdersRepository : Repository<OrganizationalOrder>, IOrganizationalOrderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrganizationalOrdersRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrganizationalOrder organizationalOrder)
        {
            _db.OrganizationalOrders.Update(organizationalOrder);
        }

        public void UpdateRange(OrganizationalOrder[] organizationalOrder)
        {
            _db.OrganizationalOrders.UpdateRange(organizationalOrder);
        }
    }
}