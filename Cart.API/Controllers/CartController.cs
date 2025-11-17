using Microsoft.AspNetCore.Mvc;
using Company.Cart.BLL.Interfaces;
using Company.Cart.Core.Models;
using CartModel = Company.Cart.Core.Models.Cart;

namespace Cart.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Get cart info by cart ID for version 1.
        /// </summary>
        /// <param name="cartId">Unique cart ID</param>
        /// <returns>Cart model or list of cart items</returns>
        [HttpGet("v1/{cartId}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<CartModel>> GetCartV1(string cartId)
        {
            var items = await _cartService.GetItemsAsync(cartId);
            return Ok(new CartModel { Id = cartId, Items = items.ToList() });
        }

        /// <summary>
        /// Get cart info by cart ID for version 2.
        /// </summary>
        /// <param name="cartId">Unique cart ID</param>
        /// <returns>List of cart items</returns>
        [HttpGet("v2/{cartId}")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartV2(string cartId)
        {
            var items = await _cartService.GetItemsAsync(cartId);
            return Ok(items);
        }

        /// <summary>
        /// Add item to cart.
        /// </summary>
        /// <param name="cartId">Unique cart ID</param>
        /// <param name="item">Cart item model</param>
        /// <returns>Status code</returns>
        [HttpPost("{cartId}/items")]
        public async Task<IActionResult> AddItem(string cartId, [FromBody] CartItem item)
        {
            await _cartService.AddItemAsync(cartId, item);
            return Ok();
        }

        /// <summary>
        /// Delete item from cart.
        /// </summary>
        /// <param name="cartId">Unique cart ID</param>
        /// <param name="itemId">Item ID</param>
        /// <returns>Status code</returns>
        [HttpDelete("{cartId}/items/{itemId}")]
        public async Task<IActionResult> DeleteItem(string cartId, int itemId)
        {
            var result = await _cartService.RemoveItemAsync(cartId, itemId);
            if (result)
                return Ok();
            return NotFound();
        }
    }
}