using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Interface;

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
        if (user != null)
        {
            var response = JsonConvert.SerializeObject(user);
            HttpContext.Session.SetString("LoginInfor", response);
            return RedirectToAction("Sell", "SellOff");
        }

        ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu.";
        return View();
    }

    public ActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Login");
    }
}