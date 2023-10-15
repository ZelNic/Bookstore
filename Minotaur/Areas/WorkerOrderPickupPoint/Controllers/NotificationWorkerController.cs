using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Minotaur.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Minotaur.Areas.WorkerOrderPickupPoint
{
    [Area("WorkerOrderPickupPoint"), Authorize(Roles = Roles.Role_Worker_Order_Pickup_Point)]
    public class NotificationWorkerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<MinotaurUser> _userManager;

        public NotificationWorkerController(ApplicationDbContext db, UserManager<MinotaurUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);


            IEnumerable<Notification> notifications = await _db.Notifications.Where(u => u.RecipientId == Guid.Parse(user.Id)).Where(s => s.IsHidden == false).ToListAsync();

            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> Send(int notificationCode, string orderId)
        {
            string text = "";
            Order? order = await _db.Orders.Where(u => u.OrderId == Guid.Parse(orderId)).FirstOrDefaultAsync();

            MinotaurUser? minotaurUser = await _userManager.GetUserAsync(User);

            if (order != null)
            {
                switch (notificationCode)
                {
                    case 0:
                        text = NotificationSD.TechnicalProblems_0;
                        break;
                    case 1:
                        text = NotificationSD.PaymentNotPassed_1;
                        break;
                    case 2:
                        text = NotificationSD.PaymentPassed_2;
                        break;
                    case 3:
                        text = NotificationSD.OrderIsGoing_3;
                        break;
                    case 4:
                        text = NotificationSD.OrderSent_4;
                        break;
                    case 5:
                        text = NotificationSD.OrderArrived_5;
                        break;
                    case 6:
                        text = NotificationSD.CourierDelivery_6;
                        break;
                    case 7:
                        text = NotificationSD.OrderСancelled_7;
                        break;
                    default:
                        NotFound("Неверная команда.");
                        break;
                }

                order.OrderStatus = text;

                Notification notification = new()
                {
                    SenderId = Guid.Parse(minotaurUser.Id),
                    RecipientId = order.UserId,
                    OrderId = order.OrderId,
                    Text = text,
                    IsHidden = false,
                    SendingTime = MoscowTime.GetTime(),
                };

                _db.Orders.Update(order);
                await _db.Notifications.AddAsync(notification);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
