using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MvcModule
{
    public abstract class BaseViewComponent:ViewComponent
    {
        protected BaseViewComponent()
        {
        }

        protected virtual string GetViewPath(string moduleName, string defaultView)
        {
            string defaultModule = GetType().Name;
            string resolvedModuleName = moduleName ?? defaultModule;
            string resolvedViewName = defaultView ?? $"{defaultModule}.cshtml";
            return $"~/Views/{resolvedModuleName}/{resolvedViewName}";
        }


    }
}
