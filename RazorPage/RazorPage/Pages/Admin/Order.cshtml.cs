using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using RazorPage.Model;
using System.Drawing.Printing;

namespace RazorPage.Pages.Admin
{
    [Authorize]
    public class OrderModel : PageModel
    {

        private readonly PRN221_DBContext dBContext;
        public OrderModel(PRN221_DBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public IList<Order> Orders { get; set; }

        public IList<Order> Order { get; set; }
        public int PageSize { get; set; }
        public int PageNo { get; set; }

        public int TotalOrders { get; set; }

        [BindProperty]
        public DateTime fromDate { get; set; }

        [BindProperty]
        public DateTime toDate { get; set; }
        public async Task OnGetAsync(int p = 1, int s = 7, DateTime? fromDate = null, DateTime? toDate = null, string? status = null)
        {
            if (fromDate == null && toDate == null)
            {
                Orders = await dBContext.Orders.Include(c => c.Customer).OrderByDescending(x => x.OrderId).Skip((p - 1) * s).Take(s).ToListAsync();
            }
            else
            {
                Orders = await dBContext.Orders.Include(c => c.Customer).Where(d => d.OrderDate >= fromDate && d.OrderDate <= toDate).OrderByDescending(x => x.OrderId).Skip((p - 1) * s).Take(s).ToListAsync();
            }
            PageSize = s;
            PageNo = p;

            var count_order = await dBContext.Orders.ToListAsync();

            if (fromDate == null && toDate == null)
            {
                count_order = await dBContext.Orders.ToListAsync();
                TotalOrders = count_order.Count();
            }
            else
            {
                count_order = await dBContext.Orders.Where(d => d.OrderDate >= fromDate && d.OrderDate <= toDate).ToListAsync();
                TotalOrders = count_order.Count();
            }


            if (status != null)
            {
                if (status.Equals("complete"))
                {
                    Orders = await dBContext.Orders.Include(c => c.Customer).Where(d => d.ShippedDate != null).OrderByDescending(x => x.OrderId).Skip((p - 1) * s).Take(s).ToListAsync();
                    count_order = await dBContext.Orders.Where(d => d.ShippedDate != null).ToListAsync();
                    TotalOrders = count_order.Count();
                }
                else if (status.Equals("pending"))
                {
                    Orders = await dBContext.Orders.Include(c => c.Customer).Where(d => d.RequiredDate != null && d.ShippedDate == null).OrderByDescending(x => x.OrderId).Skip((p - 1) * s).Take(s).ToListAsync();
                    count_order = await dBContext.Orders.Where(d => d.RequiredDate != null && d.ShippedDate == null).ToListAsync();
                    TotalOrders = count_order.Count();
                }
                else if (status.Equals("cancel"))
                {
                    Orders = await dBContext.Orders.Include(c => c.Customer).Where(d => d.RequiredDate == null).OrderByDescending(x => x.OrderId).Skip((p - 1) * s).Take(s).ToListAsync();
                    count_order = await dBContext.Orders.Where(d => d.RequiredDate == null).ToListAsync();
                    TotalOrders = count_order.Count();
                }
            }
        }

        public async Task<IActionResult> OnGetCancel(int id, int pageNo, int pageSize)
        {
            var orderToUpdate = await dBContext.Orders.FirstAsync(e => e.OrderId == id);

            if (orderToUpdate == null)
            {
                return NotFound();
            }

            orderToUpdate.RequiredDate = null;

            await dBContext.SaveChangesAsync();

            return Redirect("/admin/order?p=" + pageNo + "&s=" + pageSize);
        }

        public async Task<IActionResult> OnPostExport(int pageNo, int pageSize, DateTime? fromDate = null, DateTime? toDate = null, string? status = null)
        {
            var orders = await dBContext.Orders.Include(c => c.Customer).OrderBy(x => x.OrderId)
                .Select(d => new
                {
                    d.OrderId,
                    d.OrderDate,
                    d.ShippedDate,
                    d.Customer.ContactName,
                    d.Freight,
                    Status = d.ShippedDate != null ? "Complete" : 
                             d.RequiredDate!= null && d.ShippedDate == null ? "Pending" :
                             d.RequiredDate == null ? "Canceled" : "",

                }).ToListAsync();

            if(fromDate != null && toDate != null)
            {
                orders = await dBContext.Orders.Include(c => c.Customer).Where(d => d.OrderDate >= fromDate && d.OrderDate <= toDate).OrderBy(x => x.OrderId)
                .Select(d => new
                {
                    d.OrderId,
                    d.OrderDate,
                    d.ShippedDate,
                    d.Customer.ContactName,
                    d.Freight,
                    Status = d.ShippedDate != null ? "Complete" :
                             d.RequiredDate != null && d.ShippedDate == null ? "Pending" :
                             d.RequiredDate == null ? "Canceled" : "",

                }).ToListAsync();
            }

            if (status != null)
            {
                if (status.Equals("complete"))
                {
                    orders = await dBContext.Orders.Include(c => c.Customer).Where(d => d.ShippedDate != null).OrderBy(x => x.OrderId)
                        .Select(d => new
                        {
                            d.OrderId,
                            d.OrderDate,
                            d.ShippedDate,
                            d.Customer.ContactName,
                            d.Freight,
                            Status = "Complete",

                        }).ToListAsync();
                }
                else if (status.Equals("pending"))
                {
                    orders = await dBContext.Orders.Include(c => c.Customer).Where(d => d.RequiredDate != null && d.ShippedDate == null).OrderBy(x => x.OrderId)
                        .Select(d => new
                        {
                            d.OrderId,
                            d.OrderDate,
                            d.ShippedDate,
                            d.Customer.ContactName,
                            d.Freight,
                            Status = "Pending",

                        }).ToListAsync();
                }
                else if (status.Equals("cancel"))
                {
                    orders = await dBContext.Orders.Include(c => c.Customer).Where(d => d.RequiredDate == null).OrderBy(x => x.OrderId)
                        .Select(d => new
                        {
                            d.OrderId,
                            d.OrderDate,
                            d.ShippedDate,
                            d.Customer.ContactName,
                            d.Freight,
                            Status = "Cancel",

                        }).ToListAsync();
                }
            }

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(orders, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"Orders-{DateTime.Now.ToString("yyyyMMdd")}.xlsx";
            if (fromDate != null && toDate != null)
            {
                excelName = $"Orders" + "-" +fromDate + "-" + toDate + ".xlsx";
            }
            if(status != null)
            {
                excelName = $"Orders-" + status + ".xlsx";
            }
            // above I define the name of the file using the current datetime.

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName); // this will be the actual export.
        }
    }
}
