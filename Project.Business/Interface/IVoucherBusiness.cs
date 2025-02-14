using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Interface
{
    public interface IVoucherBusiness
    {
        Task<Pagination<Voucher>> GetAllAsync(VoucherQueryModel queryModel);

        /// <summary>
        /// Gets list of vouchers.
        /// </summary>
        /// <param name="queryModel">The query model.</param>
        /// <returns>The list of vouchers.</returns>
        Task<IEnumerable<Voucher>> ListAllAsync(VoucherQueryModel queryModel);

        /// <summary>
        /// Count the number of vouchers by query model.
        /// </summary>
        /// <param name="queryModel">The vouchers query model.</param>
        /// <returns>The number of vouchers by query model.</returns>
        Task<int> GetCountAsync(VoucherQueryModel queryModel);

        /// <summary>
        /// Gets list of vouchers by ids.
        /// </summary>
        /// <param name="ids">The list of ids.</param>
        /// <returns>The list of vouchers.</returns>
        Task<IEnumerable<Voucher>> ListByIdsAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Gets a voucher.
        /// </summary>
        /// <param name="id">The voucher id.</param>
        /// <returns>The voucher.</returns>
        Task<Voucher> FindAsync(Guid id);

        /// <summary>
        /// Deletes a voucher.
        /// </summary>
        /// <param name="id">The voucher id.</param>
        /// <returns>The deleted voucher.</returns>
        Task<Voucher> DeleteAsync(Guid id);

        /// <summary>
        /// Deletes a list of vouchers.
        /// </summary>
        /// <param name="deleteIds">The list of voucher ids.</param>
        /// <returns>The deleted vouchers.</returns>
        Task<IEnumerable<Voucher>> DeleteAsync(Guid[] deleteIds);

        /// <summary>
        /// Saves a voucher.
        /// </summary>
        /// <param name="voucher"></param>
        /// <returns></returns>
        Task<Voucher> SaveAsync(Voucher voucher);

        /// <summary>
        /// Saves vouchers.
        /// </summary>
        /// <param name="vouchers"></param>
        /// <returns></returns>
        Task<IEnumerable<Voucher>> SaveAsync(IEnumerable<Voucher> vouchers);

        /// <summary>
        /// Updates a voucher.
        /// </summary>
        /// <param name="voucher"></param>
        /// <returns></returns>
        Task<Voucher> PatchAsync(Voucher voucher);
    }
}

