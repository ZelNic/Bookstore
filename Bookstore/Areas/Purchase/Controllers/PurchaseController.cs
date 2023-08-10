using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.SD;
using Bookstore.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

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
        public IActionResult DeliveryInformation(int purchaseАmount)
        {
            if (purchaseАmount == 0)
            {
                return RedirectToAction("Index", "ShoppingBasket", new { area = "Customer" });
            }

            Order order = new()
            {
                UserId = _user.UserId,
                PurchaseAmount = purchaseАmount,
            };

            return View(order);
        }




        //-----------------------------------------------------------------------Разбить на несколько методов.-------------------------------------------------------------------

        [HttpPost]
        public IActionResult Payment(Order orderData)
        {
            if (_user.PersonalWallet >= orderData.PurchaseAmount)
            {
                _user.PersonalWallet -= orderData.PurchaseAmount;

                User? admin = _db.User.Find(1);
                if (admin != null)
                {
                    admin.PersonalWallet += orderData.PurchaseAmount;

                    _db.User.UpdateRange(_user, admin);
                    orderData.PurchaseDate = MoscowTime.GetTime();
                }

                return View(orderData);
            }
            else
            {
                return NotFound("Не хватает средств");
            }
        }


        [HttpPost]
        public IActionResult FundsVerification(Order orderData)
        {

            var sb = _db.ShoppingBasket.Where(u => u.UserId == _user.UserId);


            IEnumerable<ProductData> productData = _db.ShoppingBasket
                .Where(sb => sb.UserId == _user.UserId)
                .Join(_db.Books, sb => sb.ProductId, b => b.BookId, (sb, b) => new { sb, b })
                .Select(x => new ProductData
                {
                    ProdId = x.sb.ProductId,
                    Price = x.b.Price,
                    Count = x.sb.CountProduct
                }).ToList();

            string prodDataJson = JsonConvert.SerializeObject(productData);

            //
            orderData.ProductData = prodDataJson;
            orderData.OrderStatus = SD.StatusPending;
            orderData.DeliveryAddress += orderData.Region + ' ' + orderData.City + ' ' + orderData.Street + ' ' + orderData.HouseNumber;



            _db.ShoppingBasket.RemoveRange(sb);

            _db.Order.Add(orderData);

            _db.SaveChanges();
            return RedirectToAction("Index", "Orders", new { area = "Customer" });
        }
    }
}
