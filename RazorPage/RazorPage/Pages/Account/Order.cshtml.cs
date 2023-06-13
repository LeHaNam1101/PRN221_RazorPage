using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPage.Model;
using System.Text.Json;

namespace RazorPage.Pages.Account
{
    public class OrderModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;
        public OrderModel(PRN221_DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        //Khai báo 2 thuộc tính đại diện cho 2 đối tượng kiểu: Account, Customer 

        [BindProperty]

        public IList<OrderDetail> OrderDetail { get; set; }

        [BindProperty]

        public IList<Order> Order { get; set; }
        public async Task OnGetAsync()
        {
            String json = HttpContext.Session.GetString("CusSession");
            if (json == null)
            {
                return;
            }
            else
            {
                Model.Account acc = JsonSerializer.Deserialize<Model.Account>(json);

                Order = await dBContext.Orders.Where(x => x.CustomerId.Equals(acc.CustomerId)).ToListAsync();

                OrderDetail = await dBContext.OrderDetails.Include(o => o.Product)
                            .Where(x=>x.OrderId.Equals(x.Order.OrderId)).ToListAsync();
            }
        }
    }
}
