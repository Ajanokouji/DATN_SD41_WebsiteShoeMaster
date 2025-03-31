using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.Business.Interface;
using Project.Business.Model;
using Project.Common;
using Project.Common.Constants;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using VNPAY.NET;
using VNPAY.NET.Models;
using VNPAY.NET.Enums;

namespace Project.MVC.Controllers
{
    public class VNPayController : Controller
    {
        private readonly IBillBusiness _billBusiness;
        private readonly ICartBusiness _cartBusiness;
        private readonly IConfiguration _configuration;
        private readonly IVnpay _vnpay;
        private const string CartSessionKey = "CartSession";

        public VNPayController(
            IBillBusiness billBusiness,
            ICartBusiness cartBusiness,
            IConfiguration configuration,
            IVnpay vnpay)
        {
            _billBusiness = billBusiness;
            _cartBusiness = cartBusiness;
            _configuration = configuration;
            _vnpay = vnpay;

            // Khởi tạo VNPay
            _vnpay.Initialize(
                _configuration["Vnpay:TmnCode"] ?? "",
                _configuration["Vnpay:HashSecret"] ?? "",
                _configuration["Vnpay:BaseUrl"] ?? "",
                _configuration["Vnpay:ReturnUrl"] ?? ""
            );
        }

        [HttpGet]
        public async Task<IActionResult> CreatePayment(Guid billId, decimal totalAmount)
        {
            try
            {
                // Lấy thông tin hóa đơn
                var bill = await _billBusiness.GetBillById(billId);
                if (bill == null || bill.Id == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn hàng";
                    return RedirectToAction("Index", "Cart");
                }

                // Lấy địa chỉ IP của khách hàng
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

                // Tạo yêu cầu thanh toán VNPay
                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = (double)totalAmount,
                    Description = $"Thanh toan don hang {billId}",
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY,
                    CreatedDate = DateTime.Now,
                    Currency = Currency.VND,
                    Language = DisplayLanguage.Vietnamese
                };

                // Lấy URL thanh toán từ VNPay
                var paymentUrl = _vnpay.GetPaymentUrl(request);

                // Cập nhật phương thức thanh toán vào hóa đơn
                await _billBusiness.UpdatePaymentMethod(billId, "VNPay");

                // Chuyển hướng đến trang thanh toán VNPay
                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentReturn()
        {
            try
            {
                // Lấy kết quả thanh toán từ VNPay
                var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                
                if (paymentResult.IsSuccess)
                {
                    // Lấy mã đơn hàng từ kết quả thanh toán
                    string orderInfo = paymentResult.Description;
                    string orderIdStr = orderInfo.Replace("Thanh toan don hang ", "");
                    
                    // Chuyển đổi thành Guid
                    if (!Guid.TryParse(orderIdStr, out Guid billId))
                    {
                        TempData["ErrorMessage"] = "Không thể xác định mã đơn hàng";
                        return RedirectToAction("Index", "Cart");
                    }
                    
                    // Cập nhật trạng thái đơn hàng
                    await _billBusiness.UpdatePaymentStatus(billId, BillConstants.PaymentStatusPaid);
                    //await _billBusiness.UpdateBillStatus(billId, BillConstants.StatusConfirmed);
                    
                    // Xóa giỏ hàng
                    HttpContext.Session.Remove(CartSessionKey);
                    
                    TempData["SuccessMessage"] = "Thanh toán thành công!";
                    return RedirectToAction("ThankYou", "Checkout", new { orderId = billId });
                }
                else
                {
                    // Hiển thị lỗi cụ thể từ VNPay
                    string errorMessage = $"Thanh toán thất bại: {paymentResult.PaymentResponse?.Description}";
                    if (paymentResult.TransactionStatus?.Description != null)
                    {
                        errorMessage += $" - {paymentResult.TransactionStatus.Description}";
                    }
                    
                    TempData["ErrorMessage"] = errorMessage;
                    return RedirectToAction("Index", "Cart");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Xử lý thanh toán gặp lỗi: " + ex.Message;
                return RedirectToAction("Index", "Cart");
            }
        }
    }
}
