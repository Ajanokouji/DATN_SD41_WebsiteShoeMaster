using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
namespace Project.Business.Interface
{
    public interface IUserRepository : IRepository<UserEntity, UserQueryModel>
    {
        protected const string MessageNoTFound = "User not found";
        Task<UserEntity> SaveAsync(UserEntity article);
        Task<IEnumerable<UserEntity>> SaveAsync(IEnumerable<UserEntity> userEntities);
        Task<IEnumerable<UserEntity>> LocUserTheoNhieuDK(UserQueryModel userQueryModel);
        UserEntity UserLogin(String username, String password);
    }
}
