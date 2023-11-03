using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.SD;

namespace Minotaur.Areas.Analytics.Controllers
{
    [Area("Analytics"), Authorize(Roles = $"{Roles.Role_Analyst}, {Roles.Role_Admin}")]
    public class WorkerAnalysisController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;

        public WorkerAnalysisController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
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
            var workerData = (await _unitOfWork.WorkerReviews.GetAllAsync()).Join(_unitOfWork.Workers.GetAll(), wr => wr.WorkerId, w => w.WorkerId, (wr, w) => new
            {
                w.WorkerId,
                w.OfficeId,
                w.OfficeName,
                w.Post,
                wr.Rating,
                wr.WorkerReviewText
            });

            return Json(new { data = workerData });
        }
    }
}
