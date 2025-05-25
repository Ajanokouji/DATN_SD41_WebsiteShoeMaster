using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.DbManagement.Entity;
using SERP.Framework.ApiUtils.Controllers;
using SERP.Framework.ApiUtils.Responses;
using SERP.Framework.ApiUtils.Utils;
using SERP.NewsMng.Business.Models.QueryModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentBaseController : BaseControllerApi
    {
        private readonly IContentBaseBusiness _contentBaseBusiness;

        public ContentBaseController(
            IHttpRequestHelper httpRequestHelper,
            ILogger<ApiControllerBase> logger,
            IContentBaseBusiness contentBaseBusiness)
            : base(httpRequestHelper, logger)
        {
            _contentBaseBusiness = contentBaseBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContentBase(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var content = await _contentBaseBusiness.FindAsync(id);
                return content;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetContentBases([FromQuery] ContentBaseQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var contents = await _contentBaseBusiness.GetAllAsync(queryModel);
                return contents;
            });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetContentBaseCount([FromQuery] ContentBaseQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _contentBaseBusiness.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateContentBase([FromBody] ContentBase contentBase)
        {
            return await ExecuteFunction(async () =>
            {
                var createdContent = await _contentBaseBusiness.SaveAsync(contentBase);

                // Gọi update liên kết ProductContent sau khi tạo thành công
                //await _productContentBusiness.UpdateAllProductContentRelationsAsync();

                return createdContent;
            });
        }

        [HttpPost("batch")]
        public async Task<IActionResult> CreateContentBases([FromBody] IEnumerable<ContentBase> contentBases)
        {
            return await ExecuteFunction(async () =>
            {
                var createdContents = await _contentBaseBusiness.SaveAsync(contentBases);

                // Có thể gọi update nếu cần, nhưng với batch nhiều content thì nên xử lý riêng
                //await _productContentBusiness.UpdateAllProductContentRelationsAsync();

                return createdContents;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<ContentBase>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchContentBase(Guid id, [FromBody] ContentBase contentBase)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _contentBaseBusiness.FindAsync(id);
                if (exist == null || id != contentBase.Id)
                    throw new ArgumentException("ContentBase not found or ID mismatch");

                var updatedContent = await _contentBaseBusiness.PatchAsync(contentBase);

                // Gọi update liên kết ProductContent sau khi sửa thành công
                //await _productContentBusiness.UpdateAllProductContentRelationsAsync();

                return updatedContent;
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContentBase(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var deletedContent = await _contentBaseBusiness.DeleteAsync(id);
                return deletedContent;
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteContentBases([FromBody] Guid[] ids)
        {
            return await ExecuteFunction(async () =>
            {
                var deletedContents = await _contentBaseBusiness.DeleteAsync(ids);
                return deletedContents;
            });
        }
    }
}
