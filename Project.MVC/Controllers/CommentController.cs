using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Implement;
using Project.Business.Interface;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.Entity;
using Project.MvcModule.Models;
using SERP.Framework.Common;

namespace Project.MVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly ICommentsBusiness _commentsBusiness;
        private readonly IUserBusiness _userBusiness;

        public CommentController(ICommentsBusiness commentsBusiness, IUserBusiness userBusiness)
        {
            _commentsBusiness = commentsBusiness;
            _userBusiness = userBusiness;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddComment([FromBody] CommentsEntity input)
        {
            var checkUser = await CheckUserIsLoginning();
            if (!string.IsNullOrEmpty(checkUser.Err))
            {
                return BadRequest(new { message = checkUser.Err });
            }

            if (string.IsNullOrWhiteSpace(input.Message))
            {
                return BadRequest(new { message = "Vui lòng điền vào nội dung đánh giá!" });
            }

            var comment = new CommentsEntity
            {
                Id = Guid.NewGuid(),
                UserId = checkUser.User.Id,
                Username = checkUser.User.Username,
                ObjectId = input.ObjectId,
                Message = input.Message,
                Status = 0,
                IsPublish = true,
                TolalReply = 0,
            };

            //Thêm đánh giá chờ duyệt status = 0
            try
            {
                var result = await _commentsBusiness.SaveAsync(comment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi trong quá trình gửi đánh giá! Vui lòng thử lại!" });
            }

            return Ok(new { message = "Gửi đánh giá thành công! Chúng tôi sẽ xem xét và duyệt đánh giá của bạn trong thời gian sớm nhất! Trân trọng!" });
        }

        [HttpGet("LoadMoreComments")]
        public async Task<IActionResult> LoadMoreComments(Guid productId, int page)
        {
            var data = await _commentsBusiness.GetAllAsync(new CommentsModel()
            {
                PageSize = 4,
                ObjectId = productId,
                Status = 1,
                CurrentPage = page,
            });

            var res = AutoMapperUtils.AutoMap<CommentsEntity, CommentsViewModel>(data.Content.ToList());

            return PartialView("~/Views/CommentList/_CommentItemsPartial.cshtml", res); // View chứa các item riêng lẻ
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
                            errAndUserEntity.Err = "Vui lòng đăng nhập để gửi đánh giá!";
                        }
                    }
                    else
                    {
                        errAndUserEntity.Err = "Vui lòng đăng nhập để gửi đánh giá!";
                    }
                }
                else
                {
                    errAndUserEntity.Err = "Vui lòng đăng nhập để gửi đánh giá!";
                }
            }
            else
            {
                errAndUserEntity.Err = "Vui lòng đăng nhập để gửi đánh giá!";
            }

            return errAndUserEntity;
        }
    }
}
