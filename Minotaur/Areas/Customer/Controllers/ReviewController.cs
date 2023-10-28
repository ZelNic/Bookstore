using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models.ModelReview;
using Newtonsoft.Json;

namespace Minotaur.Areas.Customer.Controllers
{
    [Area("Custmomer"), Authorize]

    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;


        public ReviewController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewsOnProduct(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetAllAsync(r => r.ProductId == productId);

            return Json(reviews);
        }

        public async Task<IActionResult> PostReview(string review)
        {
            Review? r = JsonConvert.DeserializeObject<Review>(review);





            return Ok();
        }

    }
}
