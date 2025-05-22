using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.ViewModels;

namespace Project.Business.Interface.Repositories;

public interface IBillRepository : IRepository<BillEntity, BillQueryModel>
{
    protected const string MessageNotFound = "Message not found";
    Task<BillEntity> SaveAsync(BillEntity bills);
    Task<IEnumerable<BillEntity>> SaveAsync(IEnumerable<BillEntity> bills);
    Task<BillEntity> FindByCodeAsync(string billCode);
    public List<BillEntity> GetAllPendingBill();
    public bool CreatePendingBill(Guid idEmployee);
    public bool DeletePendingBill(Guid idBill);
    public BillViewModel GetPDBillById(Guid idBill);
    public bool PaymentBill(PaymentBillRequest bill);
}