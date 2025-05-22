using Project.Business.Model;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.ModelFactory
{
    public interface ICartDetailFactory
    {
        Task<CartItemModel> ConvertToModel(CartDetails cartDetail);
        Task<IEnumerable<CartItemModel>> ConvertToModels(IEnumerable<CartDetails> cartDetails);
    }
}
