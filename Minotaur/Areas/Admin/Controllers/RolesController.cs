using Microsoft.AspNetCore.Mvc;

namespace Minotaur.Areas.Admin.Controllers
{
    public class RolesController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
