using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.Customer
{
    [Area("Customer")]
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
