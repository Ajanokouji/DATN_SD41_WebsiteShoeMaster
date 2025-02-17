using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Business.Interface
{
    public interface IProductCategoriesRelationBusiness
    {
        Task<Pagination<ProductCategoriesRelation>> GetAllAsync(ProductCategoriesRelationQueryModel queryModel);

        Task<IEnumerable<ProductCategoriesRelation>> ListAllAsync(ProductCategoriesRelationQueryModel queryModel);

        Task<int> GetCountAsync(ProductCategoriesRelationQueryModel queryModel);

        Task<IEnumerable<ProductCategoriesRelation>> ListByIdsAsync(IEnumerable<Guid> ids);

        Task<ProductCategoriesRelation> FindAsync(Guid id);

        Task<ProductCategoriesRelation> DeleteAsync(Guid id);

        Task<IEnumerable<ProductCategoriesRelation>> DeleteAsync(Guid[] deleteIds);

        Task<ProductCategoriesRelation> SaveAsync(ProductCategoriesRelation entity);

        Task<IEnumerable<ProductCategoriesRelation>> SaveAsync(IEnumerable<ProductCategoriesRelation> entities);
    }
}
