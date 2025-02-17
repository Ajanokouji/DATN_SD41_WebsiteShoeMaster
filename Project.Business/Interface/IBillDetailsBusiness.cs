using Project.Business.Model;
using Project.DbManagement;
using SERP.Framework.Common;

namespace Project.Business.Interface;

public interface IBillDetailsBusiness
{
    Task<Pagination<BillDetails>> GetAllAsync(BillDetailsQueryModel queryModel);

    Task<IEnumerable<BillDetails>> ListAllAsync(BillDetailsQueryModel queryModel);

    Task<int> GetCountAsync(BillDetailsQueryModel queryModel);

    Task<IEnumerable<BillDetails>> ListByIdsAsync(IEnumerable<Guid> ids);

    Task<BillDetails> FindAsync(Guid contentId);

    Task<BillDetails> DeleteAsync(Guid contentId);
    
    Task<IEnumerable<BillDetails>> DeleteAsync(Guid[] deleteIds);

    Task<BillDetails> SaveAsync(BillDetails article);

    Task<IEnumerable<BillDetails>> SaveAsync(IEnumerable<BillDetails> article);

    Task<BillDetails> PatchAsync(BillDetails article);
}