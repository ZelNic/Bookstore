using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.OrganizationalDocumentation.HR;
using Minotaur.Models.OrganizationalDocumentation.SDHR;
using Minotaur.Models.SD;
using Minotaur.Utility;
using Newtonsoft.Json;

namespace Minotaur.Areas.HR.Controllers
{
    [Area("HR"), Authorize(Roles = $"{Roles.Role_HR}, {Roles.Role_Admin}")]
    public class WorkerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<MinotaurUser> _userManager;

        public WorkerController(ApplicationDbContext db, UserManager<MinotaurUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> GetDataByWorker()
        {
            var workers = await _db.Workers.ToArrayAsync();

            return Json(new { workers });
        }

        public async Task<IActionResult> FindUserForHiring(string? email = null, string? userId = null)
        {
            MinotaurUser? user = null;
            if (email != null)
            {
                user = await _userManager.FindByEmailAsync(email);
            }
            else if (userId != null)
            {
                user = await _userManager.FindByIdAsync(userId);
            }

            Office[] office = await _db.Offices.ToArrayAsync();
            IdentityRole[] roles = await _db.Roles.ToArrayAsync();

            return Json(new { user, office, roles });
        }

        public async Task<IActionResult> RegisterNewWorker(string userId, string officeId, string role)
        {
            if ((_userManager.FindByIdAsync(userId) != null) && (await _db.Offices.Where(u=>u.Id == Guid.Parse(officeId)).FirstOrDefaultAsync()!= null))
            {
                Worker worker = new()
                {
                    Status = StatusWorker.Works,
                    UserId = userId,
                    OfficeId = officeId,
                    Role = role,
                };

                OrganizationalOrder order = new()
                {
                    Name = "Приказ о приеме на работу",
                    Date = MoscowTime.GetTime().ToString("dd.MM.yyyy"),
                };

                await _userManager.AddToRoleAsync(await _userManager.FindByIdAsync(userId), role);

                await _db.OrganizationalOrders.AddAsync(order);
                await _db.Workers.AddAsync(worker);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest(new { error = "Введенные данные недействительны" }); ;
            }
        }

        public async Task<IActionResult> FireEmployee(string workerId)
        {
            Worker? exWorker = await _db.Workers.Where(w => w.WorkerId.ToString() == workerId).FirstOrDefaultAsync();
            if (exWorker == null) { return BadRequest(new { error = "Работник не найден" }); }

            exWorker.Status = StatusWorker.Fired;

            MinotaurUser minotaurUser = await _userManager.FindByIdAsync(exWorker.UserId);

            var userRoles = await _userManager.GetRolesAsync(minotaurUser);

            await _userManager.RemoveFromRolesAsync(minotaurUser, userRoles);

            await _userManager.AddToRoleAsync(minotaurUser, Roles.Role_Customer);
            return Ok();
        }

    }
}
