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
    public class PaymentMethodsController : BaseControllerApi
    {
        private readonly IPaymentMethodsBusiness _paymentMethodsBusiness;

        public PaymentMethodsController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, IPaymentMethodsBusiness paymentMethodsBusiness) : base(httpRequestHelper, logger)
        {
            _paymentMethodsBusiness = paymentMethodsBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentMethod(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var paymentMethod = await _paymentMethodsBusiness.FindAsync(id);
                return paymentMethod;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetPaymentMethods([FromQuery] PaymentMethodsQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var paymentMethods = await _paymentMethodsBusiness.GetAllAsync(queryModel);
                return paymentMethods;
            });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetPaymentMethodCount([FromQuery] PaymentMethodsQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _paymentMethodsBusiness.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentMethod([FromBody] PaymentMethods paymentMethods)
        {
            return await ExecuteFunction(async () =>
            {
                var createdPaymentMethod = await _paymentMethodsBusiness.SaveAsync(paymentMethods);
                return createdPaymentMethod;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PaymentMethods>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchPaymentMethod(Guid id, [FromBody] PaymentMethods paymentMethods)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _paymentMethodsBusiness.FindAsync(id);
                if (id != paymentMethods.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedPaymentMethod = await _paymentMethodsBusiness.PatchAsync(paymentMethods);
                return updatedPaymentMethod;
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<PaymentMethods>> DeletePaymentMethod(Guid id)
        {
            var deletedPaymentMethod = await _paymentMethodsBusiness.DeleteAsync(id);
            return Ok(deletedPaymentMethod);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<PaymentMethods>>> DeletePaymentMethods([FromBody] Guid[] ids)
        {
            var deletedPaymentMethods = await _paymentMethodsBusiness.DeleteAsync(ids);
            return Ok(deletedPaymentMethods);
        }
    }
}
