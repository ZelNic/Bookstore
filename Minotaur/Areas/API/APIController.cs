using Minotaur.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minotaur.Models.Models;

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
            Office[] orderPickupPoint = await _db.Offices.Where(p =>p.Type == "Пункт выдачи").ToArrayAsync();
            return Json(new { data = orderPickupPoint });
        }
    }
}
