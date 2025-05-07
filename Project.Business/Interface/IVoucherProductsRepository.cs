using Project.Business.Model;
using Project.DbManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SERP.Framework.Common;
using Project.Business.Interface.Repositories;
using Project.DbManagement.Entity;

namespace Project.Business.Interface
{
    public interface IVoucherProductsRepository : IRepository<VoucherProducts, VoucherProductsQueryModel>
    {
        protected const string MessageNotFound = "VoucherProducts not found";
        Task<VoucherProducts> SaveAsync(VoucherProducts article);
        Task<IEnumerable<VoucherProducts>> SaveAsync(IEnumerable<VoucherProducts> voucherProducts);
        Task<IEnumerable<ProductEntity>> GetProductsByIds(Guid[] ids);
        Task<Pagination<ProductEntity>> SearchProduct(ProductQueryModel queryModel);
        Task<IEnumerable<VoucherProducts>> FindByVcidPidVaid(IEnumerable<VoucherProducts> voucherProducts);
    }
}
