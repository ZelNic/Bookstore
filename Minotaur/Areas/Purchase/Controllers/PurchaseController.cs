using Minotaur.Areas.Customer;
using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Minotaur.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace Minotaur.Areas.Purchase
{
    [Area("Purchase")]
    [Authorize(Roles = Roles.Role_Customer)]
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<MinotaurUser> _userManager;

        public PurchaseController(ApplicationDbContext db, UserManager<MinotaurUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetInfomationAboutBuyer()
        {
            MinotaurUser? user = await _userManager.GetUserAsync(User);

            if (user == null) { return BadRequest(new { error = "Пользователь не найден" }); }

            Order order = new()
            {
                UserId = Guid.Parse(user.Id),
                ReceiverName = user.FirstName,
                ReceiverLastName = user.LastName,
                OrderStatus = OperationByOrder.StatusPending_0,
                Region = user.Region,
                City = user.City,
                Street = user.Street,
                HouseNumber = user.HouseNumber,
                PhoneNumber = user.PhoneNumber,
            };

            return Json(new { data = order });
        }


        public async Task<OrderProductData[]> GetVerifiedProductData()
        {
            MinotaurUser? user = await _userManager.GetUserAsync(User);


            ShoppingBasket? shoppingBasket = await _db.ShoppingBaskets.Where(u => u.UserId == Guid.Parse(user.Id)).FirstOrDefaultAsync();
            if (shoppingBasket == null) { return null; }

            Dictionary<int, int> productIdAndCount = ShoppingBasketController.ParseProductData(shoppingBasket.ProductIdAndCount);

            OrderProductData[] purchaseData = await _db.Products
                .Where(u => productIdAndCount.Keys.Contains(u.ProductId))
                .Select(p => new OrderProductData
                {
                    Id = p.ProductId,
                    Price = p.Price,
                    Count = productIdAndCount[p.ProductId]
                }).ToArrayAsync();


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
            MinotaurUser? user = await _userManager.GetUserAsync(User);

            int sumOnWallet = user.PersonalWallet;
            OrderProductData[] purchaseData = await GetVerifiedProductData();
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
            MinotaurUser? user = await _userManager.GetUserAsync(User);

            user.PersonalWallet += sum;
            await _userManager.UpdateAsync(user);
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Payment(string dataDelivery)
        {
            MinotaurUser? user = await _userManager.GetUserAsync(User);

            Order? order = JsonConvert.DeserializeObject<Order>(dataDelivery);
            if (order == null) return BadRequest("Ошибка в отправленных данных.");

            OrderProductData[] purchaseData = await GetVerifiedProductData();
            int confirmedPrice = purchaseData.Sum(product => product.Count * product.Price);
            if (confirmedPrice != order.PurchaseAmount) { return BadRequest("Ошибочная стоимость заказа."); }

            ShoppingBasket? sb = await _db.ShoppingBaskets.Where(u => u.UserId == Guid.Parse(user.Id)).FirstOrDefaultAsync();

            if (sb == null) return BadRequest("Запись о списке покупок не найдена.");

            Dictionary<int, int> productIdAndCount = ShoppingBasketController.ParseProductData(sb.ProductIdAndCount);


            OrderProductData[] productData = await GetVerifiedProductData();
            string prodDataJson = JsonConvert.SerializeObject(productData);


            if (user.PersonalWallet >= order.PurchaseAmount)
            {
                MinotaurUser? admin = await _userManager.FindByIdAsync("604c075d-c691-49d6-9d6f-877cfa866e59");
                if (admin != null)
                {
                    order.ProductData = prodDataJson;
                    order.OrderStatus = OperationByOrder.StatusApproved_1;
                    order.PurchaseDate = MoscowTime.GetTime();


                    user.PersonalWallet -= order.PurchaseAmount;
                    admin.PersonalWallet += order.PurchaseAmount;

                    _db.ShoppingBaskets.Remove(sb);

                    await _userManager.UpdateAsync(admin);
                    await _userManager.UpdateAsync(user);

                    await _db.Orders.AddAsync(order);

                    await _db.SaveChangesAsync();

                    return Ok();
                }
                else
                {
                    return BadRequest("Технические проблемы со стороны магазина.");
                }
            }
            else
            {
                return BadRequest("Не хватает средств.");
            }
        }
    }
}
