using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models.ModelReview;
using Minotaur.Utility;
using Newtonsoft.Json;

namespace Minotaur.Areas.Customer.Controllers
{
    [Area("Custmomer"), Authorize]

    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ReviewController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewsOnProduct(int productId)
        {
            var reviews = await _unitOfWork.ProductReviews.GetAllAsync(r => r.ProductId == productId);

            return Json(reviews);
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> PostReview(BaseProductReviews productReviews)
        {
            List<string> filePaths = new List<string>();

            if (productReviews.Photos != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                Parallel.ForEach(productReviews.Photos, photo =>
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                    string productPath = @"fileStorage\productReviewFiles\";
                    string finalPath = Path.Combine(wwwRootPath, productPath);

                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);


                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        photo.CopyTo(fileStream);
                        filePaths.Add(fileName);
                    }
                });

                ProductReview productReview = new()
                {
                    OrderId = productReviews.OrderId,
                    ProductId = productReviews.ProductId,
                    UserId = productReviews.UserId,
                    FilePaths = JsonConvert.SerializeObject(filePaths),
                    Rating = productReviews.Rating,
                    ProductReviewText = productReviews.ProductReviewText,
                    PurchaseDate = MoscowTime.GetTime(),
                    IsShowReview = false,
                    IsAnonymous = productReviews.IsAnonymous,
                };



                await _unitOfWork.ProductReviews.AddAsync(productReview);
                await _unitOfWork.SaveAsync();
            }

            return Ok();
        }

    }
}

//TODO: сделать так, что повторно отзыв нельзя было оставить на один и тот же продукт в ОДНОМ заказе