using Microsoft.AspNetCore.Mvc;

namespace Project.MVC.Areas.Admin.Controllers;

public class AdminController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}