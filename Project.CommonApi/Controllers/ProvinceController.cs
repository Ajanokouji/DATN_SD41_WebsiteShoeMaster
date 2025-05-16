using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.ApiUtils.Controllers;
using SERP.Framework.ApiUtils.Responses;
using SERP.Framework.ApiUtils.Utils;
using SERP.Framework.Common;

namespace Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinceController : BaseControllerApi
    {
        private readonly IProvinceBusiness _provinceBusiness;

        public ProvinceController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, IProvinceBusiness provinceBusiness) : base(httpRequestHelper, logger)
        {
            _provinceBusiness = provinceBusiness;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseObject<Province>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProvince(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var province = await _provinceBusiness.FindAsync(id);
                return province;
            });
        }

        [HttpGet("filter")]
        [ProducesResponseType(typeof(ResponsePagination<Province>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProvinces([FromQuery] ProvinceQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var provinces = await _provinceBusiness.GetAllAsync(queryModel);
                return provinces;
            });
        }

        [HttpPost("count")]
        [ProducesResponseType(typeof(ResponseObject<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProvinceCount([FromQuery] ProvinceQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _provinceBusiness.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<Province>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateProvince([FromBody] Province province)
        {
            return await ExecuteFunction(async () =>
            {
                var createdProvince = await _provinceBusiness.SaveAsync(province);
                return createdProvince;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<Province>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchProvince(Guid id, [FromBody] Province province)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _provinceBusiness.FindAsync(id);
                if (id != province.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedProvince = await _provinceBusiness.SaveAsync(province);
                return updatedProvince;
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseObject<Province>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Province>> DeleteProvince(Guid id)
        {
            var deletedProvince = await _provinceBusiness.DeleteAsync(id);
            return Ok(deletedProvince);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(ResponseList<Province>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Province>>> DeleteProvinces([FromBody] Guid[] ids)
        {
            var deletedProvinces = await _provinceBusiness.DeleteAsync(ids);
            return Ok(deletedProvinces);
        }
    }
}
