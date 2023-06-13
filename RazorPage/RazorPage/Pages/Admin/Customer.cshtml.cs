using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPage.Model;

namespace RazorPage.Pages.Admin
{
    [Authorize]
    public class CustomerModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;
        public CustomerModel(PRN221_DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public IList<Model.Account> Accounts { get; set; }
        public int PageSize { get; set; }
        public int PageNo { get; set; }

        public int TotalCustomers { get; set; }

        public async Task OnGetAsync(string? search, int p = 1, int s = 7)
        {
            if (search == null)
            {
                Accounts = await dBContext.Accounts
                        .Include(c => c.Customer).Where(x => x.CustomerId != null).OrderByDescending(x => x.AccountId).Skip((p - 1) * s).Take(s).ToListAsync();

                var count_account = await dBContext.Accounts.ToListAsync();
                TotalCustomers = count_account.Count();
            }
            else
            {
                Accounts = await dBContext.Accounts
                        .Include(c => c.Customer).Where(x => x.Customer.ContactName.Contains(search)).OrderByDescending(x => x.AccountId).Skip((p - 1) * s).Take(s).ToListAsync();

                var count_account = await dBContext.Accounts.Where(x => x.Customer.ContactName.Contains(search)).ToListAsync();
                TotalCustomers = count_account.Count();
            }

            PageSize = s;
            PageNo = p;

            ViewData["txtSearch"] = search;
            if (Accounts.Count < 1)
            {
                ViewData["msgNullCustomer"] = "Not found customer data";
            }
        }
    }
}
