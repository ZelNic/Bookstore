using Minotaur.Models;

namespace Minotaur.DataAccess.Repository.IRepository
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
                productFromDb.ISBN = product.ISBN;
                productFromDb.Name = product.Name;
                productFromDb.Description = product.Description;
                productFromDb.Price = product.Price;
                productFromDb.Author = product.Author;
                productFromDb.Price = product.Price;
                productFromDb.Category = product.Category;
                productFromDb.EditorId = product.EditorId;
                productFromDb.ImageURL = product.ImageURL;
                productFromDb.ProductRating = product.ProductRating;

                _db.Update(productFromDb);
            }
        }
    }
}
