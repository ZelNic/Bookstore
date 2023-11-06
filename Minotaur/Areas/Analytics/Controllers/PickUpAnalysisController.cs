using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.SD;

namespace Minotaur.Areas.Analytics.Controllers
{
    [Area("Analytics"), Authorize(Roles = $"{Roles.Role_Analyst}, {Roles.Role_Admin}")]
    public class PickUpAnalysisController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;
        public PickUpAnalysisController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
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
            var pickUpData = (await _unitOfWork.PickUpReviews.GetAllAsync())
                .Join(_unitOfWork.Orders.GetAll(o => o.IsCourierDelivery == false), p => p.OrderId, o => o.OrderId, (p, o) => new
            {
                p.OrderId,
                p.PickUpRating,
                p.PickUpReviewText,
                NamePickUp = _unitOfWork.Offices.GetAsync(n => n.Id == o.OrderPickupPointId),
            });

            return Json(new { data = pickUpData });
        }
    }
}
