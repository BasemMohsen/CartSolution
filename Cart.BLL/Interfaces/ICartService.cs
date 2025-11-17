using Company.Cart.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Cart.BLL.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartItem>> GetItemsAsync(string cartId);
        Task AddItemAsync(string cartId, CartItem item);
        Task<bool> RemoveItemAsync(string cartId, int itemId);
    }
}
