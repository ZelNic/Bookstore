using Bookstore.Areas.Customer;
using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.SD;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public async Task<IActionResult> GetInfomationAboutBuyer()
        {
            Order order = new()
            {
                UserId = _user.UserId,
                ReceiverName = _user.FirstName,
                ReceiverLastName = _user.LastName,
                OrderStatus = SD.StatusPending_0,
                DeliveryAddress = $"{ _user.City}, {_user.Street} {_user.HouseNumber}",
                Region = _user.Region,
                City = _user.City,
                Street = _user.Street,
                HouseNumber = _user.HouseNumber,
                PhoneNumber = _user.PhoneNumber,                
            };

            return Json(new { data = order });
        }

        [HttpGet]
        public async Task<IActionResult> GetInfomationDelivery()
        {


            
            return Json(new { data = "@"  });
        }


        public async Task<IActionResult> GetVerifiedProductData()
        {
            ShoppingBasket? shoppingBasket = await _db.ShoppingBasket.Where(u => u.UserId == _user.UserId).FirstOrDefaultAsync();
            if (shoppingBasket == null) { return BadRequest(new { error = $"Корзина пользовтеля {_user.UserId} не найден." }); }

            Dictionary<int, int> productIdAndCount = ShoppingBasketController.ParseProductData(shoppingBasket.ProductIdAndCount);

            var purchaseData = await _db.Products.Where(u => productIdAndCount.Keys.Contains(u.ProductId))
                .Join(_db.Products, b => b.ProductId, c => c.ProductId, (b, c) => new
                {
                    price = b.Price,
                    productId = b.ProductId,
                    count = productIdAndCount[b.ProductId]
                }).ToArrayAsync();


            int total = purchaseData.Sum(product => product.count * product.price);

            return Json(new { purchaseData, total });
        }

        [HttpGet]
        public async Task<IActionResult> FillRecipientDate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult FillDeliveryDate(string buyerData)
        {

            return View();
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
