using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPage.Model
{
    public partial class Account
    {
        public int AccountId { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email must not empty")]
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password must not empty")]
        public string? Password { get; set; }

        public string? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public int? Role { get; set; }
        public DateTime? CreateAt { get; set; }
        public bool IsActive { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
