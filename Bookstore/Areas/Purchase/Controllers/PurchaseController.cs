using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.Purchase.Controllers
{
    public class PurchaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
