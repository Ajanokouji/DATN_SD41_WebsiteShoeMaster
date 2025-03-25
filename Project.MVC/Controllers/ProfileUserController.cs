using Microsoft.AspNetCore.Mvc;

namespace Project.MVC.Controllers
{
    public class ProfileUserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
