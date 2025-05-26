using Microsoft.Extensions.Caching.Memory;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using SERP.Framework.Common;
using Serilog;
using Project.Common.Constants;
using Project.Business.ModelFactory;
using System.Net.WebSockets;
using Project.DbManagement.ViewModels;

namespace Project.Business.Implement;

public class BillDetailsBusiness : IBillDetailsBusiness
{
    private readonly IBillDetailsRepository _billDetailsRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger _logger;
    private readonly IBillDetailModelFactory _billDetailModelFactory;
    private const string BillDetailsListCacheKey = "BillDetailsList";
    private readonly MemoryCacheEntryOptions _cacheOptions;

    public BillDetailsBusiness(
        IBillDetailModelFactory billDetailModelFactory,
        IBillDetailsRepository billDetailsRepository, 
        IMemoryCache cache)
    {
        _billDetailsRepository = billDetailsRepository;
        _cache = cache;
        _billDetailModelFactory = billDetailModelFactory;
        _logger = Log.ForContext<BillDetailsBusiness>();
        _cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
    }

    public async Task<BillDetailModel> DeleteAsync(Guid contentId)
    {
        try
        {
            var billDetail = await _billDetailsRepository.DeleteAsync(contentId);
       
            if (billDetail != null)
            {
                _cache.Remove(BillDetailsListCacheKey);
                _logger.Information("Bill details {BillDetailsId} deleted successfully", contentId);
            }
            var res = await _billDetailModelFactory.CreateModel(billDetail);
            return res;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error deleting bill details {BillDetailsId}", contentId);
            throw;
        }
    }

    public async Task<IEnumerable<BillDetailModel>> DeleteAsync(Guid[] deleteIds)
    {
        try
        {
            var billDetails = await _billDetailsRepository.DeleteAsync(deleteIds);
            if (billDetails != null && billDetails.Any())
            {
                _cache.Remove(BillDetailsListCacheKey);
                _logger.Information("Multiple bill details deleted successfully: {BillDetailsIds}", string.Join(", ", deleteIds));
            }
            var res = await _billDetailModelFactory.CreateModels(billDetails);
            return res;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error deleting multiple bill details: {BillDetailsIds}", string.Join(", ", deleteIds));
            throw;
        }
    }

    public async Task<BillDetailModel> FindAsync(Guid contentId)
    {
        try
        {
            var billDetail = await _billDetailsRepository.FindAsync(contentId);
            if (billDetail == null)
            {
                _logger.Warning("Bill details {BillDetailsId} not found", contentId);
            }
            var res = await _billDetailModelFactory.CreateModel(billDetail);
            return res;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error finding bill details {BillDetailsId}", contentId);
            throw;
        }
    }

    public async Task<Pagination<BillDetailModel>> GetAllAsync(BillDetailsQueryModel queryModel)
    {
        try
        {
      

            var billDetails = await _billDetailsRepository.GetAllAsync(queryModel);
            var res = new Pagination<BillDetailModel>
            {
                Content = await _billDetailModelFactory.CreateModels(billDetails.Content),
                PageSize = billDetails.PageSize,
                CurrentPage= billDetails.CurrentPage,
                TotalPages = billDetails.TotalPages,
                TotalRecords=   billDetails.TotalRecords,
                NumberOfRecords = billDetails.NumberOfRecords
            };
       
            return res;
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

    public async Task<IEnumerable<BillDetailModel>> ListAllAsync(BillDetailsQueryModel queryModel)
    {
        try
        {
            var billDetails =  await _billDetailsRepository.ListAllAsync(queryModel);
            var res = await _billDetailModelFactory.CreateModels(billDetails);
            return res;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error listing all bill details");
            throw;
        }
    }

    public async Task<IEnumerable<BillDetailModel>> ListByIdsAsync(IEnumerable<Guid> ids)
    {
        try
        {
            var billdetails = await _billDetailsRepository.ListByIdsAsync(ids);
            var res = await _billDetailModelFactory.CreateModels(billdetails);
            return res;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error listing bill details by ids: {BillDetailsIds}", string.Join(", ", ids));
            throw;
        }
    }

    public async Task<BillDetailModel> PatchAsync(BillDetailsEntity model)
    {
        var exist = await _billDetailsRepository.FindAsync(model.Id);

        if (exist == null)
        {
            throw new ArgumentException(BillConstants.BillNotFound);
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

    public async Task<BillDetailModel> SaveAsync(BillDetailsEntity billDetails)
    {
        try
        {
            var bildetail = await _billDetailsRepository.SaveAsync(billDetails);
            var res = await _billDetailModelFactory.CreateModel(bildetail);
            if (bildetail != null)
            {
                _cache.Remove(BillDetailsListCacheKey);
                _logger.Information("Bill details {BillDetailsId} saved successfully", billDetails.Id);
            }
            return res;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error saving bill details {BillDetailsId}", billDetails.Id);
            throw;
        }
    }

    public async Task<IEnumerable<BillDetailModel>> SaveAsync(IEnumerable<BillDetailsEntity> billDetailsEntities)
    {
        try
        {
            var billDetails  = await _billDetailsRepository.SaveAsync(billDetailsEntities);
            var res = await _billDetailModelFactory.CreateModels(billDetails);
            if (billDetails != null && billDetails.Any())
            {
                _cache.Remove(BillDetailsListCacheKey);
                _logger.Information("Multiple bill details saved successfully");
            }
            return res;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error saving multiple bill details");
            throw;
        }
    }

    public async Task<BillDetailModel> UpdateBillDetailsAsync(BillDetailsEntity billDetails)
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

    public async Task<IEnumerable<BillDetailModel>> GetBillDetailsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var billDetails = await _billDetailsRepository.ListAllAsync(new BillDetailsQueryModel 
            { 
                StartDate = startDate,
                EndDate = endDate
            });
            
            var res=  await _billDetailModelFactory.CreateModels(billDetails);
            _logger.Information("Retrieved bill details for period {StartDate} to {EndDate}", startDate, endDate);
            return res;
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

    public async Task<List<BillDetailModel>> GetBillDetailsByBillId(Guid billId)
    {
        try
        {
            var billDetails = await _billDetailsRepository.ListAllAsync(new BillDetailsQueryModel { BillId = billId });
            var res = await _billDetailModelFactory.CreateModels(billDetails);
            return res.ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting bill details for bill {BillId}", billId);
            throw new Exception("Error getting bill details for bill");
        } 
    }

    public async Task<bool> CreateBillDetails(List<BillDetailsEntity> billDetails, Guid billId)
    {
        try
        {
            if (billDetails == null || !billDetails.Any())
            {
                return false;
            }

            var entities = billDetails.Select(detail => new BillDetailsEntity
            {
                Id =detail.Id,
                BillId = billId,
                ProductId = detail.ProductId,
                ProductImage = detail.ProductImage,
                BillDetailCode = $"{billId}-{detail.ProductId}",
                Quantity = detail.Quantity,
                CreatedByUserId = detail.CreatedByUserId,
                LastModifiedByUserId= detail.LastModifiedByUserId,
                Color=  detail.Color,
                LastModifiedOnDate = detail.LastModifiedOnDate,
                Notes = detail.Notes,
                IsDeleted = false,
                SKU = detail.SKU,
                ProductName = detail.ProductName,
                Size = detail.Size,
                TotalPrice = detail.TotalPrice,
                Price = detail.Price,
                Status = 0, // Pending
                CreatedOnDate = DateTime.Now
            }).ToList();

            await SaveAsync(entities);

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error creating bill details for bill {BillId}", billId);
            return false;
            
        }
    }

    public async Task<bool> UpdateBillDetailsStatus(Guid billDetailId, int status)
    {
        try
        {

            var billDetail = await FindAsync(billDetailId);
            
            if (billDetail == null)
            {
                return false;
            }

            billDetail.Status = status;
            billDetail.LastModifiedOnDate = DateTime.Now;
            await SaveAsync(await _billDetailModelFactory.ConvertEntity(billDetail));

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error updating bill detail status for bill detail {BillDetailId}", billDetailId);
            return false;
        }
    }
    public async Task<bool> SaveBillDetails(BillDetailsRequest request)
    {
        var result = await _billDetailsRepository.SaveBillDetails(request);
        return result;
    }

    public async Task<BillDetailsEntity> UpdateQuantity(Guid idBillDetails, int quantity)
    {
        var billdetails = await _billDetailsRepository.UpdateQuantity(idBillDetails, quantity);
        return billdetails;
    }

    public async Task<bool> DeleteBillDetails(Guid idBillDetails)
    {
        var result = await _billDetailsRepository.DeleteBillDetails(idBillDetails);
        return result;
    }

    public async Task<List<BillDetailsViewModel>> GetBillDetailsByIdBill(Guid idBill)
    {
        return await _billDetailsRepository.GetBillDetailsByIdBill(idBill);
    }

    public async Task<IEnumerable<BillDetailsEntity>> ListAllByIdBill(Guid idBill)
    {
        return await _billDetailsRepository.ListAllAsync(new BillDetailsQueryModel()
        {
            BillId = idBill
        });
    }

    public async Task<IEnumerable<BillDetailModel>> ListAllByBillId(Guid idBill)
    {
        return (IEnumerable<BillDetailModel>)await _billDetailsRepository.GetBillDetailsByIdBill(idBill);
    }
}