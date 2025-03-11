using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.Business.Model;
using Project.MvcModule.Model;
using Project.MvcModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MvcModule
{
    public class BlockProductsByFilter : BaseViewComponent
    {
        private readonly IProductBusiness _productBusiness;
        public BlockProductsByFilter(IProductBusiness productBusiness)
        {
            _productBusiness=productBusiness;
        }

        public async Task<IViewComponentResult> InvokeAsync(BlockProductsByFilterRequestModel moduleConfig)
        {
            var viewData = new BlockProductsByFilterViewModel();
            var res = await _productBusiness.GetAllAsync(moduleConfig.QueryModel);
            viewData.ProductsPagination = res;
            var viewPath = GetViewPath("BlockProductsByFilter", $"{moduleConfig.ViewName}.cshtml"?? "BlockProductsByFilter.cshtml");
            return View(viewPath, viewData);
        }
    }
}
