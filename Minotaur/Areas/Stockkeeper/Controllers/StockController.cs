﻿//using Minotaur.DataAccess;
//using Minotaur.Models;
//using Minotaur.Models.Models;
//using Minotaur.Models.SD;
//using Minotaur.Utility;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.CodeAnalysis;
//using Microsoft.EntityFrameworkCore;
//using System.Text.RegularExpressions;

//namespace Minotaur.Areas.Stockkeeper
//{
//    [Area("Stockkeeper")]
//    public class StockController : Controller
//    {

//        private readonly ApplicationDbContext _db;
//        private readonly IHttpContextAccessor _contextAccessor;
//        private readonly User? _stockkeeper;
//        private readonly Stock? _stock;
//        public StockController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
//        {
//            _db = db;
//            _contextAccessor = contextAccessor;

//            if (_contextAccessor.HttpContext.Session.GetString("Id") != null)
//            {
//                string? userId = _contextAccessor.HttpContext.Session.GetString("Id");
//                if ((userId != null) && (_db.Employees.Where(u => u.Id == userId) != null))
//                {
//                    if (_db.User.Find(userId) != null)
//                    {
//                        _stockkeeper = _db.User.Find(userId);
//                        _stock = _db.Stocks.Where(u => u.ResponsiblePersonId == _stockkeeper.Id).FirstOrDefault();
//                        if (_stock == null || _stockkeeper == null)
//                        {
//                            NotFound(SD.AccessDenied);
//                        }
//                    }
//                }
//                else
//                {
//                    NotFound(SD.AccessDenied);
//                }
//            }
//        }

//        public async Task<IActionResult> Index()
//        {
//            List<RecordStock>? recordWarehouses = await _db.StockJournal.Where(u => u.ResponsiblePersonId == _stockkeeper.Id).ToListAsync();
//            Stock? stock = await _db.Stocks.Where(u => u.ResponsiblePersonId == _stockkeeper.Id).FirstOrDefaultAsync();

//            StockVM stockVM = new()
//            {
//                Stock = stock,
//                WarehouseJournal = recordWarehouses
//            };

//            return View(stockVM);
//        }

//        [HttpPost]
//        public async Task<IActionResult> AddProductInStock(int productId, int numberShelf, int productCount)
//        {
//            var book = await _db.Products.FindAsync(productId);
//            if (book == null)
//            {
//                return BadRequest(new { error = "Такого товара нет в базе. Проверьте правильность введенных данных." });
//            }


//            RecordStock? stockJouselectedRecordrnal = await _db.StockJournal.Where(u => u.ProductId == productId).Where(s => s.ShelfNumber == numberShelf).FirstOrDefaultAsync();

//            if (stockJouselectedRecordrnal != null)
//            {
//                if (stockJouselectedRecordrnal.ProductId == productId && stockJouselectedRecordrnal.ShelfNumber == numberShelf)
//                {
//                    stockJouselectedRecordrnal.Count += productCount;
//                    _db.StockJournal.Update(stockJouselectedRecordrnal);
//                }
//            }
//            else if (stockJouselectedRecordrnal == null)
//            {
//                RecordStock record = new()
//                {
//                    StockId = _stock.Id,
//                    ResponsiblePersonId = _stockkeeper.Id,
//                    Time = MoscowTime.GetTime(),
//                    ProductId = productId,
//                    Count = productCount,
//                    ShelfNumber = numberShelf,
//                    Operation = OperationStock.ReceiptOfGoods,
//                    IsOrder = await _db.StockJournal.Where(u => u.ProductId == productId).Select(u => u.IsOrder).FirstOrDefaultAsync(),
//                };
//                await _db.StockJournal.AddAsync(record);
//            }
//            await _db.SaveChangesAsync();
//            return Ok("Товар " + book.Name + " добавлен на полку " + numberShelf + " в количестве " + productCount + " шт.");

//        }

//        [HttpPost]
//        public async Task<IActionResult> ChangeShelfProduct(int recordId, int productCount, int newShelfNumber)
//        {
//            RecordStock? selectedRecord = await _db.StockJournal.Where(u => u.ResponsiblePersonId == _stockkeeper.Id).Where(i => i.Id == recordId).FirstOrDefaultAsync();
//            if (selectedRecord == null)
//            {
//                return NotFound();
//            }

//            if (selectedRecord.ShelfNumber == newShelfNumber)
//            {
//                return Ok();
//            }

//            RecordStock? productOnShelf = await _db.StockJournal.Where(u => u.ProductId == selectedRecord.ProductId).Where(s => s.ShelfNumber == newShelfNumber).FirstOrDefaultAsync();

//            selectedRecord.Count -= productCount;
//            if (selectedRecord.Count <= 0)
//            {
//                _db.StockJournal.Remove(selectedRecord);
//            }
//            else
//            {
//                _db.StockJournal.Update(selectedRecord);
//            }


//            if (productOnShelf != null)
//            {
//                productOnShelf.Count += productCount;

//                _db.StockJournal.Update(productOnShelf);
//                await _db.SaveChangesAsync();
//                return Ok();
//            }
//            else
//            {
//                RecordStock newRecord = new()
//                {
//                    StockId = _stock.Id,
//                    ResponsiblePersonId = _stockkeeper.Id,
//                    Time = MoscowTime.GetTime(),
//                    ProductId = selectedRecord.ProductId,
//                    ShelfNumber = newShelfNumber,
//                    Count = productCount,
//                    Operation = OperationStock.MovementOfGoods,
//                    IsOrder = await _db.StockJournal.Where(u => u.ProductId == selectedRecord.ProductId).Select(u => u.IsOrder).FirstOrDefaultAsync(),
//                };

//                await _db.StockJournal.AddAsync(newRecord);
//                await _db.SaveChangesAsync();

//                return Ok();
//            }
//        }

//        public async Task<IActionResult> GetStock()
//        {
//            var stockJournal = await _db.StockJournal.Where(u => u.ResponsiblePersonId == _stockkeeper.Id)
//                .Join(_db.Products, s => s.ProductId, b => b.ProductId, (s, b) => new
//                {
//                    id = s.Id,
//                    productId = s.ProductId,
//                    time = s.Time.ToString("dd/MM/yyyy hh:mm"),
//                    operation = s.Operation,
//                    nameProduct = b.Name,
//                    count = s.Count,
//                    totalProduct = _db.StockJournal.Where(i => i.ProductId == s.ProductId).Sum(s => s.Count),
//                    shelfNumber = s.ShelfNumber,
//                    isOrder = s.IsOrder,
//                }).ToListAsync();

//            return Json(new { data = stockJournal });
//        }

//        [HttpPost]
//        public async Task<IActionResult> SelectProductToPurchase(int productId, params int[] products)
//        {
//            IEnumerable<RecordStock> allProduct = await _db.StockJournal.Where(i => i.ProductId == productId).ToListAsync();

//            foreach (var product in allProduct)
//            {
//                product.IsOrder = product.IsOrder ? false : true;
//            }

//            _db.StockJournal.UpdateRange(allProduct);
//            await _db.SaveChangesAsync();

//            return Ok();
//        }

//        [HttpGet]
//        public IActionResult PurchaseRequest()
//        {
//            return View();
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetTablePurchaseRequest()
//        {
//            var request = await _db.StockJournal.Where(u => u.IsOrder == true).Join(_db.Products, s => s.ProductId, b => b.ProductId, (s, b) => new
//            {
//                ProductId = s.ProductId,
//                TitleProduct = _db.Products.Where(u => u.ProductId == s.ProductId).Select(u => u.Name).FirstOrDefault(),
//                TotalProduct = _db.StockJournal.Where(u => u.IsOrder == true).Where(i => i.ProductId == s.ProductId).Select(u => u.Count).Sum(),
//            }).Distinct().ToListAsync();

//            return Json(new { data = request });
//        }


//        [HttpPost]
//        public async Task<IActionResult> OrderProducts([FromBody] RecordStock[] purchaseRequestData)
//        {
//            if (purchaseRequestData == null)
//            {
//                return BadRequest(new { error = "Нет товаров на закупку." });
//            }

//            string productDataOnPurchase = "";

//            foreach (var item in purchaseRequestData)
//            {
//                productDataOnPurchase += item.ProductId.ToString() + ":";
//                productDataOnPurchase += item.Count.ToString() + "/";
//            }

//            RecordStock recordStockPurchase = new()
//            {
//                StockId = _stock.Id,
//                ResponsiblePersonId = _stockkeeper.Id,
//                Time = MoscowTime.GetTime(),
//                Operation = OperationStock.ApplicationForPurchaseOfGoods,
//                ProductDataOnPurchase = productDataOnPurchase
//            };

//            await _db.StockJournal.AddAsync(recordStockPurchase);
//            await _db.SaveChangesAsync();

//            //-----------------------------------------------------------------------------------------------------------------------------


//            string templatePath = "F:\\GitHub\\Minotaur\\Minotaur\\Areas\\Stockkeeper\\Sample\\Заявка на закупку товаров.docx";
//            string nameFile = "Заявка" + recordStockPurchase.Time.ToString("_dd.MM.yyyy");
//            string filledFilePath = $"F:\\GitHub\\Minotaur\\Minotaur\\Areas\\Stockkeeper\\PurchaseRequisitions\\{nameFile}.docx";



//            System.IO.File.Copy(templatePath, filledFilePath, true);

//            using (WordprocessingDocument docx = WordprocessingDocument.Open(filledFilePath, true))
//            {
//                // Получаем главный документ Word
//                MainDocumentPart? mainPart = docx.MainDocumentPart;

//                if (mainPart == null)
//                {
//                    return NotFound();
//                }

//                string docText = null;
//                using (StreamReader sr = new StreamReader(docx.MainDocumentPart.GetStream()))
//                {
//                    docText = sr.ReadToEnd();
//                }


//                Dictionary<string, string> replacements = new Dictionary<string, string>
//                {
//                    {"Number", recordStockPurchase.Id.ToString()},
//                    {"Street", _stock.Street.ToString()},
//                    {"City", _stock.City.ToString()},
//                    {"ApplicationTime", recordStockPurchase.Time.ToString("dd.MM.yyyy")},
//                    {"FirstName", _stockkeeper.FirstName.ToString()},
//                    {"SecondName", _stockkeeper.LastName.ToString()},
//                    {"Phone", _stockkeeper.PhoneNumber.ToString()}
//                };

//                Regex regexText = new Regex(string.Join("|", replacements.Keys));
//                docText = regexText.Replace(docText, match => replacements[match.Value]);

//                using (StreamWriter sw = new StreamWriter(docx.MainDocumentPart.GetStream(FileMode.Create)))
//                {
//                    sw.Write(docText);
//                }

//                //-----------------------------------------------------------------------------------------------------------------------
//                //-----------------------------------------------------------------------------------------------------------------------
//                //-----------------------------------------------------------------------------------------------------------------------

//                Table table = mainPart.Document.Body.Elements<Table>().FirstOrDefault();
//                TableProperties tableProperties = table.GetFirstChild<TableProperties>();

//                foreach (var replacement in purchaseRequestData)
//                {
//                    TableRow newRow = new TableRow();

//                    TableCell cellId = new TableCell(new Paragraph(new Run(new Text("-"))));
//                    TableCell productNameCell = new TableCell(new Paragraph(new Run(new Text(replacement.ProductName + ", " + replacement.ProductId.ToString()))));
//                    TableCell countCell = new TableCell(new Paragraph(new Run(new Text(replacement.Count.ToString()))));

//                    newRow.AppendChild(cellId);
//                    newRow.AppendChild(productNameCell);
//                    newRow.AppendChild(countCell);

//                    // Clone the TableProperties and apply it to each new cell
//                    TableCellProperties cellProperties = new TableCellProperties();
//                    cellProperties.Append(tableProperties.CloneNode(true));

//                    cellId.AppendChild(cellProperties.CloneNode(true));
//                    productNameCell.AppendChild(cellProperties.CloneNode(true));
//                    countCell.AppendChild(cellProperties.CloneNode(true));

//                    table.AppendChild(newRow);
//                }
//            }

//            byte[] fileBytes = System.IO.File.ReadAllBytes(filledFilePath);
//            string fileName = "example.docx";

//            // Устанавливаем заголовки ответа
//            Response.Headers.Add("Content-Disposition", "attachment; filename=" + fileName);
//            Response.Headers.Add("X-Content-Type-Options", "nosniff");

//            return new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
//        }
//    }
//}
