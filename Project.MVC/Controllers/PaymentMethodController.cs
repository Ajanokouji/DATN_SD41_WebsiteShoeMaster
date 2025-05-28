using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Interface.Services;
using Project.Business.Model;
using Project.Business.Model.VnPayments;
using Project.Common.Constants;
using Project.Common;
using Project.Business.Interface;
using SERP.Framework.Models;
using Project.Business.Implement;
using Project.DbManagement;
using Project.DbManagement.Enum;
using Project.DbManagement.Entity;
using Project.MVC.Models;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class PaymentMethodController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly IBillBusiness _billBusiness;
        private readonly IBillDetailsBusiness _billDetailsBusiness;
        private readonly ICartBusiness _cartBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly ICustomerBusiness _customerBusiness;
        private const string CartSessionKey = "CartSession";
        private const string AppliedVoucher = "AppliedVoucher";

        public PaymentMethodController(IBillDetailsBusiness billDetailsBusiness , ICustomerBusiness customerBusiness, IVnPayService vnPayService, IBillBusiness billBusiness, ICartBusiness cartBusiness, IUserBusiness userBusiness)
        {
            _billDetailsBusiness = billDetailsBusiness;
            _customerBusiness = customerBusiness;
            _vnPayService = vnPayService;
            _billBusiness = billBusiness;
            _cartBusiness = cartBusiness;
            _userBusiness = userBusiness;
        }

        [HttpPost]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            var data = new List<CartItemModel>();
            var user = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("UserSession") ?? string.Empty);
            var applidvoucher = new AppliedVoucher();
            var appliedVoucherJson = HttpContext.Session.GetString(AppliedVoucher);

            if (!string.IsNullOrEmpty(appliedVoucherJson))
            {
                applidvoucher = JsonConvert.DeserializeObject<AppliedVoucher>(appliedVoucherJson);
            }

            var paymentMethods = new List<PaymentMethodModel>
    {
        new PaymentMethodModel { Code = "COD", Name = "Thanh toán khi nhận hàng" },
        new PaymentMethodModel { Code = "VNPay", Name = "VNPay" }
    };

            ViewData["PaymentMethods"] = paymentMethods;

            var cartItemData = new List<CartItem>();

            if (user != null)
            {
                var res = await _cartBusiness.GetCartItemsByUserId(user.Id);
                if (res != null && res.Any())
                {
                    foreach (var item in res)
                    {
                        cartItemData.Add(new CartItem()
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            ProductImage = item.ProductImage,
                            SKU = item.SKU,
                            Size = item.Size,
                            Color = item.Color,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            Total = item.Total,
                            Id = item.Id
                        });
                    }
                }
            }
            else
            {
                var cartSession = HttpContext.Session.GetString(CartSessionKey);
                if (string.IsNullOrEmpty(cartSession))
                {
                    return Json(new { success = false, message = "Giỏ hàng trống" });
                }
                cartItemData = JsonConvert.DeserializeObject<List<CartItem>>(cartSession);
            }

            var cartItemsResult = await _cartBusiness.GetCartItems(cartItemData);

            if (!cartItemsResult.IsSuccess || cartItemsResult.Data == null || !cartItemsResult.Data.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng trống" });
            }

            var createdByUserId = user?.Id ?? Guid.NewGuid();

            var billModel = new BillModel
            {
                Id = model.BillId.Value,
                CustomerId = model.CustomerInfo.UserId ?? Guid.NewGuid(),
                CreatedByUserId = createdByUserId,
                CustomerName = model.CustomerInfo.FullName,
                CustomerPhone = model.CustomerInfo.PhoneNumber,
                CustomerEmail = model.CustomerInfo.Email,
                CustomerAddress = $"{model.CustomerInfo.Address}, {model.CustomerInfo.District}, {model.CustomerInfo.City}",
                Note = model.CustomerInfo.Notes,
                Source = Source.Website,
                Status = BillConstants.PendingConfirmation,
                PaymentStatus = BillConstants.PaymentStatusUnpaid,
                BillDetails = cartItemsResult.Data.Select(item => new BillDetailModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductImage = item.ProductImage,
                    SKU = item.SKU,
                    Size = item.Size,
                    Color = item.Color,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    TotalPrice = item.Total
                }).ToList(),
                TotalAmount = cartItemsResult.Data.Sum(x => x.Total)
            };

            if (applidvoucher != null)
            {
                billModel.AmountAfterDiscount = billModel.TotalAmount - applidvoucher.DiscountAmount;
                billModel.VoucherCode = applidvoucher.VoucherCode;
                billModel.VoucherId = applidvoucher.VoucherId;
                billModel.AmountToPay = billModel.TotalAmount - applidvoucher.DiscountAmount;
                billModel.DiscountAmount = applidvoucher.DiscountAmount;
            }
            else
            {
                billModel.AmountToPay = billModel.TotalAmount;
            }

            var bill = await _billBusiness.CreateBill(billModel);

            var existCustomer = await _customerBusiness.FindAsync(model.CustomerInfo.UserId ?? billModel.CustomerId.Value);
            if (existCustomer == null)
            {
                await _customerBusiness.SaveAsync(new CustomersEntity()
                {
                    Id = billModel.CustomerId.Value,
                    Name = model.CustomerInfo.FullName,
                    PhoneNumber = model.CustomerInfo.PhoneNumber,
                    Email = model.CustomerInfo.Email,
                    Address = $"{model.CustomerInfo.Address}, {model.CustomerInfo.District}, {model.CustomerInfo.City}",
                });
            }


                if (user==null) {
                    await _userBusiness.CreateUserFromCustomerInfo(model.CustomerInfo);
                }

            var viewModel = new PaymentViewModel()
            {
                TotalAmount = model.Total,
                PaymentInformationModel = new PaymentInformationModel()
                {
                    Amount = (double)billModel.AmountToPay,
                    CustomerName = model.CustomerInfo.FullName,
                    OrderDescription = model.Description ?? string.Empty,
                    BillId = billModel.Id,
                },
            };

            HttpContext.Session.Remove(AppliedVoucher);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentViewModel model)
        {
            var user = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("UserSession")??string.Empty);
            var billDetail = await _billDetailsBusiness.GetBillDetailsByBillId(model.BillId);
            if (user!=null)
            {
               foreach(var item in billDetail)
                {
                    await _cartBusiness.RemoveFromCartDb(user.Id, item.ProductId.Value, item.SKU);
                }
            }
            else
            {
                HttpContext.Session.Remove(CartSessionKey);
            }
             var selectedMethod = model.SelectedPaymentMethod;

            if (selectedMethod == "VNPay")
            {

                model.PaymentInformationModel.PayMethod = "VNPay";
                TempData["VnPayModel"] = Newtonsoft.Json.JsonConvert.SerializeObject(model.PaymentInformationModel);
                return RedirectToAction("VnPayRedirect");
            }
            else if (selectedMethod == "COD")
            {
                model.PaymentInformationModel.PayMethod = "COD";
                return RedirectToAction("ThankYou", "Checkout", new { billId = model.BillId});
            }
            else if (selectedMethod == "Banking")
            {
                model.PaymentInformationModel.PayMethod = "Banking";
                return RedirectToAction("BankingInfo");
            }

            return View("Index", model);
        }

        public IActionResult VnPayRedirect()
        {
            if (TempData["VnPayModel"] is not string json)
                return RedirectToAction("Index");

            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentInformationModel>(json);
            var paymentUrl = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(paymentUrl);
        }
        
        public IActionResult Success()
        {
            return View(); 
        }

        public IActionResult BankingInfo()
        {
            return View(); 
        }
    }
}
