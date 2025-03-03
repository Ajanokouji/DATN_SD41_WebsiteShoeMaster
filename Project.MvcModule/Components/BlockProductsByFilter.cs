using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MvcModule
{
    public class BlockProductsByFilter : BaseViewComponent
    {
        public BlockProductsByFilter()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var viewPath = GetViewPath("BlockProductByFilter", "BlockProductByFilter.cshtml");
            return View(viewPath);
        }
    }
}
