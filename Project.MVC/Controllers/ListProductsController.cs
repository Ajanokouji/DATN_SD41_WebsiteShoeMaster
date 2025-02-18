using Microsoft.AspNetCore.Mvc;

namespace Project.MVC.Controllers
{
    public class ListProductsController : Controller
    {
        public IActionResult ListProducts()
        {
            return View();
        }
    }
}
