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
    public class ProductCategoriesRelationController : BaseControllerApi
    {
        private readonly IProductCategoriesRelationBusiness _productCategoriesRelationBusiness;

        public ProductCategoriesRelationController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, IProductCategoriesRelationBusiness productCategoriesRelationBusiness)
            : base(httpRequestHelper, logger)
        {
            _productCategoriesRelationBusiness = productCategoriesRelationBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductCategoriesRelation(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var relation = await _productCategoriesRelationBusiness.FindAsync(id);
                return relation;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetProductCategoriesRelations([FromQuery] ProductCategoriesRelationQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var relations = await _productCategoriesRelationBusiness.GetAllAsync(queryModel);
                return relations;
            });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetProductCategoriesRelationCount([FromQuery] ProductCategoriesRelationQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _productCategoriesRelationBusiness.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductCategoriesRelation([FromBody] ProductCategoriesRelation entity)
        {
            return await ExecuteFunction(async () =>
            {
                var createdRelation = await _productCategoriesRelationBusiness.SaveAsync(entity);
                return createdRelation;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<ProductCategoriesRelation>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchProductCategoriesRelation(Guid id, [FromBody] ProductCategoriesRelation entity)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _productCategoriesRelationBusiness.FindAsync(id);
                if (id != entity.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedRelation = await _productCategoriesRelationBusiness.SaveAsync(entity);
                return updatedRelation;
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductCategoriesRelation>> DeleteProductCategoriesRelation(Guid id)
        {
            var deletedRelation = await _productCategoriesRelationBusiness.DeleteAsync(id);
            return Ok(deletedRelation);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<ProductCategoriesRelation>>> DeleteProductCategoriesRelations([FromBody] Guid[] ids)
        {
            var deletedRelations = await _productCategoriesRelationBusiness.DeleteAsync(ids);
            return Ok(deletedRelations);
        }
    }
}
