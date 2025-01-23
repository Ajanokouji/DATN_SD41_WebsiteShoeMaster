using SERP.Framework.Common;

namespace Project.Common
{
    public interface IRepository<T, Q>
    {
        Task<Pagination<T>> GetAllAsync(Guid applicationId, Q queryModel);

        /// <summary>
        /// Gets list of contents.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <param name="queryModel">The query model.</param>
        /// <returns>The list of contents.</returns>
        Task<IEnumerable<T>> ListAllAsync(Guid applicationId, Q queryModel);

        /// <summary>
        /// Count the number of contents by query model.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <param name="queryModel">The contents query model.</param>
        /// <returns>The number of contents by query model.</returns>
        Task<int> GetCountAsync(Guid applicationId, Q queryModel);

        /// <summary>
        /// Gets list of contents.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <param name="ids">The list of ids.</param>
        /// <returns>The list of contents.</returns>
        Task<IEnumerable<T>> ListByIdsAsync(Guid applicationId, IEnumerable<Guid> ids);

        /// <summary>
        /// Gets a content.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <param name="contentId">The content id.</param>
        /// <returns>The content.</returns>
        Task<T> FindAsync(Guid applicationId, Guid contentId);

        /// <summary>
        /// Deletes a content.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <param name="contentId">The content id.</param>
        /// <param name="actor">The actor.</param>
        /// <returns>The deleted content.</returns>
        Task<T> DeleteAsync( Guid contentId );

        /// <summary>
        /// Deletes a list of contents.
        /// </summary>
        /// <param name="applicationId">The application id.</param>
        /// <param name="deleteIds">The list of content ids.</param>
        /// <param name="actor">The actor.</param>
        /// <returns>The deleted content.</returns>
        Task<IEnumerable<T>> DeleteAsync( Guid[] deleteIds);
    }
}
