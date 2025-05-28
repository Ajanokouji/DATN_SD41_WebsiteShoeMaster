using Microsoft.AspNetCore.Mvc;
using Project.Business.Implement;
using Project.Business.Interface;
using Project.Business.Model;
using Project.Business.Model.PatchModel;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.ApiUtils.Responses;
using SERP.Framework.ApiUtils.Utils;

namespace Project.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : BaseControllerApi
    {
        private readonly IBillBusiness _billBusiness;
        private readonly IProductBusiness _productBusiness;
        private readonly IBillDetailsBusiness _billDetailsBusiness;

        public BillController(IProductBusiness productBusiness, IBillDetailsBusiness billDetailsBusiness, IHttpRequestHelper httpRequestHelper, ILogger<BaseControllerApi> logger, IBillBusiness billBusiness) : base(httpRequestHelper, logger)
        {
            _billDetailsBusiness = billDetailsBusiness;
            _productBusiness = productBusiness;
            _billBusiness = billBusiness;
        }

        [ProducesResponseType(typeof(ResponseObject<BillModel>), StatusCodes.Status200OK)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBillById(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var bill = await _billBusiness.GetBillById(id);
                if (bill == null)
                {
                    throw new ArgumentException("Not Found");
                }
                return bill;
            });
        }

        [ProducesResponseType(typeof(ResponsePagination<BillModel>), StatusCodes.Status200OK)]
        [HttpPost("filter")]
        public async Task<IActionResult> GetAllBills([FromQuery] BillQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var bills = await _billBusiness.GetAllAsync(queryModel);
                return bills;
            });
        }

        [HttpGet("check-inventory/{Id}")]
        [ProducesResponseType(typeof(ResponseList<CheckProductStockDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckInventory(Guid Id)
        {
            return await ExecuteFunction(async () =>
            {
                var res = new List<CheckProductStockDetail>();
                var billDetails = await _billDetailsBusiness.GetBillDetailsByIdBill(Id);
                var products = await _productBusiness.ListByIdsAsync(billDetails.Select(x => x.IdProduct.Value).ToList());
                foreach (var item in billDetails)
                {
                    var productCurrentStock = (products.FirstOrDefault(x => x.Id == item.IdProduct.Value)).VariantObjs.FirstOrDefault(x => x.Sku == item.Sku).Stock;
                    res.Add(new CheckProductStockDetail
                   {
                       ProductId = item.IdProduct.Value,
                       ProductName = item.Name,
                       Sku =item.Sku,
                       AvailableQuantity = productCurrentStock.Value,
                       RequestedQuantity = item.Quantity,
                       IsAvailable =(item.Quantity>productCurrentStock.Value)? false : true
                    });

                }
                return res;
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
        public async Task<IActionResult> UpdateBill(Guid id, [FromBody] BillPatchModel bill)
        {
            return await ExecuteFunction(async () =>
            {
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
