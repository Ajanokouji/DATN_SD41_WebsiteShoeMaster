using Microsoft.AspNetCore.Mvc;

namespace Project.MVC.Controllers
{
    public class PaymentMethodController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
