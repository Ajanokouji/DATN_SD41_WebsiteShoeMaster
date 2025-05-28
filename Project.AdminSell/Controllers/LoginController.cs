using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Interface;
using Project.DbManagement;

namespace Project.AdminSell.Controllers;

public class LoginController : Controller
{
    private readonly IUserBusiness _userBusiness;

    public LoginController(IUserBusiness userBusiness)
    {
        _userBusiness = userBusiness;
    }
    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("LoginInfor") != null)
        {
            return RedirectToAction("Sell", "SellOff");
        }
        return View();
    }
    
    [HttpPost]
    public ActionResult Login(string username, string password)
    {
        var user = _userBusiness.UserLogin(username, password);
        if (user == null)
        {
            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
            return View();
        }
        if (user.IsActive != true)
        {
            ViewBag.Error = "Tài khoản đã bị khóa!";
            return View();
        }
        if (user.Type == UserTypeEnum.Customer)
        {
            ViewBag.Error = "Tài khoản không có quyền truy cập!";
            return View();
        }

        var response = JsonConvert.SerializeObject(user);
        HttpContext.Session.SetString("LoginInfor", response);
        return RedirectToAction("Sell", "SellOff");
    }

    public ActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Login");
    }
}