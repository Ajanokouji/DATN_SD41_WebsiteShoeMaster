using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Implement;
using Project.Business.Interface;
using Project.Business.Interface.Project.Business.Interface;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.Entity;
using Project.MVC.Models;
using SERP.Framework.ApiUtils.Responses;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class ProfileUserController : Controller
    {
        private readonly IVoucherBusiness _voucherBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IVoucherUsersBusiness _voucherUsersBusiness;
        private readonly HttpClient _httpClient;

        public ProfileUserController(IVoucherBusiness voucherBusiness,IVoucherUsersBusiness voucherUsersBusiness, IUserBusiness userBusiness, HttpClient httpClient)
        {
            _voucherBusiness  = voucherBusiness;
            _voucherUsersBusiness = voucherUsersBusiness;
            _userBusiness = userBusiness;
            _httpClient = httpClient;
        }

        public async Task<IActionResult> ProfileUser()
        {
            var errAndUserEntity = await CheckUserIsLoginning();
            TempData["ErrProfileUserMs"] = errAndUserEntity.Err;
            return View(errAndUserEntity.User);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileUser(IFormCollection form, IFormFile? AvatarFile)
        {
            var errAndUserEntity = await CheckUserIsLoginning();
            if (!String.IsNullOrWhiteSpace(errAndUserEntity.Err))
            {
                TempData["ErrProfileUserMs"] = errAndUserEntity.Err;
                return View(new UserEntity());
            }

            var user = errAndUserEntity.User;
            user.Name = form["Name"];
            user.Email = form["Email"];
            user.PhoneNumber = form["PhoneNumber"];
            user.Address = form["Address"];

            //Lưu thông tin thông thường  
            try
            {
                user.Name = user.Name?.Trim();
                user.Email = user.Email?.Trim();
                user.PhoneNumber = user.PhoneNumber?.Trim();
                user.Address = user.Address?.Trim();

                //Tìm xem SĐT đã được sử dụng chưa
                if (!String.IsNullOrWhiteSpace(user.PhoneNumber))
                {
                    var lstUserFoundByPhone = _userBusiness.LocUserTheoNhieuDK(new UserQueryModel
                    {
                        PhoneNumber = user.PhoneNumber
                    }).Result.Where(u => u.IsDeleted == false && u.Id != user.Id);

                    //Nếu đã tồn tại
                    if (lstUserFoundByPhone.Any())
                    {
                        TempData["ErrProfileUserUpdateMs"] = "Số điện thoại đã được sử dụng! Vui lòng lưu số khác!";
                        return RedirectToAction("ProfileUser");
                    }
                }

                //Tìm xem Email đã được sử dụng chưa
                if (!String.IsNullOrWhiteSpace(user.Email))
                {
                    var lstUserFoundByEmail = _userBusiness.LocUserTheoNhieuDK(new UserQueryModel
                    {
                        Email = user.Email
                    }).Result.Where(u => u.IsDeleted == false && u.Id != user.Id);

                    //Nếu đã tồn tại
                    if (lstUserFoundByEmail.Any())
                    {
                        TempData["ErrProfileUserUpdateMs"] = "Email đã được sử dụng! Vui lòng lưu email khác!";
                        return RedirectToAction("ProfileUser");
                    }
                }

                await _userBusiness.PatchAsync(user);
            }
            catch (Exception)
            {
                TempData["ErrProfileUserUpdateMs"] = "Lỗi khi cập nhật thông tin hồ sơ! Vui lòng thử lại sau!";
                return RedirectToAction("ProfileUser");
            }

            //Lưu ảnh  
            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                try
                {
                    using var formData = new MultipartFormDataContent();

                    // Đọc nội dung file và thêm vào form-data  
                    var streamContent = new StreamContent(AvatarFile.OpenReadStream());
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(AvatarFile.ContentType);
                    formData.Add(streamContent, "File", AvatarFile.FileName);

                    // Thêm tên file (Tên file unique nhằm tránh trường hợp đè file)
                    formData.Add(new StringContent(Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("dd-MM-yyyyHH.mm.ss.fff")), "FileName");

                    // Gửi tới API  
                    var response = await _httpClient.PostAsync("https://localhost:7293/api/files/upload", formData);

                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrProfileUserUpdateMs"] = "Lỗi khi tải ảnh lên máy chủ!";
                        return RedirectToAction("ProfileUser");
                    }

                    // Đọc và parse dữ liệu từ JSON  
                    var json = await response.Content.ReadAsStringAsync();

                    var uploadResult = System.Text.Json.JsonSerializer.Deserialize<ResponseObject<ImageFileEntity>>(json, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (uploadResult == null || uploadResult.Data == null)
                    {
                        TempData["ErrProfileUserUpdateMs"] = "Phản hồi không hợp lệ từ API ảnh!";
                        return RedirectToAction("ProfileUser");
                    }

                    var imageFile = uploadResult.Data;

                    // Cập nhật vào user  
                    user.AvartarUrl = imageFile.CompleteFilePath;
                    await _userBusiness.PatchAsync(user);
                }
                catch (Exception)
                {
                    TempData["ErrProfileUserUpdateMs"] = "Lỗi khi cập nhật ảnh đại diện người dùng! Vui lòng thử lại sau!";
                    return RedirectToAction("ProfileUser");
                }
            }

            TempData["ErrProfileUserUpdateMs"] = "";
            TempData["SuccessProfileUserUpdateMs"] = "Cập nhật hồ sơ thành công!";
            HttpContext.Session.SetString(UserConstants.UserSessionKey, JsonConvert.SerializeObject(user));
            return RedirectToAction("ProfileUser");
        }

     
        //Check user có trong session + tồn tại + chưa bị xóa + IsActive = true + type = Customer  
      

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
                            errAndUserEntity.Err = "Không tìm thấy người dùng này! Vui lòng kiểm tra lại!";
                        }
                    }
                    else
                    {
                        errAndUserEntity.Err = "Không tìm thấy người dùng này! Vui lòng kiểm tra lại!";
                    }
                }
                else
                {
                    errAndUserEntity.Err = "Phiên đăng nhập đã hết hạn hoặc người dùng chưa đăng nhập.";
                }
            }
            else
            {
                errAndUserEntity.Err = "Phiên đăng nhập đã hết hạn hoặc người dùng chưa đăng nhập.";
            }

            return errAndUserEntity;
        }
    }
}
