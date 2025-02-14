using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Project.Business.Interface
{
    interface IUserRepository : IRepository<User, UserQueryModel>
    {
        protected const string MessageNoTFound = "User not found";
        Task<User> SaveAsync(User article);
        Task<IEnumerable<User>> SaveAsync(IEnumerable<User> userEntities);
    }
}
