using Microsoft.Extensions.Caching.Memory;
using Nest;
using Project.Business.Interface;
using Project.Business.Model;
using Project.Common;
using Project.Common.Constants;
using Project.DbManagement;
using Project.DbManagement.Entity;
using Serilog;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Project.DbManagement.ViewModels;

namespace Project.Business.Implement
{
    public class CustomerBusiness : ICustomerBusiness
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private const string CustomerListCacheKey = "CustomerList";
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CustomerBusiness(ICustomerRepository customerRepository, IMemoryCache cache)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = Log.ForContext<CustomerBusiness>();
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        }

        public async Task<CustomersEntity> DeleteAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Invalid customer ID", nameof(id));
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await _customerRepository.DeleteAsync(id);
                    if (result != null)
                    {
                        _cache.Remove(CustomerListCacheKey);
                        _logger.Information("Customer {CustomerId} deleted successfully", id);
                    }
                    else
                    {
                        _logger.Warning("Customer {CustomerId} not found for deletion", id);
                    }

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting customer {CustomerId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<CustomersEntity>> DeleteAsync(Guid[] deleteIds)
        {
            try
            {
                if (deleteIds == null || !deleteIds.Any())
                {
                    throw new ArgumentException("Delete IDs cannot be null or empty", nameof(deleteIds));
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await _customerRepository.DeleteAsync(deleteIds);
                    if (result != null && result.Any())
                    {
                        _cache.Remove(CustomerListCacheKey);
                        _logger.Information("Multiple customers deleted successfully: {CustomerIds}", string.Join(", ", deleteIds));
                    }

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting multiple customers: {CustomerIds}", string.Join(", ", deleteIds));
                throw;
            }
        }

        public async Task<CustomersEntity> FindAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Invalid customer ID", nameof(id));
                }

                var customer = await _customerRepository.FindAsync(id);
                if (customer == null)
                {
                    _logger.Warning("Customer {CustomerId} not found", id);
                }
                return customer;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error finding customer {CustomerId}", id);
                throw;
            }
        }

        public async Task<CustomersEntity> FindByPhoneNumberAsync(string phone)
        {
            try
            {

                var customer = (await _customerRepository.ListAllAsync(new CustomerQueryModel()
                {
                    PhoneNumber = phone
                })).First();
                if (customer == null)
                {
                    _logger.Warning("Customer not found");
                }
                return customer;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error finding customer ");
                throw;
            }
        }
        public async Task<Pagination<CustomersEntity>> GetAllAsync(CustomerQueryModel queryModel)
        {
            try
            {
                if (queryModel == null)
                {
                    throw new ArgumentNullException(nameof(queryModel));
                }

                if (queryModel.PageSize == 0 && queryModel.CurrentPage == 0)
                {
                    if (_cache.TryGetValue(CustomerListCacheKey, out Pagination<CustomersEntity> cachedCustomers))
                    {
                        _logger.Debug("Retrieved customers from cache");
                        return cachedCustomers;
                    }

                    var customers = await _customerRepository.GetAllAsync(queryModel);
                    _cache.Set(CustomerListCacheKey, customers, _cacheOptions);
                    _logger.Debug("Customers cached successfully");
                    return customers;
                }

                return await _customerRepository.GetAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting all customers");
                throw;
            }
        }

        public async Task<int> GetCountAsync(CustomerQueryModel queryModel)
        {
            try
            {
                if (queryModel == null)
                {
                    throw new ArgumentNullException(nameof(queryModel));
                }

                return await _customerRepository.GetCountAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting customer count");
                throw;
            }
        }

        public async Task<IEnumerable<CustomersEntity>> ListAllAsync(CustomerQueryModel queryModel)
        {
            try
            {
                if (queryModel == null)
                {
                    throw new ArgumentNullException(nameof(queryModel));
                }

                return await _customerRepository.ListAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing all customers");
                throw;
            }
        }

        public async Task<IEnumerable<CustomersEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    throw new ArgumentException("IDs cannot be null or empty", nameof(ids));
                }

                return await _customerRepository.ListByIdsAsync(ids);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing customers by ids: {CustomerIds}", string.Join(", ", ids));
                throw;
            }
        }

        public async Task<CustomersEntity> SaveAsync(CustomersEntity customer)
        {
            try
            {
                if (customer == null)
                {
                    throw new ArgumentNullException(nameof(customer));
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await _customerRepository.SaveAsync(customer);
                    if (result != null)
                    {
                        _cache.Remove(CustomerListCacheKey);
                        _logger.Information("Customer {CustomerId} saved successfully", customer.Id);
                    }

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error saving customer {CustomerId}", customer?.Id);
                throw;
            }
        }

        public async Task<IEnumerable<CustomersEntity>> SaveAsync(IEnumerable<CustomersEntity> customers)
        {
            try
            {
                if (customers == null || !customers.Any())
                {
                    throw new ArgumentException("Customers cannot be null or empty", nameof(customers));
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await _customerRepository.SaveAsync(customers);
                    if (result != null && result.Any())
                    {
                        _cache.Remove(CustomerListCacheKey);
                        _logger.Information("Multiple customers saved successfully: {CustomerIds}", 
                            string.Join(", ", result.Select(c => c.Id)));
                    }

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error saving multiple customers");
                throw;
            }
        }

        public async Task<CustomersEntity> PatchAsync(CustomersEntity customer)
        {
            try
            {
                if (customer == null)
                {
                    throw new ArgumentNullException(nameof(customer));
                }

                var exist = await _customerRepository.FindAsync(customer.Id);
                if (exist == null)
                {
                    _logger.Warning("Customer {CustomerId} not found for update", customer.Id);
                    throw new ArgumentException(CustomerConstants.CustomerNotFound);
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Update customer information
                    exist.Name = customer.Name ?? exist.Name;
                    exist.Email = customer.Email ?? exist.Email;
                    exist.PhoneNumber = customer.PhoneNumber ?? exist.PhoneNumber;
                    exist.Address = customer.Address ?? exist.Address;
           
                    exist.LastModifiedOnDate = DateTime.UtcNow;
                    exist.LastModifiedByUserId = customer.LastModifiedByUserId;

                    var result = await SaveAsync(exist);
                    if (result != null)
                    {
                        _logger.Information("Customer {CustomerId} updated successfully", customer.Id);
                    }

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating customer {CustomerId}", customer?.Id);
                throw;
            }
        }

        public async Task<IEnumerable<CustomersEntity>> GetTopCustomersAsync(int count, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                if (count <= 0)
                {
                    throw new ArgumentException("Count must be greater than zero", nameof(count));
                }

                var customers = await _customerRepository.ListAllAsync(new CustomerQueryModel 
                { 
                    //StartDate = startDate,
                    //EndDate = endDate,
                    //PageSize = count,
                    //SortBy = "TotalPurchases",
                    //SortDirection = "DESC"
                });

                _logger.Information("Retrieved top {Count} customers for period {StartDate} to {EndDate}", 
                    count, startDate, endDate);
                
                return customers;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting top customers");
                throw;
            }
        }

        public async Task<CustomerStatistics> GetCustomerStatisticsAsync(Guid customerId)
        {
            try
            {
                if (customerId == Guid.Empty)
                {
                    throw new ArgumentException("Invalid customer ID", nameof(customerId));
                }

                var customer = await FindAsync(customerId);
                if (customer == null)
                {
                    throw new ArgumentException(CustomerConstants.CustomerNotFound);
                }

                var stats = new CustomerStatistics
                {
                    CustomerId = customerId,
                    TotalOrders = customer.TotalOrders,
                    TotalSpent = customer.TotalSpent,
                    LastOrderDate = customer.LastOrderDate,
                    JoinDate = customer.CreatedOnDate ?? DateTime.MinValue
                };

                _logger.Information("Retrieved statistics for customer {CustomerId}", customerId);
                return stats;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting statistics for customer {CustomerId}", customerId);
                throw;
            }
        }
        
        public async Task<List<CustomerViewModel>> GetCustomerByPhoneNumber(string phoneNumber)
        {
            return  await _customerRepository.GetCustomerByPhoneNumber(phoneNumber);
        }

        public bool AddCustomerSell(CustomerViewModel customer)
        {
            return _customerRepository.AddCustomerSell(customer);
        }
    }

    public class CustomerStatistics
    {
        public Guid CustomerId { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public DateTime JoinDate { get; set; }
    }
}
