using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.SD;
using Bookstore.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Xceed.Words.NET;

namespace Bookstore.Areas.Stockkeeper
{
    [Area("Stockkeeper")]
    public class StockController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User? _stockkeeper;
        private readonly int _stockId;
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
                        _stockId = _db.Stocks.Where(u => u.ResponsiblePersonId == _stockkeeper.UserId).FirstOrDefault().Id;
                        if (_stockId == 0 || _stockkeeper == null)
                        {
                            NotFound(SD.AccessDenied);
                        }
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
                return BadRequest(new { error = "Такого товара нет в базе. Проверьте правильность введенных данных." });
            }


            RecordStock? stockJouselectedRecordrnal = await _db.StockJournal.Where(u => u.ProductId == productId).Where(s => s.ShelfNumber == numberShelf).FirstOrDefaultAsync();

            if (stockJouselectedRecordrnal != null)
            {
                if (stockJouselectedRecordrnal.ProductId == productId && stockJouselectedRecordrnal.ShelfNumber == numberShelf)
                {
                    stockJouselectedRecordrnal.Count += productCount;
                    _db.StockJournal.Update(stockJouselectedRecordrnal);
                }
            }
            else if (stockJouselectedRecordrnal == null)
            {
                RecordStock record = new()
                {
                    StockId = _stockId,
                    ResponsiblePersonId = _stockkeeper.UserId,
                    Time = MoscowTime.GetTime(),
                    ProductId = productId,
                    Count = productCount,
                    ShelfNumber = numberShelf,
                    Operation = OperationStock.ReceiptOfGoods,
                    IsOrder = await _db.StockJournal.Where(u => u.ProductId == productId).Select(u => u.IsOrder).FirstOrDefaultAsync(),
                };
                await _db.StockJournal.AddAsync(record);
            }
            await _db.SaveChangesAsync();
            return Ok("Товар " + book.Title + " добавлен на полку " + numberShelf + " в количестве " + productCount + " шт.");

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
                    StockId = _stockId,
                    ResponsiblePersonId = _stockkeeper.UserId,
                    Time = MoscowTime.GetTime(),
                    ProductId = selectedRecord.ProductId,
                    ShelfNumber = newShelfNumber,
                    Count = productCount,
                    Operation = OperationStock.MovementOfGoods,
                    IsOrder = await _db.StockJournal.Where(u => u.ProductId == selectedRecord.ProductId).Select(u => u.IsOrder).FirstOrDefaultAsync(),
                };

                await _db.StockJournal.AddAsync(newRecord);
                await _db.SaveChangesAsync();

                return Ok();
            }
        }

        public async Task<IActionResult> GetStock()
        {
            var stockJournal = await _db.StockJournal.Where(u => u.ResponsiblePersonId == _stockkeeper.UserId)
                .Join(_db.Books, s => s.ProductId, b => b.BookId, (s, b) => new
                {
                    id = s.Id,
                    productId = s.ProductId,
                    time = s.Time.ToString("dd/MM/yyyy hh:mm"),
                    operation = s.Operation,
                    nameProduct = b.Title,
                    count = s.Count,
                    totalProduct = _db.StockJournal.Where(i => i.ProductId == s.ProductId).Sum(s => s.Count),
                    shelfNumber = s.ShelfNumber,
                    isOrder = s.IsOrder,
                }).ToListAsync();

            return Json(new { data = stockJournal });
        }

        [HttpPost]
        public async Task<IActionResult> SelectProductToPurchase(int productId, params int[] products)
        {
            IEnumerable<RecordStock> allProduct = await _db.StockJournal.Where(i => i.ProductId == productId).ToListAsync();

            foreach (var product in allProduct)
            {
                product.IsOrder = product.IsOrder ? false : true;
            }

            _db.StockJournal.UpdateRange(allProduct);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public IActionResult PurchaseRequest()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTablePurchaseRequest()
        {
            var request = await _db.StockJournal.Where(u => u.IsOrder == true).Join(_db.Books, s => s.ProductId, b => b.BookId, (s, b) => new
            {
                ProductId = s.ProductId,
                TitleProduct = _db.Books.Where(u => u.BookId == s.ProductId).Select(u => u.Title).FirstOrDefault(),
                TotalProduct = _db.StockJournal.Where(u => u.IsOrder == true).Where(i => i.ProductId == s.ProductId).Select(u => u.Count).Sum(),
            }).Distinct().ToListAsync();

            return Json(new { data = request });
        }


        [HttpPost]
        public async Task<IActionResult> OrderProducts([FromBody] RecordStock[] purchaseRequestData)
        {
            if (purchaseRequestData == null)
            {
                return BadRequest(new { error = "Нет товаров на закупку." });
            }

            string productDataOnPurchase = "";

            foreach (var item in purchaseRequestData)
            {
                productDataOnPurchase += item.ProductId.ToString() + ":";
                productDataOnPurchase += item.Count.ToString() + "/";
            }

            RecordStock recordStockPurchase = new()
            {
                StockId = _stockId,
                ResponsiblePersonId = _stockkeeper.UserId,
                Time = MoscowTime.GetTime(),
                Operation = OperationStock.ApplicationForPurchaseOfGoods,
                ProductDataOnPurchase = productDataOnPurchase
            };

            //await _db.StockJournal.AddAsync(recordStockPurchase);
            //await _db.SaveChangesAsync();

            string templatePath = "/Areas/Stockkeeper/Sample/Заявка на закупку товаров.docx";
            string filledFilePath = "/Areas/Stockkeeper/PurchaseRequisitions/имя_файла.docx";



            using (DocX document = DocX.Create(templatePath))
            {
                //foreach (var product in purchaseRequestData)
                //{
                //    document.ReplaceText("{{City}}", product.Id.ToString());
                //    document.ReplaceText("{{Street}}", product.ProductName);
                //}
                try
                {
                    document.ReplaceText("{{Street}}", purchaseRequestData[0].ProductName);
                    document.ReplaceText("{{City}}", purchaseRequestData[0].Id.ToString());
                }
                catch 
                {
                    return Ok("error");
                }

                document.SaveAs(filledFilePath);
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filledFilePath);
            return File(fileBytes, "имя_файла.docx");
        }
    }
}
