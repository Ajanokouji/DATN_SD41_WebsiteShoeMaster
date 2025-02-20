﻿using Microsoft.AspNetCore.Mvc;

namespace Project.MVC.Areas.Admin.Controllers;

public class HomeController : Controller
{
    // GET
    [Area("Admin")]
    public IActionResult Home()
    {
        return View();
    }
}