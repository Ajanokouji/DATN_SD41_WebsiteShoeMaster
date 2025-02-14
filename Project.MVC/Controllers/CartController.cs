using Microsoft.AspNetCore.Mvc;

namespace Project.MVC.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Cart()
        {
            return View();
        }
    }
}
