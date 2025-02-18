using Microsoft.AspNetCore.Mvc;

namespace Project.MVC.Areas.Admin.Controllers;

public class HomeController : Controller
{
    // GET
    public IActionResult Home()
    {
        return View();
    }
}