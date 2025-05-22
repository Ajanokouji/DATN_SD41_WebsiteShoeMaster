using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Interface;
using Project.Business.Interface.Services;
using Project.Business.Model;
using Project.Business.Model.PatchModel;
using Project.Business.Model.VnPayments;
using Project.Common;
using Project.Common.Constants;
using Project.DbManagement.Entity;
using System.Collections.Generic;


namespace Project.MVC.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IBillBusiness _billBusiness;
        private readonly IVoucherBusiness _voucherBusiness;
        private readonly IProductBusiness _productBusiness;
        private readonly ICartBusiness _cartBusiness;
        private readonly IBillDetailsBusiness _billDetailsBusiness;
        private readonly IVnPayService _vnPayService;
        private readonly IUserBusiness _userBusiness;
        private const string CartSessionKey = "CartSession";

        public CheckoutController(
            IBillBusiness billBusiness,
            IVoucherBusiness voucherBusiness,
            ICartBusiness cartBusiness,
            IProductBusiness productBusiness,
            IBillDetailsBusiness billDetailsBusiness,
            IVnPayService vnPayService,
            IUserBusiness userBusiness)
        {
            _productBusiness = productBusiness;
            _billBusiness = billBusiness;
            _voucherBusiness = voucherBusiness;
            _cartBusiness = cartBusiness;
            _billDetailsBusiness = billDetailsBusiness;
            _vnPayService = vnPayService;
            _userBusiness =userBusiness;
        }

        public IActionResult Checkout()
        {
            return View(); 
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = new List<CartItemModel>();
            var user = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("UserSession"));

            if (user != null)
            {
                data = (await _cartBusiness.GetCartItemsByUserId(user.Id));
            }
            else
            {
                var cartSession = HttpContext.Session.GetString(CartSessionKey);
                if (!string.IsNullOrEmpty(cartSession))
                {
                    data = JsonConvert.DeserializeObject<List<CartItemModel>>(cartSession);
                }
            }

            if (data == null || !data.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel
            {
                BillId = Guid.NewGuid(),
                CartItems =data,
                CustomerInfo = new CustomerInfoModel()
                {
                    Address=user.Address??string.Empty,
                    Email=user.Email??string.Empty,
                    FullName=user.Name ?? string.Empty,
                    PhoneNumber=user.PhoneNumber ?? string.Empty,
                    UserId=user.Id
                },
                SubTotal = data.Sum(x => x.Total),
                Total = data.Sum(x => x.Total),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Vui lòng kiểm tra lại thông tin" });
                }

                var cartSession = HttpContext.Session.GetString(CartSessionKey);
                if (string.IsNullOrEmpty(cartSession))
                {
                    return Json(new { success = false, message = "Giỏ hàng trống" });
                }

                var cartSessions = JsonConvert.DeserializeObject<List<CartItem>>(cartSession);
                var cartItemsResult = await _cartBusiness.GetCartItems(cartSessions);
                if (!cartItemsResult.IsSuccess || cartItemsResult.Data == null || !cartItemsResult.Data.Any())
                {
                    return Json(new { success = false, message = "Giỏ hàng trống" });
                }

                // Tạo đơn hàng
                var billModel = new BillModel
                {
                    CustomerName = model.CustomerInfo.FullName,
                    CustomerPhone = model.CustomerInfo.PhoneNumber,
                    CustomerEmail = model.CustomerInfo.Email,
                    CustomerAddress = $"{model.CustomerInfo.Address}, {model.CustomerInfo.District}, {model.CustomerInfo.City}",
                    Note = model.CustomerInfo.Notes,
                    PaymentMethod = model.paymentMethodModel.Code,
                    Status = BillConstants.PendingConfirmation,
                    PaymentStatus = BillConstants.PaymentStatusUnpaid,
                    BillDetails = cartItemsResult.Data.Select(item => new BillDetailModel
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        ProductImage = item.ProductImage,
                        Size = item.Size,
                        Color = item.Color,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        TotalPrice = item.Total
                    }).ToList(),
                    TotalAmount = cartItemsResult.Data.Sum(x => x.Total)
                };

                var bill = await _billBusiness.CreateBill(billModel);



                await _userBusiness.CreateUserFromCustomerInfo(model.CustomerInfo);

                if (bill == null || bill.Id == null)
                {
                    return Json(new { success = false, message = "Không thể tạo đơn hàng" });
                }

                // Nếu thanh toán COD, xóa giỏ hàng và chuyển đến trang cảm ơn
                if (model.paymentMethodModel.Code == "COD")
                {
                    // Cập nhật trạng thái đơn hàng
                   // await _billBusiness.UpdateBillStatus(bill.Id.Value, BillConstants.StatusConfirmed);
                    
                    // Xóa giỏ hàng
                    HttpContext.Session.Remove(CartSessionKey);

                    return RedirectToAction("ThankYou",new { billId = bill.Id });
                }
                // Nếu thanh toán VNPay, trả về orderId để client tạo URL thanh toán
                else if (model.paymentMethodModel.Code == "VNPay")
                {
                    return Json(new { success = true, orderId = bill.Id });
                }
                else
                {
                    return Json(new { success = false, message = "Phương thức thanh toán không hợp lệ" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra khi đặt hàng: {ex.Message}" });
            }
        }

        public async Task<IActionResult> ThankYou(Guid billId)
        {
            if (billId == null)
            {
                return RedirectToAction("Index", "Home");
            }

             var bill = await _billBusiness.GetBillById(billId);

            if (bill == null)
            {
                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.Remove(CartSessionKey);
            return View(bill);
        }


        public async Task<IActionResult> CheckBillStatus(string billCode)
        {
            if (string.IsNullOrEmpty(billCode))
            {
                return View("OrderNotFound");
            }

            var bill = await _billBusiness.GetBillByCode(billCode);

            if (bill != null)
            {
                return View("ThankYou", bill);
            }
            else
            {
                return View("OrderNotFound");
            }
        }
        //[HttpPost]
        //public async Task<IActionResult> ApplyVoucher(string voucherCode)
        //{

        //    if (string.IsNullOrEmpty(voucherCode))
        //    {
        //        return Json(new { isSuccess = false, message = "Vui lòng nhập mã giảm giá" });
        //    }

        //    try
        //    {

        //        var voucher = await _voucherBusiness.FindByCodeAsync(voucherCode);
        //        if (voucher ==null)
        //            return Json(new { isSuccess = false, message = "Voucher không tìm thấy" });

        //        var voucherType = voucher.VoucherType;

        //        var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
        //        var cartSessions = string.IsNullOrEmpty(cartSessionJson)
        //            ? new List<CartItem>()
        //            : JsonConvert.DeserializeObject<List<CartItem>>(cartSessionJson);

        //        var OrderAmount = cartSessions.Sum(x => x.Total);

        //        if (voucher.MinimumOrderAmount != null && OrderAmount<voucher.MinimumOrderAmount)
        //        {
        //            Json(new { isSuccess = false, message = "Voucher không đủ điều kiện để dùng" });
        //        } 

        //    }
            //catch (Exception ex)
            //{
            //    return Json(new { isSuccess = false, message = ex.Message });
            //}
        

        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.GetPaymentResult(Request.Query);
            var paymentInformationModel = JsonConvert.DeserializeObject<PaymentInformationModel>(response.OrderDescription);
            if (response.Success)
            {
               var bill= await _billBusiness.PatchAsync(new BillPatchModel()
                {
                    Id = paymentInformationModel.BillId.Value,
                    PaymentStatus = BillConstants.PaymentStatusPaid,
                    PaymentMethod =paymentInformationModel.PayMethod,
                    Status = BillConstants.Paid,
                });

                var billDetails = await _billDetailsBusiness.GetBillDetailsByBillId(bill.Id);

        
               if(billDetails!=null&& billDetails.Any())
                {
                    foreach (var item in billDetails)
                    {
                        var product = await _productBusiness.FindAsync(item.ProductId.Value);
                        if (product != null)
                        {
                            var existVariant = product.VariantObjs?.FirstOrDefault(x => x.Sku==item.SKU);
                            await _productBusiness.PatchVariantStockBySKUAsync(product.Id, new Variant()
                            {
                                Stock = existVariant.Stock - item.Quantity,
                                Sku = item.SKU,
                            });
                        }
                    }
                }

            }
            else
            {
                await _billBusiness.PatchAsync(new BillPatchModel()
                {
                    Id = paymentInformationModel.BillId.Value,
                    PaymentStatus = BillConstants.PaymentStatusUnpaid,
                    PaymentMethod = paymentInformationModel.PayMethod,
                    Status = BillConstants.Cancelled,
                });
            }
                return RedirectToAction("ThankYou", new { billId = paymentInformationModel.BillId });
        }
    }
}
