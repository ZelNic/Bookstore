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
    [Area("Customer"), Authorize]

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
            var reviews = (await _unitOfWork.ProductReviews.GetAllAsync(r => r.IsShowReview == true))
               .Join<ProductReview, MinotaurUser, Guid, dynamic>((await _unitOfWork.MinotaurUsers.GetAllAsync()), r => r.UserId, u => Guid.Parse(u.Id), (r, u) => new
               {
                   NameUser = r.IsAnonymous == true ? "Пользователь" : $"{u.FirstName} {u.LastName}",
                   r.Id,
                   r.ProductId,
                   r.Rating,
                   r.PurchaseDate,
                   r.ProductReviewText,
                   r.OrderId,
                   r.UserId,
                   r.IsShowReview,
                   r.IsRejected,
                   CountLike = r.IdWhoLiked == null ? 0 : DeserializationUserId(r.IdWhoLiked.ToString()).Count(),
                   CountDislike = r.IdWhoDisiked == null ? 0 : DeserializationUserId(r.IdWhoDisiked.ToString()).Count(),
                   Photo = r.FilePaths == null ? null : JsonConvert.DeserializeObject<string[]>(r.FilePaths),
               });


            if (reviews == null) { return BadRequest("Отзывов на модерацию нет"); }

            return Json(new { data = reviews });
        }

        public IActionResult Index()
        {
            return View();
        }
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
            }


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

            return Ok();
        }

        // IDEA: создать систему рекомендаций на основание купленных книг и сопоставлением их по категориям.

        private List<Guid> DeserializationUserId(string ids)
        {
            List<Guid>? result = new List<Guid>();
            try
            {
                result = JsonConvert.DeserializeObject<List<Guid>>(ids);
            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }
        private string Serialization(List<Guid> ids)
        {
            string json = JsonConvert.SerializeObject(ids);

            return json;
        }

        [HttpPost]
        public async Task<IActionResult> RateReview(string reviewId, bool isLiked)
        {
            var user = await _userManager.GetUserAsync(User);
            var review = await _unitOfWork.ProductReviews.GetAsync(r => r.Id == Guid.Parse(reviewId));
            if (review == null) return BadRequest("Отзыв не найден");

            var liked = DeserializationUserId(review.IdWhoLiked.ToString());
            var disliked = DeserializationUserId(review.IdWhoDisiked.ToString());

            bool isAddLiked = false;
            bool isDisliked = false;

            if (isLiked)
            {
                if ((!liked.Contains(Guid.Parse(user.Id))) && (!disliked.Contains(Guid.Parse(user.Id))))
                {
                    liked.Add(Guid.Parse(user.Id));
                    isAddLiked = true;
                }
                else if ((!liked.Contains(Guid.Parse(user.Id))) && (disliked.Contains(Guid.Parse(user.Id))))
                {
                    disliked.Remove(Guid.Parse(user.Id));
                    liked.Add(Guid.Parse(user.Id));
                    isAddLiked = true;
                }
            }
            else
            {
                if ((!disliked.Contains(Guid.Parse(user.Id))) && (!liked.Contains(Guid.Parse(user.Id))))
                {
                    disliked.Add(Guid.Parse(user.Id));
                    isDisliked = true;
                }
                else if ((!disliked.Contains(Guid.Parse(user.Id))) && (liked.Contains(Guid.Parse(user.Id))))
                {
                    liked.Remove(Guid.Parse(user.Id));
                    disliked.Add(Guid.Parse(user.Id));
                    isDisliked = true;
                }
            }

            string changedList = "";

            if (isAddLiked == true)
            {
                changedList = Serialization(liked);
                review.IdWhoLiked = changedList;
            }
            else if (isDisliked == true)
            {
                changedList = Serialization(disliked);
                review.IdWhoDisiked = changedList;
            }
            else
            {
                return BadRequest("Отзыв уже оценен");
            }

            _unitOfWork.ProductReviews.Update(review);
            await _unitOfWork.SaveAsync(); 

            return Ok();
        }
    }
}

//TODO: сделать так, что повторно отзыв нельзя было оставить на один и тот же продукт в ОДНОМ заказе