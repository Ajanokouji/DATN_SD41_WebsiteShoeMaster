using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.DbManagement.ViewModels;

namespace Project.Business
{
    public interface ICustomerRepository : IRepository<CustomersEntity, CustomerQueryModel>
    {
        protected const string MessageNoTFound = "Customer not found";
        Task<CustomersEntity> SaveAsync(CustomersEntity article);
        Task<IEnumerable<CustomersEntity>> SaveAsync(IEnumerable<CustomersEntity> customerEntities);
        Task<List<CustomerViewModel>> GetCustomerByPhoneNumber(string phoneNumber);
        bool AddCustomerSell(CustomerViewModel customer);
    }
}
