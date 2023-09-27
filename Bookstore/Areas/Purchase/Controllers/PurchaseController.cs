using Bookstore.Areas.Customer;
using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.SD;
using Bookstore.Utility;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
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

        [HttpGet]
        public async Task<IActionResult> GetInfomationAboutBuyer()
        {
            Order order = new()
            {
                UserId = _user.UserId,
                ReceiverName = _user.FirstName,
                ReceiverLastName = _user.LastName,
                OrderStatus = SD.StatusPending_0,
                DeliveryAddress = $"{_user.City}, {_user.Street} {_user.HouseNumber}",
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

            return Json(new { data = "@" });
        }


        public async Task<List<PurchaseData>> GetVerifiedProductData()
        {
            ShoppingBasket? shoppingBasket = await _db.ShoppingBasket.Where(u => u.UserId == _user.UserId).FirstOrDefaultAsync();
            if (shoppingBasket == null) { return null; }

            Dictionary<int, int> productIdAndCount = ShoppingBasketController.ParseProductData(shoppingBasket.ProductIdAndCount);

            List<PurchaseData> purchaseData = await _db.Products
                .Where(u => productIdAndCount.Keys.Contains(u.ProductId))
                .Select(p => new PurchaseData
                {
                    Price = p.Price,
                    ProductId = p.ProductId,
                    Count = productIdAndCount[p.ProductId]
                }).ToListAsync();

            return purchaseData;
        }

        [HttpGet]
        public async Task<IActionResult> FillDeliveryDate()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPersonalWalletAndPurchaseAmount()
        {
            int sumOnWallet = _user.PersonalWallet;
            List<PurchaseData> purchaseData = await GetVerifiedProductData();
            int purchaseAmount = purchaseData.Sum(product => product.Count * product.Price);
            var data = new
            {
                sumOnWallet,
                purchaseAmount
            };
            return Json(new { data });
        }

        //***********************************************************************ONLY DURING DEVELOPMENT***********************************************************************

        [HttpPost]
        public async Task<IActionResult> AddMoneyOnWallet(int sum)
        {
            _user.PersonalWallet += sum;
            _db.User.Update(_user);
            await _db.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Payment(string dataDelivery)
        {
            Order? order = JsonConvert.DeserializeObject<Order>(dataDelivery);
            if (order == null) { return BadRequest(new { error = "Ошибка в отправленных данных." }); }

            List<PurchaseData> purchaseData = await GetVerifiedProductData();
            int confirmedPrice = purchaseData.Sum(product => product.Count * product.Price);
            if (confirmedPrice != order.PurchaseAmount)
            {
                return BadRequest(new { error = "Ошибочная стоимость заказа." });
            }


            if (_user.PersonalWallet >= order.PurchaseAmount)
            {
                User? admin = await _db.User.FindAsync(1);
                if (admin != null)
                {
                    _user.PersonalWallet -= order.PurchaseAmount;
                    admin.PersonalWallet += order.PurchaseAmount;

                    _db.User.UpdateRange(_user, admin);
                    await _db.Order.AddAsync(order);
                    await _db.SaveChangesAsync();
                    FundsVerification(order);
                    return Ok();
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


        [HttpPost]
        public async Task<IActionResult> FundsVerification(Order orderData)
        {

            ShoppingBasket sb = await _db.ShoppingBasket.Where(u => u.UserId == _user.UserId).FirstOrDefaultAsync();

            Dictionary<int, int> ProductIdAndCount = ShoppingBasketController.ParseProductData(sb.ProductIdAndCount);



            var productData = await _db.ShoppingBa
                           .Where(sb => sb.UserId == _user.UserId)
                           .Join(_db.Products, sb => sb.ProductIdAndCount, b => b.ProductId, (sb, b) => new { sb, b })
                           .Select(x => new ProductData
                           {
                               ProdId = x.sb.ProductId,
                               Price = x.b.Price,
                               Count = x.sb.CountProduct
                           }).ToListAsync();

            string prodDataJson = JsonConvert.SerializeObject(productData);

            orderData.ProductData = prodDataJson;
            orderData.OrderStatus = SD.StatusPending_0;
            orderData.DeliveryAddress += orderData.City + ',' + orderData.Street + ' ' + orderData.HouseNumber;
            orderData.PurchaseDate = MoscowTime.GetTime();

            _db.ShoppingBasket.RemoveRange(sb);

            await _db.Order.AddAsync(orderData);

            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Orders", new { area = "Customer" });
        }
    }
}
