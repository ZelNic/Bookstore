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



        public async Task<IActionResult> GetDataByOrder(string id)
        {
            Order? order = await _unitOfWork.Orders.GetAsync(u => u.OrderId == Guid.Parse(id));

            var requiredData = new
            {
                id = order.OrderId,
                time = order.PurchaseDate.ToString("dd.MM.yyyy"),
                status = order.OrderStatus,
                product = JsonConvert.DeserializeObject<OrderProductData>(order.ShippedProducts)
            };



            return Json(new { data = requiredData });
        }


        public async Task<IActionResult> Deliver(string orderId, int? statusCode)
        {
            Order? order = await _unitOfWork.Orders.GetAsync(o => o.OrderId == Guid.Parse(orderId));
            if (order != null)
            {
                switch (statusCode)
                {
                    case 0:
                        order.OrderStatus = StatusByOrder.StatusPending_0;
                        break;
                    case 1:
                        order.OrderStatus = StatusByOrder.StatusApproved_1;
                        break;
                    case 2:
                        order.OrderStatus = StatusByOrder.StatusInProcess_2;
                        break;
                    case 3:
                        order.OrderStatus = StatusByOrder.StatusShipped_3;
                        break;
                    case 4:
                        order.OrderStatus = StatusByOrder.StatusDelivered_4;
                        break;
                    case 5:
                        order.OrderStatus = StatusByOrder.StatusCancelled_5;
                        break;
                    case 6:
                        order.OrderStatus = StatusByOrder.StatusRefunded_6;
                        break;
                }
                _unitOfWork.Orders.Update(order);
                _unitOfWork.Save();
            }
            return RedirectToAction("Index", "Orders", new { area = "Customer" });
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

            // +++
            var user = await _userManager.GetUserAsync(User);
            Worker? workerOrderPUp = await _unitOfWork.Workers.GetAsync(w => w.UserId == Guid.Parse(user.Id));


            Notification notification = new()
            {
                OrderId = Guid.Parse(orderId),
                RecipientId = workerOrderPUp.WorkerId,
                SenderId = order.UserId,
                SendingTime = MoscowTime.GetTime(),
                Text = NotificationSD.IssueCode + ' ' + confirmationСode + ' ' + "Скажите его оператору для выдачи заказа."
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
                order.OrderStatus = StatusByOrder.StatusDelivered_4;
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
