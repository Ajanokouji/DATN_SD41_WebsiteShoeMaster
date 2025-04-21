using Project.Business.Model;
using Project.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Business.Interface
{
    public interface IPaymentService
    {
        

        Task<ServiceResult<decimal>> ApplyVoucher(string voucherCode, decimal totalAmount);
        Task<ServiceResult<long>> SaveCustomerInfo(CustomerInfoModel customerInfo);
        Task<ServiceResult<bool>> ProcessPayment(PaymentViewModel model);
    }
} 