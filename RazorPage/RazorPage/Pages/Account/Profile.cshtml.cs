using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPage.Model;
using System.Text.Json;

namespace RazorPage.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;
        public ProfileModel(PRN221_DBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        [BindProperty]
        public Model.Account Account { get; set; }
        public void OnGet()
        {
            String json = HttpContext.Session.GetString("CusSession");
            Model.Account acc = JsonSerializer.Deserialize<Model.Account>(json);
            Account = dBContext.Accounts.Include(e => e.Customer).First(e => e.AccountId == acc.AccountId);
        }
    }
}
