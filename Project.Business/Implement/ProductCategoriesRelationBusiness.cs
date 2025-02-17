using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Business.Implement
{
    public class ProductCategoriesRelationBusiness : IProductCategoriesRelationBusiness
    {
        private readonly IProductCategoriesRelationRepository _repository;

        public ProductCategoriesRelationBusiness(IProductCategoriesRelationRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductCategoriesRelation> DeleteAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProductCategoriesRelation>> DeleteAsync(Guid[] deleteIds)
        {
            return await _repository.DeleteAsync(deleteIds);
        }

        public async Task<ProductCategoriesRelation> FindAsync(Guid id)
        {
            return await _repository.FindAsync(id);
        }

        public async Task<Pagination<ProductCategoriesRelation>> GetAllAsync(ProductCategoriesRelationQueryModel queryModel)
        {
            return await _repository.GetAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(ProductCategoriesRelationQueryModel queryModel)
        {
            return await _repository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<ProductCategoriesRelation>> ListAllAsync(ProductCategoriesRelationQueryModel queryModel)
        {
            return await _repository.ListAllAsync(queryModel);
        }

        public async Task<IEnumerable<ProductCategoriesRelation>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _repository.ListByIdsAsync(ids);
        }

        public async Task<ProductCategoriesRelation> SaveAsync(ProductCategoriesRelation entity)
        {
            var res = await SaveAsync(new[] { entity });
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<ProductCategoriesRelation>> SaveAsync(IEnumerable<ProductCategoriesRelation> entities)
        {
            return await _repository.SaveAsync(entities);
        }
    }
}
