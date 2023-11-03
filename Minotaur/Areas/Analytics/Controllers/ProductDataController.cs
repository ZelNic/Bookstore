using Microsoft.AspNetCore.Mvc;

namespace Minotaur.Areas.Analytics.Controllers
{
    // TODO: сделать аналитз продукции
    public class ProductDataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
