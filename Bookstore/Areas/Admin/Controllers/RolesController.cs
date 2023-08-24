using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.Admin.Controllers
{
    public class RolesController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
