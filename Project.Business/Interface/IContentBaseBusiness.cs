using Project.DbManagement.Entity;
using SERP.Framework.Common;
using SERP.NewsMng.Business.Models.QueryModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Business.Interface
{
    public interface IContentBaseBusiness
    {
        /// <summary>
        /// Gets all content.
        /// </summary>
        /// <param name="queryModel">The query model.</param>
        /// <returns>The paginated list of content.</returns>
        Task<Pagination<ContentBase>> GetAllAsync(ContentBaseQueryModel queryModel);

        /// <summary>
        /// Lists all content based on the query model.
        /// </summary>
        /// <param name="queryModel">The query model.</param>
        /// <returns>The list of content.</returns>
        Task<IEnumerable<ContentBase>> ListAllAsync(ContentBaseQueryModel queryModel);

        /// <summary>
        /// Counts the number of content entries based on the query model.
        /// </summary>
        /// <param name="queryModel">The content query model.</param>
        /// <returns>The count of content entries.</returns>
        Task<int> GetCountAsync(ContentBaseQueryModel queryModel);

        /// <summary>
        /// Gets content by its ID.
        /// </summary>
        /// <param name="contentId">The content ID.</param>
        /// <returns>The content.</returns>
        Task<ContentBase> FindAsync(Guid contentId);

        /// <summary>
        /// Deletes content by its ID.
        /// </summary>
        /// <param name="contentId">The content ID.</param>
        /// <returns>The deleted content.</returns>
        Task<ContentBase> DeleteAsync(Guid contentId);

        /// <summary>
        /// Deletes a list of content by their IDs.
        /// </summary>
        /// <param name="deleteIds">The list of content IDs.</param>
        /// <returns>The deleted content list.</returns>
        Task<IEnumerable<ContentBase>> DeleteAsync(Guid[] deleteIds);

        /// <summary>
        /// Saves a content.
        /// </summary>
        /// <param name="contentBase">The content entity.</param>
        /// <returns>The saved content.</returns>
        Task<ContentBase> SaveAsync(ContentBase contentBase);

        /// <summary>
        /// Saves a list of content.
        /// </summary>
        /// <param name="contentBases">The list of content entities.</param>
        /// <returns>The saved content list.</returns>
        Task<IEnumerable<ContentBase>> SaveAsync(IEnumerable<ContentBase> contentBases);

        /// <summary>
        /// Updates a content.
        /// </summary>
        /// <param name="contentBase">The content entity.</param>
        /// <returns>The updated content.</returns>
        Task<ContentBase> PatchAsync(ContentBase contentBase);
        Task<List<ContentBase>> SearchContentBaseByKeywordsAsync(string input);
        Task<List<ContentBase>> GetByIdsAsync(IEnumerable<Guid> ids);


    }
}
