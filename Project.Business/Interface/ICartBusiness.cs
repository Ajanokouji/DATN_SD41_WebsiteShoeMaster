using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Interface
{
    public interface ICartBusiness
    {
        Task<Pagination<Cart>> GetAllAsync(CartQueryModel queryModel);

        /// <summary>
        /// Gets list of carts.
        /// </summary>
        /// <param name="queryModel">The query model.</param>
        /// <returns>The list of carts.</returns>
        Task<IEnumerable<Cart>> ListAllAsync(CartQueryModel queryModel);

        /// <summary>
        /// Count the number of carts by query model.
        /// </summary>
        /// <param name="queryModel">The carts query model.</param>
        /// <returns>The number of carts by query model.</returns>
        Task<int> GetCountAsync(CartQueryModel queryModel);

        /// <summary>
        /// Gets list of carts by ids.
        /// </summary>
        /// <param name="ids">The list of ids.</param>
        /// <returns>The list of carts.</returns>
        Task<IEnumerable<Cart>> ListByIdsAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Gets a cart.
        /// </summary>
        /// <param name="cartId">The cart id.</param>
        /// <returns>The cart.</returns>
        Task<Cart> FindAsync(Guid cartId);

        /// <summary>
        /// Deletes a cart.
        /// </summary>
        /// <param name="cartId">The cart id.</param>
        /// <returns>The deleted cart.</returns>
        Task<Cart> DeleteAsync(Guid cartId);

        /// <summary>
        /// Deletes a list of carts.
        /// </summary>
        /// <param name="deleteIds">The list of cart ids.</param>
        /// <returns>The deleted carts.</returns>
        Task<IEnumerable<Cart>> DeleteAsync(Guid[] deleteIds);

        /// <summary>
        /// Saves a cart.
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        Task<Cart> SaveAsync(Cart cart);


        /// <summary>
        /// Saves a list of carts.
        /// </summary>
        /// <param name="carts"></param>
        /// <returns></returns>
        Task<IEnumerable<Cart>> SaveAsync(IEnumerable<Cart> carts);
        /// <summary>
        /// Patches a cart.
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        Task<Cart> PatchAsync(Cart cart);
    }
}
