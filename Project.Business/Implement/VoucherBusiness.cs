using Microsoft.Extensions.Caching.Memory;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.Entity;
using Serilog;
using SERP.Framework.Common;

namespace Project.Business.Implement
{
    public class VoucherBusiness : IVoucherBusiness
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private const string VoucherListCacheKey = "VoucherList";
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public VoucherBusiness(IVoucherRepository voucherRepository, IMemoryCache cache)
        {
            _voucherRepository = voucherRepository;
            _cache = cache;
            _logger = Log.ForContext<VoucherBusiness>();
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(15))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        }

        public async Task<Voucher> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _voucherRepository.DeleteAsync(id);
                if (result != null)
                {
                    _cache.Remove(VoucherListCacheKey);
                    _logger.Information("Voucher {VoucherId} deleted successfully", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting voucher {VoucherId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Voucher>> DeleteAsync(Guid[] deleteIds)
        {
            try
            {
                var result = await _voucherRepository.DeleteAsync(deleteIds);
                if (result != null && result.Any())
                {
                    _cache.Remove(VoucherListCacheKey);
                    _logger.Information("Multiple vouchers deleted successfully: {VoucherIds}", string.Join(", ", deleteIds));
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting multiple vouchers: {VoucherIds}", string.Join(", ", deleteIds));
                throw;
            }
        }

        public async Task<Voucher> FindAsync(Guid id)
        {
            try
            {
                var voucher = await _voucherRepository.FindAsync(id);
                if (voucher == null)
                {
                    _logger.Warning("Voucher {VoucherId} not found", id);
                }
                return voucher;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error finding voucher {VoucherId}", id);
                throw;
            }
        }

        public async Task<Pagination<Voucher>> GetAllAsync(VoucherQueryModel queryModel)
        {
            try
            {
                if (queryModel.PageSize == 0 && queryModel.CurrentPage == 0)
                {
                    if (_cache.TryGetValue(VoucherListCacheKey, out Pagination<Voucher> cachedVouchers))
                    {
                        return cachedVouchers;
                    }

                    var vouchers = await _voucherRepository.GetAllAsync(queryModel);
                    _cache.Set(VoucherListCacheKey, vouchers, _cacheOptions);
                    return vouchers;
                }

                return await _voucherRepository.GetAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting all vouchers");
                throw;
            }
        }

        public async Task<int> GetCountAsync(VoucherQueryModel queryModel)
        {
            try
            {
                return await _voucherRepository.GetCountAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting voucher count");
                throw;
            }
        }

        public async Task<IEnumerable<Voucher>> ListAllAsync(VoucherQueryModel queryModel)
        {
            try
            {
                return await _voucherRepository.ListAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing all vouchers");
                throw;
            }
        }

        public async Task<IEnumerable<Voucher>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            try
            {
                return await _voucherRepository.ListByIdsAsync(ids);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing vouchers by ids: {VoucherIds}", string.Join(", ", ids));
                throw;
            }
        }

        public async Task<Voucher> PatchAsync(Voucher model)
        {
            var exist = await _voucherRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException("Voucher not found");
            }
            var update = new Voucher
            {
                Id = exist.Id,
                Code = exist.Code,
                Description = exist.Description,
                DiscountAmount = exist.DiscountAmount,
                MinimumOrderAmount = exist.MinimumOrderAmount,
                DiscountPercentage = exist.DiscountPercentage,
                VoucherName = exist.VoucherName,
                VoucherType = exist.VoucherType,
                StartDate = exist.StartDate,
                EndDate = exist.EndDate,
                Status = exist.Status
            };

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                update.Code = model.Code;
            }
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                update.Description = model.Description;
            }
            if (model.DiscountAmount >= 0)
            {
                update.DiscountAmount = model.DiscountAmount;
            }
            if (model.MinimumOrderAmount >= 0)
            {
                update.MinimumOrderAmount = model.MinimumOrderAmount;
            }
            if (model.DiscountPercentage >= 0)
            {
                update.DiscountPercentage = model.DiscountPercentage;
            }
            if (!string.IsNullOrWhiteSpace(model.VoucherName))
            {
                update.VoucherName = model.VoucherName;
            }
            if (model.VoucherType >= 0)
            {
                update.VoucherType = model.VoucherType;
            }
            if (model.StartDate != null)
            {
                update.StartDate = model.StartDate;
            }
            if (model.EndDate != null)
            {
                update.EndDate = model.EndDate;
            }
            if (model.Status >= 0)
            {
                update.Status = model.Status;
            }
            return await SaveAsync(update);
        }

        public async Task<Voucher> SaveAsync(Voucher voucher)
        {
            try
            {
                var result = await _voucherRepository.SaveAsync(voucher);
                if (result != null)
                {
                    _cache.Remove(VoucherListCacheKey);
                    _logger.Information("Voucher {VoucherId} saved successfully", voucher.Id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error saving voucher {VoucherId}", voucher.Id);
                throw;
            }
        }

        public async Task<IEnumerable<Voucher>> SaveAsync(IEnumerable<Voucher> vouchers)
        {
            return await _voucherRepository.SaveAsync(vouchers);
        }

        public async Task<Voucher> UpdateVoucherAsync(Voucher voucher)
        {
            try
            {
                var exist = await _voucherRepository.FindAsync(voucher.Id);
                if (exist == null)
                {
                    _logger.Warning("Voucher {VoucherId} not found for update", voucher.Id);
                    throw new ArgumentException("Voucher not found");
                }

                // Update voucher information
                exist.Code = voucher.Code ?? exist.Code;
                exist.VoucherName = voucher.VoucherName ?? exist.VoucherName;
                exist.Description = voucher.Description ?? exist.Description;
                exist.DiscountAmount = voucher.DiscountAmount > 0 ? voucher.DiscountAmount : exist.DiscountAmount;
                exist.StartDate = voucher.StartDate ?? exist.StartDate;
                exist.EndDate = voucher.EndDate ?? exist.EndDate;
                exist.Status = voucher.Status;
                exist.LastModifiedOnDate = DateTime.UtcNow;

                var result = await SaveAsync(exist);
                _logger.Information("Voucher {VoucherId} updated successfully", voucher.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating voucher {VoucherId}", voucher.Id);
                throw;
            }
        }

        public async Task<bool> IsVoucherValidAsync(string voucherCode, decimal orderAmount)
        {
            try
            {
                var voucher = await _voucherRepository.FindByCodeAsync(voucherCode);
                if (voucher == null)
                {
                    _logger.Warning("Voucher with code {VoucherCode} not found", voucherCode);
                    return false;
                }

                var isValid = voucher.Status == 1 &&
                            DateTime.UtcNow >= voucher.StartDate &&
                            DateTime.UtcNow <= voucher.EndDate &&
                            orderAmount >= voucher.MinimumOrderAmount;

                _logger.Information("Voucher {VoucherCode} validation result: {IsValid}", voucherCode, isValid);
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating voucher {VoucherCode}", voucherCode);
                throw;
            }
        }

        public async Task<decimal> CalculateDiscountAsync(string voucherCode, decimal orderAmount)
        {
            try
            {
                var voucher = await _voucherRepository.FindByCodeAsync(voucherCode);
                if (voucher == null || !await IsVoucherValidAsync(voucherCode, orderAmount))
                {
                    return 0;
                }

                var discount = Math.Min(voucher.DiscountAmount ?? 0, orderAmount * (voucher.DiscountPercentage ?? 0 / 100));
                _logger.Information("Calculated discount for voucher {VoucherCode}: {Discount}", voucherCode, discount);
                return discount;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error calculating discount for voucher {VoucherCode}", voucherCode);
                throw;
            }
        }

        //Check code đã tồn tại hay chưa khi Tạo mới hoặc Cập nhật
        public async Task<bool> IsVoucherCodeExist(string code, Guid? voucherId)
        {
            try
            {
                var isCodeExist = await _voucherRepository.IsVoucherCodeExist(code, voucherId);
                return isCodeExist;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error checking if voucher code {VoucherCode} exists", code);
                throw;
            }
        }
    }
}