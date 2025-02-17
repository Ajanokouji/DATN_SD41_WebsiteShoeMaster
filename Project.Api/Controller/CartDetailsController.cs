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
    public class CartDetailsController : BaseControllerApi
    {
        private readonly ICartDetailsBusiness _cartDetailsBusiness;

        public CartDetailsController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, ICartDetailsBusiness cartDetailsBusiness) : base(httpRequestHelper, logger)
        {
            _cartDetailsBusiness = cartDetailsBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartDetail(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var cartDetail = await _cartDetailsBusiness.FindAsync(id);
                return cartDetail;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetCartDetails([FromQuery] CartDetailsQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var cartDetails = await _cartDetailsBusiness.GetAllAsync(queryModel);
                return cartDetails;
            });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetCartDetailsCount([FromQuery] CartDetailsQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _cartDetailsBusiness.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCartDetail([FromBody] CartDetails cartDetail)
        {
            return await ExecuteFunction(async () =>
            {
                var createdCartDetail = await _cartDetailsBusiness.SaveAsync(cartDetail);
                return createdCartDetail;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<CartDetails>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchCartDetail(Guid id, [FromBody] CartDetails cartDetail)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _cartDetailsBusiness.FindAsync(id);
                if (id != cartDetail.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedCartDetail = await _cartDetailsBusiness.PatchAsync(cartDetail);
                return updatedCartDetail;
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CartDetails>> DeleteCartDetail(Guid id)
        {
            var deletedCartDetail = await _cartDetailsBusiness.DeleteAsync(id);
            return Ok(deletedCartDetail);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<CartDetails>>> DeleteCartDetails([FromBody] Guid[] ids)
        {
            var deletedCartDetails = await _cartDetailsBusiness.DeleteAsync(ids);
            return Ok(deletedCartDetails);
        }
    }
}
