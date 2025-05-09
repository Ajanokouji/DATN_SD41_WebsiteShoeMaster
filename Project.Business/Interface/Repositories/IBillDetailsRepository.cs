using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.ViewModels;

namespace Project.Business.Interface.Repositories;

public interface IBillDetailsRepository : IRepository<BillDetailsEntity, BillDetailsQueryModel>
{
    protected const string MessageNotFound = "Bill not found";
    Task<BillDetailsEntity> SaveAsync(BillDetailsEntity billDetails);
    Task<IEnumerable<BillDetailsEntity>> SaveAsync(IEnumerable<BillDetailsEntity> billDetails);
   
    public Task<bool> SaveBillDetails(BillDetailsRequest request);
    public Task<BillDetailsEntity> UpdateQuantity(Guid idBillDetails, int quantity);
    public Task<bool> DeleteBillDetails(Guid idBillDetails);
    public Task<List<BillDetailsViewModel>> GetBillDetailsByIdBill(Guid idBill);
}