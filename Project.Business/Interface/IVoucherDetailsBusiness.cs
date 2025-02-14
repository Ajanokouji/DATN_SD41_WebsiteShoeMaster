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
    public interface IVoucherDetailsBusiness
    {
        Task<Pagination<VoucherDetails>> GetAllAsync(VoucherDetailsQueryModel queryModel);

        /// <summary>
        /// Gets list of voucher details.
        /// </summary>
        /// <param name="queryModel">The query model.</param>
        /// <returns>The list of voucher details.</returns>
        Task<IEnumerable<VoucherDetails>> ListAllAsync(VoucherDetailsQueryModel queryModel);

        /// <summary>
        /// Count the number of voucher details by query model.
        /// </summary>
        /// <param name="queryModel">The voucher details query model.</param>
        /// <returns>The number of voucher details by query model.</returns>
        Task<int> GetCountAsync(VoucherDetailsQueryModel queryModel);

        /// <summary>
        /// Gets list of voucher details by ids.
        /// </summary>
        /// <param name="ids">The list of ids.</param>
        /// <returns>The list of voucher details.</returns>
        Task<IEnumerable<VoucherDetails>> ListByIdsAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Gets a voucher detail.
        /// </summary>
        /// <param name="contentId">The content id.</param>
        /// <returns>The voucher detail.</returns>
        Task<VoucherDetails> FindAsync(Guid contentId);

        /// <summary>
        /// Deletes a voucher detail.
        /// </summary>
        /// <param name="contentId">The content id.</param>
        /// <returns>The deleted voucher detail.</returns>
        Task<VoucherDetails> DeleteAsync(Guid contentId);

        /// <summary>
        /// Deletes a list of voucher details.
        /// </summary>
        /// <param name="deleteIds">The list of content ids.</param>
        /// <returns>The deleted voucher details.</returns>
        Task<IEnumerable<VoucherDetails>> DeleteAsync(Guid[] deleteIds);

        /// <summary>
        /// Saves a voucher detail.
        /// </summary>
        /// <param name="voucherDetails"></param>
        /// <returns></returns>
        Task<VoucherDetails> SaveAsync(VoucherDetails voucherDetails);

        /// <summary>
        /// Saves voucher details.
        /// </summary>
        /// <param name="voucherDetails"></param>
        /// <returns></returns>
        Task<IEnumerable<VoucherDetails>> SaveAsync(IEnumerable<VoucherDetails> voucherDetails);

        /// <summary>
        /// Updates a voucher detail.
        /// </summary>
        /// <param name="voucherDetails"></param>
        /// <returns></returns>
        Task<VoucherDetails> PatchAsync(VoucherDetails voucherDetails);
    }
}

