using Project.Business.Model;
using Project.DbManagement.Entity;

namespace Project.Business.Interface.Repositories
{
    public interface IProductRepository : IRepository<ProductEntity, ProductQueryModel>
    {
        protected const string MessageNoTFound = "Product not found";
        Task<ProductEntity> SaveAsync(ProductEntity article);
        Task<IEnumerable<ProductEntity>> SaveAsync(IEnumerable<ProductEntity> productEntities);

    }
}
