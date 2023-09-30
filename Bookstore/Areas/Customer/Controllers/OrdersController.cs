using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Minotaur.Areas.Customer
{
    [Area("Customer")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _user;

        public OrdersController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
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

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> GetOrders()
        {
            Order[] ordersDB = await _db.Order.Where(u => u.UserId == _user.UserId).OrderByDescending(u => u.OrderId).ToArrayAsync();
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
