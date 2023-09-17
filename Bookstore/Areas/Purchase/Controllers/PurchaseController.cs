using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.SD;
using Bookstore.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult FillRecipientDate(int purchaseАmount)
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

        [HttpPost]
        public IActionResult FillDeliveryDate(Order orderData)
        {


            return View(orderData);
        }


        [HttpPost]
        public async Task<IActionResult> Payment(Order orderData)
        {
            if (_user.PersonalWallet >= orderData.PurchaseAmount)
            {
                User? admin = await _db.User.FindAsync(1);
                if (admin != null)
                {
                    _user.PersonalWallet -= orderData.PurchaseAmount;
                    admin.PersonalWallet += orderData.PurchaseAmount;

                    _db.User.UpdateRange(_user, admin);
                    await _db.SaveChangesAsync();

                    return View(orderData);
                }
                else
                {
                    return NotFound("Технические проблемы.");
                }
            }
            else
            {
                return NotFound("Не хватает средств.");
            }
        }


        //[HttpPost]
        //public async Task<IActionResult> FundsVerification(Order orderData)
        //{

        //    var sb = await _db.ShoppingBasket.Where(u => u.UserId == _user.UserId).ToListAsync();


        //    IEnumerable<ProductData> productData = await _db.ShoppingBasket
        //        .Where(sb => sb.UserId == _user.UserId)
        //        .Join(_db.Books, sb => sb.ProductId, b => b.BookId, (sb, b) => new { sb, b })
        //        .Select(x => new ProductData
        //        {
        //            ProdId = x.sb.ProductId,
        //            Price = x.b.Price,
        //            Count = x.sb.CountProduct
        //        }).ToListAsync();

        //    string prodDataJson = JsonConvert.SerializeObject(productData);

        //    orderData.ProductData = prodDataJson;
        //    orderData.OrderStatus = SD.StatusPending_0;
        //    orderData.DeliveryAddress += orderData.City + ',' + orderData.Street + ' ' + orderData.HouseNumber;
        //    orderData.PurchaseDate = MoscowTime.GetTime();

        //    _db.ShoppingBasket.RemoveRange(sb);

        //    await _db.Order.AddAsync(orderData);

        //    await _db.SaveChangesAsync();
        //    return RedirectToAction("Index", "Orders", new { area = "Customer" });
        //}
    }
}
