using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RazorPage.Model;
using SignalR;
using System.Text;

namespace RazorPage.Pages.Admin
{
    [Authorize]
    public class CreateCustomerModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;

        private readonly IHubContext<HubServer> hubContext;
        public CreateCustomerModel(PRN221_DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;
        }

        [BindProperty]
        public Model.Account Account { get; set; }

        [BindProperty]
        public Model.Customer Customer { get; set; }
        public async Task OnGetAsync()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var account = await dBContext.Accounts.SingleOrDefaultAsync(x => x.Email.Equals(Account.Email));
            if (account == null)
            {
                // Xử lí insert xuống: Customer, Account
                var customer = new Customer()
                {
                    CustomerId = GenerateCusID(5),
                    CompanyName = Customer.CompanyName,
                    ContactName = Customer.ContactName,
                    Address = Customer.Address,
                    ContactTitle = Customer.ContactTitle
                };

                var new_account = new Model.Account()
                {
                    Email = Account.Email,
                    Password = Encrypt(Account.Password),
                    CustomerId = customer.CustomerId,
                    Role = 2,
                    CreateAt = DateTime.Now
                };

                // Bổ sung 2 đối tượng vào DBContext
                await dBContext.AddAsync(customer);
                await dBContext.AddAsync(new_account);
                await dBContext.SaveChangesAsync();
                await hubContext.Clients.All.SendAsync("ReloadProduct");
                ViewData["msg"] = "Successfully!!";
                return RedirectToPage("/admin/customer");
            }
            else
            {
                ViewData["msg"] = "Email is existed, pls choose another.";
                return Page();
            }
        }

        private string GenerateCusID(int length)
        {
            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();
            char letter;
            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }

        public string Encrypt(string text)
        {
            var encryptString = EncryptDecryptManager.Encrypt(text);
            return encryptString;
        }
    }
}
