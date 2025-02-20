using Project.Business.Model;
using Project.DbManagement;
using SERP.Framework.Common;

namespace Project.Business.Interface;

public interface IBillDetailsBusiness
{
    Task<Pagination<BillDetailsEntity>> GetAllAsync(BillDetailsQueryModel queryModel);

    Task<IEnumerable<BillDetailsEntity>> ListAllAsync(BillDetailsQueryModel queryModel);

    Task<int> GetCountAsync(BillDetailsQueryModel queryModel);

    Task<IEnumerable<BillDetailsEntity>> ListByIdsAsync(IEnumerable<Guid> ids);

    Task<BillDetailsEntity> FindAsync(Guid contentId);

    Task<BillDetailsEntity> DeleteAsync(Guid contentId);
    
    Task<IEnumerable<BillDetailsEntity>> DeleteAsync(Guid[] deleteIds);

    Task<BillDetailsEntity> SaveAsync(BillDetailsEntity article);

    Task<IEnumerable<BillDetailsEntity>> SaveAsync(IEnumerable<BillDetailsEntity> article);

    Task<BillDetailsEntity> PatchAsync(BillDetailsEntity article);
}