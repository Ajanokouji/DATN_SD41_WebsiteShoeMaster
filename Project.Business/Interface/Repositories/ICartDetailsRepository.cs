using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Interface.Repositories
{
    public interface ICartDetailsRepository : IRepository<CartDetails, CartDetailsQueryModel>
    {
        protected const string MessageNoTFound = "cart details not found";
        Task<CartDetails> SaveAsync(CartDetails article);
        Task<IEnumerable<CartDetails>> SaveAsync(IEnumerable<CartDetails> cartDetailses);
    }
}
