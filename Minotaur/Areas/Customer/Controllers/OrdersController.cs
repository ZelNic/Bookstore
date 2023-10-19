using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Minotaur.Models.SD;
using Minotaur.DataAccess.Repository.IRepository;

namespace Minotaur.Areas.Customer
{
    [Area("Customer")]
    [Authorize(Roles = Roles.Role_Customer)]
    public class OrdersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;
        public OrdersController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> GetOrders()
        {
            var user = await _userManager.GetUserAsync(User);

            var ordersDB = _unitOfWork.Orders.GetAll(u => u.UserId == Guid.Parse(user.Id)).OrderByDescending(u => u.OrderId);            

            var formatedOrders = ordersDB.Select(o => new
            {
                o.OrderId,
                o.UserId,
                o.ReceiverName,
                o.ReceiverLastName,
                o.PhoneNumber,
                OrderedProducts = JsonConvert.DeserializeObject<IEnumerable<OrderProductData>>(o.OrderedProducts),
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
            });

            Dictionary<int, string> prodIdAndName = new();

            foreach (var order in formatedOrders)
            {
                IEnumerable<OrderProductData> opd = order.OrderedProducts;
                foreach (var productData in opd)
                {
                    prodIdAndName.TryAdd(productData.Id, _unitOfWork.Products.GetAsync(u => u.ProductId == productData.Id).Result.Name);
                }
            }

            return Json(new { data = formatedOrders, prodIdAndName });
        }
    }
}
