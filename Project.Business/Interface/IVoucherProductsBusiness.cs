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
    public interface IVoucherProductsBusiness
    {
        Task<Pagination<VoucherProducts>> GetAllAsync(VoucherProductsQueryModel queryModel);

        Task<IEnumerable<VoucherProducts>> ListAllAsync(VoucherProductsQueryModel queryModel);

        Task<int> GetCountAsync(VoucherProductsQueryModel queryModel);

        Task<IEnumerable<VoucherProducts>> ListByVoucherIdAsync(Guid id);
        Task<IEnumerable<VoucherProducts>> ListByIdsAsync(IEnumerable<Guid> ids);

        Task<VoucherProducts> FindAsync(Guid id);

        Task<VoucherProducts> DeleteAsync(Guid id);

        Task<IEnumerable<VoucherProducts>> DeleteAsync(Guid[] deleteIds);

        Task<VoucherProducts> SaveAsync(VoucherProducts voucherProduct);

        Task<IEnumerable<VoucherProducts>> SaveAsync(IEnumerable<VoucherProducts> voucherProducts);

        Task<VoucherProducts> PatchAsync(VoucherProducts voucherProduct);
        Task<IEnumerable<ProductEntity>> GetProductsByIds(Guid[] ids);
        Task<Pagination<ProductEntity>> SearchProduct(ProductQueryModel queryModel);
        Task<IEnumerable<VoucherProducts>> FindByVcidPidVaid(IEnumerable<VoucherProducts> voucherProducts);
    }
}
