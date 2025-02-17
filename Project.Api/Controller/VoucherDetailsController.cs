using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement;
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
    public class VoucherDetailsController : BaseControllerApi
    {
        private readonly IVoucherDetailsBusiness _voucherDetailsBusiness;

        public VoucherDetailsController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, IVoucherDetailsBusiness voucherDetailsBusiness) : base(httpRequestHelper, logger)
        {
            _voucherDetailsBusiness = voucherDetailsBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucherDetail(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var voucherDetail = await _voucherDetailsBusiness.FindAsync(id);
                return voucherDetail;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetVoucherDetails([FromQuery] VoucherDetailsQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var voucherDetails = await _voucherDetailsBusiness.GetAllAsync(queryModel);
                return voucherDetails;
            });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetVoucherDetailsCount([FromQuery] VoucherDetailsQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _voucherDetailsBusiness.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucherDetail([FromBody] VoucherDetails voucherDetails)
        {
            return await ExecuteFunction(async () =>
            {
                var createdVoucherDetail = await _voucherDetailsBusiness.SaveAsync(voucherDetails);
                return createdVoucherDetail;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<VoucherDetails>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchVoucherDetail(Guid id, [FromBody] VoucherDetails voucherDetails)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _voucherDetailsBusiness.FindAsync(id);
                if (id != voucherDetails.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedVoucherDetail = await _voucherDetailsBusiness.PatchAsync(voucherDetails);
                return updatedVoucherDetail;
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<VoucherDetails>> DeleteVoucherDetail(Guid id)
        {
            var deletedVoucherDetail = await _voucherDetailsBusiness.DeleteAsync(id);
            return Ok(deletedVoucherDetail);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<VoucherDetails>>> DeleteVoucherDetails([FromBody] Guid[] ids)
        {
            var deletedVoucherDetails = await _voucherDetailsBusiness.DeleteAsync(ids);
            return Ok(deletedVoucherDetails);
        }
    }
}
