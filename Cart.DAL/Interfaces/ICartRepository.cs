using Company.Cart.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Cart.DAL.Interfaces
{
    public interface ICartRepository
    {
        Task<Company.Cart.Core.Models.Cart?> GetCartAsync(string cartId);
        Task<IEnumerable<CartItem>> GetCartItemsAsync(string cartId);
        Task AddOrUpdateItemAsync(string cartId, CartItem item);
        Task RemoveItemAsync(string cartId, int itemId);
        Task DeleteCartAsync(string cartId);
    }
}
