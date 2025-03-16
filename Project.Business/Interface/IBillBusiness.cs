using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using SERP.Framework.Common;

namespace Project.Business.Interface;

public interface IBillBusiness
{
    Task<Pagination<BillEntity>> GetAllAsync(BillQueryModel queryModel);

    Task<IEnumerable<BillEntity>> ListAllAsync(BillQueryModel queryModel);

    Task<int> GetCountAsync(BillQueryModel queryModel);

    Task<IEnumerable<BillEntity>> ListByIdsAsync(IEnumerable<Guid> ids);

    Task<BillEntity> FindAsync(Guid contentId);

    Task<BillEntity> DeleteAsync(Guid contentId);
    
    Task<IEnumerable<BillEntity>> DeleteAsync(Guid[] deleteIds);

    Task<BillEntity> SaveAsync(BillEntity article);

    Task<IEnumerable<BillEntity>> SaveAsync(IEnumerable<BillEntity> article);

    Task<BillEntity> PatchAsync(BillEntity article);

    Task <BillModel> CreateBill(BillModel model);
    Task <BillModel> GetBillById(long id);
    Task <BillModel> GetBillByCode(string code);
    Task <List<BillModel>> GetBillsByUserId(long userId);
    Task <bool> UpdateBillStatus(long billId, int status);
    Task <bool> UpdatePaymentStatus(long billId, int paymentStatus);
    Task <bool> UpdatePaymentMethod(long billId, string paymentMethod);
    Task <decimal> ApplyVoucher(string voucherCode, decimal totalAmount);
    Task <string> GenerateBillCode();
    Task <BillModel> CheckoutFromCart(
        List<CartItemModel> cartItems, 
        CustomerInfoModel customerInfo, 
        string voucherCode);
}