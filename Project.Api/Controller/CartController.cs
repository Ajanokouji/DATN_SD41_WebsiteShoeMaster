using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.ApiUtils.Controllers;
using SERP.Framework.ApiUtils.Responses;
using SERP.Framework.ApiUtils.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : BaseControllerApi
    {
        private readonly ICartBusiness _cartBusiness;

        public CartController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, ICartBusiness cartBusiness) : base(httpRequestHelper, logger)
        {
            _cartBusiness = cartBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCart(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var cart = await _cartBusiness.FindAsync(id);
                return cart;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetCarts([FromQuery] CartQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var carts = await _cartBusiness.GetAllAsync(queryModel);
                return carts;
            });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetCartCount([FromQuery] CartQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _cartBusiness.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody] Cart cart)
        {
            return await ExecuteFunction(async () =>
            {
                var createdCart = await _cartBusiness.SaveAsync(cart);
                return createdCart;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<Cart>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchCart(Guid id, [FromBody] Cart cart)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _cartBusiness.FindAsync(id);
                if (id != cart.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedCart = await _cartBusiness.PatchAsync(cart);
                return updatedCart;
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Cart>> DeleteCart(Guid id)
        {
            var deletedCart = await _cartBusiness.DeleteAsync(id);
            return Ok(deletedCart);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<Cart>>> DeleteCarts([FromBody] Guid[] ids)
        {
            var deletedCarts = await _cartBusiness.DeleteAsync(ids);
            return Ok(deletedCarts);
        }
    }
}
