using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Interface;
using Project.Business.Model;
using Project.Common;
using Project.MVC.Models;
using Project.Project.Business.Model;

namespace Project.MVC.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IBillBusiness _billBusiness;
        private readonly ICartBusiness _cartBusiness;
        private const string CartSessionKey = "CartSession";

        public CheckoutController(IBillBusiness billBusiness, ICartBusiness cartBusiness)
        {
            _billBusiness = billBusiness;
            _cartBusiness = cartBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Lấy giỏ hàng từ session
            var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
            if (string.IsNullOrEmpty(cartSessionJson))
            {
                return RedirectToAction("Cart", "Cart");
            }

            var cartSessions = JsonConvert.DeserializeObject<List<CartSession>>(cartSessionJson);
            if (cartSessions == null || !cartSessions.Any())
            {
                return RedirectToAction("Cart", "Cart");
            }

            // Chuyển đổi từ CartSession sang CartItemModel
            var cartItemsResult = await _cartBusiness.GetCartItems(cartSessions);
            if (!cartItemsResult.IsSuccess)
            {
                return RedirectToAction("Cart", "Cart");
            }

            var cartItems = cartItemsResult.Data;
            var totalAmountResult = await _cartBusiness.CalculateCartTotal(cartItems);
            var totalAmount = totalAmountResult.IsSuccess ? totalAmountResult.Data : cartSessions.Sum(item => item.Total);

            var checkoutViewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                CustomerInfo = new CustomerInfoModel(),
                TotalAmount = totalAmount
            };

            return View(checkoutViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCheckout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            // Lấy giỏ hàng từ session
            var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
            if (string.IsNullOrEmpty(cartSessionJson))
            {
                return RedirectToAction("Cart", "Cart");
            }

            var cartSessions = JsonConvert.DeserializeObject<List<CartSession>>(cartSessionJson);
            if (cartSessions == null || !cartSessions.Any())
            {
                return RedirectToAction("Cart", "Cart");
            }

            // Chuyển đổi từ CartSession sang CartItemModel
            var cartItemsResult = await _cartBusiness.GetCartItems(cartSessions);
            if (!cartItemsResult.IsSuccess)
            {
                return RedirectToAction("Cart", "Cart");
            }

            var cartItems = cartItemsResult.Data;

            // Xử lý đặt hàng
            var result = await _billBusiness.CheckoutFromCart(cartItems, model.CustomerInfo, model.VoucherCode);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message);
                model.CartItems = cartItems;
                return View("Index", model);
            }

            // Lưu ID hóa đơn vào session để xử lý thanh toán
            HttpContext.Session.SetString("PendingBillId", result.Data.Id.ToString());

            // Chuyển đến trang chọn phương thức thanh toán
            return RedirectToAction("PaymentMethod", new { billId = result.Data.Id });
        }

        [HttpGet]
        public async Task<IActionResult> PaymentMethod(long billId)
        {
            var bill = await _billBusiness.GetBillById(billId);
            if (!bill.IsSuccess)
            {
                return RedirectToAction("Index", "Home");
            }

            var paymentViewModel = new PaymentViewModel
            {
                BillId = billId,
                TotalAmount = bill.Data.FinalAmount,
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
            if (!updateResult.IsSuccess)
            {
                ModelState.AddModelError("", updateResult.Message);
                return View("PaymentMethod", model);
            }

            // Xử lý theo phương thức thanh toán
            switch (model.SelectedPaymentMethod)
            {
                case "COD":
                    // Cập nhật trạng thái đơn hàng
                    await _billBusiness.UpdateBillStatus(model.BillId, 1); // Confirmed
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
        public async Task<IActionResult> OrderSuccess(long billId)
        {
            var bill = await _billBusiness.GetBillById(billId);
            if (!bill.IsSuccess)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(bill.Data);
        }

        [HttpGet]
        public async Task<IActionResult> BankingInfo(long billId)
        {
            var bill = await _billBusiness.GetBillById(billId);
            if (!bill.IsSuccess)
            {
                return RedirectToAction("Index", "Home");
            }

            var bankingInfo = new BankingInfoViewModel
            {
                BillId = billId,
                BillCode = bill.Data.BillCode,
                Amount = bill.Data.FinalAmount,
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
            return Json(new { 
                isSuccess = result.IsSuccess, 
                message = result.Message, 
                data = result.IsSuccess ? result.Data : 0 
            });
        }
    }
}
