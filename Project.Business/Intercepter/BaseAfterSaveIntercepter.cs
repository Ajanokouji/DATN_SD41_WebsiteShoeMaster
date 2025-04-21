using SERP.Framework.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Intercepter
{

    public interface BaseAfterSaveIntercepter <T>
    {
        int Order { get; set; }

        Task Intercept(T oldNodeEntity, T updatedNodeEntity);
    }
}
