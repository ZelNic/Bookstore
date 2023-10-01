//using Minotaur.DataAccess;
//using Minotaur.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Minotaur.Areas.Identity
//{
//    [Area("Identity")]
//    public class UserController : Controller
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly IHttpContextAccessor _httpContextAccessor;

//        public UserController(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
//        {
//            _db = db;
//            _httpContextAccessor = httpContextAccessor;
//        }

//        public async Task<IActionResult> LogIn()
//        {
//            User? user = null;
//            return View(user);
//        }

//        [HttpPost]
//        public async Task<IActionResult> LogIn(string email, string password)
//        {
//            User? user = await _db.User.FirstOrDefaultAsync(u => u.Email == email);
//            if (user == null)
//            {
//                return NotFound("Пользователь не найден.");
//            }
//            if (user.Password == password)
//            {
//                _httpContextAccessor.HttpContext.Session.SetString("Id", user.Id);
//                return RedirectToAction("Index", "Home", new { area = "Customer" });
//            }
//            else
//            {
//                return NotFound("Пользователь не найден.");
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> GoOut()
//        {
//            _httpContextAccessor.HttpContext.Session.Remove("Id");
//            return RedirectToAction("LogIn");
//        }

//        [HttpGet]
//        public async Task<IActionResult> Registration()
//        {
//            User? user = null;

//            return View(user);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Registration(User newUser)
//        {
//            await _db.User.AddAsync(newUser);
//            await _db.SaveChangesAsync();

//            _httpContextAccessor.HttpContext.Session.SetString("Id", newUser.Id);

//            return RedirectToAction("Index", "Home", new { area = "Customer" });
//        }

//        [HttpGet]
//        public async Task<IActionResult> Profile()
//        {
//            int? id = _httpContextAccessor.HttpContext.Session.GetInt32("Id");
//            if (id != null)
//            {
//                //User user = await _db.User.FirstOrDefaultAsync(u => u.Id == id);
//                //return View(user);
//            }
//            else
//            {
//                return NotFound();
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> Profile(User user)
//        {
//            User oldVerUser = await _db.User.FirstOrDefaultAsync(u => u.Id == user.Id);

//            if (oldVerUser != null)
//            {
//                oldVerUser.FirstName = user.FirstName;
//                oldVerUser.LastName = user.LastName;
//                oldVerUser.Email = user.Email;
//                oldVerUser.PhoneNumber = user.PhoneNumber;
//                oldVerUser.DateofBirth = user.DateofBirth;

//                _db.User.Update(oldVerUser);
//                await _db.SaveChangesAsync();
//            }

//            return RedirectToAction("Profile");
//        }

//    }
//}
