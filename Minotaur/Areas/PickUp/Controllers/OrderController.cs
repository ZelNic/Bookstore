using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Minotaur.Utility;
using Newtonsoft.Json;

namespace Minotaur.Areas.PickUp
{
    [Area("PickUp"), Authorize(Roles = Roles.Role_Worker_Order_Pickup_Point)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;

        public OrderController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetDataByOrder(string id)
        {
            Guid? guid = null;
            if (Guid.TryParse(id, out Guid result))
            {
                guid = result;
            }

            if (guid == null) { return BadRequest("Некорректные данные"); }


            Order? order = await _unitOfWork.Orders.GetAsync(u => u.OrderId == guid);
            if (order == null) return BadRequest("Заказ не найден");


            List<OrderProductData> products = JsonConvert.DeserializeObject<List<OrderProductData>>(order.ShippedProducts);
            var requiredData = new
            {
                id = order.OrderId,
                time = order.PurchaseDate,
                status = order.OrderStatus,
                products,
                receiverData = $"{order.ReceiverLastName} {order.ReceiverName}  {order.PhoneNumber}",
                methodDelivery = order.IsCourierDelivery,
            };

            return Json(new { data = requiredData });
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrderArrival(string id)
        {
            Order? order = await _unitOfWork.Orders.GetAsync(o => o.OrderId == Guid.Parse(id));
            if (order == null) return BadRequest("Заказ не найден");

            try
            {
                order.OrderStatus = StatusByOrder.DeliveredToPickUp;

                var user = await _userManager.GetUserAsync(User);
                var workerPickUp = await _unitOfWork.Workers.GetAsync(w => w.UserId == Guid.Parse(user.Id));

                var pickUp = await _unitOfWork.Offices.GetAsync(o => o.Id == workerPickUp.OfficeId);

                Notification notification = new()
                {
                    OrderId = order.OrderId,
                    SenderId = workerPickUp.WorkerId,
                    RecipientId = order.UserId,
                    SendingTime = MoscowTime.GetTime(),
                    Text = $"Заказ прибыл в пункт выдачи {pickUp.Name}. По адресу: {pickUp.City}, {pickUp.Street} {pickUp.BuildingNumber}.",
                    TypeNotification = NotificationSD.SimpleNotification,
                };

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.Notifications.AddAsync(notification);
                _unitOfWork.Save();

                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SendConfirmationCode(string orderId)
        {
            var order = await _unitOfWork.Orders.GetAsync(o => o.OrderId == Guid.Parse(orderId));

            if (order == null)
            {
                return BadRequest("Заказ не найден");
            }

            //IDEI: сделать отправку кода на телеграмм

            Random random = new();
            int confirmationСode = random.Next(100000, 999999);
                        
            var user = await _userManager.GetUserAsync(User);
            Worker? workerOrderPUp = await _unitOfWork.Workers.GetAsync(w => w.UserId == Guid.Parse(user.Id));


            Notification notification = new()
            {
                OrderId = Guid.Parse(orderId),
                RecipientId = workerOrderPUp.WorkerId,
                SenderId = order.UserId,
                SendingTime = MoscowTime.GetTime(),
                Text = $"Код выдачи заказа: {confirmationСode}. Сообщите оператору код выдачи для получения заказа.",
            };

            order.ConfirmationCode = confirmationСode;

            _unitOfWork.Orders.Update(order);
            _unitOfWork.Notifications.AddAsync(notification);
            _unitOfWork.Save();
            return Ok();
        }

        public async Task<IActionResult> CheckVerificationCode(string orderId, int confirmationCode)
        {
            Order? order = await _unitOfWork.Orders.GetAsync(o => o.OrderId == Guid.Parse(orderId));

            if (order == null)
            {
                return BadRequest("Заказ не найден");
            }

            if (order.ConfirmationCode == confirmationCode)
            {
                order.ConfirmationCode = 0;
                order.OrderStatus = StatusByOrder.Delivered;
                _unitOfWork.Orders.Update(order);
                _unitOfWork.Save();
                return Ok();
            }
            else
            {
                return BadRequest("Код подтверждения неверный");
            }
        }
    }
}
