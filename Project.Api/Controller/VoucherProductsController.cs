using Microsoft.AspNetCore.Mvc;
using Nest;
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
    public class VoucherProductsController : BaseControllerApi
    {
        private readonly IVoucherProductsBusiness _voucherProductsBusiness;

        public VoucherProductsController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, IVoucherProductsBusiness voucherProductsBusiness) : base(httpRequestHelper, logger)
        {
            _voucherProductsBusiness = voucherProductsBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucherProduct(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var voucher = await _voucherProductsBusiness.FindAsync(id);
                return voucher;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetVoucherProducts([FromQuery] VoucherProductsQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var vouchers = await _voucherProductsBusiness.GetAllAsync(queryModel);
                return vouchers;
            });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetVoucherProductsCount([FromQuery] VoucherProductsQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _voucherProductsBusiness.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucherProducts([FromBody] IEnumerable<VoucherProducts> voucherProduct)
        {
            return await ExecuteFunction(async () =>
            {
                var createdVoucher = await _voucherProductsBusiness.SaveAsync(voucherProduct);
                return createdVoucher;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<VoucherProducts>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchVoucher(Guid id, [FromBody] VoucherProducts voucherProduct)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _voucherProductsBusiness.FindAsync(id);
                if (id != voucherProduct.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedVoucherProduct = await _voucherProductsBusiness.PatchAsync(voucherProduct);
                return updatedVoucherProduct;
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<VoucherProducts>> DeleteVoucherProduct(Guid id)
        {
            var deletedVoucherPr = await _voucherProductsBusiness.DeleteAsync(id);
            return Ok(deletedVoucherPr);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<VoucherProducts>>> DeleteVoucherProducts([FromBody] Guid[] ids)
        {
            var deletedVoucherPrs = await _voucherProductsBusiness.DeleteAsync(ids);
            return Ok(deletedVoucherPrs);
        }

        [HttpPost("GetProductsByIds")]
        public async Task<ActionResult<IEnumerable<ProductEntity>>> GetProductsByIds([FromBody] Guid[] ids)
        {
            var products = await _voucherProductsBusiness.GetProductsByIds(ids);
            return Ok(products);
        }

        [HttpPost("SearchProduct")]
        public async Task<IActionResult> SearchProduct(ProductQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var products = await _voucherProductsBusiness.SearchProduct(queryModel);
                return products;
            });
        }

        [HttpPost("FindByVcidPidVaid")]
        public async Task<IActionResult> FindByVcidPidVaid(IEnumerable<VoucherProducts> voucherProducts)
        {
            return await ExecuteFunction(async () =>
            {
                var voucherProductsFind = await _voucherProductsBusiness.FindByVcidPidVaid(voucherProducts);
                return voucherProductsFind;
            });
        }
    }
}
