using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RazorPage.Model
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name must not empty")]
        public string ProductName { get; set; } = null!;
        
        [Required(ErrorMessage = "Category must not empty")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Quantity per unit must not empty")]
        public string? QuantityPerUnit { get; set; }

        [DataType(DataType.Currency)]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid decimal Number")]
        [Required(ErrorMessage = "Unit price per unit must not empty")]
        public decimal? UnitPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Required(ErrorMessage = "Unit in stock must not empty")]
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }

        public bool Discontinued { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
