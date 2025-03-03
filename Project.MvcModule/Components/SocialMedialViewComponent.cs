

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Project.MvcModule.SocialMedialViewComponent
{
    [ViewComponent]
    public class SocialMedialViewComponent: BaseViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var viewPath = GetViewPath("SocialMedial", "SocialMedial.cshtml");
            return View(viewPath);
        }
    }
}
