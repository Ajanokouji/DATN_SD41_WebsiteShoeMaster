using Project.Business.Model;
using Project.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Business.Interface
{
    public interface IPaymentService
    {
        /// <summary>
        /// Tạo URL thanh toán VNPay
        /// </summary>
        Task<ServiceResult<string>> CreateVNPayPaymentUrl(PaymentViewModel model, string ipAddress);

        /// <summary>
        /// Xử lý kết quả thanh toán từ VNPay
        /// </summary>
        Task<ServiceResult<string>> ProcessVNPayReturn(string queryString);

        Task<ServiceResult<decimal>> ApplyVoucher(string voucherCode, decimal totalAmount);
        Task<ServiceResult<long>> SaveCustomerInfo(CustomerInfoModel customerInfo);
        Task<ServiceResult<bool>> ProcessPayment(PaymentViewModel model);
    }
} 