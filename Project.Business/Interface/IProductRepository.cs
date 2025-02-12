using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Interface
{
    public interface IProductRepository : IRepository<ProductEntity, ProductQueryModel>
    {
        protected const string MessageNoTFound = "Product not found";
        Task<ProductEntity> SaveAsync(ProductEntity article);
        Task<IEnumerable<ProductEntity>> SaveAsync(IEnumerable<ProductEntity> productEntities);


    }
}
