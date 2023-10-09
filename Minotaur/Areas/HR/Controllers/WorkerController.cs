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


        public async Task<IActionResult> Index()
        {
            return View();
        }


        public async Task<IActionResult> GetDataByWorker()
        {
            var worker = await _db.Workers.Join(_userManager.Users, e => e.UserId, u => u.Id, (e, u) => new { Worker = e, User = u })
                .Select(w => new
                {
                    w.Worker.WorkerId,
                    w.Worker.UserId,
                    w.Worker.Status,
                    w.Worker.OfficeId,
                    w.Worker.OfficeName,
                    LFS = w.User.LastName + " " + w.User.FirstName + " " + w.User.Surname,
                    w.Worker.AccessRights,
                    w.Worker.Post,
                    w.User.Email,
                }).ToArrayAsync();

            return Json(new { data = worker });
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

        public async Task<IActionResult> RegisterNewWorker(string userId, string officeId, string post)
        {
            if ((_userManager.FindByIdAsync(userId) != null))
            {
                var office = await _db.Offices.Where(u => u.Id == Guid.Parse(officeId)).FirstOrDefaultAsync();
                if (office == null) { return BadRequest(new { error = "Офис не найден" }); }

                Worker worker = new()
                {
                    Status = StatusWorker.Works,
                    UserId = userId,
                    OfficeId = Guid.Parse(officeId),
                    OfficeName = office.Name,
                    Post = post,
                };

                OrganizationalOrder order = new()
                {
                    Name = "Приказ о приеме на работу",
                    Date = MoscowTime.GetTime().ToString("dd.MM.yyyy"),
                };


                await _db.OrganizationalOrders.AddAsync(order);
                await _db.Workers.AddAsync(worker);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest(new
                {
                    error = "Работник не найден в качестве пользователя. " +
                    "Необходимо зарегестироваться в качестве пользователя или проверить правильность данных."
                }); ;
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
