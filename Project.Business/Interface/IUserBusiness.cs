using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.Common;

namespace Project.Business.Interface
{
    public interface IUserBusiness
    {
        Task<Pagination<User>> GetAllAsync(UserQueryModel queryModel);

        /// <summary>
        /// Gets list of users.
        /// </summary>
        /// <param name="queryModel">The query model.</param>
        /// <returns>The list of users.</returns>
        Task<IEnumerable<User>> ListAllAsync(UserQueryModel queryModel);

        /// <summary>
        /// Count the number of users by query model.
        /// </summary>
        /// <param name="queryModel">The users query model.</param>
        /// <returns>The number of users by query model.</returns>
        Task<int> GetCountAsync(UserQueryModel queryModel);

        /// <summary>
        /// Gets list of users by ids.
        /// </summary>
        /// <param name="ids">The list of ids.</param>
        /// <returns>The list of users.</returns>
        Task<IEnumerable<User>> ListByIdsAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Gets a user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <returns>The user.</returns>
        Task<User> FindAsync(Guid id);

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <returns>The deleted user.</returns>
        Task<User> DeleteAsync(Guid id);

        /// <summary>
        /// Deletes a list of users.
        /// </summary>
        /// <param name="deleteIds">The list of user ids.</param>
        /// <returns>The deleted users.</returns>
        Task<IEnumerable<User>> DeleteAsync(Guid[] deleteIds);

        /// <summary>
        /// Saves a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<User> SaveAsync(User user);

        /// <summary>
        /// Saves users.
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        Task<IEnumerable<User>> SaveAsync(IEnumerable<User> users);

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<User> PatchAsync(User user);
    }
}

