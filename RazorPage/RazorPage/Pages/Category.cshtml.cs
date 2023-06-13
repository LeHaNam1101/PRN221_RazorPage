using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPage.Model;

namespace RazorPage.Pages
{
    public class CategoryModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;
        public CategoryModel(PRN221_DBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public IList<Category> Category { get; set; }

        public IList<Model.Product> Product { get; set; }
        public int PageSize { get; set; }
        public int PageNo { get; set; }

        public int TotalProducts { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string selected, int p = 1, int s = 5)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category = await dBContext.Categories.ToListAsync();

            var count_product = await dBContext.Products.Where(p => p.CategoryId == id).ToListAsync();
            

            if(selected == null)
            {
                Product = await dBContext.Products.Where(p => p.CategoryId == id).Skip((p - 1) * s).Take(s).ToListAsync();
            }
            else
            {
                if (selected.Equals("desc"))
                {
                    Product = await dBContext.Products.Where(p => p.CategoryId == id).OrderByDescending(x => x.UnitPrice).Skip((p - 1) * s).Take(s).ToListAsync();
                }
                else if (selected.Equals("asc"))
                {
                    Product = await dBContext.Products.Where(p => p.CategoryId == id).OrderBy(x => x.UnitPrice).Skip((p - 1) * s).Take(s).ToListAsync();
                }
            }


            PageSize = s;
            PageNo = p;

            TotalProducts = count_product.Count();

            if (Product.Count == 0)
            {
                ViewData["msg"] = "Product not found.";
            }

            ViewData["cateName"] = dBContext.Categories.Where(c=>c.CategoryId == id).First().CategoryName;

            return Page();

        }
    }
}
