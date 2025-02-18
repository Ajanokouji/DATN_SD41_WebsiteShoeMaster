using Microsoft.AspNetCore.Mvc;

namespace Project.MVC.Controllers
{
    public class ProductDetailsController : Controller
    {
        public IActionResult ProductDetails()
        {
            return View();
        }
    }
}
