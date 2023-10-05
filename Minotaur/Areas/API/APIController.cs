using Minotaur.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Minotaur.Areas.API
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
            var orderPickupPoint = await _db.Offices.ToListAsync();
            return Json(new { data = orderPickupPoint });
        }
    }
}
