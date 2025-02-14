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
    public interface ICartRepository : IRepository<Cart, CartQueryModel>
    {
        protected const string MessageNoTFound = "Cart not found";
        Task<Cart> SaveAsync(Cart article);
        Task<IEnumerable<Cart>> SaveAsync(IEnumerable<Cart> cart);
    }
}
