using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Newtonsoft.Json;

namespace Minotaur.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = Roles.Role_Admin)]
    public class OfficeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;

        public OfficeController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Office()
        {
            return View();
        }

        public async Task<IActionResult> GetDataOffice()
        {
            var offices = await _unitOfWork.Offices.GetAllAsync();

            return Json(new { data = offices });
        }

        public IActionResult GetDataForFormNewOffice()
        {
            string[]? officeTypes = Enum.GetValues(typeof(TypesOfOffices)).Cast<TypesOfOffices>().Select(e => Enum.GetName(typeof(TypesOfOffices), e)).ToArray();
            string[]? officeStatus = Enum.GetValues(typeof(OfficeStatus)).Cast<OfficeStatus>().Select(e => Enum.GetName(typeof(OfficeStatus), e)).ToArray();

            return Json(new { officeTypes, officeStatus });
        }


        [HttpPost]
        public async Task<IActionResult> AddOffice(string dataOffice)
        {
            if (dataOffice == null) { return BadRequest(new { error = "Неверно заполнены данные" }); }

            Office? office = JsonConvert.DeserializeObject<Office>(dataOffice);

            if (office != null)
            {
                await _unitOfWork.Offices.AddAsync(office);
                await _unitOfWork.Save();
            }

            return Ok();
        }   
    }
}
