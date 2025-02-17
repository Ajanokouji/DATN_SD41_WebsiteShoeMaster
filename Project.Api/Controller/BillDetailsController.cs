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
    public class BillDetailsController : BaseControllerApi
    {
        private readonly IBillDetailsBusiness _billDetailsBusiness;

        public BillDetailsController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, IBillDetailsBusiness billDetailsBusiness) : base(httpRequestHelper, logger)
        {
            _billDetailsBusiness = billDetailsBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBillDetails(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var billDetails = await _billDetailsBusiness.FindAsync(id);
                return billDetails;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetBillDetails([FromQuery] BillDetailsQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var billDetails = await _billDetailsBusiness.GetAllAsync(queryModel);
                return billDetails;
            });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetBillDetailsCount([FromQuery] BillDetailsQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _billDetailsBusiness.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateBillDetails([FromBody] BillDetails billDetails)
        {
            return await ExecuteFunction(async () =>
            {
                var createdBillDetails = await _billDetailsBusiness.SaveAsync(billDetails);
                return createdBillDetails;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<BillDetails>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchBillDetails(Guid id, [FromBody] BillDetails billDetails)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _billDetailsBusiness.FindAsync(id);
                if (id != billDetails.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedBillDetails = await _billDetailsBusiness.PatchAsync(billDetails);
                return updatedBillDetails;
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BillDetails>> DeleteBillDetails(Guid id)
        {
            var deletedBillDetails = await _billDetailsBusiness.DeleteAsync(id);
            return Ok(deletedBillDetails);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<BillDetails>>> DeleteBillDetails([FromBody] Guid[] ids)
        {
            var deletedBillDetails = await _billDetailsBusiness.DeleteAsync(ids);
            return Ok(deletedBillDetails);
        }
    }
}
