using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement.Entity;
using Project.MvcModule.Model;
using System.Net.WebSockets;

namespace Project.MvcModule.Components
{
    public class BlockProductDetailViewComponent : BaseViewComponent
    {
        private readonly IProductBusiness _productBusiness;
        public BlockProductDetailViewComponent(IProductBusiness productBusiness)
        {
            _productBusiness=productBusiness;
        }

        public async  Task<IViewComponentResult> InvokeAsync(ProductEntity  entity )
        {
            var viewPath = GetViewPath("BlockProductDetail", "BlockProductDetail.cshtml");
            return View(viewPath, entity);
        }

    }
}
