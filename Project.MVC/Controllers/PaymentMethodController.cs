using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface.Services;
using Project.Business.Model;
using Project.Business.Model.VnPayments;

namespace Project.MVC.Controllers
{
    public class PaymentMethodController : Controller
    {
        private readonly IVnPayService _vnPayService;

        public PaymentMethodController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }
        private List<PaymentMethodModel> GetPaymentMethods()
        {
            return new List<PaymentMethodModel>
            {
                new PaymentMethodModel { Code = "COD", Name = "Thanh toán khi nhận hàng" },
                new PaymentMethodModel { Code = "Banking", Name = "Chuyển khoản ngân hàng" },
                new PaymentMethodModel { Code = "VNPay", Name = "VNPay" }
            };
        }

        public IActionResult Index()
        {
            var model = new PaymentViewModel
            {
                PaymentMethods = GetPaymentMethods(),
                TotalAmount = 1000000
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult ProcessPayment(PaymentViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    model.PaymentMethods = GetPaymentMethods();
            //    return View("Index", model);
            //}

            var selectedMethod = model.SelectedPaymentMethod;

            if (selectedMethod == "VNPay")
            {
                var paymentInfo = new PaymentInformationModel
                {
                    Amount = 1000000,
                    Name = "Khách hàng A",
                    OrderDescription = "Thanh toán đơn hàng ShoeMaster",
                    OrderType = "other"
                };

                TempData["VnPayModel"] = Newtonsoft.Json.JsonConvert.SerializeObject(paymentInfo);
                return RedirectToAction("VnPayRedirect");
            }
            else if (selectedMethod == "COD")
            {
                return RedirectToAction("Success");
            }
            else if (selectedMethod == "Banking")
            {
                return RedirectToAction("BankingInfo");
            }

            model.PaymentMethods = GetPaymentMethods();
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
