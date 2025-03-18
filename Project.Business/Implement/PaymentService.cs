using Project.Business.Interface;
using Project.Business.Interface.Services;
using Project.Business.Model;
using Project.Common;
using Project.Common.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Business.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBillBusiness _billBusiness;

        public PaymentService(IBillBusiness billBusiness)
        {
            _billBusiness = billBusiness;
        }

        public async Task<ServiceResult<string>> CreateVNPayPaymentUrl(PaymentViewModel model, string ipAddress)
        {
            // Logic to create VNPay payment URL
            // This part is not implemented as per your request
            return new ServiceResult<string>
            {
                IsSuccess = true,
                Data = "https://vnpay.vn/payment-url"
            };
        }

        public async Task<ServiceResult<PaymentViewModel>> ProcessVNPayReturn(Dictionary<string, string> vnpayData)
        {
            // Logic to process VNPay return
            // This part is not implemented as per your request
            return new ServiceResult<PaymentViewModel>
            {
                IsSuccess = true,
                Data = new PaymentViewModel()
            };
        }

        public async Task<ServiceResult<decimal>> ApplyVoucher(string voucherCode, decimal totalAmount)
        {
            // Logic to apply voucher
            // For simplicity, let's assume a fixed discount of 10%
            decimal discountAmount = totalAmount * 0.1m;
            return new ServiceResult<decimal>
            {
                IsSuccess = true,
                Data = discountAmount
            };
        }

        public async Task<ServiceResult<long>> SaveCustomerInfo(CustomerInfoModel customerInfo)
        {
            // Logic to save customer info
            // For simplicity, let's assume the customer info is saved successfully and return a dummy ID
            return new ServiceResult<long>
            {
                IsSuccess = true,
                Data = 1
            };
        }

        public async Task<ServiceResult<bool>> ProcessPayment(PaymentViewModel model)
        {
            // Logic to process payment
            // Update the bill status and payment status based on the selected payment method
            var updatePaymentMethodResult = await _billBusiness.UpdatePaymentMethod(model.BillId, model.SelectedPaymentMethod);
            if (!updatePaymentMethodResult)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = "Failed to update payment method"
                };
            }

            var updatePaymentStatusResult = await _billBusiness.UpdatePaymentStatus(model.BillId, BillConstants.PaymentStatusPaid);
            if (!updatePaymentStatusResult)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = "Failed to update payment status"
                };
            }

            return new ServiceResult<bool>
            {
                IsSuccess = true,
                Data = true
            };
        }
    }
}
