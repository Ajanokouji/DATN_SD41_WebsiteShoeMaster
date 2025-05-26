using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.DbManagement.ViewModels;

namespace Project.Business.Interface;

public interface IBillDetailsBusiness
{
    Task<Pagination<BillDetailModel>> GetAllAsync(BillDetailsQueryModel queryModel);

    Task<IEnumerable<BillDetailModel>> ListAllAsync(BillDetailsQueryModel queryModel);

    Task<int> GetCountAsync(BillDetailsQueryModel queryModel);

    Task<IEnumerable<BillDetailsEntity>> ListAllByIdBill(Guid idBill);

    Task<BillDetailModel> FindAsync(Guid contentId);

    Task<IEnumerable<BillDetailModel>> ListAllByBillId(Guid contentId);

    Task<BillDetailModel> DeleteAsync(Guid contentId);
    
    Task<IEnumerable<BillDetailModel>> DeleteAsync(Guid[] deleteIds);

    Task<BillDetailModel> SaveAsync(BillDetailsEntity article);

    Task<IEnumerable<BillDetailModel>> SaveAsync(IEnumerable<BillDetailsEntity> article);

    Task<BillDetailModel> PatchAsync(BillDetailsEntity article);

    Task<BillDetailModel> UpdateBillDetailsAsync(BillDetailsEntity billDetails);

    Task<decimal> GetBillTotalAsync(Guid billId);

    Task<IEnumerable<BillDetailModel>> GetBillDetailsByDateRangeAsync(DateTime startDate, DateTime endDate);

    Task<Dictionary<Guid?, int>> GetTopSellingProductsAsync(DateTime startDate, DateTime endDate, int topCount = 10);

    Task<List<BillDetailModel>> GetBillDetailsByBillId(Guid billId);

    Task<bool> CreateBillDetails(List<BillDetailsEntity> billDetails, Guid billId);

    Task<bool> UpdateBillDetailsStatus(Guid billDetailId, int status);
    public Task<bool> SaveBillDetails(BillDetailsRequest request);
    public Task<BillDetailsEntity> UpdateQuantity(Guid idBillDetails, int quantity);
    public Task<bool> DeleteBillDetails(Guid idBillDetails);
    public Task<List<BillDetailsViewModel>> GetBillDetailsByIdBill(Guid idBill);
}