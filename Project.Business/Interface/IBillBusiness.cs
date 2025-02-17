using Project.Business.Model;
using Project.DbManagement;
using SERP.Framework.Common;

namespace Project.Business.Interface;

public interface IBillBusiness
{
    Task<Pagination<BillEntity>> GetAllAsync(BillQueryModel queryModel);

    Task<IEnumerable<BillEntity>> ListAllAsync(BillQueryModel queryModel);

    Task<int> GetCountAsync(BillQueryModel queryModel);

    Task<IEnumerable<BillEntity>> ListByIdsAsync(IEnumerable<Guid> ids);

    Task<BillEntity> FindAsync(Guid contentId);

    Task<BillEntity> DeleteAsync(Guid contentId);
    
    Task<IEnumerable<BillEntity>> DeleteAsync(Guid[] deleteIds);

    Task<BillEntity> SaveAsync(BillEntity article);

    Task<IEnumerable<BillEntity>> SaveAsync(IEnumerable<BillEntity> article);

    Task<BillEntity> PatchAsync(BillEntity article);
}