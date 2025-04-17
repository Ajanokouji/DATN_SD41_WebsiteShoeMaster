using Project.DbManagement;

namespace Project.AdminSell.IServices;

public interface ISellOffService
{
    public List<BillEntity> GetAllPendingBill();
    public bool CreatePendingBill(Guid idEmployee);
    public bool DeletePendingBill(Guid idBill);
}