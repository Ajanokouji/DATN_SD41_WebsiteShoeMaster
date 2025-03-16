using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Business.Interface;

public interface IBillDetailsBusiness
{
    Task<Pagination<BillDetailsEntity>> GetAllAsync(BillDetailsQueryModel queryModel);

    Task<IEnumerable<BillDetailsEntity>> ListAllAsync(BillDetailsQueryModel queryModel);

    Task<int> GetCountAsync(BillDetailsQueryModel queryModel);

    Task<IEnumerable<BillDetailsEntity>> ListByIdsAsync(IEnumerable<Guid> ids);

    Task<BillDetailsEntity> FindAsync(Guid contentId);

    Task<BillDetailsEntity> DeleteAsync(Guid contentId);
    
    Task<IEnumerable<BillDetailsEntity>> DeleteAsync(Guid[] deleteIds);

    Task<BillDetailsEntity> SaveAsync(BillDetailsEntity article);

    Task<IEnumerable<BillDetailsEntity>> SaveAsync(IEnumerable<BillDetailsEntity> article);

    Task<BillDetailsEntity> PatchAsync(BillDetailsEntity article);

    Task<BillDetailsEntity> UpdateBillDetailsAsync(BillDetailsEntity billDetails);

    Task<decimal> GetBillTotalAsync(Guid billId);

    Task<IEnumerable<BillDetailsEntity>> GetBillDetailsByDateRangeAsync(DateTime startDate, DateTime endDate);

    Task<Dictionary<Guid?, int>> GetTopSellingProductsAsync(DateTime startDate, DateTime endDate, int topCount = 10);

    Task<ServiceResult<List<BillDetailModel>>> GetBillDetailsByBillId(long billId);

    Task<ServiceResult<bool>> CreateBillDetails(List<BillDetailModel> billDetails, Guid billId);

    Task<ServiceResult<bool>> UpdateBillDetailsStatus(long billDetailId, int status);
}