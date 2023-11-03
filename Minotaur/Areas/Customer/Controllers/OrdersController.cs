using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Newtonsoft.Json;

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

            var formatedOrders = (await _unitOfWork.Orders.GetAllAsync(u => u.UserId == Guid.Parse(user.Id)))
                .Select(o => new
                {
                    o.OrderId,
                    o.UserId,
                    o.ReceiverName,
                    o.ReceiverLastName,
                    o.PhoneNumber,
                    OrderedProducts = o.OrderedProducts != null ? JsonConvert.DeserializeObject<List<OrderProductData>>(o.OrderedProducts) : null,
                    ShippedProducts = o.ShippedProducts != null ? JsonConvert.DeserializeObject<List<OrderProductData>>(o.ShippedProducts) : null,
                    PurchaseDate = o.PurchaseDate.ToString("dd.MM.yyyy HH:mm"),
                    o.PurchaseAmount,
                    o.RefundAmount,
                    o.City,
                    o.Street,
                    o.HouseNumber,
                    o.IsCourierDelivery,
                    o.OrderPickupPointId,
                    o.OrderStatus,
                }).ToList();


            return Json(new { data = formatedOrders });
        }
    }
}
