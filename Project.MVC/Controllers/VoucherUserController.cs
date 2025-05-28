using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Interface;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.Entity;

namespace Project.MVC.Controllers
{
    [Route("voucher-user")]
    public class VoucherUserController : Controller
    {
        private readonly IVoucherUsersBusiness _voucherUserBusiness;
        private readonly IUserBusiness _userBusiness;

        public VoucherUserController(IVoucherUsersBusiness voucherUserBusiness, IUserBusiness userBusiness)
        {
            _voucherUserBusiness = voucherUserBusiness;
            _userBusiness = userBusiness;
        }


        [HttpPost("save")]
        public async Task<IActionResult> SaveVoucher(Guid id)
        {
            // Kiểm tra session
            var errAndUserEntity = await CheckUserIsLoginning();
            if (!String.IsNullOrWhiteSpace(errAndUserEntity.Err))
            {
                return BadRequest(new { message = errAndUserEntity.Err });
            }

            // Kiểm tra voucher user đã tồn tại chưa
            VoucherUsers voucherUser = new VoucherUsers()
            {
                Id = Guid.NewGuid(),
                UserId = errAndUserEntity.User.Id,
                VoucherId = id,
                IsUsed = false,
            };
            List<VoucherUsers> voucherUsers = new List<VoucherUsers>();
            voucherUsers.Add(voucherUser);

            try
            {
                var voucherUserFound = await _voucherUserBusiness.FindByVidUid(voucherUsers);

                if (voucherUserFound != null && voucherUserFound.Count() > 0)
                {
                    return BadRequest(new { message = "Voucher đã được lưu từ trước đó!" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi trong quá trình lưu Voucher! Vui lòng thử lại!" });
            }

            //Thêm voucher user
            try
            {
                var result = await _voucherUserBusiness.SaveAsync(voucherUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi trong quá trình lưu Voucher! Vui lòng thử lại!" });
            }

            return Ok(new { message = "Lưu Voucher thành công!" });
        }


        //Check user có trong session + tồn tại + chưa bị xóa + IsActive = true + type = Customer  
        private class ErrAndUserEntity
        {
            public string? Err { get; set; }
            public UserEntity? User { get; set; }
        }

        private async Task<ErrAndUserEntity> CheckUserIsLoginning()
        {
            ErrAndUserEntity errAndUserEntity = new ErrAndUserEntity()
            {
                Err = "",
                User = new UserEntity()
            };

            string userJson = HttpContext.Session.GetString(UserConstants.UserSessionKey);

            if (!string.IsNullOrEmpty(userJson))
            {
                var user = JsonConvert.DeserializeObject<UserEntity>(userJson);
                if (user != null)
                {
                    var userFound = await _userBusiness.FindAsync(user.Id);
                    if (userFound != null)
                    {
                        if (userFound.IsDeleted == false && userFound.Type == UserTypeEnum.Customer)
                        {
                            if (userFound.IsActive == false)
                            {
                                errAndUserEntity.Err = "Tài khoản người dùng hiện đã bị tạm dừng hoạt động! Vui lòng kiểm tra lại!";
                            }
                            else
                            {
                                errAndUserEntity.Err = "";
                                errAndUserEntity.User = userFound;
                            }
                        }
                        else
                        {
                            errAndUserEntity.Err = "Vui lòng đăng nhập để lưu voucher!";
                        }
                    }
                    else
                    {
                        errAndUserEntity.Err = "Vui lòng đăng nhập để lưu voucher!";
                    }
                }
                else
                {
                    errAndUserEntity.Err = "Vui lòng đăng nhập để lưu voucher!";
                }
            }
            else
            {
                errAndUserEntity.Err = "Vui lòng đăng nhập để lưu voucher!";
            }

            return errAndUserEntity;
        }
    }
}
