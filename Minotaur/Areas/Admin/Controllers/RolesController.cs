using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;

namespace Minotaur.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = Roles.Role_Admin)]
    public class RolesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;

        public RolesController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ViewRoles() => View();

        [HttpGet]
        public async Task<IActionResult> GetDataUserRoles()
        {
            var worker = await _unitOfWork.Workers.GetAllAsync();

            var dataWorkerWithUserData = worker.Join(_unitOfWork.MinotaurUsers.GetAll(), w => w.UserId, u => Guid.Parse(u.Id), (w, u) => new { Worker = w, User = u })
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
                }).ToArray();

            return Json(new { data = dataWorkerWithUserData });
        }

        private List<string> ParseWorkerRoles(string role)
        {
            List<string>? arrayRoles = null;

            if (role == null)
            {
                return arrayRoles;
            }
            else
            {
                arrayRoles = role.Split('|').ToList();
            }

            return arrayRoles;
        }

        private string SerializationWorkerRoles(List<string> arrayRoles)
        {
            string stringRoles = string.Join('|', arrayRoles);

            return stringRoles;
        }



        [HttpPost]
        public async Task<IActionResult> SetRoleWorker(string userId, string role)
        {
            MinotaurUser? minotaurUser = await _userManager.FindByIdAsync(userId);
            if (minotaurUser == null) { return BadRequest(new { error = "Пользователь не найден" }); }

            Worker? worker = await _unitOfWork.Workers.GetAsync(u => u.UserId == Guid.Parse(minotaurUser.Id));
            if (worker == null) { return BadRequest(new { error = "Пользователь не зарегистрирован в качестве сотрудника" }); }

            List<string> arrayRoles = ParseWorkerRoles(worker.AccessRights);

            if (arrayRoles.Contains(role))
            {
                return Ok();
            }

            arrayRoles.Add(role);
            worker.AccessRights = SerializationWorkerRoles(arrayRoles);

            _unitOfWork.Workers.Update(worker);
            _unitOfWork.SaveAsync();

            await _userManager.AddToRoleAsync(minotaurUser, role);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRoleWorker(string userId, string role)
        {
            MinotaurUser? minotaurUser = await _userManager.FindByIdAsync(userId);
            if (minotaurUser == null) { return BadRequest(new { error = "Работник не найден" }); }

            Worker worker = await _unitOfWork.Workers.GetAsync(u => u.UserId == Guid.Parse(minotaurUser.Id));
            if (worker == null) { return BadRequest(new { error = "Пользователь не зарегистрирован в качестве сотрудника" }); }

            List<string> arrayRoles = ParseWorkerRoles(worker.AccessRights);
            if (!arrayRoles.Contains(role))
            {
                return Ok();
            }

            arrayRoles.Remove(role);
            worker.AccessRights = SerializationWorkerRoles(arrayRoles);

            _unitOfWork.Workers.Update(worker);
            _unitOfWork.SaveAsync();

            await _userManager.RemoveFromRoleAsync(minotaurUser, role);
            return Ok();
        }
    }
}
