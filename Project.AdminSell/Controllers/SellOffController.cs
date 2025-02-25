using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project.AdminSell.Models;

namespace Project.AdminSell.Controllers;

public class SellOffController : Controller
{
    private readonly ILogger<SellOffController> _logger;

    public SellOffController(ILogger<SellOffController> logger)
    {
        _logger = logger;
    }

    public IActionResult SellOff()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
