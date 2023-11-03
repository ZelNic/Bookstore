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
        #region ProductReview
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetReviewsOnProduct(int productId)
        {
            var reviews = (await _unitOfWork.ProductReviews.GetAllAsync(r => r.ProductId == productId && r.IsShowReview == true))
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
                   CountLike = r.IdWhoLiked == null ? 0 : JsonConvert.DeserializeObject<Guid[]>(r.IdWhoLiked.ToString()).Count(),
                   CountDislike = r.IdWhoDisiked == null ? 0 : JsonConvert.DeserializeObject<Guid[]>(r.IdWhoDisiked.ToString()).Count(),
                   Photo = r.FilePaths == null ? null : JsonConvert.DeserializeObject<string[]>(r.FilePaths),
               });
            return Json(new { data = reviews });
        }
        [HttpGet]
        public async Task<IActionResult> GetRatingReviewProduct(string id)
        {
            ProductReview? review = await _unitOfWork.ProductReviews.GetAsync(r => r.Id == Guid.Parse(id));
            if (review == null) return BadRequest("Отзыв не найден");

            int countLike = review.IdWhoLiked == null ? 0 : JsonConvert.DeserializeObject<Guid[]>(review.IdWhoLiked.ToString()).Count();
            int countDislike = review.IdWhoDisiked == null ? 0 : JsonConvert.DeserializeObject<Guid[]>(review.IdWhoDisiked.ToString()).Count();

            return Json(new { data = new { CountLike = countLike, CountDislike = countDislike } });
        }
        [HttpGet]
        public async Task<IActionResult> CheckForRefeedback(string orderId, int productId)
        {
            ProductReview? review = await _unitOfWork.ProductReviews.GetAsync(r => r.OrderId == Guid.Parse(orderId) && r.ProductId == productId);
            if (review == null) return Ok();
            else
            {
                if (review.IsRejected == true || review.IsShowReview == false)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string productPath = @"fileStorage\productReviewFiles\";
                    var filePaths = JsonConvert.DeserializeObject<List<string>>(review.FilePaths);

                    foreach (var filePath in filePaths)
                    {
                        string fullPath = Path.Combine(wwwRootPath, productPath, filePath);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }

                    _unitOfWork.ProductReviews.Remove(review);
                    await _unitOfWork.SaveAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Отзыв по данному заказу на данный товар уже оставлен");
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostReview(BaseProductReviews productReviews)
        {
            List<string> filePaths = new List<string>();
            if (productReviews.Photos.Count >= 10) { productReviews.Photos.RemoveRange(9, productReviews.Photos.Count - 10); }

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
        [HttpPost]
        public async Task<IActionResult> RateReview(string reviewId, bool isLiked)
        {
            var user = await _userManager.GetUserAsync(User);
            var review = await _unitOfWork.ProductReviews.GetAsync(r => r.Id == Guid.Parse(reviewId));
            if (review == null) return BadRequest("Отзыв не найден");

            List<Guid> liked = review.IdWhoLiked == null ? new List<Guid> { } : JsonConvert.DeserializeObject<List<Guid>>(review.IdWhoLiked.ToString());
            List<Guid> disliked = review.IdWhoDisiked == null ? new List<Guid> { } : JsonConvert.DeserializeObject<List<Guid>>(review.IdWhoLiked.ToString());

            bool isChangeLiked = false;
            bool isChangeDisliked = false;

            if (isLiked)
            {
                if ((!liked.Contains(Guid.Parse(user.Id))) && (!disliked.Contains(Guid.Parse(user.Id))))
                {
                    liked.Add(Guid.Parse(user.Id));
                    isChangeLiked = true;
                }
                else if ((!liked.Contains(Guid.Parse(user.Id)) && (disliked.Contains(Guid.Parse(user.Id)))))
                {
                    disliked.Remove(Guid.Parse(user.Id));
                    liked.Add(Guid.Parse(user.Id));
                    isChangeLiked = true;
                    isChangeDisliked = true;
                }
            }
            else
            {
                if ((!disliked.Contains(Guid.Parse(user.Id))) && (!liked.Contains(Guid.Parse(user.Id))))
                {
                    disliked.Add(Guid.Parse(user.Id));
                    isChangeDisliked = true;
                }
                else if ((!disliked.Contains(Guid.Parse(user.Id))) && (liked.Contains(Guid.Parse(user.Id))))
                {
                    liked.Remove(Guid.Parse(user.Id));
                    disliked.Add(Guid.Parse(user.Id));
                    isChangeLiked = true;
                    isChangeDisliked = true;
                }
            }

            string changedList = "";

            if (isChangeLiked == true || isChangeDisliked == true)
            {
                if (isChangeLiked == true)
                {
                    changedList = JsonConvert.SerializeObject(liked);
                    review.IdWhoLiked = changedList;
                }
                if (isChangeDisliked == true)
                {
                    changedList = JsonConvert.SerializeObject(disliked);
                    review.IdWhoDisiked = changedList;
                }
            }
            else
            {
                return BadRequest("Вы уже оценили отзыв");
            }

            _unitOfWork.ProductReviews.Update(review);
            await _unitOfWork.SaveAsync();

            return Ok();
        }
        #endregion

        #region OrderReview
        [HttpPost]
        public async Task<IActionResult> PostReviewOrder(BaseOrderReviews orderReview)
        {
            var order = await _unitOfWork.Orders.GetAsync(o => o.OrderId == orderReview.OrderId);
            if (order == null) { return BadRequest("Заказ не найден"); }

            try
            {
                DeliveryReview deliveryReview = new()
                {
                    OrderId = orderReview.OrderId,
                    Rating = orderReview.DeliveryRating,
                    DeliveryReviewText = orderReview.DeliveryReviewText,
                };

                PickUpReview pickUpReview = new()
                {
                    OrderId = orderReview.OrderId,
                    PickUpRating = orderReview.PickUpRating,
                    PickUpReviewText = orderReview.PickUpReviewText,
                };

                WorkerReview workerReview = new()
                {
                    OrderId = orderReview.OrderId,
                    WorkerId = order.AssemblyResponsibleWorkerId,
                    Rating = orderReview.WorkerRating,
                    WorkerReviewText = orderReview.WorkerReviewText,
                };

                await _unitOfWork.DeliveryReviews.AddAsync(deliveryReview);
                await _unitOfWork.PickUpReviews.AddAsync(pickUpReview);
                await _unitOfWork.WorkerReviews.AddAsync(workerReview);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }


        public async Task<IActionResult> CheckForRefeedbackOrder(string orderId)
        {
            if (await _unitOfWork.DeliveryReviews.GetAsync(dr => dr.OrderId == Guid.Parse(orderId)) != null)
            {
                return BadRequest("Заказ уже оценен");
            }
            else { return Ok(); }
        }
        #endregion
    }
}


// IDEA: создать систему рекомендаций на основание купленных книг и сопоставлением их по категориям.