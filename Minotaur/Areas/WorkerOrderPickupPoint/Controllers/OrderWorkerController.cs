using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Minotaur.Models.ViewModel;
using Minotaur.Utility;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Minotaur.Areas.WorkerOrderPickupPoint
{
    [Area("WorkerOrderPickupPoint"), Authorize(Roles = Roles.Role_Worker_Order_Pickup_Point)]
    public class OrderWorkerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<MinotaurUser> _userManager;

        public OrderWorkerController(ApplicationDbContext db, UserManager<MinotaurUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Order(string operation)
        {
            if (operation == null)
            {
                return NotFound("Такой операции нет.");
            }

            switch (operation)
            {
                case OperationOrderPickUp.ViewOrderDetails:
                    ViewBag.operation = OperationOrderPickUp.ViewOrderDetails;
                    break;
                case OperationOrderPickUp.AcceptOrder:
                    ViewBag.operation = OperationOrderPickUp.AcceptOrder;
                    break;
                case OperationOrderPickUp.IssueOrder:
                    ViewBag.operation = OperationOrderPickUp.IssueOrder;
                    break;
                default:
                    return NotFound(operation);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Order(string searchOrderId, string operation)
        {
            OrderVM orderVM = new()
            {
                Order = await _db.Orders.Where(u => u.OrderId == Guid.Parse(searchOrderId)).FirstOrDefaultAsync(),
                OperationName = operation
            };

            if (orderVM.Order == null)
            {
                return BadRequest("Заказ не найден");
            }

            return View(orderVM);
        }


        public IActionResult Deliver(int orderId, int? statusCode)
        {
            Order? order = _db.Orders.Find(orderId);
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
                _db.Orders.Update(order);
                _db.SaveChanges();
            }
            return RedirectToAction("Index", "Orders", new { area = "Customer" });
        }


        [HttpPost]
        public async Task<IActionResult> SendConfirmationCode(string orderId)
        {
            var order = _db.Orders.Find(orderId);

            if (order == null)
            {
                return BadRequest("Заказ не найден");
            }

            Random random = new();
            int confirmationСode = random.Next(100000, 999999);

            // +++
            var user = await _userManager.GetUserAsync(User);
            Worker? workerOrderPUp = await _db.Workers.FirstOrDefaultAsync(w => w.UserId == user.Id);


            Notification notification = new()
            {
                OrderId = Guid.Parse(orderId),
                RecipientId = workerOrderPUp.WorkerId,
                SenderId = order.UserId,
                SendingTime = MoscowTime.GetTime(),
                Text = NotificationSD.IssueCode + ' ' + confirmationСode + ' ' + "Скажите его оператору для выдачи заказа."
            };

            order.ConfirmationCode = confirmationСode;

            _db.Orders.Update(order);
            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> CheckVerificationCode(string orderId, int confirmationCode)
        {
            Order? order = await _db.Orders.FindAsync(orderId);

            if (order == null)
            {
                return BadRequest("Заказ не найден");
            }

            if (order.ConfirmationCode == confirmationCode)
            {
                order.ConfirmationCode = 0;
                order.OrderStatus = StatusByOrder.StatusDelivered_4;
                _db.Orders.Update(order);
                _db.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest("Код подтверждения неверный");
            }
        }
    }
}
