using Microsoft.AspNetCore.Mvc;

namespace Project.MVC.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Checkout()
        {
            return View();
        }
    }
}
