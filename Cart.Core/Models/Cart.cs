using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Cart.Core.Models
{
    public class Cart
    {
        // Unique cart id maintained by client (e.g. GUID string)
        public string Id { get; set; } = null!;
        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
