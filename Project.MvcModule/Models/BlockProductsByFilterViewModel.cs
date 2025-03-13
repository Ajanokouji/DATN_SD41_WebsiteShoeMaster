using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MvcModule.Model
{
    public class BlockProductsByFilterViewModel
    {
        public Pagination<ProductEntity> ProductsPagination { get; set; }
    }
}
