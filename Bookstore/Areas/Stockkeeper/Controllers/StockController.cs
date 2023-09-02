using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.SD;
using Bookstore.Utility;
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
        public async Task<IActionResult> AddProductInStock(int productId, int numberShelf, int productCount)
        {
            var book = await _db.Books.FindAsync(productId);
            if (book == null)
            {
                return BadRequest(new { error = "Такого товара нет в базе. Проверьте правильность введенных данных." }.ToString());
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
                return Ok("Товар " + book.Title + " добавлен на полку " + numberShelf + " в количестве " + productCount + " шт.");
            }
            else
            {
                return BadRequest(new { error = "Склад не найден." }.ToString());
            }
        }


        [HttpPost]
        public async Task<IActionResult> ChangeShelfProduct(int recordId, int productCount, int newShelfNumber)
        {
            RecordStock? selectedRecord = await _db.StockJournal.Where(u => u.ResponsiblePersonId == _stockkeeper.UserId).Where(i => i.Id == recordId).FirstOrDefaultAsync();
            if (selectedRecord == null)
            {
                return NotFound();
            }

            if (selectedRecord.ShelfNumber == newShelfNumber)
            {
                return Ok();
            }

            RecordStock? productOnShelf = await _db.StockJournal.Where(u => u.ProductId == selectedRecord.ProductId).Where(s => s.ShelfNumber == newShelfNumber).FirstOrDefaultAsync();

            selectedRecord.Count -= productCount;
            if (selectedRecord.Count <= 0)
            {
                _db.StockJournal.Remove(selectedRecord);
            }
            else
            {
                _db.StockJournal.Update(selectedRecord);
            }


            if (productOnShelf != null)
            {
                productOnShelf.Count += productCount;

                _db.StockJournal.Update(productOnShelf);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                RecordStock newRecord = new()
                {
                    StockId = await _db.Stocks.Where(u => u.ResponsiblePersonId == _stockkeeper.UserId).Select(u => u.Id).FirstOrDefaultAsync(),
                    ResponsiblePersonId = _stockkeeper.UserId,
                    ProductId = selectedRecord.ProductId,
                    ShelfNumber = newShelfNumber,
                    Count = productCount,
                    Operation = OperationStock.MovementOfGoods
                };

                await _db.StockJournal.AddAsync(newRecord);
                await _db.SaveChangesAsync();

                return Ok();
            }
        }

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

        [HttpGet]
        public async Task<IActionResult> GetPurchaseRequest()
        {
            PurchaseRequest request = new()
            {
                ApplicationTime = MoscowTime.GetTime(),
                ResponsiblePersonId = _stockkeeper.UserId,
                StockId = await _db.Stocks.Where(u => u.ResponsiblePersonId == _stockkeeper.UserId).Select(u => u.Id).FirstOrDefaultAsync()
            };

            return View(request);
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
