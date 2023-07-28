using Bookstore.DataAccess;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace Bookstore.Areas.Identity
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
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
            if(user.Password == password)
            {
                return RedirectToAction("Index", "Home");
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

            HttpContext.Session.SetString("Username", newUser.FirstName);

            return LocalRedirect("~/Customer/Views/Home/Index");
        }

    }
}
