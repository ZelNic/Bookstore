using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.SD;
using Newtonsoft.Json;

namespace Minotaur.Areas.Analytics.Controllers
{
    [Area("Analytics"), Authorize(Roles = $"{Roles.Role_Analyst}, {Roles.Role_Admin}")]
    public class DeliveryAnalysisController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;
        public DeliveryAnalysisController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var deliveryData = (await _unitOfWork.DeliveryReviews.GetAllAsync())
                .Join(_unitOfWork.OrderMovementHistory.GetAll(), r => r.OrderId, o => o.OrderId, (r, o) => new
                {
                    r.OrderId,
                    r.Rating,
                    o.DispatchTime,
                    o.TimeOfReceiving,
                    HistoryMovement = JsonConvert.DeserializeObject<List<string>>(o.HistoryOfСonversion),
                    r.DeliveryReviewText,
                });

            return Json(new { data = deliveryData });
        }
    }
}
