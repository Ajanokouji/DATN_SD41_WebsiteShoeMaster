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
    public interface ICommentsBusiness
    {
        Task<CommentsEntity> DeleteAsync(Guid id);
        Task<IEnumerable<CommentsEntity>> DeleteAsync(Guid[] deleteIds);
        Task<CommentsEntity> FindAsync(Guid id);
        Task<Pagination<CommentsEntity>> GetAllAsync(CommentsModel queryModel);
        Task<int> GetCountAsync(CommentsModel queryModel);
        Task<IEnumerable<CommentsEntity>> ListAllAsync(CommentsModel queryModel);
        Task<IEnumerable<CommentsEntity>> ListByIdsAsync(IEnumerable<Guid> ids);
        Task<CommentsEntity> PatchAsync(CommentsEntity model);
        Task<CommentsEntity> SaveAsync(CommentsEntity comment);
        Task<IEnumerable<CommentsEntity>> SaveAsync(IEnumerable<CommentsEntity> comments);
    }
}
