using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minotaur.DataAccess;
using Minotaur.DataAccess.Repository;
using Minotaur.DataAccess.Repository.IRepository;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;

        public WorkerController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> GetDataByWorkers(string? workerId = null)
        {
            var request = await _unitOfWork.Workers.GetAllAsync();
            var workers = request.Join<Worker, MinotaurUser, Guid, dynamic>(_unitOfWork.MinotaurUsers.GetAll(), e => e.UserId, u => Guid.Parse(u.Id), (e, u) => new { Worker = e, User = u })
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
                     w.Worker.AdmissionOrder,
                     w.Worker.OrderDismissal,
                 });

            if (workerId != null)
            {
                foreach (var worker in request)
                {
                    if (worker.WorkerId == Guid.Parse(workerId))
                    {
                        var status = StatusWorker.GetStatus();
                        var offices = _unitOfWork.Offices.GetAllAsync().Result.Select(o => new
                        {
                            o.Id,
                            o.Name,
                            o.Type
                        });

                        return Json(new { data = worker, status, offices });
                    }
                }
                return BadRequest(new { error = "Работник с таким UserId не найден" });
            }

            return Json(new { data = workers });
        }

        public async Task<IActionResult> FindUserForHiring(string email)
        {
            MinotaurUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest("Пользователь не найден. Необходимо зарегестировать работника в качестве сотрудника или проверить введеные данные.");

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

            var office = await _unitOfWork.Offices.GetAllAsync();

            return Json(new { data = userWithNeedProperties, office });
        }

        [HttpPost]
        public async Task<IActionResult> UpsertEmployeeUserData(string dataUser)
        {
            var newDatauser = JsonConvert.DeserializeObject<MinotaurUser>(dataUser);
            if (newDatauser == null) { return BadRequest("Ошибка в обработке данных"); }

            var oldDataUser = await _userManager.FindByIdAsync(newDatauser.Id);
            if (oldDataUser == null) { return BadRequest("Пользователь не найден."); }

            oldDataUser.FirstName = newDatauser.FirstName;
            oldDataUser.LastName = newDatauser.LastName;
            oldDataUser.Surname = newDatauser.Surname;
            oldDataUser.DateofBirth = newDatauser.DateofBirth;
            oldDataUser.Region = newDatauser.Region;
            oldDataUser.City = newDatauser.City;
            oldDataUser.Street = newDatauser.Street;
            oldDataUser.HouseNumber = newDatauser.HouseNumber;

            await _userManager.UpdateAsync(oldDataUser);

            if (await _unitOfWork.Workers.GetAsync(u => u.UserId == Guid.Parse(oldDataUser.Id)) == null)
            {
                var result = AddNewWorker(oldDataUser);
                return Json(new { data = result });
            }
            else
            {
                var workerData = await _unitOfWork.Workers.GetAllAsync(u => u.UserId == Guid.Parse(oldDataUser.Id));
                return Json(new { data = workerData });
            }
        }

        private async Task<IActionResult> AddNewWorker(MinotaurUser oldDataUser)
        {
            Worker worker = new()
            {
                UserId = Guid.Parse(oldDataUser.Id),
                Status = StatusWorker.Confirmation_Required,
                AccessRights = Roles.Role_Customer,
            };

            try
            {
                await _unitOfWork.Workers.AddAsync(worker);
                await _unitOfWork.Save();

                var hr = await _userManager.GetUserAsync(User);
                var hrId = hr.Id;

                var registeredWorker = await _unitOfWork.Workers.GetAsync(w => w.UserId == Guid.Parse(oldDataUser.Id));

                OrganizationalOrder order = new()
                {
                    Name = "Приказ о приеме на работу",
                    WorkerId = registeredWorker.WorkerId,
                    HrID = Guid.Parse(hrId),
                    Date = MoscowTime.GetTime().ToString("dd.MM.yyyy HH:mm")
                };

                await _unitOfWork.OrganizationalOrders.AddAsync(order);
                await _unitOfWork.Save();

                worker.AdmissionOrder = order.Id;
                _unitOfWork.Workers.Update(worker);
                await _unitOfWork.Save();

                return Json(new { data = worker });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpsertWorkerData(string dataWorker)
        {
            Worker? newDataWorker = JsonConvert.DeserializeObject<Worker>(dataWorker);

            if (newDataWorker == null) { return BadRequest(new { error = "Ошибка. Данные не сохранены." }); }

            if (await _userManager.FindByIdAsync(newDataWorker.UserId.ToString()) != null)
            {
                var office = await _unitOfWork.Offices.GetAsync(u => u.Id == newDataWorker.OfficeId);
                if (office == null) { return BadRequest("Офис не найден"); }

                Worker? oldDataWorker = await _unitOfWork.Workers.GetAsync(u => u.WorkerId == newDataWorker.WorkerId);
                if (oldDataWorker == null) { return BadRequest("Работник не найден"); }

                var admissionOrder = await _unitOfWork.OrganizationalOrders.GetAsync(w => w.WorkerId == oldDataWorker.WorkerId);

                oldDataWorker.Status = newDataWorker.Status;
                oldDataWorker.OfficeId = newDataWorker.OfficeId;
                oldDataWorker.OfficeName = office.Name;
                oldDataWorker.Post = newDataWorker.Post;
                oldDataWorker.AdmissionOrder = admissionOrder.Id;
                oldDataWorker.OrderDismissal = newDataWorker.OrderDismissal;
                _unitOfWork.Workers.Update(oldDataWorker);
                await _unitOfWork.Save();
                return Ok();
            }
            else
            {
                return BadRequest("Работник не найден в качестве пользователя. Необходимо зарегестироваться в качестве пользователя или проверить правильность данных.");
            }
        }

        public async Task<IActionResult> FireEmployee(string workerId)
        {
            Worker? exWorker = await _unitOfWork.Workers.GetAsync(w => w.WorkerId.ToString() == workerId);
            if (exWorker == null) { return BadRequest("Работник не найден"); }

            exWorker.Status = StatusWorker.Fired;

            MinotaurUser? minotaurUser = await _userManager.FindByIdAsync(exWorker.UserId.ToString());

            var userRoles = await _userManager.GetRolesAsync(minotaurUser);

            await _userManager.RemoveFromRolesAsync(minotaurUser, userRoles);

            await _userManager.AddToRoleAsync(minotaurUser, Roles.Role_Customer);
            return Ok();
        }
    }
}
