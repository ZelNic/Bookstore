using Bookstore.DataAccess;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.Identity
{
    [Area("Identity")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult LogIn()
        {
            User? user = null;
            return View(user);
        }

        [HttpPost]
        public IActionResult LogIn(string email, string password)
        {
            User? user = _db.User.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }
            if (user.Password == password)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("Username", user.UserId);
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
            else
            {
                return NotFound("Пользователь не найден.");
            }
        }

        [HttpGet]
        public IActionResult Registration()
        {
            User? user = null;

            return View(user);
        }

        [HttpPost]
        public IActionResult Registration(User newUser)
        {
            _db.User.Add(newUser);
            _db.SaveChanges();

            _httpContextAccessor.HttpContext.Session.SetInt32("Username", newUser.UserId);

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        [HttpGet]
        public IActionResult Profile()
        {
            int? id = _httpContextAccessor.HttpContext.Session.GetInt32("Username");
            if (id != null)
            {
                User user = _db.User.FirstOrDefault(u => u.UserId == id);
                return View(user);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult Profile(User user)
        {
            User oldVerUser = _db.User.FirstOrDefault(u => u.UserId == user.UserId);

            if (oldVerUser != null)
            {
                oldVerUser.FirstName = user.FirstName;
                oldVerUser.LastName = user.LastName;
                oldVerUser.Email = user.Email;
                oldVerUser.PhoneNumber = user.PhoneNumber;
                oldVerUser.DateofBirth = user.DateofBirth;

                _db.User.Update(oldVerUser);
                _db.SaveChanges();
            }

            return RedirectToAction("Profile");
        }

    }
}
