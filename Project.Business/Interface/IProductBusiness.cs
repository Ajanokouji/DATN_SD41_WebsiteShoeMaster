using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Interface
{
    public interface IProductBusiness
    {
        Task<Pagination<ProductEntity>> GetAllAsync(ProductQueryModel queryModel);

        /// <summary>
        /// Gets list of contents.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <param name="queryModel">The query model.</param>
        /// <returns>The list of contents.</returns>
        Task<IEnumerable<ProductEntity>> ListAllAsync(ProductQueryModel queryModel);


        /// <summary>
        /// Count the number of contents by query model.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <param name="queryModel">The contents query model.</param>
        /// <returns>The number of contents by query model.</returns>
        Task<int> GetCountAsync(ProductQueryModel queryModel);

        /// <summary>
        /// Gets list of contents.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <param name="ids">The list of ids.</param>
        /// <returns>The list of contents.</returns>
        Task<IEnumerable<ProductEntity>> ListByIdsAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Gets a content.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <param name="contentId">The content id.</param>
        /// <returns>The content.</returns>
        Task<ProductEntity> FindAsync(Guid contentId);

        /// <summary>
        /// Deletes a content.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <param name="contentId">The content id.</param>
        /// <param name="actor">The actor.</param>
        /// <returns>The deleted content.</returns>
        Task<ProductEntity> DeleteAsync(Guid contentId);

        /// <summary>
        /// Deletes a list of contents.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <param name="deleteIds">The list of content ids.</param>
        /// <param name="actor">The actor.</param>
        /// <returns>The deleted content.</returns>
        Task<IEnumerable<ProductEntity>> DeleteAsync(Guid[] deleteIds);

        /// <summary>
        /// Saves a product.
        /// </summary>
        /// <param name="productEntity"></param>
        /// <returns></returns>
        Task<ProductEntity> SaveAsync(ProductEntity  productEntity);

        /// <summary>
        /// Saves  products.
        /// </summary>
        /// <param name="productEntity"></param>
        /// <returns></returns>
        Task<IEnumerable<ProductEntity>> SaveAsync(IEnumerable<ProductEntity> productEntity);

        /// <summary>
        /// Updates a product.
        /// </summary>
        /// <param name="productEntity"></param>
        /// <returns></returns>
        Task<ProductEntity> PatchAsync(ProductEntity productEntity);


    }
}
