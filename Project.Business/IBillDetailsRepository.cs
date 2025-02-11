using Project.Business.Model;
using Project.DbManagement;

namespace Project.Business;

public interface IBillDetailsRepository : IRepository<BillDetails, BillDetailsQueryModel>
{
    protected const string MessageNotFound = "Message not found";
    Task<BillDetails> saveAsync(BillDetails billDetails);
    Task<IEnumerable<BillDetails>> saveAsync(IEnumerable<BillDetails> billDetails);
}