using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Minotaur.Utility;

namespace Minotaur.Areas.Customer
{
    [Area("Customer")]
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;
        public NotificationController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetDataNotification()
        {
            MinotaurUser? user = await _userManager.GetUserAsync(User);
            var notHiddenNotifications = (await _unitOfWork.Notifications.GetAllAsync(u => u.RecipientId == Guid.Parse(user.Id)))
                .OrderByDescending(n => n.SendingTime)
                .Select(n => new
                {
                    n.Id,
                    n.OrderId,
                    SendingTime = n.SendingTime.ToString("dd.MM.yyyy HH:mm"),
                    n.Text,
                    n.TypeNotification,
                });

            if (notHiddenNotifications == null)
            {
                return BadRequest("Уведомлений нет");
            }

            return Json(new { data = notHiddenNotifications });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveNotification(string notificationId)
        {
            Notification? notification = await _unitOfWork.Notifications.GetAsync(n => n.Id == Guid.Parse(notificationId));
            if (notification == null) { return BadRequest("Уведомлений нет"); }

            _unitOfWork.Notifications.Remove(notification);
            await _unitOfWork.SaveAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAllNotifications()
        {
            var user = await _userManager.GetUserAsync(User);
            var notifications = await _unitOfWork.Notifications.GetAllAsync(u => u.RecipientId == Guid.Parse(user.Id));

            if (notifications == null) { return BadRequest("Уведомлений нет"); }

            _unitOfWork.Notifications.RemoveRange(notifications.ToArray());
            await _unitOfWork.SaveAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SendReplyIncompleteOrder(string notificationId, bool isAgree)
        {
            Notification? notification = await _unitOfWork.Notifications.GetAsync(n => n.Id == Guid.Parse(notificationId));
            if (notification == null) { return BadRequest("Уведомление не найдено"); }

            Order? order = await _unitOfWork.Orders.GetAsync(o => o.OrderId == notification.OrderId);
            if (order == null)
            {
                _unitOfWork.Notifications.Remove(notification);
                await _unitOfWork.SaveAsync();
                return BadRequest("Заказ не найден");
            }

            Notification notificationForCustomer = new()
            {
                OrderId = order.OrderId,
                RecipientId = order.UserId,
                TypeNotification = NotificationSD.SimpleNotification,
                SendingTime = MoscowTime.GetTime(),
            };

            if (isAgree)
            {
                order.OrderStatus = StatusByOrder.BuyerAgreesNeedSend;
                notificationForCustomer.Text = "Ожидается отправка товара";
            }
            else
            {
                order.OrderStatus = StatusByOrder.BuyerDontAgreesNeedRefunded;
                notificationForCustomer.Text = "Ожидается возврат средств";

                Notification notificationForPicker = new()
                {
                    OrderId = order.OrderId,
                    SenderId = order.UserId,
                    RecipientId = order.AssemblyResponsibleWorkerId,
                    TypeNotification = NotificationSD.SimpleNotification,
                    Text = $"Покупатель отказался от заказа {order.OrderId}.",
                    SendingTime = MoscowTime.GetTime(),
                };

                await _unitOfWork.Notifications.AddAsync(notificationForPicker);
            }

            _unitOfWork.Notifications.Update(notification);
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.Notifications.AddAsync(notificationForCustomer);
            await _unitOfWork.SaveAsync();

            return Ok();
        }
    }
}
