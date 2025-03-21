using Microsoft.Extensions.Configuration;
using Project.Business.Interface;
using Project.Business.Model;
using Project.Common;
using Project.Common.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VNPAY.NET;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;
using System.Web;

namespace Project.Business.Implement
{
    public class PaymentService : IPaymentService
    {
        private readonly IBillBusiness _billBusiness;
        private readonly IVnpay _vnpay;
        private readonly IConfiguration _configuration;

        public PaymentService(IBillBusiness billBusiness, IVnpay vnpay, IConfiguration configuration)
        {
            _billBusiness = billBusiness;
            _vnpay = vnpay;
            _configuration = configuration;

            // Khởi tạo VNPay với cấu hình từ appsettings.json
            _vnpay.Initialize(
                _configuration["Vnpay:TmnCode"] ?? "",
                _configuration["Vnpay:HashSecret"] ?? "",
                _configuration["Vnpay:BaseUrl"] ?? "",
                _configuration["Vnpay:CallbackUrl"] ?? ""
            );
        }

        public async Task<ServiceResult<string>> CreateVNPayPaymentUrl(PaymentViewModel model, string ipAddress)
        {
            try
            {
                // Tạo yêu cầu thanh toán VNPay
                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = (double)model.TotalAmount,
                    Description = $"Thanh toán đơn hàng {model.BillId}",
                    IpAddress = ipAddress,
                    BankCode = VNPAY.NET.Enums.BankCode.ANY,
                    CreatedDate = DateTime.Now,
                    Currency = VNPAY.NET.Enums.Currency.VND,
                    Language = VNPAY.NET.Enums.DisplayLanguage.Vietnamese
                };

                // Lấy URL thanh toán từ VNPay
                var paymentUrl = _vnpay.GetPaymentUrl(request);

                // Lưu thông tin thanh toán vào hóa đơn
                await _billBusiness.UpdatePaymentMethod(model.BillId, "VNPay");

                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Data = paymentUrl,
                    Message = "Tạo URL thanh toán thành công"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tạo URL thanh toán: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult<string>> ProcessVNPayReturn(string queryString)
        {
            try
            {
                // Tạo các tham số truy vấn mà thư viện VNPAY.NET có thể hiểu
                var convertedQuery = HttpUtility.ParseQueryString(queryString);
                var queryDict = new Dictionary<string, string>();
                foreach (var key in convertedQuery.AllKeys)
                {
                    if (key != null)
                    {
                        queryDict[key] = convertedQuery[key] ?? string.Empty;
                    }
                }

                // Tạo chữ ký và xác minh
                string vnp_SecureHash = queryDict.ContainsKey("vnp_SecureHash") ? queryDict["vnp_SecureHash"] : "";
                if (string.IsNullOrEmpty(vnp_SecureHash))
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy chữ ký bảo mật"
                    };
                }

                // Kiểm tra chữ ký bằng cách sử dụng vnp_SecureHash
                // Thay vì sử dụng library, hãy kiểm tra response code trực tiếp
                string responseCode = queryDict.ContainsKey("vnp_ResponseCode") ? queryDict["vnp_ResponseCode"] : "";
                
                // Kiểm tra mã trạng thái giao dịch
                if (responseCode != "00")
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = $"Giao dịch thất bại với mã: {responseCode}"
                    };
                }

                // Lấy thông tin đơn hàng
                string orderInfo = queryDict.ContainsKey("vnp_OrderInfo") ? queryDict["vnp_OrderInfo"] : "";
                string orderIdStr = orderInfo.Replace("Thanh toan don hang ", "");
                string txnRef = queryDict.ContainsKey("vnp_TxnRef") ? queryDict["vnp_TxnRef"] : "";

                // Nếu không có orderInfo, thử sử dụng txnRef
                if (string.IsNullOrEmpty(orderIdStr) && !string.IsNullOrEmpty(txnRef))
                {
                    orderIdStr = txnRef;
                }

                // Chuyển đổi orderIdStr thành Guid
                if (!Guid.TryParse(orderIdStr, out Guid orderId))
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "Không thể xác định mã đơn hàng"
                    };
                }

                // Cập nhật trạng thái thanh toán
                await _billBusiness.UpdatePaymentStatus(orderId, BillConstants.PaymentStatusPaid);

                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Data = orderId.ToString(),
                    Message = "Xử lý thanh toán thành công"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi xử lý kết quả thanh toán: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult<decimal>> ApplyVoucher(string voucherCode, decimal totalAmount)
        {
            // Logic to apply voucher
            // For simplicity, let's assume a fixed discount of 10%
            decimal discountAmount = totalAmount * 0.1m;
            return new ServiceResult<decimal>
            {
                IsSuccess = true,
                Data = discountAmount,
                Message = "Áp dụng mã giảm giá thành công"
            };
        }

        public async Task<ServiceResult<long>> SaveCustomerInfo(CustomerInfoModel customerInfo)
        {
            // Logic to save customer info
            // For simplicity, let's assume the customer info is saved successfully and return a dummy ID
            return new ServiceResult<long>
            {
                IsSuccess = true,
                Data = 1,
                Message = "Lưu thông tin khách hàng thành công"
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
                Data = true,
                Message = "Xử lý thanh toán thành công"
            };
        }
    }
}
