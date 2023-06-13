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
    public class CreateProductModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;

        private readonly IHubContext<HubServer> hubContext;
        public CreateProductModel(PRN221_DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;
        }

        [BindProperty]
        public Model.Product Product { get; set; }

        public IList<Model.Category> Category { get; set; }
        public async Task OnGetAsync()
        {
            Category = await dBContext.Categories.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Category = await dBContext.Categories.ToListAsync();
                return Page();
            }

            dBContext.Products.Add(Product);
            await dBContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("ReloadProduct");
            Category = await dBContext.Categories.ToListAsync();
            return Page();
        }
    }
}
