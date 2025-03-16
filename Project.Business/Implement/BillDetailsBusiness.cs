using Microsoft.Extensions.Caching.Memory;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using SERP.Framework.Common;
using Serilog;

namespace Project.Business.Implement;

public class BillDetailsBusiness : IBillDetailsBusiness
{
    private readonly IBillDetailsRepository _billDetailsRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger _logger;
    private const string BillDetailsListCacheKey = "BillDetailsList";
    private readonly MemoryCacheEntryOptions _cacheOptions;

    public BillDetailsBusiness(IBillDetailsRepository billDetailsRepository, IMemoryCache cache)
    {
        _billDetailsRepository = billDetailsRepository;
        _cache = cache;
        _logger = Log.ForContext<BillDetailsBusiness>();
        _cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
    }

    public async Task<BillDetailsEntity> DeleteAsync(Guid contentId)
    {
        try
        {
            var result = await _billDetailsRepository.DeleteAsync(contentId);
            if (result != null)
            {
                _cache.Remove(BillDetailsListCacheKey);
                _logger.Information("Bill details {BillDetailsId} deleted successfully", contentId);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error deleting bill details {BillDetailsId}", contentId);
            throw;
        }
    }

    public async Task<IEnumerable<BillDetailsEntity>> DeleteAsync(Guid[] deleteIds)
    {
        try
        {
            var result = await _billDetailsRepository.DeleteAsync(deleteIds);
            if (result != null && result.Any())
            {
                _cache.Remove(BillDetailsListCacheKey);
                _logger.Information("Multiple bill details deleted successfully: {BillDetailsIds}", string.Join(", ", deleteIds));
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error deleting multiple bill details: {BillDetailsIds}", string.Join(", ", deleteIds));
            throw;
        }
    }

    public async Task<BillDetailsEntity> FindAsync(Guid contentId)
    {
        try
        {
            var billDetails = await _billDetailsRepository.FindAsync(contentId);
            if (billDetails == null)
            {
                _logger.Warning("Bill details {BillDetailsId} not found", contentId);
            }
            return billDetails;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error finding bill details {BillDetailsId}", contentId);
            throw;
        }
    }

    public async Task<Pagination<BillDetailsEntity>> GetAllAsync(BillDetailsQueryModel queryModel)
    {
        try
        {
            if (queryModel.PageSize == 0 && queryModel.CurrentPage == 0)
            {
                if (_cache.TryGetValue(BillDetailsListCacheKey, out Pagination<BillDetailsEntity> cachedBillDetails))
                {
                    return cachedBillDetails;
                }

                var billDetails = await _billDetailsRepository.GetAllAsync(queryModel);
                _cache.Set(BillDetailsListCacheKey, billDetails, _cacheOptions);
                return billDetails;
            }

            return await _billDetailsRepository.GetAllAsync(queryModel);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting all bill details");
            throw;
        }
    }

    public async Task<int> GetCountAsync(BillDetailsQueryModel queryModel)
    {
        try
        {
            return await _billDetailsRepository.GetCountAsync(queryModel);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting bill details count");
            throw;
        }
    }

    public async Task<IEnumerable<BillDetailsEntity>> ListAllAsync(BillDetailsQueryModel queryModel)
    {
        try
        {
            return await _billDetailsRepository.ListAllAsync(queryModel);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error listing all bill details");
            throw;
        }
    }

    public async Task<IEnumerable<BillDetailsEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
    {
        try
        {
            return await _billDetailsRepository.ListByIdsAsync(ids);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error listing bill details by ids: {BillDetailsIds}", string.Join(", ", ids));
            throw;
        }
    }

    public async Task<BillDetailsEntity> PatchAsync(BillDetailsEntity model)
    {
        var exist = await _billDetailsRepository.FindAsync(model.Id);

        if (exist == null)
        {
            throw new ArgumentException(BillConstant.BillNotFound);
        }

        var update = new BillDetailsEntity()
        {
            Id = exist.Id,
            BillId = exist.BillId,
            ProductId = exist.ProductId,
            BillDetailCode = exist.BillDetailCode,
            Status = exist.Status,
            Quantity = exist.Quantity,
            Price = exist.Price,
            Notes = exist.Notes
        };

        if (!string.IsNullOrWhiteSpace(model.BillDetailCode))
        {
            update.BillDetailCode = model.BillDetailCode;
        }

        if (model.BillId != null)
        {
            update.BillId = model.BillId;
        }
        
        if (model.ProductId != null)
        {
            update.ProductId = model.ProductId;
        }
        
        if (model.Status > 0)
        {
            update.Status = model.Status;
        }

        if (model.Quantity > 0)
        {
            update.Quantity = model.Quantity;
        }
        
        if (model.Price > 0)
        {
            update.Price = model.Price;
        }

        if (!string.IsNullOrWhiteSpace(model.Notes))
        {
            update.Notes = model.Notes;
        }
        
        return await SaveAsync(update);
    }

    public async Task<BillDetailsEntity> SaveAsync(BillDetailsEntity billDetails)
    {
        try
        {
            var result = await _billDetailsRepository.SaveAsync(billDetails);
            if (result != null)
            {
                _cache.Remove(BillDetailsListCacheKey);
                _logger.Information("Bill details {BillDetailsId} saved successfully", billDetails.Id);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error saving bill details {BillDetailsId}", billDetails.Id);
            throw;
        }
    }

    public async Task<IEnumerable<BillDetailsEntity>> SaveAsync(IEnumerable<BillDetailsEntity> billDetailsEntities)
    {
        try
        {
            var result = await _billDetailsRepository.SaveAsync(billDetailsEntities);
            if (result != null && result.Any())
            {
                _cache.Remove(BillDetailsListCacheKey);
                _logger.Information("Multiple bill details saved successfully");
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error saving multiple bill details");
            throw;
        }
    }

    public async Task<BillDetailsEntity> UpdateBillDetailsAsync(BillDetailsEntity billDetails)
    {
        try
        {
            var exist = await _billDetailsRepository.FindAsync(billDetails.Id);
            if (exist == null)
            {
                _logger.Warning("Bill details {BillDetailsId} not found for update", billDetails.Id);
                throw new ArgumentException("Bill details not found");
            }

            // Update bill details information
            exist.BillId = billDetails.BillId;
            exist.ProductId = billDetails.ProductId;
            exist.Quantity = billDetails.Quantity;
            exist.Price = billDetails.Price;
            exist.Status = billDetails.Status;
            exist.LastModifiedOnDate = DateTime.UtcNow;

            var result = await SaveAsync(exist);
            _logger.Information("Bill details {BillDetailsId} updated successfully", billDetails.Id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error updating bill details {BillDetailsId}", billDetails.Id);
            throw;
        }
    }

    public async Task<decimal> GetBillTotalAsync(Guid billId)
    {
        try
        {
            var billDetails = await _billDetailsRepository.ListAllAsync(new BillDetailsQueryModel { Id = billId });
            var total = billDetails.Sum(bd => bd.Price * bd.Quantity);
            _logger.Information("Calculated total for bill {BillId}: {Total}", billId, total);
            return (decimal)total;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error calculating total for bill {BillId}", billId);
            throw;
        }
    }

    public async Task<IEnumerable<BillDetailsEntity>> GetBillDetailsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var billDetails = await _billDetailsRepository.ListAllAsync(new BillDetailsQueryModel 
            { 
                StartDate = startDate,
                EndDate = endDate
            });
            
            _logger.Information("Retrieved bill details for period {StartDate} to {EndDate}", startDate, endDate);
            return billDetails;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving bill details for period {StartDate} to {EndDate}", 
                startDate, endDate);
            throw;
        }
    }

    public async Task<Dictionary<Guid?, int>> GetTopSellingProductsAsync(DateTime startDate, DateTime endDate, int topCount = 10)
    {
        try
        {
            var billDetails = await GetBillDetailsByDateRangeAsync(startDate, endDate);
            var topProducts = billDetails
                .GroupBy(bd => bd.ProductId)
                .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(bd => bd.Quantity) })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(topCount)
                .ToDictionary(x => x.ProductId, x => x.TotalQuantity);

            _logger.Information("Retrieved top {TopCount} selling products for period {StartDate} to {EndDate}", 
                topCount, startDate, endDate);
            return topProducts;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving top selling products for period {StartDate} to {EndDate}", 
                startDate, endDate);
            throw;
        }
    }

    public async Task<ServiceResult<List<BillDetailModel>>> GetBillDetailsByBillId(long billId)
    {
        try
        {
            var billGuid = Guid.Parse(billId.ToString());
            var billDetails = await _billDetailsRepository.ListAllAsync(new BillDetailsQueryModel { BillId = billGuid });
            
            if (billDetails == null || !billDetails.Any())
            {
                return new ServiceResult<List<BillDetailModel>>
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy chi tiết hóa đơn",
                    Data = new List<BillDetailModel>()
                };
            }

            var result = billDetails.Select(d => new BillDetailModel
            {
                Id = long.Parse(d.Id.ToString()),
                BillId = long.Parse(d.BillId.ToString()),
                ProductId = d.ProductId ?? Guid.Empty,
                ProductName = "", // Cần lấy thêm thông tin sản phẩm
                ProductImage = "", // Cần lấy thêm thông tin sản phẩm
                Size = 0, // Không có thông tin size
                Color = "", // Không có thông tin màu
                Quantity = d.Quantity,
                Price = (decimal)d.Price,
                TotalPrice = (decimal)(d.Price * d.Quantity)
            }).ToList();

            return new ServiceResult<List<BillDetailModel>>
            {
                IsSuccess = true,
                Data = result,
                Message = "Lấy chi tiết hóa đơn thành công"
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting bill details for bill {BillId}", billId);
            return new ServiceResult<List<BillDetailModel>>
            {
                IsSuccess = false,
                Message = $"Lỗi khi lấy chi tiết hóa đơn: {ex.Message}",
                Data = new List<BillDetailModel>()
            };
        }
    }

    public async Task<ServiceResult<bool>> CreateBillDetails(List<BillDetailModel> billDetails, Guid billId)
    {
        try
        {
            if (billDetails == null || !billDetails.Any())
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = "Không có chi tiết hóa đơn để tạo",
                    Data = false
                };
            }

            var entities = billDetails.Select(detail => new BillDetailsEntity
            {
                Id = Guid.NewGuid(),
                BillId = billId,
                ProductId = detail.ProductId,
                BillDetailCode = $"{billId}-{detail.ProductId}",
                Quantity = detail.Quantity,
                Price = (double)detail.Price,
                Status = 0, // Pending
                CreatedOnDate = DateTime.Now
            }).ToList();

            await SaveAsync(entities);

            return new ServiceResult<bool>
            {
                IsSuccess = true,
                Data = true,
                Message = "Tạo chi tiết hóa đơn thành công"
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error creating bill details for bill {BillId}", billId);
            return new ServiceResult<bool>
            {
                IsSuccess = false,
                Message = $"Lỗi khi tạo chi tiết hóa đơn: {ex.Message}",
                Data = false
            };
        }
    }

    public async Task<ServiceResult<bool>> UpdateBillDetailsStatus(long billDetailId, int status)
    {
        try
        {
            var billDetailGuid = Guid.Parse(billDetailId.ToString());
            var billDetail = await FindAsync(billDetailGuid);
            
            if (billDetail == null)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy chi tiết hóa đơn",
                    Data = false
                };
            }

            billDetail.Status = status;
            billDetail.LastModifiedOnDate = DateTime.Now;
            await SaveAsync(billDetail);

            return new ServiceResult<bool>
            {
                IsSuccess = true,
                Data = true,
                Message = "Cập nhật trạng thái chi tiết hóa đơn thành công"
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error updating bill detail status for bill detail {BillDetailId}", billDetailId);
            return new ServiceResult<bool>
            {
                IsSuccess = false,
                Message = $"Lỗi khi cập nhật trạng thái chi tiết hóa đơn: {ex.Message}",
                Data = false
            };
        }
    }
}