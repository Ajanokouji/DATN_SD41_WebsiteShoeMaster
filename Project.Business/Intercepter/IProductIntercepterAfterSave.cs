using Project.DbManagement.Entity;
using SERP.Framework.Business.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Intercepter
{
    public interface IProductIntercepterAfterSave : IAfterSavedInterceptor<ProductEntity>
    {
    }
}
