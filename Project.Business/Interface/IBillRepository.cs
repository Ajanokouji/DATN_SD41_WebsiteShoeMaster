using Project.Business.Model;
using Project.DbManagement;

namespace Project.Business.Interface;

public interface IBillRepository : IRepository<BillEntity, BillQueryModel>
{
    protected const string MessageNotFound = "Message not found";
    Task<BillEntity> saveAsync(BillEntity bills);
    Task<IEnumerable<BillEntity>> saveAsync(IEnumerable<BillEntity> bills);
}