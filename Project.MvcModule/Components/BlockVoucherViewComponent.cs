using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Implement;
using Project.Business.Interface;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.Entity;
using Project.DbManagement.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MvcModule.Components
{
    public class BlockVoucherViewComponent : BaseViewComponent
    {
        private readonly IVoucherBusiness _voucherBusiness;

        public BlockVoucherViewComponent(IVoucherBusiness voucherBusiness)
        {
            _voucherBusiness = voucherBusiness;
        }

        public async Task<IViewComponentResult> InvokeAsync(ProductEntity entity)
        {
            var vouchers = await _voucherBusiness.RecommendVoucherAllShop();

            // Sử dụng GetViewPath như cách của BlockProductsByFilter
            var viewPath = GetViewPath("BlockVoucher", "BlockVoucher.cshtml");

            return View(viewPath, vouchers);
        }
    }
}
