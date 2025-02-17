using Project.Business.Model;
using Project.DbManagement;

namespace Project.Business.Interface.Repositories;

public interface IBillRepository : IRepository<BillEntity, BillQueryModel>
{
    protected const string MessageNotFound = "Message not found";
    Task<BillEntity> SaveAsync(BillEntity bills);
    Task<IEnumerable<BillEntity>> SaveAsync(IEnumerable<BillEntity> bills);
}