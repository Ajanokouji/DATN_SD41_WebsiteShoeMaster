using Project.Business.Model;
using Project.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Business.Interface.Services
{
    public interface IPaymentService
    {
        Task<ServiceResult<string>> CreateVNPayPaymentUrl(PaymentViewModel model, string ipAddress);
        Task<ServiceResult<PaymentViewModel>> ProcessVNPayReturn(Dictionary<string, string> vnpayData);
        Task<ServiceResult<decimal>> ApplyVoucher(string voucherCode, decimal totalAmount);
        Task<ServiceResult<long>> SaveCustomerInfo(CustomerInfoModel customerInfo);
        Task<ServiceResult<bool>> ProcessPayment(PaymentViewModel model);
    }
} 