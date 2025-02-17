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
    public class CategoriesController : BaseControllerApi
    {
        private readonly ICategoriesBusiness _categoriesBusiness;

        public CategoriesController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, ICategoriesBusiness categoriesBusiness) : base(httpRequestHelper, logger)
        {
            _categoriesBusiness = categoriesBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var category = await _categoriesBusiness.FindAsync(id);
                return category;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetCategories([FromQuery] CategoriesQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var categories = await _categoriesBusiness.GetAllAsync(queryModel);
                return categories;
            });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetCategoryCount([FromQuery] CategoriesQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _categoriesBusiness.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoriesEntity categoryEntity)
        {
            return await ExecuteFunction(async () =>
            {
                var createdCategory = await _categoriesBusiness.SaveAsync(categoryEntity);
                return createdCategory;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<CategoriesEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchCategory(Guid id, [FromBody] CategoriesEntity categoryEntity)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _categoriesBusiness.FindAsync(id);
                if (id != categoryEntity.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedCategory = await _categoriesBusiness.PatchAsync(categoryEntity);
                return updatedCategory;
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoriesEntity>> DeleteCategory(Guid id)
        {
            var deletedCategory = await _categoriesBusiness.DeleteAsync(id);
            return Ok(deletedCategory);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<CategoriesEntity>>> DeleteCategories([FromBody] Guid[] ids)
        {
            var deletedCategories = await _categoriesBusiness.DeleteAsync(ids);
            return Ok(deletedCategories);
        }
    }
}
