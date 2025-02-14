
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
            var exist = await _paymentMethodsRepository.FindAsync(model.id_phuong_thuc_thanh_toan);

            if (exist == null)
            {
                throw new ArgumentException("Payment method not found");
            }

            var update = new PaymentMethods
            {
                id_phuong_thuc_thanh_toan = exist.id_phuong_thuc_thanh_toan,
                ma_phuong_thuc_thanh_toan = exist.ma_phuong_thuc_thanh_toan,
                ten_phuong_thuc_thanh_toan = exist.ten_phuong_thuc_thanh_toan,
                trang_thai = exist.trang_thai,
                create_by = exist.create_by,
                create_on_date = exist.create_on_date,
                last_modifi_on_date = exist.last_modifi_on_date,
                update_by = exist.update_by
            };

            if (!string.IsNullOrWhiteSpace(model.ma_phuong_thuc_thanh_toan))
            {
                update.ma_phuong_thuc_thanh_toan = model.ma_phuong_thuc_thanh_toan;
            }
            if (!string.IsNullOrWhiteSpace(model.ten_phuong_thuc_thanh_toan))
            {
                update.ten_phuong_thuc_thanh_toan = model.ten_phuong_thuc_thanh_toan;
            }
            if (model.trang_thai >= 0)
            {
                update.trang_thai = model.trang_thai;
            }
            if (!string.IsNullOrWhiteSpace(model.update_by))
            {
                update.update_by = model.update_by;
            }
            if (model.last_modifi_on_date != null)
            {
                update.last_modifi_on_date = model.last_modifi_on_date;
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
