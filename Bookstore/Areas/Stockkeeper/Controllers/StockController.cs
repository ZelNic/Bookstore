using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.SD;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Areas.Stockkeeper
{
    [Area("Stockkeeper")]
    public class StockController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _stockkeeper;
        public StockController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _contextAccessor = contextAccessor;

            if (_contextAccessor.HttpContext.Session.GetInt32("Username") != null)
            {
                int? userId = _contextAccessor.HttpContext.Session.GetInt32("Username");
                if ((userId != null) && (_db.Employees.Where(u => u.UserId == userId) != null))
                {
                    if (_db.User.Find(userId) != null)
                    {
                        _stockkeeper = _db.User.Find(userId);
                    }
                }
                else
                {
                    NotFound(SD.AccessDenied);
                }
            }
        }

        public async Task<IActionResult> Index()
        {
            List<RecordStock>? recordWarehouses = await _db.StockJournal.Where(u => u.ResponsiblePersonId == _stockkeeper.UserId).ToListAsync();
            Stock? stock = await _db.Stocks.Where(u => u.ResponsiblePersonId == _stockkeeper.UserId).FirstOrDefaultAsync();

            StockVM stockVM = new()
            {
                Stock = stock,
                WarehouseJournal = recordWarehouses
            };

            return View(stockVM);
        }

        [HttpPost]
        public async Task<bool> AddProductInStock(int productId, int numberShelf, int productCount)
        {
            if (await _db.Books.FindAsync(productId) == null)
            {
                return false;
            }

            Stock? stock = await _db.Stocks.Where(u => u.ResponsiblePersonId == _stockkeeper.UserId).FirstOrDefaultAsync();

            if (stock != null)
            {
                RecordStock? stockJournal = await _db.StockJournal.Where(u => u.ProductId == productId).Where(s => s.ShelfNumber == numberShelf).FirstOrDefaultAsync();

                if (stockJournal != null)
                {
                    if (stockJournal.ProductId == productId && stockJournal.ShelfNumber == numberShelf)
                    {
                        stockJournal.Count += productCount;
                        _db.StockJournal.Update(stockJournal);
                    }
                }
                else if (stockJournal == null)
                {
                    RecordStock record = new()
                    {
                        StockId = stock.Id,
                        ResponsiblePersonId = _stockkeeper.UserId,
                        ProductId = productId,
                        Count = productCount,
                        ShelfNumber = numberShelf,
                        Operation = OperationStock.ReceiptOfGoods
                    };
                    await _db.StockJournal.AddAsync(record);
                }
                await _db.SaveChangesAsync();
                return true;
            }
            else { return false; }
        }


        //[HttpPost]
        //public async Task<IActionResult> ChangeShelfProduct(int recordId, int productCount, int newShelfNumber)
        //{
        //    Stock? record = await _db.Stocks.Where(u => u.ResponsiblePersonId == _stockkeeper.UserId).Where(i => i.Id == recordId).FirstOrDefaultAsync();
        //    if (record == null)
        //    {
        //        return NotFound();
        //    }

        //    if (record.ShelfNumber == newShelfNumber)
        //    {
        //        return Ok();
        //    }

        //    Stock newRecord = new()
        //    {
        //        City = record.City,
        //        Street = record.Street,
        //        ResponsiblePerson = record.ResponsiblePerson,
        //        ProductId = record.ProductId,

        //        ShelfNumber = newShelfNumber,
        //        Count = productCount
        //    };

        //    record.Count -= productCount;
        //    if (record.Count <= 0)
        //    {
        //        _db.Stocks.Remove(record);
        //    }
        //    else
        //    {
        //        _db.Stocks.Update(record);
        //    }

        //    await _db.Stocks.AddAsync(newRecord);
        //    await _db.SaveChangesAsync();

        //    return Ok();
        //}

        public async Task<IActionResult> GetStock()
        {
            var stock = await _db.StockJournal.Where(u => u.ResponsiblePersonId == _stockkeeper.UserId)
                .Join(_db.Books, s => s.ProductId, b => b.BookId, (s, b) => new
                {
                    id = s.Id,
                    productId = s.ProductId,
                    nameProduct = b.Title,
                    count = s.Count,
                    shelfNumber = s.ShelfNumber
                }).ToListAsync();

            return Json(new { data = stock });
        }

        //public async Task<IActionResult> OrderProducts(int productId)
        //{
        //    var product = await _db.Books.FindAsync(productId);

        //    PurchaseRequest request = new()
        //    {
        //        ApplicationTime = MoscowTime.GetTime(),
        //        ResponsiblePersonId = _stockkeeper.UserId,
        //        City = 
        //    };

        //    // Откройте шаблон документа Word
        //    using (DocX document = DocX.Load("путь_к_шаблону.docx"))
        //    {
        //        // Заполните данные в шаблоне, используя модель представления
        //        document.ReplaceText("{{OrderNumber}}", model.OrderNumber);
        //        document.ReplaceText("{{CustomerName}}", model.CustomerName);
        //        // Замените другие метки в шаблоне соответствующими данными из модели представления

        //        // Сохраните заполненный документ в файл
        //        string filePath = "путь_к_сохранению_заполненного_документа.docx";
        //        document.SaveAs(filePath);
        //    }

        //    // Верните файл пользователю для скачивания
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        //    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "имя_файла.docx");
        //}
    }
}
