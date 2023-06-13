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
    public class ActivationModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;
        public ActivationModel(PRN221_DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public async Task<IActionResult> OnGetActiveAsync(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var accountToUpdate = await dBContext.Accounts.FirstAsync(a => a.AccountId == id);
            if(accountToUpdate == null)
            {
                return NotFound();
            }
            accountToUpdate.IsActive = true;
            await dBContext.SaveChangesAsync();

            return Redirect("/admin/customer");
        }

        public async Task<IActionResult> OnGetDeactiveAsync(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var accountToUpdate = await dBContext.Accounts.FirstAsync(a => a.AccountId == id);
            if (accountToUpdate == null)
            {
                return NotFound();
            }
            accountToUpdate.IsActive = false;
            await dBContext.SaveChangesAsync();

            return Redirect("/admin/customer");
        }
    }
}
