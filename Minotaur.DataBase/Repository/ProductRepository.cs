using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;

namespace Minotaur.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            Product? productFromDb = _db.Products.FirstOrDefault(i => i.ProductId == product.ProductId);
            if (productFromDb != null)
            {
                _db.Products.Update(product);
            }
        }

        public void UpdateRange(Product[] products)
        {
            _db.Products.UpdateRange(products);
        }
    }
}
