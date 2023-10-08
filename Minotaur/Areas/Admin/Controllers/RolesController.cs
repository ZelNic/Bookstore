using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using System.Linq;

namespace Minotaur.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = Roles.Role_Admin)]
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<MinotaurUser> _userManager;

        public RolesController(ApplicationDbContext db, UserManager<MinotaurUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ViewRoles() => View();

        [HttpGet]
        public async Task<IActionResult> GetDataUserRoles()
        {
            var offices = await _db.Offices.Select(o => new { o.Id, o.Name }).ToListAsync();

            var worker = await _db.Workers.Join(_userManager.Users, e => e.UserId, u => u.Id, (e, u) => new { Worker = e, User = u }).Select(w => new
            {
                w.Worker.WorkerId,
                w.Worker.UserId,
                w.Worker.Status,
                OfficeName = offices.AsEnumerable().FirstOrDefault(i => i.Id == Guid.Parse(w.Worker.OfficeId)),
                w.Worker.OfficeId,
                LFS = w.User.LastName + " " + w.User.FirstName + " " + w.User.Surname,
                w.Worker.Role,
                w.User.Email,
            }).ToListAsync();





            return Json(new { data = worker, offices });
        }

        [HttpPost]
        public async Task<IActionResult> SetRoleWorker(string userId, string role)
        {
            MinotaurUser? minotaurUser = await _userManager.FindByIdAsync(userId);
            if (minotaurUser != null) { return BadRequest(new { error = "Работник не найден" }); }

            await _userManager.AddToRoleAsync(minotaurUser, role);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRoleWorker(string userId, string role)
        {

            MinotaurUser? minotaurUser = await _userManager.FindByIdAsync(userId);
            if (minotaurUser != null) { return BadRequest(new { error = "Работник не найден" }); }

            await _userManager.RemoveFromRoleAsync(minotaurUser, role);
            return Ok();
        }
    }
}
