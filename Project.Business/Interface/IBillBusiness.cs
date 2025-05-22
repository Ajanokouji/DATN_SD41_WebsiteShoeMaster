using Project.Business.Model;
using Project.Business.Model.PatchModel;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.ViewModels;
using SERP.Framework.Common;

namespace Project.Business.Interface;

public interface IBillBusiness
{
    Task<Pagination<BillModel>> GetAllAsync(BillQueryModel queryModel);

    Task<IEnumerable<BillEntity>> ListAllAsync(BillQueryModel queryModel);

    Task<int> GetCountAsync(BillQueryModel queryModel);

    Task<IEnumerable<BillEntity>> ListByIdsAsync(IEnumerable<Guid> ids);

    Task<BillEntity> FindAsync(Guid contentId);


    Task<BillEntity> FindByCodeAsync(string billCode);

    Task<BillEntity> DeleteAsync(Guid contentId);
    
    Task<IEnumerable<BillEntity>> DeleteAsync(Guid[] deleteIds);

    Task<BillEntity> SaveAsync(BillEntity  billEntity);

    Task<IEnumerable<BillEntity>> SaveAsync(IEnumerable<BillEntity> article);

    Task<BillEntity> PatchAsync(BillPatchModel  model);

    Task <BillModel> CreateBill(BillModel model);
    Task <BillModel> GetBillById(Guid id);
    Task <BillModel> GetBillByCode(string code);
    Task <List<BillModel>> GetBillsByUserId(Guid userId);
    Task <bool> UpdateBillStatus(Guid billId, string status);
    Task <bool> UpdatePaymentStatus(Guid billId, string paymentStatus);
    Task <bool> UpdatePaymentMethod(Guid billId, string paymentMethod);
    Task <decimal> ApplyVoucher(string voucherCode, decimal totalAmount);
    Task <string> GenerateBillCode();
    Task <BillModel> CheckoutFromCart(
        List<CartItemModel> cartItems, 
        CustomerInfoModel customerInfo, 
        string voucherCode);
    
    public List<BillEntity> GetAllPendingBill();
    public bool CreatePendingBill(Guid idEmployee);
    public bool DeletePendingBill(Guid idBill);
    public BillViewModel GetPDBillById(Guid idBill);
    public bool PaymentBill(PaymentBillRequest bill);
}