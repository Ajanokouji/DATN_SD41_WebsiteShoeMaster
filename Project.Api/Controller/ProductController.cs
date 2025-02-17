using Microsoft.AspNetCore.Mvc;
using Project.Business.Implement;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.ApiUtils.Controllers;
using SERP.Framework.ApiUtils.Responses;
using SERP.Framework.ApiUtils.Utils;
using SERP.Framework.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseControllerApi
    {
        private readonly IProductBusiness _productBusiness;

        public ProductController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, IProductBusiness productBusiness) : base(httpRequestHelper, logger)
        {
            _productBusiness = productBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var product = await _productBusiness.FindAsync(id);
                return product;
            });
        }
        [HttpPost("filter")]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var products = await _productBusiness.GetAllAsync(queryModel);
                return products;
            });

        }

        [HttpPost("count")]
        public async Task<IActionResult> GetProductCount([FromQuery] ProductQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
              {
                  var count = await _productBusiness.GetCountAsync(queryModel);
                  return count;
              });
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductEntity productEntity)
        {
            return await ExecuteFunction(async () =>
            {
                var createdProduct = await _productBusiness.SaveAsync(productEntity);
                return createdProduct;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<ProductEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchProduct(Guid id, [FromBody] ProductEntity productEntity)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _productBusiness.FindAsync(id);
                if (id != productEntity.Id || exist ==null)
                    throw new ArgumentException("Not Found");
                var updatedProduct = await _productBusiness.PatchAsync(productEntity);
                return updatedProduct;
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductEntity>> DeleteProduct(Guid id)
        {
            var deletedProduct = await _productBusiness.DeleteAsync(id);
            return Ok(deletedProduct);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<ProductEntity>>> DeleteProducts([FromBody] Guid[] ids)
        {
            var deletedProducts = await _productBusiness.DeleteAsync(ids);
            return Ok(deletedProducts);
        }
    }
}
