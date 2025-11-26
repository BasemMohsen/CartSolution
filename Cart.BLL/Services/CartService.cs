using Cart.BLL.Interfaces;
using Company.Cart.Core.Models;
using Company.Cart.DAL.Interfaces;
using Cart.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.BLL.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repo;

        public CartService(ICartRepository repo)
        {
            _repo = repo;
        }

        public async Task UpdateProductInCartsAsync(ProductUpdatedIntegrationEvent productUpdate)
        {
            var carts = await _repo.GetAllCartsAsync();

            foreach (var cart in carts)
            {
                var updated = false;

                foreach (var item in cart.Items)
                {
                    if (item.ProductId == productUpdate.ProductId)
                    {
                        item.Name = productUpdate.Name;
                        item.Price = productUpdate.Price;
                        item.ImageUrl = productUpdate.ImageUrl;
                        updated = true;
                    }
                }

                if (updated)
                {
                    await _repo.AddOrUpdateCartAsync(cart);
                }
            }
        }

        public async Task<IEnumerable<CartItem>> GetItemsAsync(string cartId)
        {
            return await _repo.GetCartItemsAsync(cartId);
        }

        public async Task AddItemAsync(string cartId, CartItem item)
        {
            await _repo.AddOrUpdateItemAsync(cartId, item);
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
