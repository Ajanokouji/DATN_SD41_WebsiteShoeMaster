using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business
{
    public interface ICustomerRepository : IRepository<Customers, CustomerQueryModel>
    {
        protected const string MessageNoTFound = "Customer not found";
        Task<Customers> SaveAsync(Customers article);
        Task<IEnumerable<Customers>> SaveAsync(IEnumerable<Customers> customerEntities);
    }
}
