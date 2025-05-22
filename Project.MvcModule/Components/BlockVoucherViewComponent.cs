using Microsoft.AspNetCore.Mvc;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MvcModule.Components
{
    public class BlockVoucherViewComponent: BaseViewComponent
    {
        public BlockVoucherViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(ProductEntity entity)
        {
            var viewPath = GetViewPath("BlockVoucher", "BlockVoucher.cshtml");
            return View(viewPath, entity);
        }
    }
}
