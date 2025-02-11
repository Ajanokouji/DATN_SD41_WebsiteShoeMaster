using Project.Business.Model;
using Project.DbManagement;

namespace Project.Business;

public interface IBillRepository : IRepository<Bill, BillQueryModel>
{
    protected const string MessageNotFound = "Message not found";
    Task<Bill> saveAsync(Bill bills);
    Task<IEnumerable<Bill>> saveAsync(IEnumerable<Bill> bills);
}