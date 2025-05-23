using Microsoft.AspNetCore.Mvc;
using Nest;
using Project.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MvcModule.Components
{
    public class UserBillStatusTabViewComponent : BaseViewComponent
    {
        public UserBillStatusTabViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var statuses = new Dictionary<string, string>
        {
                { "All", "Tất cả" },
                { "PendingConfirmation", "Chờ thanh toán" },
                { "Shipping", "Vận chuyển" },
                { "Delivered", "Đã giao hàng" },
                { "Completed", "Hoàn thành" },
                { "Cancelled", "Đã hủy" }
        };

            ViewData["ActiveStatus"] = statuses;

            var viewPath = GetViewPath(" UserBillStatusTab", " UserBillStatusTab.cshtml");
            return View(viewPath);
        }
    }
}
