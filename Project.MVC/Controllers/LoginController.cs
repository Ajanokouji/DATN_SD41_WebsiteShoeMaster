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
    public class LoginController : Controller
    {
        private readonly IUserBusiness _userBusiness;
        private readonly ICartBusiness _cartBusiness;

        public LoginController(IUserBusiness userBusiness, ICartBusiness cartBusiness)
        {
            _userBusiness = userBusiness;
            _cartBusiness = cartBusiness;
        }

        public IActionResult Login()
        {
            if (TempData["LoginUserData"] != null && !String.IsNullOrWhiteSpace(TempData["LoginUserData"].ToString()))
            {
                var user = JsonConvert.DeserializeObject<UserEntity>(TempData["LoginUserData"].ToString());
                return View(user);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserEntity user)
        {
            //Check trống
            if(user.Username == null || user.Password == null)
            {
                TempData["ErrLoginMs"] = "Tên đăng nhập hoặc mật khẩu không được để trống";
                TempData["LoginUserData"] = JsonConvert.SerializeObject(user);
                return RedirectToAction("Login");
            }

            //Tìm user dựa trên username
            var listUserFoundByUsName = await _userBusiness.LocUserTheoNhieuDK(new UserQueryModel 
            {
                Username = user.Username
            });
            var userFound = listUserFoundByUsName?.FirstOrDefault(x =>x.IsDeleted == false);

            //Nếu user tìm bằng username không tồn tại thì tìm tiếp bằng số điện thoại
            if (userFound == null)
            {
                var listUserFoundBySdt = await _userBusiness.LocUserTheoNhieuDK(new UserQueryModel
                {
                    PhoneNumber = user.Username
                });
                userFound = listUserFoundBySdt?.FirstOrDefault(x => x.IsDeleted == false);

                //Nếu user tìm bằng SĐT không tồn tại thì tìm tiếp bằng Email
                if (userFound == null)
                {
                    var listUserFoundByEmail = await _userBusiness.LocUserTheoNhieuDK(new UserQueryModel
                    {
                        Email = user.Username
                    });
                    userFound = listUserFoundByEmail?.FirstOrDefault(x => x.IsDeleted == false);

                    //Nếu user tìm bằng Email không tồn tại thì xác định user hoàn toàn không tồn tại
                    if (userFound == null)
                    {
                        TempData["ErrLoginMs"] = "Thông tin đăng nhập chưa chính xác, vui lòng kiểm tra lại";
                        TempData["LoginUserData"] = JsonConvert.SerializeObject(user);
                        return RedirectToAction("Login");
                    }
                }
            }

            //Password đúng
            if (userFound.Password != user.Password)
            {
                TempData["ErrLoginMs"] = "Thông tin đăng nhập chưa chính xác, vui lòng kiểm tra lại";
                TempData["LoginUserData"] = JsonConvert.SerializeObject(user);
                return RedirectToAction("Login");
            }

            //Check Active
            if (userFound.IsActive == false)
            {
                TempData["ErrLoginMs"] = "Tài khoản đã bị tạm dừng hoạt động";
                TempData["LoginUserData"] = JsonConvert.SerializeObject(user);
                return RedirectToAction("Login");
            }

            //Đăng nhập thành công
            if (userFound.Type == UserTypeEnum.Customer) //Nếu type là Khách hàng
            {
                string ms = "";

                //Lưu user vào session
                HttpContext.Session.SetString(UserConstants.UserSessionKey, JsonConvert.SerializeObject(userFound));

                ////Đồng bộ session cart với cart của user
                //Kiểm tra session cart
                var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
                //Nếu session cart json không trống
                if (!string.IsNullOrEmpty(cartSessionJson))
                {
                    //Giải json session cart
                    var cartSessions = JsonConvert.DeserializeObject<List<CartItem>>(cartSessionJson);
                    //Nếu list session cart không trống
                    if (cartSessions != null && cartSessions.Any())
                    {
                        //Lấy thông tin sản phẩm trong cart Db
                        var cartItemsResult = await _cartBusiness.GetCartItems(cartSessions);
                        //Lấy thành công
                        if (cartItemsResult.IsSuccess)
                        {
                            //Thêm cart item của cart session vào cart Db của user
                            var rs = await _cartBusiness.AddCartSessionToCartDb(userFound, cartItemsResult.Data);

                            //Xóa session cart
                            HttpContext.Session.Remove(CartConstants.CartSessionKey);

                            //In ra thông báo trạng thái thành công của việc thêm cart session vào cart user
                            ms = rs.Message;
                        }
                    }
                }

                //Chuyển hướng tới trang chủ
                return RedirectToAction("Index","Home");
            }
            else
            {
                //Tài khoản không phải khách hàng
                TempData["ErrLoginMs"] = "Thông tin đăng nhập chưa chính xác, vui lòng kiểm tra lại";
                TempData["LoginUserData"] = JsonConvert.SerializeObject(user);
                return RedirectToAction("Login");
            }
        }

        public IActionResult Logout()
        {
            //Xóa session user
            HttpContext.Session.Remove(UserConstants.UserSessionKey);
            //Chuyển hướng về trang chủ
            return RedirectToAction("Login");
        }
    }
}
