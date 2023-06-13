using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RazorPage.Model;

namespace RazorPage.Pages.Admin
{
    [Authorize]
    public class ProductModel : PageModel
    {
        private Microsoft.Extensions.Hosting.IHostingEnvironment _environment;

        private readonly PRN221_DBContext dBContext;
        public ProductModel(Microsoft.Extensions.Hosting.IHostingEnvironment environment, PRN221_DBContext dBContext)
        {
            _environment = environment;
            this.dBContext = dBContext;
        }

        [BindProperty]
        public IFormFile UploadedExcelFile { get; set; }

        public IList<Model.Product> Product { get; set; }
        public IList<Model.Category> Category { get; set; }
        public int PageSize { get; set; }
        public int PageNo { get; set; }

        public int TotalProducts { get; set; }

        public async Task OnGetAsync(int? category, string? search, int p = 1, int s = 7)
        {
            if(category == null)
            {
                if(search == null)
                {
                    Product = await dBContext.Products
                            .Include(c => c.Category).OrderByDescending(x => x.ProductId).Skip((p - 1) * s).Take(s).ToListAsync();

                    var count_product = await dBContext.Products.ToListAsync();
                    TotalProducts = count_product.Count();
                }
                else
                {
                    Product = await dBContext.Products
                            .Include(c => c.Category).Where(s=>s.ProductName.Contains(search)).OrderByDescending(x => x.ProductId).Skip((p - 1) * s).Take(s).ToListAsync();

                    var count_product = await dBContext.Products.Where(s => s.ProductName.Contains(search)).ToListAsync();
                    TotalProducts = count_product.Count();
                }
            }
            else
            {
                if(search == null)
                {
                    Product = await dBContext.Products
                            .Include(c => c.Category).Where(a => a.CategoryId == category).OrderByDescending(x => x.ProductId).Skip((p - 1) * s).Take(s).ToListAsync();

                    var count_product = await dBContext.Products.Where(a => a.CategoryId == category).ToListAsync();
                    TotalProducts = count_product.Count();
                }
                else
                {
                    Product = await dBContext.Products
                            .Include(c => c.Category).Where(a => a.CategoryId == category && a.ProductName.Contains(search)).OrderByDescending(x => x.ProductId).Skip((p - 1) * s).Take(s).ToListAsync();

                    var count_product = await dBContext.Products.Where(a => a.CategoryId == category && a.ProductName.Contains(search)).ToListAsync();
                    TotalProducts = count_product.Count();
                }
            }



            Category = await dBContext.Categories.ToListAsync();

            PageSize = s;
            PageNo = p;

            ViewData["txtSearch"] = search;
            if(Product.Count < 1)
            {
                ViewData["msgNullProduct"] = "Not found product data";
            }
        }

        public async Task<IActionResult> OnGetDelete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var product_detele = await dBContext.Products.FindAsync(id);

            if(product_detele == null)
            {
                return NotFound();
            }

            var check_product_in_order = await dBContext.OrderDetails.Where(p => p.ProductId == id).FirstOrDefaultAsync();

            if (check_product_in_order == null)
            {
                dBContext.Products.Remove(product_detele);

                await dBContext.SaveChangesAsync();

                return RedirectToPage("/admin/product");
            }
            else
            {
                HttpContext.Session.SetString("msgErrDel", "This product already ordered, you can not delete it");
                return RedirectToPage("/admin/product");
            }
        }


        public async Task<IActionResult> OnPostExcelUploadAsync()
        {
            return await Import(UploadedExcelFile);

        }

        public async Task<IActionResult> Import(IFormFile formFile)
        {
            if (formFile == null || formFile.Length <= 0)
            {
                HttpContext.Session.SetString("msgErrDel", "This is not a valid file.");
                return RedirectToPage("/admin/product");
            }

            if (formFile.Length > 500000) // byte
            {
                HttpContext.Session.SetString("msgErrDel", "File should be less then 0.5 Mb");
                return RedirectToPage("/admin/product");
            }

            if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                HttpContext.Session.SetString("msgErrDel", "Wrong file format. Should be xlsx.");
                return RedirectToPage("/admin/product");
            }

            var newProduct = new List<Model.Product>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;


                        for (int row = 2; row <= rowCount; row++)
                        {
                            newProduct.Add(new Model.Product
                            {
                                ProductName = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                CategoryId = int.Parse(worksheet.Cells[row, 3].Value.ToString().Trim()),
                                QuantityPerUnit = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                UnitPrice = decimal.Parse(worksheet.Cells[row, 5].Value.ToString().Trim()),
                                UnitsInStock = short.Parse(worksheet.Cells[row, 6].Value.ToString().Trim()),
                                UnitsOnOrder = 0,
                                ReorderLevel = 0,
                                Discontinued = bool.Parse(worksheet.Cells[row, 7].Value.ToString().Trim()),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("msgErrDel", "Error while parsing the file. Check the column order and format.");
                return RedirectToPage("/admin/product");
            }

            dBContext.Products.AddRangeAsync(newProduct);
            dBContext.SaveChangesAsync();
            //oldInvoiceList = _context.InvoiceTable.ToList();

            return RedirectToPage("/admin/product");
        }

        public async Task<IActionResult> OnPostExport()
        {
            var products = await dBContext.Products.Include(c => c.Category).OrderBy(x => x.ProductId)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Category.CategoryName,
                    p.QuantityPerUnit,
                    p.UnitPrice,
                    p.UnitsInStock,
                    p.Discontinued,

                }).ToListAsync();

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(products, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"Products-{DateTime.Now.ToString("yyyyMMdd")}.xlsx";
            // above I define the name of the file using the current datetime.

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName); // this will be the actual export.
        }

    }
}
