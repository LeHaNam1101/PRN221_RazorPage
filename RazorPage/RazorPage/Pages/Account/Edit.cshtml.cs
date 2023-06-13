using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPage.Model;
using System;
using System.Text.Json;

namespace RazorPage.Pages.Account
{
    public class EditModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;
        public EditModel(PRN221_DBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        [BindProperty]
        public Model.Account Account { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            String json = HttpContext.Session.GetString("CusSession");
            if (json == null)
            {
                return NotFound();
            }
            else
            {
                Model.Account acc = JsonSerializer.Deserialize<Model.Account>(json);
                Account = await dBContext.Accounts.Include(c => c.Customer).FirstAsync(a => a.AccountId == acc.AccountId);
                if (Account == null)
                {
                    return NotFound();
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var accountToUpdate = await dBContext.Accounts.Include(e=>e.Customer).FirstAsync(e=>e.AccountId == id);

            if (accountToUpdate == null)
            {
                return NotFound();
            }

            accountToUpdate.Customer.ContactName = Account.Customer.ContactName;
            accountToUpdate.Customer.ContactTitle = Account.Customer.ContactTitle;
            accountToUpdate.Customer.CompanyName = Account.Customer.CompanyName;
            accountToUpdate.Customer.Address = Account.Customer.Address;

            await dBContext.SaveChangesAsync();

            return Redirect("/account/profile");
        }

    }
}
