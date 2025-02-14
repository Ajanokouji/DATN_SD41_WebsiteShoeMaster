using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business
{
    public interface IContactRepository : IRepository<Contacts, ContactQueryModel>
    {
        protected const string MessageNoTFound = "Product not found";
        Task<Contacts> SaveAsync(Contacts article);
        Task<IEnumerable<Contacts>> SaveAsync(IEnumerable<Contacts> contacts);
    }
}
