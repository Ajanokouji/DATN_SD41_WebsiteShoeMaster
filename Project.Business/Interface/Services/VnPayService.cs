using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Project.Business.Libraries;
using Project.Business.Model.VnPayments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Interface.Services
{
    public class VnPayService : IVnPayService
    {
        private string _tmnCode;
        private string _hashSecret;
        private string _callbackUrl;
        private string _baseUrl;
        private string _version;
        private string _orderType;

        private readonly IConfiguration _configuration;

        public void Initialize(string tmnCode,
            string hashSecret,
            string baseUrl,
            string callbackUrl,
            string version = "2.1.0",
            string orderType = "other")
        {
            _tmnCode = tmnCode;
            _hashSecret = hashSecret;
            _callbackUrl = callbackUrl;
            _baseUrl = baseUrl;
            _version = version;
            _orderType = orderType;

            EnsureParametersBeforePayment();
        }


        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["Vnpay:ReturnUrl"];

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{JsonConvert.SerializeObject(model)}");
            pay.AddRequestData("vnp_OrderType", model.OrderType.ToString());
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        public PaymentResponseModel GetPaymentResult(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            return response;
        }

        private void EnsureParametersBeforePayment()
        {
            if (string.IsNullOrEmpty(_baseUrl) || string.IsNullOrEmpty(_tmnCode) || string.IsNullOrEmpty(_hashSecret) || string.IsNullOrEmpty(_callbackUrl))
            {
                throw new ArgumentException("Không tìm thấy BaseUrl, TmnCode, HashSecret, hoặc CallbackUrl");
            }
        }

    }
}
