using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Minotaur.Utility;
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
        public async Task<IActionResult> Get()
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
                }).OrderByDescending(u => u.PurchaseDate).ToList();


            return Json(new { data = formatedOrders });
        }
        public async Task<IActionResult> Cancel(string orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetAsync(i => i.OrderId == Guid.Parse(orderId));
                var picker = await _unitOfWork.Workers.GetAsync(w => w.UserId == Guid.Parse(_userManager.GetUserId(User)));
                order.OrderStatus = StatusByOrder.StatusCancelled;
                order.RefundAmount = order.PurchaseAmount;

                Notification notificationAboutCancellation = new()
                {
                    OrderId = order.OrderId,
                    RecipientId = Guid.Parse("604c075d-c691-49d6-9d6f-877cfa866e59"),
                    SenderId = picker.WorkerId,
                    SendingTime = MoscowTime.GetTime(),
                    TypeNotification = NotificationSD.Refund,
                    Text = $"Необходимо осуществить возврат средств в сумме {order.RefundAmount} за заказ под номером: {order.OrderId}."
                };

                await _unitOfWork.Notifications.AddAsync(notificationAboutCancellation);
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
