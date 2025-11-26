using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Cart.Core.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? ImageAlt { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
    }
}
