using Project.Business.Model;
using Project.DbManagement;

namespace Project.Business.Interface.Repositories;

public interface IBillDetailsRepository : IRepository<BillDetailsEntity, BillDetailsQueryModel>
{
    protected const string MessageNotFound = "Bill not found";
    Task<BillDetailsEntity> SaveAsync(BillDetailsEntity billDetails);
    Task<IEnumerable<BillDetailsEntity>> SaveAsync(IEnumerable<BillDetailsEntity> billDetails);
}