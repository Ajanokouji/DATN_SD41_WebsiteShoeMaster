using Project.DbManagement.Entity;
using Project.Business.Model;
using SERP.Framework.Common;
using Project.Business.Interface.Repositories;


namespace Project.Business.Interface
{
    public interface ICommentsRepository :IRepository<CommentsEntity, CommentsModel>
    {
        Task<CommentsEntity> FindAsync(Guid id);
        Task<Pagination<CommentsEntity>> GetAllAsync(CommentsModel queryModel);
        Task<int> GetCountAsync(CommentsModel queryModel);
        Task<IEnumerable<CommentsEntity>> ListAllAsync(CommentsModel queryModel);
        Task<IEnumerable<CommentsEntity>> ListByIdsAsync(IEnumerable<Guid> ids);
        Task<IEnumerable<CommentsEntity>> SaveAsync(IEnumerable<CommentsEntity> entities);
        Task<CommentsEntity> DeleteAsync(Guid id);
        Task<IEnumerable<CommentsEntity>> DeleteAsync(Guid[] ids);
    }
}
