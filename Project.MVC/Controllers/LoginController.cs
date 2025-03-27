using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Interface;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserBusiness _userBusiness;

        public LoginController(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserEntity user)
        {
            //Check trống
            if(user.Username == null || user.Password == null)
            {
                TempData["ErrMs"] = "Tên đăng nhập hoặc mật khẩu không được để trống";
                return View(user);
            }

            //Tìm user dựa trên username
            var listUser = await _userBusiness.ListAllAsync(new UserQueryModel { Username = user.Username });
            var userFound = listUser?.FirstOrDefault();

            //Check user tồn tại, Chưa bị xóa và password đúng
            if (userFound == null || userFound.Isdeleted == true || userFound.Password != user.Password)
            {
                TempData["ErrMs"] = "Tên đăng nhập hoặc mật khẩu không đúng";

                return View(user);
            }

            //Check Active
            if (userFound.IsActive == false)
            {
                TempData["ErrMs"] = "Tài khoản đã bị tạm dừng hoạt động";

                return View(user);
            }

            //Đăng nhập thành công
            if (userFound.Type == "0")
            {
                //Đăng nhập thành công dưới quyền Admin
                TempData["ErrMs"] = "Đăng nhập thành công dưới quyền Admin";

                return View(user);
            }
            else if (userFound.Type == "1")
            {
                //Đăng nhập thành công dưới quyền khách hàng
                TempData["ErrMs"] = "Đăng nhập thành công dưới quyền khách hàng";

                return View(user);
            }
            else
            {
                //Không xác định được quyền user
                TempData["ErrMs"] = "Lỗi không xác định được quyền user";

                return View(user);
            }
        }
    }
}
