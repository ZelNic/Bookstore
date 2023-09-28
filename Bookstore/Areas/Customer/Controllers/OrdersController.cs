using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Bookstore.Areas.Customer
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


        public async Task<IActionResult> Index()
        {
            Order [] orders = await _db.Order.Where(u => u.UserId == _user.UserId).ToArrayAsync();
            

            var formatedOrders = orders.Select(o => new {
                o.OrderId,
                o.UserId,
                o.ReceiverName,
                o.ReceiverLastName,
                ProductData = JsonConvert.DeserializeObject<IEnumerable<OrderProductData>>(o.ProductData),
                o.PurchaseDate,
                o.PurchaseAmount,
            }).ToArray();



            foreach (var order in formatedOrders)
            {
                IEnumerable<OrderProductData> orderProductData = order.ProductData;
                foreach (var productData in orderProductData)
                {
                    
                }
            }



            return Json(formatedOrders);
        }
    }
}
