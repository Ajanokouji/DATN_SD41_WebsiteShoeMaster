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
    public class CustomerController : BaseControllerApi
    {
        private readonly ICustomerBusiness _customerBusiness;

        public CustomerController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, ICustomerBusiness customerBusiness) : base(httpRequestHelper, logger)
        {
            _customerBusiness = customerBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var customer = await _customerBusiness.FindAsync(id);
                return customer;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetCustomers([FromQuery] CustomerQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var customers = await _customerBusiness.GetAllAsync(queryModel);
                return customers;
            });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetCustomerCount([FromQuery] CustomerQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _customerBusiness.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] Customers customerEntity)
        {
            return await ExecuteFunction(async () =>
            {
                var createdCustomer = await _customerBusiness.SaveAsync(customerEntity);
                return createdCustomer;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<Customers>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchCustomer(Guid id, [FromBody] Customers customerEntity)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _customerBusiness.FindAsync(id);
                if (id != customerEntity.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedCustomer = await _customerBusiness.PatchAsync(customerEntity);
                return updatedCustomer;
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Customers>> DeleteCustomer(Guid id)
        {
            var deletedCustomer = await _customerBusiness.DeleteAsync(id);
            return Ok(deletedCustomer);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<Customers>>> DeleteCustomers([FromBody] Guid[] ids)
        {
            var deletedCustomers = await _customerBusiness.DeleteAsync(ids);
            return Ok(deletedCustomers);
        }
    }
}
