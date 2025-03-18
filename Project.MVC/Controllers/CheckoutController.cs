using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Interface;
using Project.Business.Interface.Services;
using Project.Business.Model;
using Project.Common;
using Project.Common.Constants;
using Project.MVC.Models;
using Project.Project.Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IBillBusiness _billBusiness;
        private readonly ICartBusiness _cartBusiness;
        private readonly IPaymentService _paymentService;
        private const string CartSessionKey = "CartSession";

        public CheckoutController(
            IBillBusiness billBusiness,
            ICartBusiness cartBusiness,
            IPaymentService paymentService)
        {
            _billBusiness = billBusiness;
            _cartBusiness = cartBusiness;
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cartSession = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartSession))
            {
                return RedirectToAction("Index", "Cart");
            }

            var cartSessions = JsonConvert.DeserializeObject<List<CartSession>>(cartSession);
            var cartItemsResult = await _cartBusiness.GetCartItems(cartSessions);
            if (!cartItemsResult.IsSuccess || cartItemsResult.Data == null || !cartItemsResult.Data.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel
            {
                CartItems = cartItemsResult.Data,
                CustomerInfo = new CustomerInfoModel()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model, List<Guid> CartItemIds)
        {
            try
            {

                var cartSession = HttpContext.Session.GetString(CartSessionKey);
                if (string.IsNullOrEmpty(cartSession))
                {
                    return Json(new { success = false, message = "Giỏ hàng trống" });
                }

                var cartSessions = JsonConvert.DeserializeObject<List<CartSession>>(cartSession);
                var cartItemsResult = await _cartBusiness.GetCartItems(cartSessions);
                if (!cartItemsResult.IsSuccess || cartItemsResult.Data == null || !cartItemsResult.Data.Any())
                {
                    return Json(new { success = false, message = "Giỏ hàng trống" });
                }

                // Lọc các item trong giỏ hàng theo danh sách CartItemIds
                var selectedCartItems = cartItemsResult.Data.Where(item => CartItemIds.Contains(item.ProductId)).ToList();

                // Tạo đơn hàng
                var bill = await _billBusiness.CreateBill(new BillModel
                {
                    CustomerName = model.CustomerInfo.FullName,
                    CustomerPhone = model.CustomerInfo.PhoneNumber,
                    CustomerEmail = model.CustomerInfo.Email,
                    CustomerAddress = $"{model.CustomerInfo.Address}, {model.CustomerInfo.District}, {model.CustomerInfo.City}",
                    Note = model.CustomerInfo.Notes,
                    PaymentMethod = model.PaymentMethod,
                    Status = BillConstants.StatusPending,
                    PaymentStatus = BillConstants.PaymentStatusUnpaid,
                    BillDetails = selectedCartItems.Select(item => new BillDetailModel
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        TotalPrice = item.Total
                    }).ToList()
                });

                if (bill == null)
                {
                    return Json(new { success = false, message = "Không thể tạo đơn hàng" });
                }

                // Xử lý thanh toán
                if (model.PaymentMethod == "VNPay")
                {
                    var paymentUrl = await _paymentService.CreateVNPayPaymentUrl(new PaymentViewModel
                    {
                        BillId = bill.Id.Value,
                        TotalAmount = model.Total
                    }, HttpContext.Connection.RemoteIpAddress.ToString());
                    return Json(new { success = true, paymentUrl = paymentUrl.Data });
                }
                else // COD
                {
                    // Xóa dữ liệu của cart session
                    HttpContext.Session.Remove(CartSessionKey);
                    return RedirectToAction("ThankYou", new { orderId = bill.Id });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi đặt hàng" });
            }
        }


        public async Task<IActionResult> ThankYou(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                return RedirectToAction("Index", "Home");
            }

            var bill = await _billBusiness.GetBillById(Guid.Parse(orderId));
            if (bill == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(bill);
        }

        public async Task<IActionResult> VNPayReturn()
        {
            var queryString = Request.QueryString.ToString();
            //var result = await _paymentService.ProcessVNPayReturn(queryString);

            //if (result.IsSuccess)
            //{
            //    return RedirectToAction("ThankYou", new { orderId = result.Data });
            //}

            return RedirectToAction("Index", "Cart");
        }

        [HttpGet]
        public async Task<IActionResult> PaymentMethod(Guid billId)
        {
            var bill = await _billBusiness.GetBillById(billId);
            if (bill == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var paymentViewModel = new PaymentViewModel
            {
                BillId = billId,
                TotalAmount = bill.FinalAmount,
                PaymentMethods = new List<PaymentMethodModel>
                {
                    new PaymentMethodModel { Code = "COD", Name = "Thanh toán khi nhận hàng" },
                    new PaymentMethodModel { Code = "Banking", Name = "Chuyển khoản ngân hàng" },
                    new PaymentMethodModel { Code = "VNPay", Name = "Thanh toán qua VNPay" }
                }
            };

            return View(paymentViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("PaymentMethod", model);
            }

            // Cập nhật phương thức thanh toán
            var updateResult = await _billBusiness.UpdatePaymentMethod(model.BillId, model.SelectedPaymentMethod);
            if (!updateResult)
            {
                ModelState.AddModelError("", "Không thể cập nhật phương thức thanh toán");
                return View("PaymentMethod", model);
            }

            // Xử lý theo phương thức thanh toán
            switch (model.SelectedPaymentMethod)
            {
                case "COD":
                    // Cập nhật trạng thái đơn hàng
                    await _billBusiness.UpdateBillStatus(model.BillId, BillConstants.StatusConfirmed); // Confirmed
                    // Xóa giỏ hàng
                    HttpContext.Session.Remove(CartConstants.CartSessionKey);
                    HttpContext.Session.Remove(CartConstants.CartCountKey);
                    HttpContext.Session.Remove(CartConstants.CartTotalKey);
                    return RedirectToAction("OrderSuccess", new { billId = model.BillId });

                case "Banking":
                    return RedirectToAction("BankingInfo", new { billId = model.BillId });

                case "VNPay":
                    // Sẽ triển khai sau
                    return RedirectToAction("VNPayPayment", new { billId = model.BillId });

                default:
                    ModelState.AddModelError("", "Phương thức thanh toán không hợp lệ");
                    return View("PaymentMethod", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> OrderSuccess(Guid billId)
        {
            var bill = await _billBusiness.GetBillById(billId);
            if (bill == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(bill);
        }

        [HttpGet]
        public async Task<IActionResult> BankingInfo(Guid billId)
        {
            var bill = await _billBusiness.GetBillById(billId);
            if (bill == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var bankingInfo = new BankingInfoViewModel
            {
                BillId = billId,
                BillCode = bill.BillCode,
                Amount = bill.FinalAmount,
                BankAccount = "1234567890", // Thay bằng thông tin tài khoản thật
                BankName = "VietcomBank",
                AccountName = "SHOP GIAY ABC"
            };

            return View(bankingInfo);
        }

        [HttpPost]
        public async Task<IActionResult> ApplyVoucher(string voucherCode, decimal totalAmount)
        {
            if (string.IsNullOrEmpty(voucherCode))
            {
                return Json(new { isSuccess = false, message = "Vui lòng nhập mã giảm giá" });
            }

            var result = await _billBusiness.ApplyVoucher(voucherCode, totalAmount);
            return Json(new
            {
                isSuccess = result != 0,
                data = result != 0
            });
        }
    }
}
