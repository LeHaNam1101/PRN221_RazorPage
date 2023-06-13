using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPage.Model;
using System.Text.Json;

namespace RazorPage.Pages.Product
{
    public class DetailModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;
        public DetailModel(PRN221_DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public Model.Product Product { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await dBContext.Products.Where(x => x.ProductId == id).Include(c => c.Category).FirstOrDefaultAsync();

            return Page();
        }
    }
}
