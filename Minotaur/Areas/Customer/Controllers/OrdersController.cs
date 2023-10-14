using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Minotaur.Models.SD;

namespace Minotaur.Areas.Customer
{
    [Area("Customer")]
    [Authorize(Roles = Roles.Role_Customer)]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<MinotaurUser> _userManager;
        public OrdersController(ApplicationDbContext db, UserManager<MinotaurUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> GetOrders()
        {
            var user = await _userManager.GetUserAsync(User);

            Order[] ordersDB = await _db.Orders.Where(u => u.UserId == Guid.Parse(user.Id)).OrderByDescending(u => u.OrderId).ToArrayAsync();
            Product[] productsDB = await _db.Products.ToArrayAsync();

            var formatedOrders = ordersDB.Select(o => new
            {
                o.OrderId,
                o.UserId,
                o.ReceiverName,
                o.ReceiverLastName,
                o.PhoneNumber,
                ProductData = JsonConvert.DeserializeObject<IEnumerable<OrderProductData>>(o.ProductData),
                PurchaseDate = o.PurchaseDate.ToString("dd.MM.yyyy HH:mm"),
                o.PurchaseAmount,
                o.City,
                o.Street,
                o.HouseNumber,
                o.CurrentPosition,
                o.IsCourierDelivery,
                o.OrderPickupPointId,
                o.OrderStatus,
                o.TravelHistory
            }).ToArray();

            Dictionary<int, string> prodIdAndName = new();

            foreach (var order in formatedOrders)
            {
                IEnumerable<OrderProductData> opd = order.ProductData;
                foreach (var productData in opd)
                {
                    prodIdAndName.TryAdd(productData.Id, productsDB.Where(u => u.ProductId == productData.Id).FirstOrDefault()?.Name);
                }
            }

            return Json(new { data = formatedOrders, prodIdAndName });
        }
    }
}
