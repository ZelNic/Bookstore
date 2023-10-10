using Microsoft.AspNetCore.Authorization;
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
using System.Data;

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


        public async Task<IActionResult> Index() => View();

        public async Task<IActionResult> GetDataByWorkers(string? workerId = null)
       {
            var workers = await _db.Workers.Join(_userManager.Users, e => e.UserId, u => u.Id, (e, u) => new { Worker = e, User = u })
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

            if (workerId != null)
            {
                foreach (var worker in workers)
                {
                    if (worker.WorkerId == Guid.Parse(workerId))
                    {
                        return Json(new { data = worker });
                    }
                }
                return BadRequest(new { error = "Работник с таким UserId не найден" });
            }

            return Json(new { data = workers });
        }

        public async Task<IActionResult> FindUserForHiring(string email)
        {
            MinotaurUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest(new { error = "Пользователь не найден. Необходимо зарегестировать работника в качестве сотрудника или проверить введеные данные." });

            var userWithNeedProperties = new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Surname,
                user.DateofBirth,
                user.Region,
                user.City,
                user.Street,
                user.HouseNumber
            };

            Office[] office = await _db.Offices.ToArrayAsync();

            return Json(new { data = userWithNeedProperties, office });
        }

        [HttpPost]
        public async Task<IActionResult> SaveDataUser(string dataUser)
        {
            var newDatauser = JsonConvert.DeserializeObject<MinotaurUser>(dataUser);
            if (newDatauser == null) { return BadRequest(new { error = "Ошибка в обработке данных" }); }

            var oldDataUser = await _userManager.FindByIdAsync(newDatauser.Id);
            if (oldDataUser == null) { return BadRequest(new { error = "Пользователь не найден." }); }

            oldDataUser.FirstName = newDatauser.FirstName;
            oldDataUser.LastName = newDatauser.LastName;
            oldDataUser.Surname = newDatauser.Surname;
            oldDataUser.DateofBirth = newDatauser.DateofBirth;
            oldDataUser.Region = newDatauser.Region;
            oldDataUser.City = newDatauser.City;
            oldDataUser.Street = newDatauser.Street;
            oldDataUser.HouseNumber = newDatauser.HouseNumber;

            var result = await _userManager.UpdateAsync(oldDataUser);

            if (await _db.Workers.Where(u => u.UserId == newDatauser.Id).FirstAsync() == null)
            {
                Worker worker = new()
                {
                    UserId = newDatauser.Id,
                    Status = StatusWorker.Confirmation_Required
                };

                await _db.Workers.AddAsync(worker);
                await _db.SaveChangesAsync();
            }


            if (result != null)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { error = "Ошибка. Данные не сохранены." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterNewWorker(string dataWorker)
        {
            var worker = JsonConvert.DeserializeObject<Worker>(dataWorker);
            if (worker != null) { return BadRequest(new { error = "Ошибка. Данные не сохранены." }); }

            if (_userManager.FindByIdAsync(worker.UserId) != null)
            {
                var office = await _db.Offices.Where(u => u.Id == Guid.Parse(worker.UserId)).FirstOrDefaultAsync();
                if (office == null) { return BadRequest(new { error = "Офис не найден" }); }

                Worker newWorker = new()
                {
                    Status = StatusWorker.Works,
                    UserId = worker.UserId,
                    OfficeId = worker.OfficeId,
                    OfficeName = office.Name,
                    Post = worker.Post,
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
