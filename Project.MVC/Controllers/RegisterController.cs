using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Implement;
using Project.Business.Interface;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IUserBusiness _userBusiness;

        public RegisterController(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        public IActionResult Register()
        {
            if (TempData["RegisterUserData"] != null && !String.IsNullOrWhiteSpace(TempData["RegisterUserData"].ToString()))
            {
                var user = JsonConvert.DeserializeObject<UserEntity>(TempData["RegisterUserData"].ToString());
                return View(user);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserEntity user)
        {
            //Đã Validate dữ liệu ở client bằng js

            //Trim dữ liệu
            if (!String.IsNullOrWhiteSpace(user.Username))
            {
                user.Username = user.Username.Trim();
            }
            if (!String.IsNullOrWhiteSpace(user.Name))
            {
                user.Name = user.Name.Trim();
            }
            if (!String.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                user.PhoneNumber = user.PhoneNumber.Trim();
            }
            if (!String.IsNullOrWhiteSpace(user.Address))
            {
                user.Address = user.Address.Trim();
            }
            if (!String.IsNullOrWhiteSpace(user.Email))
            {
                user.Email = user.Email.Trim();
            }

            //Tìm xem tên username đã tồn tại chưa
            if (!String.IsNullOrWhiteSpace(user.Username))
            {
                var lstUserFoundByUserName = _userBusiness.LocUserTheoNhieuDK(new UserQueryModel
                {
                    Username = user.Username
                }).Result.Where(u => u.IsDeleted == false);
                //Nếu đã tồn tại
                if (lstUserFoundByUserName.Any())
                {
                    TempData["ErrRegMs"] = "Tên đăng nhập đã tồn tại";
                    TempData["SuccessRegMs"] = "";
                    TempData["RegisterUserData"] = JsonConvert.SerializeObject(user);
                    return RedirectToAction("Register");
                }
            }

            //Tìm xem SĐT đã được sử dụng chưa
            if (!String.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                var lstUserFoundByPhone = _userBusiness.LocUserTheoNhieuDK(new UserQueryModel
                {
                    PhoneNumber = user.PhoneNumber
                }).Result.Where(u => u.IsDeleted == false);

                //Nếu đã tồn tại
                if (lstUserFoundByPhone.Any())
                {
                    TempData["ErrRegMs"] = "SĐT đã được sử dụng";
                    TempData["RegisterUserData"] = JsonConvert.SerializeObject(user);
                    TempData["SuccessRegMs"] = "";
                    return RedirectToAction("Register");
                }
            }

            //Tìm xem Email đã được sử dụng chưa
            if (!String.IsNullOrWhiteSpace(user.Email))
            {
                var lstUserFoundByEmail = _userBusiness.LocUserTheoNhieuDK(new UserQueryModel
                {
                    Email = user.Email
                }).Result.Where(u => u.IsDeleted == false);

                //Nếu đã tồn tại
                if (lstUserFoundByEmail.Any())
                {
                    TempData["ErrRegMs"] = "Email đã được sử dụng";
                    TempData["RegisterUserData"] = JsonConvert.SerializeObject(user);
                    TempData["SuccessRegMs"] = "";
                    return RedirectToAction("Register");
                }
            }

            //Tạo mới user
            user.Id = Guid.NewGuid();
            user.Type = UserTypeEnum.Customer;    //Giả sử Type Khách hàng là 1
            user.IsActive = true;

            try
            {
                await _userBusiness.SaveAsync(user);
            }
            catch (Exception ex)
            {
                TempData["ErrRegMs"] = "Xảy ra lỗi trong quá trình đăng ký tài khoản! Vui lòng thử lại!";
                TempData["RegisterUserData"] = JsonConvert.SerializeObject(user);
                TempData["SuccessRegMs"] = "";
                return RedirectToAction("Register");
            }

            //Đăng ký thành công
            TempData["SuccessRegMs"] = "Đăng ký tài khoản thành công";
            TempData["RegisterUserData"] = "";
            TempData["ErrRegMs"] = "";
            return RedirectToAction("Register");
        }
    }
}
