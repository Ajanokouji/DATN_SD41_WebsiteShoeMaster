
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Business.Implement
{
    public class PaymentMethodsBusiness : IPaymentMethodsBusiness
    {
        private readonly IPaymentMethodsRepository _paymentMethodsRepository;

        public PaymentMethodsBusiness(IPaymentMethodsRepository paymentMethodsRepository)
        {
            _paymentMethodsRepository = paymentMethodsRepository;
        }

        public async Task<PaymentMethods> DeleteAsync(Guid id)
        {
            return await _paymentMethodsRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<PaymentMethods>> DeleteAsync(Guid[] deleteIds)
        {
            return await _paymentMethodsRepository.DeleteAsync(deleteIds);
        }

        public async Task<PaymentMethods> FindAsync(Guid id)
        {
            return await _paymentMethodsRepository.FindAsync(id);
        }

        public async Task<Pagination<PaymentMethods>> GetAllAsync(PaymentMethodsQueryModel queryModel)
        {
            return await _paymentMethodsRepository.GetAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(PaymentMethodsQueryModel queryModel)
        {
            return await _paymentMethodsRepository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<PaymentMethods>> ListAllAsync(PaymentMethodsQueryModel queryModel)
        {
            return await _paymentMethodsRepository.ListAllAsync(queryModel);
        }

        public async Task<IEnumerable<PaymentMethods>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _paymentMethodsRepository.ListByIdsAsync(ids);
        }

        public async Task<PaymentMethods> PatchAsync(PaymentMethods model)
        {
            var exist = await _paymentMethodsRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException("Payment method not found");
            }

            var update = new PaymentMethods
            {
                Id = exist.Id,
                PaymentMethodCode = exist.PaymentMethodCode,
                PaymentMethodName = exist.PaymentMethodName,
                Status = exist.Status,
                CreatedBy = exist.CreatedBy,
                CreatedOnDate = exist.CreatedOnDate,
                LastModifiedOnDate = exist.LastModifiedOnDate,
                UpdatedBy = exist.UpdatedBy
            };

            if (!string.IsNullOrWhiteSpace(model.PaymentMethodCode))
            {
                update.PaymentMethodCode = model.PaymentMethodCode;
            }
            if (!string.IsNullOrWhiteSpace(model.PaymentMethodName))
            {
                update.PaymentMethodName = model.PaymentMethodName;
            }
            if (model.Status >= 0)
            {
                update.Status = model.Status;
            }
            if (!string.IsNullOrWhiteSpace(model.UpdatedBy))
            {
                update.UpdatedBy = model.UpdatedBy;
            }
            if (model.LastModifiedOnDate != null)
            {
                update.LastModifiedOnDate = model.LastModifiedOnDate;
            }

            return await SaveAsync(update);
        }

        public async Task<PaymentMethods> SaveAsync(PaymentMethods paymentMethods)
        {
            var res = await SaveAsync(new[] { paymentMethods });
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<PaymentMethods>> SaveAsync(IEnumerable<PaymentMethods> paymentMethods)
        {
            return await _paymentMethodsRepository.SaveAsync(paymentMethods);
        }
    }
}
