using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using SERP.Framework.Common;

namespace Project.Business.Interface
{
    public interface IUserBusiness
    {
        Task<Pagination<UserEntity>> GetAllAsync(UserQueryModel queryModel);

        /// <summary>
        /// Gets list of users.
        /// </summary>
        /// <param name="queryModel">The query model.</param>
        /// <returns>The list of users.</returns>
        Task<IEnumerable<UserEntity>> ListAllAsync(UserQueryModel queryModel);

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
        Task<IEnumerable<UserEntity>> ListByIdsAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Gets a user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <returns>The user.</returns>
        Task<UserEntity> FindAsync(Guid id);

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <returns>The deleted user.</returns>
        Task<UserEntity> DeleteAsync(Guid id);

        /// <summary>
        /// Deletes a list of users.
        /// </summary>
        /// <param name="deleteIds">The list of user ids.</param>
        /// <returns>The deleted users.</returns>
        Task<IEnumerable<UserEntity>> DeleteAsync(Guid[] deleteIds);

        /// <summary>
        /// Saves a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<UserEntity> SaveAsync(UserEntity user);

        /// <summary>
        /// Saves users.
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        Task<IEnumerable<UserEntity>> SaveAsync(IEnumerable<UserEntity> users);

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<UserEntity> PatchAsync(UserEntity user);

        /// <summary>
        /// Tìm người dùng theo email
        /// </summary>
        /// <param name="email">Email người dùng</param>
        /// <returns>Thông tin người dùng</returns>
        Task<UserEntity> GetUserByEmail(string email);

        /// <summary>
        /// Tạo người dùng mới từ thông tin khách hàng
        /// </summary>
        /// <param name="customerInfo">Thông tin khách hàng</param>
        /// <returns>Thông tin người dùng đã tạo</returns>
        Task<UserEntity> CreateUserFromCustomerInfo(CustomerInfoModel customerInfo);

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="userId">ID người dùng</param>
        /// <param name="customerInfo">Thông tin khách hàng mới</param>
        /// <returns>Kết quả cập nhật</returns>
        Task<ServiceResult<bool>> UpdateUserInfo(Guid userId, CustomerInfoModel customerInfo);

        Task<IEnumerable<UserEntity>> LocUserTheoNhieuDK(UserQueryModel queryModel);
        UserEntity UserLogin(string username, string password);
    }
}

