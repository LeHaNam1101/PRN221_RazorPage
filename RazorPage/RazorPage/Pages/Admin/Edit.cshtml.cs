using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RazorPage.Model;
using SignalR;

namespace RazorPage.Pages.Admin
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;

        private readonly IHubContext<HubServer> hubContext;
        public EditModel(PRN221_DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;
        }

        [BindProperty]
        public Model.Product Product { get; set; }
        public IList<Model.Category> Category { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product = await dBContext.Products.Where(p => p.ProductId == id).Include(c => c.Category).FirstOrDefaultAsync();
            if (Product == null)
            {
                return NotFound();
            }

            Category = await dBContext.Categories.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                Category = await dBContext.Categories.ToListAsync();
                return Page();
            }

            var productToUpdate = await dBContext.Products.FirstAsync(e => e.ProductId == id);

            if (productToUpdate == null)
            {
                return NotFound();
            }

            productToUpdate.ProductName = Product.ProductName;
            productToUpdate.CategoryId = Product.CategoryId;
            productToUpdate.QuantityPerUnit = Product.QuantityPerUnit;
            productToUpdate.UnitPrice = Product.UnitPrice;
            productToUpdate.UnitsInStock = Product.UnitsInStock;
            productToUpdate.Discontinued = Product.Discontinued;

            await dBContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("ReloadProduct");
            Category = await dBContext.Categories.ToListAsync();

            return Redirect("/admin/product");
        }
    }
}
