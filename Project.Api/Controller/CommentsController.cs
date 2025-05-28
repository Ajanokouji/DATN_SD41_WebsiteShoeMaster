using Microsoft.AspNetCore.Mvc;
using Nest;
using Org.BouncyCastle.Crypto;
using Project.Business.Implement;
using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.ApiUtils.Controllers;
using SERP.Framework.ApiUtils.Responses;
using SERP.Framework.ApiUtils.Utils;


namespace Project.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : BaseControllerApi
    {
        private readonly ICommentsBusiness _commentsBusiness;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentsBusiness commentsBusiness, IHttpRequestHelper httpRequestHelper, ILogger<CommentsController> logger) : base(httpRequestHelper, logger)
        {
            _commentsBusiness = commentsBusiness;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseObject<CommentsEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComment(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var comment = await _commentsBusiness.FindAsync(id);
                return comment;
            });
        }

        [HttpPost("filter")]
        [ProducesResponseType(typeof(ResponsePagination<CommentsEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComments([FromQuery] CommentsModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var comments = await _commentsBusiness.GetAllAsync(queryModel);
                return comments;
            });
        }

        [HttpPost("count")]
        [ProducesResponseType(typeof(ResponseObject<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCommentCount([FromQuery] CommentsModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _commentsBusiness.GetCountAsync(queryModel);
                return count;
            });
            
        }

        [HttpPost("list")]
        [ProducesResponseType(typeof(ResponseList<CommentsEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListAllComments([FromBody] CommentsModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var comments = await _commentsBusiness.ListAllAsync(queryModel);
                return comments;
            });
           
        }

        [HttpPost("list-by-ids")]
        [ProducesResponseType(typeof(ResponseList<CommentsEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListCommentsByIds([FromBody] IEnumerable<Guid> ids)
        {
            return await ExecuteFunction(async () =>
            {
                var comments = await _commentsBusiness.ListByIdsAsync(ids);
                return comments;
            });
      
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseList<CommentsEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateComments([FromBody] IEnumerable<CommentsEntity> entities)
        {
            return await ExecuteFunction(async () =>
            {
                var createdComments = await _commentsBusiness.SaveAsync(entities);
                return createdComments;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<CommentsEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchVoucher(Guid id, [FromBody] CommentsEntity comment)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _commentsBusiness.FindAsync(id);
                if (id != comment.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedcomment = await _commentsBusiness.PatchAsync(comment);
                return updatedcomment;
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseObject<CommentsEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var deletedComment = await _commentsBusiness.DeleteAsync(id);
                return deletedComment;
            });

        }

        [HttpDelete]
        [ProducesResponseType(typeof(ResponseList<CommentsEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteComments([FromBody] Guid[] ids)
        {
            return await ExecuteFunction(async () =>
            {
                var deletedComments = await _commentsBusiness.DeleteAsync(ids);
                return deletedComments;
            });
          
        }
    }
}
