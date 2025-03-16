using Microsoft.Extensions.Caching.Memory;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.Entity;
using Serilog;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Business.Implement
{
    public class PaymentMethodsBusiness : IPaymentMethodsBusiness
    {
        private readonly IPaymentMethodsRepository _paymentMethodsRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private const string PaymentMethodsListCacheKey = "PaymentMethodsList";
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public PaymentMethodsBusiness(IPaymentMethodsRepository paymentMethodsRepository, IMemoryCache cache)
        {
            _paymentMethodsRepository = paymentMethodsRepository;
            _cache = cache;
            _logger = Log.ForContext<PaymentMethodsBusiness>();
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                .SetAbsoluteExpiration(TimeSpan.FromHours(2));
        }

        public async Task<PaymentMethods> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _paymentMethodsRepository.DeleteAsync(id);
                if (result != null)
                {
                    _cache.Remove(PaymentMethodsListCacheKey);
                    _logger.Information("Payment method {PaymentMethodId} deleted successfully", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting payment method {PaymentMethodId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<PaymentMethods>> DeleteAsync(Guid[] deleteIds)
        {
            try
            {
                var result = await _paymentMethodsRepository.DeleteAsync(deleteIds);
                if (result != null && result.Any())
                {
                    _cache.Remove(PaymentMethodsListCacheKey);
                    _logger.Information("Multiple payment methods deleted successfully: {PaymentMethodIds}", string.Join(", ", deleteIds));
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting multiple payment methods: {PaymentMethodIds}", string.Join(", ", deleteIds));
                throw;
            }
        }

        public async Task<PaymentMethods> FindAsync(Guid id)
        {
            try
            {
                var paymentMethod = await _paymentMethodsRepository.FindAsync(id);
                if (paymentMethod == null)
                {
                    _logger.Warning("Payment method {PaymentMethodId} not found", id);
                }
                return paymentMethod;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error finding payment method {PaymentMethodId}", id);
                throw;
            }
        }

        public async Task<Pagination<PaymentMethods>> GetAllAsync(PaymentMethodsQueryModel queryModel)
        {
            try
            {
                if (queryModel.PageSize == 0 && queryModel.CurrentPage == 0)
                {
                    if (_cache.TryGetValue(PaymentMethodsListCacheKey, out Pagination<PaymentMethods> cachedPaymentMethods))
                    {
                        return cachedPaymentMethods;
                    }

                    var paymentMethods = await _paymentMethodsRepository.GetAllAsync(queryModel);
                    _cache.Set(PaymentMethodsListCacheKey, paymentMethods, _cacheOptions);
                    return paymentMethods;
                }

                return await _paymentMethodsRepository.GetAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting all payment methods");
                throw;
            }
        }

        public async Task<int> GetCountAsync(PaymentMethodsQueryModel queryModel)
        {
            try
            {
                return await _paymentMethodsRepository.GetCountAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting payment methods count");
                throw;
            }
        }

        public async Task<IEnumerable<PaymentMethods>> ListAllAsync(PaymentMethodsQueryModel queryModel)
        {
            try
            {
                return await _paymentMethodsRepository.ListAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing all payment methods");
                throw;
            }
        }

        public async Task<IEnumerable<PaymentMethods>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            try
            {
                return await _paymentMethodsRepository.ListByIdsAsync(ids);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing payment methods by ids: {PaymentMethodIds}", string.Join(", ", ids));
                throw;
            }
        }

        public async Task<PaymentMethods> PatchAsync(PaymentMethods model)
        {
            var exist = await _paymentMethodsRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException("Payment method not found");
            }

            var update = new PaymentMethods
            {
                Id = exist.Id,
                PaymentMethodCode = exist.PaymentMethodCode,
                PaymentMethodName = exist.PaymentMethodName,
                Status = exist.Status,
                CreatedBy = exist.CreatedBy,
                CreatedOnDate = exist.CreatedOnDate,
                LastModifiedOnDate = exist.LastModifiedOnDate,
                UpdatedBy = exist.UpdatedBy
            };

            if (!string.IsNullOrWhiteSpace(model.PaymentMethodCode))
            {
                update.PaymentMethodCode = model.PaymentMethodCode;
            }
            if (!string.IsNullOrWhiteSpace(model.PaymentMethodName))
            {
                update.PaymentMethodName = model.PaymentMethodName;
            }
            if (model.Status >= 0)
            {
                update.Status = model.Status;
            }
            if (!string.IsNullOrWhiteSpace(model.UpdatedBy))
            {
                update.UpdatedBy = model.UpdatedBy;
            }
            if (model.LastModifiedOnDate != null)
            {
                update.LastModifiedOnDate = model.LastModifiedOnDate;
            }

            return await SaveAsync(update);
        }

        public async Task<PaymentMethods> SaveAsync(PaymentMethods paymentMethods)
        {
            var res = await SaveAsync(new[] { paymentMethods });
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<PaymentMethods>> SaveAsync(IEnumerable<PaymentMethods> paymentMethods)
        {
            return await _paymentMethodsRepository.SaveAsync(paymentMethods);
        }

        public async Task<PaymentMethods> UpdatePaymentMethodAsync(PaymentMethods paymentMethod)
        {
            try
            {
                var exist = await _paymentMethodsRepository.FindAsync(paymentMethod.Id);
                if (exist == null)
                {
                    _logger.Warning("Payment method {PaymentMethodId} not found for update", paymentMethod.Id);
                    throw new ArgumentException("Payment method not found");
                }

                var result = await SaveAsync(paymentMethod);
                _logger.Information("Payment method {PaymentMethodId} updated successfully", paymentMethod.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating payment method {PaymentMethodId}", paymentMethod.Id);
                throw;
            }
        }
    }
}
