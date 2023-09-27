using Bookstore.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Areas.API
{
    [Area("API")]
    public class APIController : Controller
    {
        private readonly ApplicationDbContext _db;

        public APIController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> GetOrderPickupPoint()
        {
            var orderPickupPoint = await _db.OrderPickupPoint.ToListAsync();
            return Json(new { data = orderPickupPoint });
        }
    }
}
