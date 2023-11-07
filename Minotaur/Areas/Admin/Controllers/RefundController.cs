using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.Areas.Accounting.Controllers;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Minotaur.Utility;

namespace Minotaur.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = $"{Roles.Role_Operator}, {Roles.Role_Admin}")]
    public class RefundController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;

        public RefundController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<ActionResult> MakeRefund(string notificationId)
        {
            try
            {
                Notification? notification = await _unitOfWork.Notifications.GetAsync(n => n.Id == Guid.Parse(notificationId));
                Order? refundOrder = await _unitOfWork.Orders.GetAsync(o => o.OrderId == notification.OrderId);

                MinotaurUser? admin = await _userManager.GetUserAsync(User);
                MinotaurUser? returnRecipient = await _unitOfWork.MinotaurUsers.GetAsync(u => u.Id == refundOrder.UserId.ToString());

                AccountingEntry entry = new()
                {
                    OrderId = notification.OrderId,
                    Sum = -refundOrder.RefundAmount,
                    Info = $"Возврат средст за заказ: {notification.OrderId} в сумме {refundOrder.RefundAmount}",
                    RecordingTime = MoscowTime.GetTime(),
                };

                Notification notificationRefund = new()
                {
                    OrderId = notification.OrderId,
                    SenderId = Guid.Parse(admin.Id),
                    RecipientId = notification.RecipientId,
                    Text = $"Возврат средст за заказ: {notification.OrderId} в сумме {refundOrder.RefundAmount} осуществлен",
                    TypeNotification = NotificationSD.SimpleNotification
                };

                if (admin.PersonalWallet > refundOrder.RefundAmount)
                {
                    admin.PersonalWallet -= refundOrder.RefundAmount;
                    returnRecipient.PersonalWallet += refundOrder.RefundAmount;
                    refundOrder.RefundAmount = 0;
                    refundOrder.OrderStatus = StatusByOrder.Refunded;
                }
                else
                {
                    return BadRequest("Нехватка средств для осуществления возврата");
                }

                await _unitOfWork.AccountingForOrders.AddAsync(entry);
                await _unitOfWork.Notifications.AddAsync(notificationRefund);
                _unitOfWork.Notifications.Remove(notification);

                //only during development
                if (admin.UserName == returnRecipient.UserName)
                {
                    _unitOfWork.MinotaurUsers.Update(admin);
                }
                else
                {
                    _unitOfWork.MinotaurUsers.UpdateRange(new[] { admin, returnRecipient });
                }

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
