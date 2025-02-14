using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
namespace Project.Business.Interface
{
    public interface IUserRepository : IRepository<User, UserQueryModel>
    {
        protected const string MessageNoTFound = "User not found";
        Task<User> SaveAsync(User article);
        Task<IEnumerable<User>> SaveAsync(IEnumerable<User> userEntities);
    }
}
