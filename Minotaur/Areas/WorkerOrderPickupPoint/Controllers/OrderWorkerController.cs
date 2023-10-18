using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minotaur.DataAccess;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Minotaur.Models.ViewModel;
using Minotaur.Utility;

namespace Minotaur.Areas.WorkerOrderPickupPoint
{
    [Area("WorkerOrderPickupPoint"), Authorize(Roles = Roles.Role_Worker_Order_Pickup_Point)]
    public class OrderWorkerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;

        public OrderWorkerController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
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
                Order = await _unitOfWork.Orders.GetAsync(u => u.OrderId == Guid.Parse(searchOrderId)),
                OperationName = operation
            };

            if (orderVM.Order == null)
            {
                return BadRequest("Заказ не найден");
            }

            return View(orderVM);
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
                _unitOfWork.SaveAsync();
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
            _unitOfWork.SaveAsync();
            return Ok();
        }

        public async Task<IActionResult> CheckVerificationCode(string orderId, int confirmationCode)
        {
            Order? order = await _unitOfWork.Orders.GetAsync(o=>o.OrderId == Guid.Parse(orderId));

            if (order == null)
            {
                return BadRequest("Заказ не найден");
            }

            if (order.ConfirmationCode == confirmationCode)
            {
                order.ConfirmationCode = 0;
                order.OrderStatus = StatusByOrder.StatusDelivered_4;
                _unitOfWork.Orders.Update(order);
                _unitOfWork.SaveAsync();
                return Ok();
            }
            else
            {
                return BadRequest("Код подтверждения неверный");
            }
        }
    }
}
