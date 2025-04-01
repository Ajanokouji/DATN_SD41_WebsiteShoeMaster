using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Implement;
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
    public class RegisterController : Controller
    {
        private readonly IUserBusiness _userBusiness;

        public RegisterController(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserEntity user)
        {
            //Đã Validate dữ liệu ở client bằng js

            //Trim dữ liệu
            user.Username = user.Username.Trim();
            user.Name = user.Name.Trim();
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
            var lstUserFoundByUserName = _userBusiness.LocUserTheoNhieuDK(new UserQueryModel
            {
                Username = user.Username
            }).Result.Where(u => u.Isdeleted == false);
            //Nếu đã tồn tại
            if (lstUserFoundByUserName.Any())
            {
                TempData["ErrRegMs"] = "Tên đăng nhập đã tồn tại";
                return View(user);
            }

            //Tìm xem SĐT đã được sử dụng chưa
            if (!String.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                var lstUserFoundByPhone = _userBusiness.LocUserTheoNhieuDK(new UserQueryModel
                {
                    PhoneNumber = user.PhoneNumber
                }).Result.Where(u => u.Isdeleted == false);

                //Nếu đã tồn tại
                if (lstUserFoundByPhone.Any())
                {
                    TempData["ErrRegMs"] = "SĐT đã được sử dụng";
                    return View(user);
                }
            }

            //Tìm xem Email đã được sử dụng chưa
            if (!String.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                var lstUserFoundByEmail = _userBusiness.LocUserTheoNhieuDK(new UserQueryModel
                {
                    Email = user.Email
                }).Result.Where(u => u.Isdeleted == false);

                //Nếu đã tồn tại
                if (lstUserFoundByEmail.Any())
                {
                    TempData["ErrRegMs"] = "Email đã được sử dụng";
                    return View(user);
                }
            }

            //Tạo mới user
            user.Id = Guid.NewGuid();
            user.Type = "1";    //Giả sử Type Khách hàng là 1
            user.IsActive = true;
            user.MetadataJson = "[]";

            try
            {
                await _userBusiness.SaveAsync(user);
            }
            catch (Exception ex)
            {
                TempData["ErrRegMs"] = $"Lỗi: {ex.Message}";
                return View(user);
            }

            //Đăng ký thành công
            TempData["ErrRegMs"] = $"Đăng ký tài khoản thành công";
            return View();
        }
    }
}
