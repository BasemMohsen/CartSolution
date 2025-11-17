using Company.Cart.Core.Models;
using Company.Cart.DAL.Interfaces;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Cart.DAL.LiteDb
{
    public class LiteDbCartRepository : ICartRepository
    {
        private readonly ILiteDatabase _db;
        private readonly ILiteCollection<Company.Cart.Core.Models.Cart> _collection;

        // Accept a LiteDB instance so we can pass MemoryStream-based DB for integration tests
        public LiteDbCartRepository(ILiteDatabase database)
        {
            _db = database;
            _collection = _db.GetCollection<Company.Cart.Core.Models.Cart>("carts");
            _collection.EnsureIndex(c => c.Id, true);
        }

        public Task<Company.Cart.Core.Models.Cart?> GetCartAsync(string cartId)
        {
            return Task.FromResult(_collection.FindById(new BsonValue(cartId)));
        }

        public Task<IEnumerable<CartItem>> GetCartItemsAsync(string cartId)
        {
            var cart = _collection.FindById(new BsonValue(cartId));
            return Task.FromResult(cart?.Items ?? Enumerable.Empty<CartItem>());
        }

        public Task AddOrUpdateItemAsync(string cartId, CartItem item)
        {
            var cart = _collection.FindById(new BsonValue(cartId)) ?? new Company.Cart.Core.Models.Cart { Id = cartId };
            
            var existing = cart.Items.FirstOrDefault(i => i.Id == item.Id);
            if (existing == null)
                cart.Items.Add(item);
            else
            {
                existing.Quantity = item.Quantity;
                existing.Name = item.Name;
                existing.Price = item.Price;
                existing.ImageUrl = item.ImageUrl;
                existing.ImageAlt = item.ImageAlt;
            }

            _collection.Upsert(cart);
            return Task.CompletedTask;
        }

        public Task RemoveItemAsync(string cartId, int itemId)
        {
            var cart = _collection.FindById(new BsonValue(cartId));
            if (cart != null)
            {
                cart.Items.RemoveAll(i => i.Id == itemId);
                _collection.Upsert(cart);
            }
            return Task.CompletedTask;
        }

        public Task DeleteCartAsync(string cartId)
        {
            _collection.Delete(new BsonValue(cartId));
            return Task.CompletedTask;
        }
    }
}
