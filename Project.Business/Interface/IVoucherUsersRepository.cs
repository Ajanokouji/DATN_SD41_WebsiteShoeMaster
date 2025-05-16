using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Interface
{
    public interface IVoucherUsersRepository : IRepository<VoucherUsers, VoucherUsersQueryModel>
    {
        protected const string MessageNotFound = "VoucheUser not found";
        Task<VoucherUsers> SaveAsync(VoucherUsers article);
        Task<IEnumerable<VoucherUsers>> SaveAsync(IEnumerable<VoucherUsers> voucherUsers);
        Task<Pagination<UserEntity>> SearchUser(UserQueryModel queryModel);
        Task<IEnumerable<UserEntity>> GetUsersByIds(Guid[] ids);
        Task<IEnumerable<VoucherUsers>> FindByVidUid(IEnumerable<VoucherUsers> voucherUsers);
    }
}
