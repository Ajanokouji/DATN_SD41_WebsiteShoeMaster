using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.Business.Model;
using Project.MVC.Models;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class ListProductsController : Controller
    {
        private readonly IProductBusiness _productBusiness;

        public ListProductsController(IProductBusiness productBusiness)
        {
            _productBusiness = productBusiness;
        }

        public async Task<IActionResult> ListProducts()
        {
            var viewdata = new  ListProductViewModel();
            var res = await _productBusiness.GetAllAsync(new ProductQueryModel());
            viewdata.ProductContent = res;
            return View(viewdata);
        }
    }
}
