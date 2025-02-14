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
    public interface ICategoriesBusiness
    {
        Task<Pagination<CategoriesEntity>> GetAllAsync(CategoriesQueryModel queryModel);

        /// <summary>
        /// Gets list of categories.
        /// </summary>
        /// <param name="queryModel">The query model.</param>
        /// <returns>The list of categories.</returns>
        Task<IEnumerable<CategoriesEntity>> ListAllAsync(CategoriesQueryModel queryModel);

        /// <summary>
        /// Count the number of categories by query model.
        /// </summary>
        /// <param name="queryModel">The categories query model.</param>
        /// <returns>The number of categories by query model.</returns>
        Task<int> GetCountAsync(CategoriesQueryModel queryModel);

        /// <summary>
        /// Gets list of categories by ids.
        /// </summary>
        /// <param name="ids">The list of ids.</param>
        /// <returns>The list of categories.</returns>
        Task<IEnumerable<CategoriesEntity>> ListByIdsAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Gets a category.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        /// <returns>The category.</returns>
        Task<CategoriesEntity> FindAsync(Guid categoryId);

        /// <summary>
        /// Deletes a category.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        /// <returns>The deleted category.</returns>
        Task<CategoriesEntity> DeleteAsync(Guid categoryId);

        /// <summary>
        /// Deletes a list of categories.
        /// </summary>
        /// <param name="deleteIds">The list of category ids.</param>
        /// <returns>The deleted categories.</returns>
        Task<IEnumerable<CategoriesEntity>> DeleteAsync(Guid[] deleteIds);

        /// <summary>
        /// Saves a category.
        /// </summary>
        /// <param name="categoryEntity">The category entity.</param>
        /// <returns>The saved category.</returns>
        Task<CategoriesEntity> SaveAsync(CategoriesEntity categoryEntity);

        /// <summary>
        /// Saves categories.
        /// </summary>
        /// <param name="categoryEntities">The list of category entities.</param>
        /// <returns>The saved categories.</returns>
        Task<IEnumerable<CategoriesEntity>> SaveAsync(IEnumerable<CategoriesQueryModel> categoryEntities);

        /// <summary>
        /// Updates a category.
        /// </summary>
        /// <param name="categoryEntity">The category entity.</param>
        /// <returns>The updated category.</returns>
        Task<CategoriesEntity> PatchAsync(CategoriesEntity categoryEntity);
    }
}
