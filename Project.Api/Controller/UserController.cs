using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    public class UserController : BaseControllerApi
    {
        private readonly IUserBusiness _userBusiness;

        public UserController(IHttpRequestHelper httpRequestHelper, ILogger<ApiControllerBase> logger, IUserBusiness userBusiness) : base(httpRequestHelper, logger)
        {
            _userBusiness = userBusiness;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            return await ExecuteFunction(async () =>
            {
                var user = await _userBusiness.FindAsync(id);
                return user;
            });
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetUsers([FromQuery] UserQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var users = await _userBusiness.GetAllAsync(queryModel);
                return users;
            });
        }

        [HttpPost("count")]
        public async Task<IActionResult> GetUserCount([FromQuery] UserQueryModel queryModel)
        {
            return await ExecuteFunction(async () =>
            {
                var count = await _userBusiness.GetCountAsync(queryModel);
                return count;
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserEntity user)
        {
            return await ExecuteFunction(async () =>
            {
                var createdUser = await _userBusiness.SaveAsync(user);
                return createdUser;
            });
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseObject<UserEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PatchUser(Guid id, [FromBody] UserEntity user)
        {
            return await ExecuteFunction(async () =>
            {
                var exist = await _userBusiness.FindAsync(id);
                if (id != user.Id || exist == null)
                    throw new ArgumentException("Not Found");
                var updatedUser = await _userBusiness.PatchAsync(user);
                return updatedUser;
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<UserEntity>> DeleteUser(Guid id)
        {
            var deletedUser = await _userBusiness.DeleteAsync(id);
            return Ok(deletedUser);
        }

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<UserEntity>>> DeleteUsers([FromBody] Guid[] ids)
        {
            var deletedUsers = await _userBusiness.DeleteAsync(ids);
            return Ok(deletedUsers);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserEntity>> Login([FromBody] UserEntity user)
        {
            var userQueryModel = new UserQueryModel
            {
                Username = user.Username,
            };

            var usersFound = await _userBusiness.LocUserTheoNhieuDK(userQueryModel);
            var userFound = usersFound.FirstOrDefault(u => u.Password == user.Password
                                                        && u.IsDeleted == false
                                                        && u.Type == DbManagement.UserTypeEnum.Admin||u.Type==UserTypeEnum.User);

            return Ok(userFound);
        }

        [HttpPost("CheckTrungCodeUser")]
        public async Task<IActionResult> CheckTrungCodeUser([FromQuery] Guid? id, [FromBody] string username)
        {
            if(id.HasValue == false)
            {
                return await ExecuteFunction(async () =>
                {
                    var userFound = _userBusiness.LocUserTheoNhieuDK(new UserQueryModel()
                    {
                        Username = username,
                    }).Result.Where(x => x.IsDeleted == false);

                    return Ok(userFound.Any());
                });
            }
            else
            {
                return await ExecuteFunction(async () =>
                {
                    var userFound = _userBusiness.LocUserTheoNhieuDK(new UserQueryModel()
                    {
                        Username = username,
                    }).Result.Where(x => x.IsDeleted == false && x.Id != id);

                    return Ok(userFound.Any());
                });
            }
        }
    }
}
