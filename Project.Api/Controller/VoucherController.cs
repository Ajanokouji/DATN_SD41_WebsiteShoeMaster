using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement;
using SERP.Framework.ApiUtils.Controllers;
using SERP.Framework.ApiUtils.Responses;
using SERP.Framework.ApiUtils.Utils;

namespace Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : BaseControllerApi
    {
        private readonly IVoucherBusiness _voucherBusiness;

        public VoucherController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, IVoucherBusiness voucherBusiness) : base(httpRequestHelper, logger)
        {
            _voucherBusiness = voucherBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucher(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var voucher = await _voucherBusiness.FindAsync(id);
                return voucher;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetVouchers([FromQuery] VoucherQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var vouchers = await _voucherBusiness.GetAllAsync(queryModel);
                return vouchers;
            });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetVoucherCount([FromQuery] VoucherQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _voucherBusiness.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] Voucher voucher)
        {
            return await ExecuteFunction(async () =>
            {
                var createdVoucher = await _voucherBusiness.SaveAsync(voucher);
                return createdVoucher;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<Voucher>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchVoucher(Guid id, [FromBody] Voucher voucher)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _voucherBusiness.FindAsync(id);
                if (id != voucher.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedVoucher = await _voucherBusiness.PatchAsync(voucher);
                return updatedVoucher;
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Voucher>> DeleteVoucher(Guid id)
        {
            var deletedVoucher = await _voucherBusiness.DeleteAsync(id);
            return Ok(deletedVoucher);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<Voucher>>> DeleteVouchers([FromBody] Guid[] ids)
        {
            var deletedVouchers = await _voucherBusiness.DeleteAsync(ids);
            return Ok(deletedVouchers);
        }
    }
}
