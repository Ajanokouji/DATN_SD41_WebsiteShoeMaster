using Microsoft.AspNetCore.Http;
using Project.Business.Model.VnPayments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Interface.Services
{
    public interface IVnPayService
    {
        void Initialize(
           string tmnCode,
           string hashSecret,
           string baseUrl,
           string callbackUrl,
           string version = "2.1.0",
           string orderType = "other");

        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel GetPaymentResult(IQueryCollection collections);

    }
}
