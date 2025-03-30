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
            var listUserFoundByUsName = await _userBusiness.LocUserTheoNhieuDK(new UserQueryModel 
            {
                Username = user.Username
            });
            var userFound = listUserFoundByUsName?.FirstOrDefault(x =>x.Isdeleted == false);

            //Nếu user tìm bằng username không tồn tại thì tìm tiếp bằng số điện thoại
            if (userFound == null)
            {
                var listUserFoundBySdt = await _userBusiness.LocUserTheoNhieuDK(new UserQueryModel
                {
                    PhoneNumber = user.Username
                });
                userFound = listUserFoundBySdt?.FirstOrDefault(x => x.Isdeleted == false);

                //Nếu user tìm bằng SĐT không tồn tại thì tìm tiếp bằng Email
                if (userFound == null)
                {
                    var listUserFoundByEmail = await _userBusiness.LocUserTheoNhieuDK(new UserQueryModel
                    {
                        Email = user.Username
                    });
                    userFound = listUserFoundByEmail?.FirstOrDefault(x => x.Isdeleted == false);

                    //Nếu user tìm bằng Email không tồn tại thì xác định user hoàn toàn không tồn tại
                    if (userFound == null)
                    {
                        TempData["ErrMs"] = "Thông tin đăng nhập chưa chính xác, vui lòng kiểm tra lại";
                        return View(user);
                    }
                }
            }

            //Password đúng
            if (userFound.Password != user.Password)
            {
                TempData["ErrMs"] = "Thông tin đăng nhập chưa chính xác, vui lòng kiểm tra lại";
                return View(user);
            }

            //Check Active
            if (userFound.IsActive == false)
            {
                TempData["ErrMs"] = "Tài khoản đã bị tạm dừng hoạt động";
                return View(user);
            }

            //Đăng nhập thành công
            if (userFound.Type == "0") //Nếu type là Admin
            {
                //Tạo session user
                HttpContext.Session.SetString(UserConstants.UserSessionKey, JsonConvert.SerializeObject(userFound));

                //Mới in ra thông báo Chưa thực hiện chuyển hướng
                TempData["ErrMs"] = "Đăng nhập thành công dưới quyền Admin";
                return View(user);
            }
            else if (userFound.Type == "1") //Nếu type là Khách hàng
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
                    var cartSessions = JsonConvert.DeserializeObject<List<CartSession>>(cartSessionJson);
                    //Nếu list session cart không trống
                    if (cartSessions != null && cartSessions.Any())
                    {
                        //Lấy thông tin sản phẩm trong cart session
                        var cartItemsResult = await _cartBusiness.GetCartItems(cartSessions);
                        //Lấy thành công
                        if (cartItemsResult.IsSuccess)
                        {
                            //Thêm cart item của cart session vào cart của user
                            var rs = await _cartBusiness.AddCartSessionToCartDb(userFound, cartItemsResult.Data);

                            //Xóa session cart
                            HttpContext.Session.Remove(CartConstants.CartSessionKey);

                            //In ra thông báo trạng thái thành công của việc thêm cart session vào cart user
                            ms = rs.Message;
                        }
                    }
                }

                //Mới in ra thông báo Chưa thực hiện chuyển hướng
                TempData["ErrMs"] = "Đăng nhập thành công dưới quyền khách hàng " + ms;
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
