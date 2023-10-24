using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;

namespace Minotaur.Areas.Customer
{
    [Area("Customer")]
    [Authorize]
    public class WishListController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;
        public WishListController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> GetWishList()
        {
            MinotaurUser user = await _userManager.GetUserAsync(User);

            WishList? wishList = await _unitOfWork.WishLists.GetAsync(u => u.UserId == user.Id);
            if (wishList == null) { return BadRequest("Список желаний пуст"); }

            List<int>? listId = wishList.ProductId.Split('|').Select(int.Parse).ToList();

            var product = await _unitOfWork.Products.GetAllAsync(u => listId.Contains(u.ProductId));

            var wishListJson = product.Join(_unitOfWork.Categories.GetAll(), b => b.Category, c => c.Id, (b, c) => new
            {
                image = b.ImageURL,
                nameProduct = b.Name,
                author = b.Author,
                category = c.Name,
                price = b.Price,
                productId = b.ProductId
            }).ToList();

            return Json(new { data = wishListJson });

        }


        [HttpPost]
        public async Task<IActionResult> AddWishList(string newProductId)
        {
            MinotaurUser? user = await _userManager.GetUserAsync(User);
            if (user == null) { return BadRequest("Необходимо войти в профиль"); }

            WishList? wishList = await _unitOfWork.WishLists.GetAsync(u => u.UserId == user.Id);

            List<int> listNewIdProducts = newProductId.Split(',').Select(int.Parse).ToList();

            if (wishList != null)
            {
                List<int> oldProductList = wishList.ProductId.Split('|').Select(int.Parse).ToList();

                for (int i = 0; i < listNewIdProducts.Count; i++)
                {
                    if (oldProductList.Contains(listNewIdProducts[i]))
                    {
                        listNewIdProducts.Remove(oldProductList[i]);
                    }
                    else
                    {
                        wishList.ProductId += "|" + listNewIdProducts[i].ToString();
                    }
                }

                _unitOfWork.WishLists.Update(wishList);
            }
            else
            {
                WishList newProductInWishList = new()
                {
                    ProductId = string.Join("|", listNewIdProducts),
                    UserId = user.Id,
                };

                await _unitOfWork.WishLists.AddAsync(newProductInWishList);
            }

            _unitOfWork.Save();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishList(int productId)
        {
            MinotaurUser user = await _userManager.GetUserAsync(User);

            WishList? wishList = await _unitOfWork.WishLists.GetAsync(i => i.UserId == user.Id);
            if (wishList == null)
                return BadRequest("Список желаемого пуст");

            List<string> listId = wishList.ProductId.Split('|').ToList();
            listId.Remove(productId.ToString());
            wishList.ProductId = string.Join("|", listId);

            if (string.IsNullOrWhiteSpace(wishList.ProductId))
            {
                _unitOfWork.WishLists.Remove(wishList);
            }
            else
            {
                _unitOfWork.WishLists.Update(wishList);
            }

            _unitOfWork.Save();

            return Ok();
        }
    }
}
