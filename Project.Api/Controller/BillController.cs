using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement;
using SERP.Framework.ApiUtils.Utils;

namespace Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : BaseControllerApi
    {
        private readonly IBillBusiness _billBusiness;

        public BillController(IHttpRequestHelper httpRequestHelper, ILogger<BaseControllerApi> logger, IBillBusiness billBusiness) : base(httpRequestHelper, logger)
        {
            _billBusiness = billBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBillById(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var bill = await _billBusiness.FindAsync(id);
                if (bill == null)
                {
                    throw new ArgumentException("Not Found");
                }
                return bill;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetAllBills([FromQuery] BillQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var bills = await _billBusiness.GetAllAsync(queryModel);
                return bills;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateBill([FromBody] BillEntity bill)
        {
            return await ExecuteFunction(async () =>
            {
                var createdBill = await _billBusiness.SaveAsync(bill);
                return createdBill;
            });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateBill(Guid id, [FromBody] BillEntity bill)
        {
            return await ExecuteFunction(async () =>
            {
                if (id != bill.Id)
                {
                    throw new ArgumentException("Not Found");
                }
                var updatedBill = await _billBusiness.PatchAsync(bill);
                return updatedBill;
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var deletedBill = await _billBusiness.DeleteAsync(id);
                return deletedBill;
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBills([FromBody] Guid[] ids)
        {
            return await ExecuteFunction(async () =>
            {
                var deletedBills = await _billBusiness.DeleteAsync(ids);
                return deletedBills;
            });
        }
    }
}
