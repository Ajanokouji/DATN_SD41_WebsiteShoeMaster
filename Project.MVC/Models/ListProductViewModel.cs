using Project.DbManagement.Entity;
using SERP.Framework.Common;

namespace Project.MVC.Models
{
    public class ListProductViewModel
    {
        public Pagination<ProductEntity> ProductContent { get; set; }
    }
}
