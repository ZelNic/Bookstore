using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.SD;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Bookstore.Areas.Purchase
{
    [Area("Purchase")]
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _user;

        public PurchaseController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _contextAccessor = contextAccessor;

            if (_contextAccessor.HttpContext.Session.GetInt32("Username") != null)
            {
                int? userId = _contextAccessor.HttpContext.Session.GetInt32("Username");
                if ((userId != null) && (_db.User.Find(userId) != null))
                {
                    _user = _db.User.Find(userId);
                }
            }
        }

        [HttpPost]
        public IActionResult Index(int purchaseАmount)
        {
            ViewBag.Number = purchaseАmount;
            return View();
        }

        [HttpPost]
        public IActionResult FundsVerification(int purchaseАmount)
        {
            if (_user.PersonalWallet >= purchaseАmount)
            {
                _user.PersonalWallet -= purchaseАmount;

                var admin = _db.User.Find(1);
                admin.PersonalWallet += purchaseАmount;

                _db.User.UpdateRange(_user, admin);


                var sb = _db.ShoppingBasket.Where(u => u.UserId == _user.UserId);

                var priceAndIdProduct = _db.ShoppingBasket
                    .Where(sb => sb.UserId == _user.UserId)
                    .Join(_db.Books, sb => sb.ProductId, b => b.BookId, (sb, b) => new { sb, b })
                    .Select(x => new { x.sb.ProductId, x.b.Price })
                    .ToList();

                List<int> productIdList = priceAndIdProduct.Select(s => s.ProductId).ToList();
                List<int> productPriceList = priceAndIdProduct.Select(s => s.Price).ToList();
                List<int> productCountList = sb.Select(s => s.CountProduct).ToList();

                string productId = string.Join(",", priceAndIdProduct);
                string productCount = string.Join(",", productCountList);
                string productPrice = string.Join(",", productCountList);

                PurchaseHistory purchase = new()
                {
                    UserId = _user.UserId,
                    ProductId = productId,
                    ProductCount = productCount,
                    ProductPrice = "test",
                    PurchaseDate = new DateTime(),
                    PurchaseAmount = 1,
                    OrderStatus = SD.StatusRefunded,
                    CurrentPosition = "Moskow",
                    TravelHistory = "SPB,Moskow"
                };

                _db.ShoppingBasket.RemoveRange(sb);
                _db.PurchaseHistory.Add(purchase);

                _db.SaveChanges();
                return RedirectToAction("Index", "Orders", new { area = "Customer" });
            }
            else
            {
                return NotFound("Не хватает средств");
            }
        }
    }
}
