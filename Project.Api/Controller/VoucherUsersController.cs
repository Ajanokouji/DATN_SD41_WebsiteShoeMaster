using Microsoft.AspNetCore.Mvc;
using Nest;
using Project.Business.Implement;
using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.ApiUtils.Controllers;
using SERP.Framework.ApiUtils.Responses;
using SERP.Framework.ApiUtils.Utils;
using SERP.Framework.Common;

namespace Project.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherUsersController : BaseControllerApi
    {
        private readonly IVoucherUsersBusiness _voucherUsersRepository;

        public VoucherUsersController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, IVoucherUsersBusiness voucherUsersRepository) : base(httpRequestHelper, logger)
        {
            _voucherUsersRepository = voucherUsersRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucherUser(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var voucher = await _voucherUsersRepository.FindAsync(id);
                return voucher;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetVoucherUsers([FromQuery] VoucherUsersQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var vouchers = await _voucherUsersRepository.GetAllAsync(queryModel);
                return vouchers;
            });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetVoucherUsersCount([FromQuery] VoucherUsersQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _voucherUsersRepository.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucherUsers([FromBody] IEnumerable<VoucherUsers> voucherUser)
        {
            return await ExecuteFunction(async () =>
            {
                var createdVoucherUser = await _voucherUsersRepository.SaveAsync(voucherUser);
                return createdVoucherUser;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<VoucherUsers>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchVoucherUser(Guid id, [FromBody] VoucherUsers voucherUser)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _voucherUsersRepository.FindAsync(id);
                if (id != voucherUser.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedVoucherUser = await _voucherUsersRepository.PatchAsync(voucherUser);
                return updatedVoucherUser;
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<VoucherUsers>> DeleteVoucherUser(Guid id)
        {
            var deletedVoucherUs = await _voucherUsersRepository.DeleteAsync(id);
            return Ok(deletedVoucherUs);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<VoucherUsers>>> DeleteVoucherUsers([FromBody] Guid[] ids)
        {
            var deletedVoucherUss = await _voucherUsersRepository.DeleteAsync(ids);
            return Ok(deletedVoucherUss);
        }

        //Tìm kiếm user dựa vào nhiều thuộc tính
        [HttpPost("SearchUser")]
        public async Task<IActionResult> SearchUser(UserQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var users = await _voucherUsersRepository.SearchUser(queryModel);
                return users;
            });
        }

        [HttpPost("GetUsersByIds")]
        public async Task<ActionResult<IEnumerable<UserEntity>>> GetUsersByIds([FromBody] Guid[] ids)
        {
            var users = await _voucherUsersRepository.GetUsersByIds(ids);
            return Ok(users);
        }

        [HttpPost("FindByVidUid")]
        public async Task<IActionResult> FindByVidUid(IEnumerable<VoucherUsers> voucherUsers)
        {
            return await ExecuteFunction(async () =>
            {
                var voucherUsersFind = await _voucherUsersRepository.FindByVidUid(voucherUsers);
                return voucherUsersFind;
            });
        }
    }
}
