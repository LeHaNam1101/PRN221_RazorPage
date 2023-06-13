using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPage.Model;
using System.Text.Json;

namespace RazorPage.Pages.Admin
{
    [Authorize]
    public class OrderDetailModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;
        public OrderDetailModel(PRN221_DBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public IList<Model.OrderDetail> OrderDetail { get; set; }

        public Model.Order Order { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            OrderDetail = await dBContext.OrderDetails.Where(x => x.OrderId == id).Include(p => p.Product).ToListAsync();
            Order = await dBContext.Orders.Where(x => x.OrderId == id).FirstOrDefaultAsync();
            return Page();
        }
    }
}
