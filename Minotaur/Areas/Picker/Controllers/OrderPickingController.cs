using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Newtonsoft.Json;

namespace Minotaur.Areas.Picker.Controllers
{
    [Area("Picker"), Authorize(Roles = Roles.Role_Order_Picker)]
    public class OrderPickingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderPickingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> GetAssemblyOrders()
        {
            var product = _unitOfWork.Products.GetAll();

            var assemblyOrders = _unitOfWork.Orders.GetAll()
                     .Where(s => s.OrderStatus == StatusByOrder.StatusApproved_1)
                     .Select(s => new
                     {
                         s.OrderId,
                         s.UserId,
                         Products = JsonConvert.DeserializeObject<List<OrderProductData>>(s.ProductData)
                             .Join(product, o => o.Id, p => p.ProductId, (o, p) => new
                             {
                                 p.ProductId,
                                 p.Name,
                                 p.Author,
                                 o.Count,                                 
                             }),
                         PurchaseDate = s.PurchaseDate.ToString("dd.MM.yyyy HH:mm"),
                     }).ToList();

            return Json(new { data = assemblyOrders });
        }


        public async Task<IActionResult> SendOrder(string orderId)
        {



            return Ok();
        }

    }
}
