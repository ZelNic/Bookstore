using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;

namespace Minotaur.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = Roles.Role_Admin)]
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
            Notification? notification = await _unitOfWork.Notifications.GetAsync(n => n.Id == Guid.Parse(notificationId));
            Order? refundOrder = await _unitOfWork.Orders.GetAsync(o => o.OrderId == notification.OrderId);

            notification.IsHidden = true;
            //IDEA создать "банк" и переместить всю логику по денежным операциям в него??

            MinotaurUser? admin = await _userManager.GetUserAsync(User);
            MinotaurUser? returnRecipient = await _unitOfWork.MinotaurUsers.GetAsync(u => u.Id == refundOrder.UserId.ToString());

            if (admin.PersonalWallet > refundOrder.RefundAmount)
            {
                admin.PersonalWallet -= refundOrder.RefundAmount;
                returnRecipient.PersonalWallet += refundOrder.RefundAmount;
            }

            _unitOfWork.Notifications.Update(notification);
            _unitOfWork.MinotaurUsers.UpdateRange(new[] { admin, returnRecipient });
            await _unitOfWork.Save();

            return Ok();
        }

    }
}
