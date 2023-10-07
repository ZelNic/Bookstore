using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Models.SD;

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

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> GetDataUserRoles()
        {
            var worker = await _db.Workers.ToArrayAsync();

            return Json(new {worker});
        }


        public async Task<IActionResult> SetRoleWorker(string workerId,string role)
        {
            MinotaurUser? minotaurUser = await _userManager.FindByIdAsync(workerId);
            if(minotaurUser != null) { return BadRequest(new { error = "Работник не найден" }); }

            await _userManager.AddToRoleAsync(minotaurUser, role);
            return Ok();
        }
    }
}
