using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.MVC.Models;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class ProductDetailsController : Controller
    {
        private readonly IProductBusiness _productBusiness;

        public ProductDetailsController(IProductBusiness productBusiness)
        {
            _productBusiness = productBusiness;
        }

        public async Task<IActionResult> ProductDetails(Guid id)
        {
            var viewData = new ProductDetailsViewModel();
            var res = await _productBusiness.FindAsync(id);
            viewData.Product = res;
            return View(viewData);
        }
    }
}