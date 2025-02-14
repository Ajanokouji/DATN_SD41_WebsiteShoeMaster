using Microsoft.AspNetCore.Mvc;

namespace Project.MVC.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult News()
        {
            return View();
        }
    }
}
