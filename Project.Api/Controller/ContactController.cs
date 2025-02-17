using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.Business.Model;
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
    public class ContactController : BaseControllerApi
    {
        private readonly IContactBusiness _contactBusiness;

        public ContactController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, IContactBusiness contactBusiness) : base(httpRequestHelper, logger)
        {
            _contactBusiness = contactBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContact(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var contact = await _contactBusiness.FindAsync(id);
                return contact;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetContacts([FromQuery] ContactQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var contacts = await _contactBusiness.GetAllAsync(queryModel);
                return contacts;
            });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetContactCount([FromQuery] ContactQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _contactBusiness.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] Contacts contactEntity)
        {
            return await ExecuteFunction(async () =>
            {
                var createdContact = await _contactBusiness.SaveAsync(contactEntity);
                return createdContact;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<Contacts>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchContact(Guid id, [FromBody] Contacts contactEntity)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _contactBusiness.FindAsync(id);
                if (id != contactEntity.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedContact = await _contactBusiness.PatchAsync(contactEntity);
                return updatedContact;
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Contacts>> DeleteContact(Guid id)
        {
            var deletedContact = await _contactBusiness.DeleteAsync(id);
            return Ok(deletedContact);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<Contacts>>> DeleteContacts([FromBody] Guid[] ids)
        {
            var deletedContacts = await _contactBusiness.DeleteAsync(ids);
            return Ok(deletedContacts);
        }
    }
}
