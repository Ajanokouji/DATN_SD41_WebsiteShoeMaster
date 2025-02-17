using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.Business;

namespace Project.Business.Interface.Repositories
{
    public interface IProductCategoriesRelationRepository : IRepository<ProductCategoriesRelation, ProductCategoriesRelationQueryModel>
    {
        protected const string MessageNoTFound = "ProductCategoriesRelation not found";
        Task<ProductCategoriesRelation> SaveAsync(ProductCategoriesRelation entity);
        Task<IEnumerable<ProductCategoriesRelation>> SaveAsync(IEnumerable<ProductCategoriesRelation> entities);
    }
}
