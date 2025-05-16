using Project.Business.Model;
using Project.DbManagement.Entity;
using Project.DbManagement.ViewModels;

namespace Project.Business.Interface.Repositories
{
    public interface IProductRepository : IRepository<ProductEntity, ProductQueryModel>
    {
        protected const string MessageNoTFound = "Product not found";
        Task<ProductEntity> SaveAsync(ProductEntity article);
        Task<IEnumerable<ProductEntity>> SaveAsync(IEnumerable<ProductEntity> productEntities);
        
        public Task<List<ListProductSellViewModel>> GetAllProduct();
        public Task<ProductDetailsViewModel> GetProductDetailsById(Guid idprd);
        public Task<List<ListProductDetailsViewModel>> ListProductDetailsById(Guid idprd);
        public Task<List<ListProductSellViewModel>> SearchProduct(string keyword);
    }
}
