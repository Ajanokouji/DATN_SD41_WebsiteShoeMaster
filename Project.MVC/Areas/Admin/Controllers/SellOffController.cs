using Microsoft.AspNetCore.Mvc;

namespace Project.MVC.Areas.Admin.Controllers;

public class SellOffController : Controller
{
    // GET
    [Area("Admin")]
    public IActionResult SellOff()
    {
        return View();
    }
}