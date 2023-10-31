using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.Models.ModelReview;
using Minotaur.Models.SD;
using Minotaur.Utility;
using Newtonsoft.Json;

namespace Minotaur.Areas.Operator.Controllers
{
    [Area("Operator"), Authorize(Roles = $"{Roles.Role_Operator}, {Roles.Role_Admin}")]
    public class ReviewsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;

        public ReviewsController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetReviewsForModeration()
        {
            var reviews = (await _unitOfWork.ProductReviews.GetAllAsync(r => r.IsShowReview == false && r.IsRejected == false))
                .Join<ProductReview, MinotaurUser, Guid, dynamic>(_unitOfWork.MinotaurUsers.GetAll(), r => r.UserId, u => Guid.Parse(u.Id), (r, u) => new
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
                    Photo = r.FilePaths == null ? null : JsonConvert.DeserializeObject<string[]>(r.FilePaths),
                });

            if (reviews.IsNullOrEmpty()) { return BadRequest("Отзывов на модерацию нет"); }

            return Json(new { data = reviews });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptReview(string id)
        {
            try
            {
                var review = await _unitOfWork.ProductReviews.GetAsync(r => r.Id == Guid.Parse(id));
                review.IsShowReview = true;

                Notification notificationForUser = new()
                {
                    OrderId = review.OrderId,
                    SenderId = Guid.Parse((await _userManager.GetUserAsync(User)).Id),
                    RecipientId = review.UserId,
                    SendingTime = MoscowTime.GetTime(),
                    Text = $"Был опубликован отзыв.",
                    TypeNotification = NotificationSD.SimpleNotification
                };
                _unitOfWork.ProductReviews.Update(review);
                await _unitOfWork.Notifications.AddAsync(notificationForUser);
                await _unitOfWork.SaveAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectReview(string id, string? comment = null)
        {
            try
            {
                var review = await _unitOfWork.ProductReviews.GetAsync(r => r.Id == Guid.Parse(id));
                review.IsRejected = true;

                // IDEA: Создать контроллер для уведомлвений и создать в нем методы на все ситуации

                Notification notificationForUser = new()
                {
                    OrderId = review.OrderId,
                    SenderId = Guid.Parse((await _userManager.GetUserAsync(User)).Id),
                    RecipientId = review.UserId,
                    SendingTime = MoscowTime.GetTime(),
                    Text = comment == null ? "Ваш отзыв был отклонен." : $"Ваш отзыв был отклонен по причине:{comment}.",
                    TypeNotification = NotificationSD.SimpleNotification
                };

                _unitOfWork.ProductReviews.Update(review);
                await _unitOfWork.Notifications.AddAsync(notificationForUser);
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


// TODO: переписать добавление роли и назначение, сделать более гибким