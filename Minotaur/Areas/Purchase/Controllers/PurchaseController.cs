using Minotaur.Areas.Customer;
using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Minotaur.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Minotaur.Areas.Purchase
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

            if (_contextAccessor.HttpContext.Session.GetInt32("UserId") != null)
            {
                int? userId = _contextAccessor.HttpContext.Session.GetInt32("UserId");
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
                Region = _user.Region,
                City = _user.City,
                Street = _user.Street,
                HouseNumber = _user.HouseNumber,
                PhoneNumber = _user.PhoneNumber,
            };

            return Json(new { data = order });
        }


        public async Task<List<OrderProductData>> GetVerifiedProductData()
        {
            ShoppingBasket? shoppingBasket = await _db.ShoppingBasket.Where(u => u.UserId == _user.UserId).FirstOrDefaultAsync();
            if (shoppingBasket == null) { return null; }

            Dictionary<int, int> productIdAndCount = ShoppingBasketController.ParseProductData(shoppingBasket.ProductIdAndCount);

            List<OrderProductData> purchaseData = await _db.Products
                .Where(u => productIdAndCount.Keys.Contains(u.ProductId))
                .Select(p => new OrderProductData
                {
                    Id = p.ProductId,
                    Price = p.Price,
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
            List<OrderProductData> purchaseData = await GetVerifiedProductData();
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

            List<OrderProductData> purchaseData = await GetVerifiedProductData();
            int confirmedPrice = purchaseData.Sum(product => product.Count * product.Price);
            if (confirmedPrice != order.PurchaseAmount) { return BadRequest(new { error = "Ошибочная стоимость заказа." }); }

            ShoppingBasket? sb = await _db.ShoppingBasket.Where(u => u.UserId == _user.UserId).FirstOrDefaultAsync();

            if (sb == null) { return BadRequest(new { error = "Запись о списке покупок не найдена." }); }

            Dictionary<int, int> productIdAndCount = ShoppingBasketController.ParseProductData(sb.ProductIdAndCount);


            List<OrderProductData> productData = await GetVerifiedProductData();
            string prodDataJson = JsonConvert.SerializeObject(productData);


            if (_user.PersonalWallet >= order.PurchaseAmount)
            {
                User? admin = await _db.User.FindAsync(1);
                if (admin != null)
                {
                    order.ProductData = prodDataJson;
                    order.OrderStatus = SD.StatusApproved_1;
                    order.PurchaseDate = MoscowTime.GetTime();


                    _user.PersonalWallet -= order.PurchaseAmount;
                    admin.PersonalWallet += order.PurchaseAmount;

                    _db.ShoppingBasket.Remove(sb);
                    _db.User.UpdateRange(_user, admin);
                    await _db.Order.AddAsync(order);
                    await _db.SaveChangesAsync();

                    return Ok();
                }
                else
                {
                    return BadRequest(new { error = "Технические проблемы со стороны магазина." });
                }
            }
            else
            {
                return BadRequest(new { error = "Не хватает средств." });
            }
        }
    }
}
