using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.ApiUtils.Controllers;
using SERP.Framework.ApiUtils.Responses;
using SERP.Framework.ApiUtils.Utils;

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
        [ProducesResponseType(typeof(ResponseObject<ProductEntity>),StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var product = await _productBusiness.FindAsync(id);
                return product;
            });
        }
        [HttpPost("filter")]
        [ProducesResponseType(typeof(ResponsePagination<ProductEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var products = await _productBusiness.GetAllAsync(queryModel);
                return products;
            });

        }

        [HttpPost("count")]
        [ProducesResponseType(typeof(ResponseObject<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductCount([FromQuery] ProductQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
              {
                  var count = await _productBusiness.GetCountAsync(queryModel);
                  return count;
              });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<ProductEntity>), StatusCodes.Status200OK)]
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
        public async Task<IActionResult> PatchProduct(Guid id, [FromBody] ProductPatchModel productEntity)
        {
            return await ExecuteFunction(async () =>
            {
               if(productEntity.VariantObjs != null)
                {
                    productEntity.VariantJson = JsonConvert.SerializeObject(productEntity.VariantObjs);
                }
                 
                    var updatedProduct = await _productBusiness.PatchAsync(productEntity);
                return updatedProduct;
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponsePagination<ProductEntity>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProductEntity>> DeleteProduct(Guid id)
        {
            var deletedProduct = await _productBusiness.DeleteAsync(id);
            return Ok(deletedProduct);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(ResponseList<ProductEntity>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductEntity>>> DeleteProducts([FromBody] Guid[] ids)
        {
            var deletedProducts = await _productBusiness.DeleteAsync(ids);
            return Ok(deletedProducts);
        }
    }
}
