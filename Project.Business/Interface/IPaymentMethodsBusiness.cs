using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Business.Interface
{
    public interface IPaymentMethodsBusiness
    {
        Task<Pagination<PaymentMethods>> GetAllAsync(PaymentMethodsQueryModel queryModel);

        Task<IEnumerable<PaymentMethods>> ListAllAsync(PaymentMethodsQueryModel queryModel);

        Task<int> GetCountAsync(PaymentMethodsQueryModel queryModel);

        Task<IEnumerable<PaymentMethods>> ListByIdsAsync(IEnumerable<Guid> ids);

        Task<PaymentMethods> FindAsync(Guid id);

        Task<PaymentMethods> DeleteAsync(Guid id);

        Task<IEnumerable<PaymentMethods>> DeleteAsync(Guid[] deleteIds);

        Task<PaymentMethods> SaveAsync(PaymentMethods paymentMethods);

        Task<IEnumerable<PaymentMethods>> SaveAsync(IEnumerable<PaymentMethods> paymentMethods);

        Task<PaymentMethods> PatchAsync(PaymentMethods paymentMethods);
    }
}