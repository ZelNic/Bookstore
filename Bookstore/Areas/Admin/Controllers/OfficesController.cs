using Bookstore.DataAccess;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.Admin
{
    public class OfficesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OfficesController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
        {
            _db = db;                          
        }

        public async static Task<IActionResult> GetOffices()
        {


            return Json(new { data = "@" });
        }
    }
}
