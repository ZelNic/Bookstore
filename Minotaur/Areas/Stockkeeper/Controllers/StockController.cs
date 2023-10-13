using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Models.ModelForWorkingControllers;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Minotaur.Utility;
using System.Text.RegularExpressions;
using Xceed.Words.NET;

namespace Minotaur.Areas.Stockkeeper
{
    [Area("Stockkeeper")]
    [Authorize(Roles = Roles.Role_Stockkeeper)]
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<MinotaurUser> _userManager;

        public StockController(ApplicationDbContext db, UserManager<MinotaurUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private async Task<DataByStock> GetStockDataAsync()
        {
            MinotaurUser? minotaurUser = await _userManager.GetUserAsync(User);

            DataByStock? dataByStock = await _db.Workers.Where(w => w.UserId == minotaurUser.Id)
                    .Join(_db.Offices, worker => worker.OfficeId, office => office.Id, (worker, office) => new { Worker = worker, Office = office })
                    .Select(result => new DataByStock
                    {
                        MinotaurUser = minotaurUser,
                        StockKeeper = result.Worker,
                        Stock = result.Office,
                        Records = _db.StockMagazine.Where(s => s.StockId == result.Office.Id).ToList(),
                    }).FirstOrDefaultAsync();

            return dataByStock;
        }

        public async Task<IActionResult> Index()
        {
            DataByStock? dataByStock = await GetStockDataAsync();
            return View(dataByStock.Stock);
        }

        public async Task<IActionResult> GetStock()
        {
            DataByStock? dataByStock = await GetStockDataAsync();

            var stockJournal = dataByStock.Records.Where(u => u.ResponsiblePersonId == dataByStock.StockKeeper.WorkerId)
                .Join(_db.Products, s => s.ProductId, b => b.ProductId, (s, b) => new
                {
                    id = s.Id,
                    productId = s.ProductId,
                    time = s.Time.ToString("dd/MM/yyyy hh:mm"),
                    operation = s.Operation,
                    nameProduct = b.Name,
                    count = s.Count,
                    totalProduct = dataByStock.Records.Where(i => i.ProductId == s.ProductId).Sum(s => s.Count),
                    shelfNumber = s.ShelfNumber,
                    isNeed = s.IsNeed,
                }).ToArray();

            return Json(new { data = stockJournal });
        }


        [HttpPost]
        public async Task<IActionResult> AddProductInStock(int productId, int numberShelf, int productCount)
        {
            Product? product = await _db.Products.FindAsync(productId);
            if (product == null) { return BadRequest("Такого товара нет в базе. Проверьте правильность введенных данных."); }

            DataByStock? dataByStock = await GetStockDataAsync();
            if (dataByStock.Stock == null) { return BadRequest("Склад не найден."); }

            RecordStock? shelfWithSameProduct = dataByStock.Records?.Where(p => p.ProductId == product.ProductId).Where(s => s.ShelfNumber == numberShelf).FirstOrDefault();

            if (shelfWithSameProduct != null)
            {
                shelfWithSameProduct.Count += productCount;
                _db.StockMagazine.Update(shelfWithSameProduct);
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                var stockkeper = await _db.Workers.FirstOrDefaultAsync(u => u.UserId == user.Id);

                RecordStock record = new()
                {
                    StockId = dataByStock.Stock.Id,
                    ResponsiblePersonId = stockkeper.WorkerId,
                    Time = MoscowTime.GetTime(),
                    ProductId = productId,
                    Count = productCount,
                    ShelfNumber = numberShelf,
                    Operation = OperationStock.ReceiptOfGoods,
                    IsNeed = await _db.StockMagazine.Where(u => u.ProductId == productId).Select(u => u.IsNeed).FirstOrDefaultAsync(),
                };
                await _db.StockMagazine.AddAsync(record);
            }

            await _db.SaveChangesAsync();
            return Ok($"Товар {product.Name} добавлен на полку {numberShelf} в количестве {productCount} шт.");

        }

        [HttpPost]
        public async Task<IActionResult> ChangeShelfProduct(string recordId, int productCount, int newShelfNumber)
        {
            DataByStock? dataByStock = await GetStockDataAsync();

            RecordStock? selectedRecord = dataByStock?.Records?.Where(i => i.Id == Guid.Parse(recordId)).FirstOrDefault();
            if (selectedRecord == null)
            {
                return BadRequest($"Запись с ID:{recordId} не найдена.");
            }

            if (selectedRecord.ShelfNumber == newShelfNumber)
            {
                return Ok($"Перемещение на ту же полку.");
            }

            RecordStock? productOnShelf = dataByStock?.Records?.Where(u => u.ProductId == selectedRecord.ProductId).Where(s => s.ShelfNumber == newShelfNumber).FirstOrDefault();
            selectedRecord.Count -= productCount;

            if (selectedRecord.Count <= 0)
            {
                dataByStock?.Records?.Remove(selectedRecord);
            }
            else
            {
                _db.StockMagazine.Update(selectedRecord);
            }


            if (productOnShelf != null)
            {
                productOnShelf.Count += productCount;

                _db.StockMagazine.Update(productOnShelf);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                RecordStock newRecord = new()
                {
                    StockId = dataByStock.Stock.Id,
                    ResponsiblePersonId = dataByStock.StockKeeper.WorkerId,
                    Time = MoscowTime.GetTime(),
                    ProductId = selectedRecord.ProductId,
                    ShelfNumber = newShelfNumber,
                    Count = productCount,
                    Operation = OperationStock.MovementOfGoods,
                    IsNeed = await _db.StockMagazine.Where(u => u.ProductId == selectedRecord.ProductId).Select(u => u.IsNeed).FirstOrDefaultAsync(),
                };

                await _db.StockMagazine.AddAsync(newRecord);
                await _db.SaveChangesAsync();

                return Ok();
            }
        }

        [HttpPost]
        public async Task<IActionResult> SelectProductToPurchase(int productId, params string[] products)
        {
            DataByStock? dataByStock = await GetStockDataAsync();

            if (dataByStock.Records == null) { BadRequest($"Отсутсвуют записи по складу {dataByStock.Stock.Name}."); }

            var recordsWithNeedProduct = dataByStock.Records.Where(p => p.ProductId == productId).ToArray();

            foreach (var product in recordsWithNeedProduct)
            {
                product.IsNeed = product.IsNeed ? false : true;
            }

            _db.StockMagazine.UpdateRange(recordsWithNeedProduct);
            await _db.SaveChangesAsync();

            return Ok();
        }

        //For PurchaseRequest

        [HttpGet]
        public IActionResult PurchaseRequest()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTablePurchaseRequest()
        {
            DataByStock? dataByStock = await GetStockDataAsync();
            if (dataByStock.Records == null) { return BadRequest($"Отсутсвуют записи по складу {dataByStock.Stock.Name}."); }


            var request = dataByStock.Records.Where(p => p.IsNeed == true)
                .Join(_db.Products, s => s.ProductId, b => b.ProductId, (s, b) => new
                {
                    s.ProductId,
                    TitleProduct = _db.Products.Where(u => u.ProductId == s.ProductId).Select(u => u.Name).FirstOrDefault(),
                    TotalProduct = _db.StockMagazine.Where(u => u.IsNeed == true).Where(i => i.ProductId == s.ProductId).Select(u => u.Count).Sum(),
                }).Distinct().ToArray();


            return Json(new { data = request });
        }


        [HttpPost]
        public async Task<IActionResult> OrderProducts([FromBody] RecordStock[] purchaseRequestData)
        {
            if (purchaseRequestData == null) { return BadRequest("Список товаров на закупку пуст."); }

            string productDataOnPurchase = "";

            DataByStock dataByStock = await GetStockDataAsync();

            foreach (var item in purchaseRequestData)
            {
                productDataOnPurchase += item.ProductId.ToString() + ":";
                productDataOnPurchase += item.Count.ToString() + "/";
            }

            RecordStock recordStockPurchase = new()
            {
                StockId = dataByStock.Stock.Id,
                ResponsiblePersonId = dataByStock.StockKeeper.WorkerId,
                Time = MoscowTime.GetTime(),
                Operation = OperationStock.ApplicationForPurchaseOfGoods,
                ProductDataOnPurchase = productDataOnPurchase
            };

            await _db.StockMagazine.AddAsync(recordStockPurchase);


            //-----------------------------------------------------------------------------------------------------------------------------


            string templatePath = "F:\\GitHub\\Minotaur\\Minotaur\\Areas\\Stockkeeper\\Sample\\Заявка на закупку товаров.docx";
            string nameFile = $"Заявка_{recordStockPurchase.Id}";
            string filledFilePath = $"F:\\GitHub\\Minotaur\\Minotaur\\Areas\\Stockkeeper\\PurchaseRequisitions\\{nameFile}.docx";


            System.IO.File.Copy(templatePath, filledFilePath, true);

            using (WordprocessingDocument doc = WordprocessingDocument.Open(templatePath, true))
            {
                // Получаем основную часть документа
                MainDocumentPart mainPart = doc.MainDocumentPart;

                // Получаем текст документа
                string docText;
                using (StreamReader sr = new StreamReader(mainPart.GetStream()))
                {
                    docText = sr.ReadToEnd();
                }

                // Заменяем значения в тексте документа
                Dictionary<string, string> replacements = new Dictionary<string, string>{
                    {"Number", recordStockPurchase.Id.ToString()},
                    {"Street", dataByStock.Stock.Street.ToString()},
                    {"City", dataByStock.Stock.City.ToString()},
                    {"ApplicationTime", recordStockPurchase.Time.ToString("dd.MM.yyyy")},
                    {"FirstName", dataByStock.MinotaurUser.FirstName.ToString()},
                    {"LastName", dataByStock.MinotaurUser.LastName.ToString()},
                    {"Surname", dataByStock.MinotaurUser.Surname.ToString()},
                    {"Phone", dataByStock.MinotaurUser.PhoneNumber.ToString()} };


                Regex regexText = new Regex(string.Join("|", replacements.Keys));
                docText = regexText.Replace(docText, match => replacements[match.Value]);

                // Записываем измененный текст обратно в документ
                using (StreamWriter sw = new StreamWriter(mainPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }

                // Добавляем таблицу в документ
                Table table = mainPart.Document.Body.Elements<Table>().FirstOrDefault();
                TableProperties tableProperties = table.GetFirstChild<TableProperties>();

                foreach (var replacement in purchaseRequestData)
                {
                    TableRow newRow = new TableRow();

                    TableCell cellId = new TableCell(new Paragraph(new Run(new Text("-"))));
                    TableCell productNameCell = new TableCell(new Paragraph(new Run(new Text(replacement.ProductName + ", " + replacement.ProductId.ToString()))));
                    TableCell countCell = new TableCell(new Paragraph(new Run(new Text(replacement.Count.ToString()))));

                    newRow.AppendChild(cellId);
                    newRow.AppendChild(productNameCell);
                    newRow.AppendChild(countCell);

                    TableCellProperties cellProperties = new TableCellProperties();
                    cellProperties.Append(tableProperties.CloneNode(true));

                    cellId.AppendChild(cellProperties.CloneNode(true));
                    productNameCell.AppendChild(cellProperties.CloneNode(true));
                    countCell.AppendChild(cellProperties.CloneNode(true));

                    table.AppendChild(newRow);
                }

                // Сохраняем изменения в документе
                mainPart.Document.Save();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filledFilePath);

            var contentDispositionHeader = new ContentDispositionHeaderValue("attachment")
            {
                FileName = nameFile
            };

            Response.Headers.Add(HeaderNames.ContentDisposition, contentDispositionHeader.ToString());
            Response.Headers.Add(HeaderNames.XContentTypeOptions, "nosniff");

            await _db.SaveChangesAsync();
            return new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
    }
}
