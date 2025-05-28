using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement;
using Project.MVC.Models;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class ListProductsController : Controller
    {
        private readonly IProductBusiness _productBusiness;
        private readonly ProjectDbContext _context;

        public ListProductsController(IProductBusiness productBusiness, ProjectDbContext context)
        {
            _productBusiness = productBusiness;
            _context = context;
        }

        [HttpGet()]
        public IActionResult ListProductsByCategory(Guid? categoryId)
        {
            var products = _context.Products.AsQueryable();

            if (categoryId.HasValue)
                products = products.Where(p => p.MainCategoryId == categoryId.Value);

            ViewBag.Categories = _context.Categories.ToList();
            return View(products.ToList());
        }

        [HttpGet("ListProducts")]
        public async Task<IActionResult> ListProducts()
        {
            ViewBag.Categories = _context.Categories.ToList();
            var viewdata = new  ListProductViewModel();
            var res = await _productBusiness.GetAllAsync(new ProductQueryModel());
            viewdata.ProductContent = res;
            return View(viewdata);
        }
    }
}
