using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPage.Model;
using System.Globalization;

namespace RazorPage.Pages.Admin
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly PRN221_DBContext dBContext;
        public DashboardModel(PRN221_DBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public IList<Order> Orders { get; set; }
        public IList<Order> TotalOrder { get; set; }

        public IList<Customer> Customer { get; set; }

        public IList<Employee> Employees { get; set; }

        public IList<Model.Account> Accounts { get; set; }
        public IList<Model.Account> TotalAccounts { get; set; }

        public int Year { get; set; }
        public async Task OnGetAsync(int? year)
        {
            DateTime today = DateTime.Now;

            DateTime now = DateTime.Now.AddDays(1);
            DateTime sub = DateTime.Now.AddDays(-7);
            Orders = await dBContext.Orders.Where(x => x.OrderDate >= sub 
                            && x.OrderDate <= now).ToListAsync();
            TotalOrder = await dBContext.Orders.OrderByDescending(x=>x.OrderId).ToListAsync();
            Customer = await dBContext.Customers.ToListAsync();
            Employees = await dBContext.Employees.ToListAsync();
            Accounts = await dBContext.Accounts.Where(x => x.CreateAt.Value.Month == today.Month).ToListAsync();
            TotalAccounts = await dBContext.Accounts.ToListAsync();

            if(year == null)
            {
                Year = today.Year;
            }
            else
            {
                Year = (int)year;
            }
        }
    }
}
