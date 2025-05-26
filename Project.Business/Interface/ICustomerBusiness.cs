using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.DbManagement.ViewModels;

namespace Project.Business.Interface
{
    public interface ICustomerBusiness
    {
      
        Task<Pagination<CustomersEntity>> GetAllAsync(CustomerQueryModel queryModel);

        /// <summary>
        /// Gets list of customers.
        /// </summary>
        /// <param name="queryModel">The query model.</param>
        /// <returns>The list of customers.</returns>
        Task<IEnumerable<CustomersEntity>> ListAllAsync(CustomerQueryModel queryModel);

        /// <summary>
        /// Count the number of customers by query model.
        /// </summary>
        /// <param name="queryModel">The customers query model.</param>
        /// <returns>The number of customers by query model.</returns>
        Task<int> GetCountAsync(CustomerQueryModel queryModel);

        /// <summary>
        /// Gets list of customers.
        /// </summary>
        /// <param name="ids">The list of ids.</param>
        /// <returns>The list of customers.</returns>
        Task<IEnumerable<CustomersEntity>> ListByIdsAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Gets a customer.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns>The customer.</returns>
        Task<CustomersEntity> FindAsync(Guid customerId);

        /// <summary>
        /// Gets a customer by phone number.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        Task<CustomersEntity> FindByPhoneNumberAsync(string phoneNumber);

        /// <summary>
        /// Deletes a customer.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <returns>The deleted customer.</returns>
        Task<CustomersEntity> DeleteAsync(Guid customerId);

        /// <summary>
        /// Deletes a list of customers.
        /// </summary>
        /// <param name="deleteIds">The list of customer ids.</param>
        /// <returns>The deleted customers.</returns>
        Task<IEnumerable<CustomersEntity>> DeleteAsync(Guid[] deleteIds);

        /// <summary>
        /// Saves a customer.
        /// </summary>
        /// <param name="Customers"></param>
        /// <returns></returns>
        Task<CustomersEntity> SaveAsync(CustomersEntity Customers);

        /// <summary>
        /// Saves customers.
        /// </summary>
        /// <param name="customerEntities"></param>
        /// <returns></returns>
        Task<IEnumerable<CustomersEntity>> SaveAsync(IEnumerable<CustomersEntity> customerEntities);

        /// <summary>
        /// Updates a customer.
        /// </summary>
        /// <param name="Customers"></param>
        /// <returns></returns>
        Task<CustomersEntity> PatchAsync(CustomersEntity Customers);
        
        Task<List<CustomerViewModel>> GetCustomerByPhoneNumber(string phoneNumber);
        bool AddCustomerSell(CustomerViewModel customer);
    }
}
