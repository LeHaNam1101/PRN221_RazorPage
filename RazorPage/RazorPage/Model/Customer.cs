using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RazorPage.Model
{
    public partial class Customer
    {
        public Customer()
        {
            Accounts = new HashSet<Account>();
            Orders = new HashSet<Order>();
        }

        public string CustomerId { get; set; } = null!;
        [Required(ErrorMessage = "Company name must not empty")]
        public string CompanyName { get; set; } = null!;
        [Required(ErrorMessage = "Contact name must not empty")]
        public string? ContactName { get; set; }
        [Required(ErrorMessage = "Contact title must not empty")]
        public string? ContactTitle { get; set; }
        [Required(ErrorMessage = "Address must not empty")]
        public string? Address { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
