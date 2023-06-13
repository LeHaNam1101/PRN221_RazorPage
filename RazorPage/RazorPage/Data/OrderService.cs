using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using RazorPage.Model;
using System.Data;

namespace RazorPage.Data
{
    public class OrderService
    {
        private readonly PRN221_DBContext dBContext;
        public DataTable GetOrderList()
        {
            var dt = new DataTable();
            dt.Columns.Add("Item");
            dt.Columns.Add("Qty");
            dt.Columns.Add("Price");
            dt.Columns.Add("Discount");
            dt.Columns.Add("Amount");
            DataRow row;

            var newestOrder = dBContext.Orders.OrderByDescending(p => p.OrderId).FirstOrDefault();
            var get_order = dBContext.OrderDetails.OrderByDescending(p => p.OrderId).Where(p => p.OrderId == newestOrder.OrderId).Include(x => x.Product).ToList();

            foreach (var item in get_order)
            {
                row = dt.NewRow();
                row["Item"] = item.Product.ProductName;
                row["Qty"] = item.Quantity;
                row["Price"] = item.UnitPrice;
                row["Discount"] = item.Discount * 100 + " %";
                row["Amount"] = item.UnitPrice * item.Quantity;
            }


            return dt;
        }
    }
}
