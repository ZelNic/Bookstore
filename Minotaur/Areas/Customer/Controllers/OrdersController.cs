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
using System.Collections.Generic;

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

            var formatedOrders = (await _unitOfWork.Orders.GetAllAsync(u => u.UserId == Guid.Parse(user.Id))).Select(o => new
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
                o.CurrentPosition,
                o.IsCourierDelivery,
                o.OrderPickupPointId,
                o.OrderStatus,
                o.TravelHistory
            }).ToList();

            Dictionary<int, string> prodIdAndName = new();

            foreach (var order in formatedOrders)
            {
                if (order.ShippedProducts != null)
                {
                    List<OrderProductData> opd = order.ShippedProducts;
                    foreach (var productData in opd)
                        prodIdAndName.TryAdd(productData.Id, (await _unitOfWork.Products.GetAsync(u => u.ProductId == productData.Id)).Name);
                }
                else
                {
                    List<OrderProductData> opd = order.OrderedProducts;
                    foreach (var productData in opd)
                        prodIdAndName.TryAdd(productData.Id, (await _unitOfWork.Products.GetAsync(u => u.ProductId == productData.Id)).Name);

                }
            }

            return Json(new { data = formatedOrders, prodIdAndName });
        }
    }
}
