using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPage.Model;
//using RazorPage.Models;
using System.Collections.Generic;
using static RazorPage.Pages.Account.CartModel;

namespace RazorPage.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;
        public IndexModel(PRN221_DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public IList<Model.Category> Category { get; set; }

        public IList<Model.Product> Product { get; set; }

        public IList<Model.OrderDetail> OrderDetail { get; set; }

        public async Task OnGetAsync()
        {
            Category = await dBContext.Categories.ToListAsync();
            Product = await dBContext.Products
                    .Include(d => d.OrderDetails).ToListAsync();
            OrderDetail = await dBContext.OrderDetails.Include(p => p.Product).ToListAsync();
        }
    }
}
