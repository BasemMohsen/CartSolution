using Company.Cart.BLL.Interfaces;
using Company.Cart.Core.Models;
using Company.Cart.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Cart.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repo;
        public CartService(ICartRepository repo)
        {
            _repo = repo;
        }
        public async Task AddItemAsync(string cartId, CartItem item)
        {
            await _repo.AddOrUpdateItemAsync(cartId, item);
        }

        public async Task<IEnumerable<CartItem>> GetItemsAsync(string cartId)
        {
            return await _repo.GetCartItemsAsync(cartId);
        }

        public async Task<bool> RemoveItemAsync(string cartId, int itemId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cartId))
                {
                    throw new ArgumentException("Cart ID cannot be null or empty", nameof(cartId));
                }
                
                await _repo.DeleteCartAsync(cartId);
                return true;
            }
            catch (ArgumentException)
            {
                throw;
            }
        }
    }
}
