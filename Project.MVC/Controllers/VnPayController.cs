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
using Project.Business.Interface.Services;
using Project.Business.Model.VnPayments;

namespace Project.MVC.Controllers
{
    public class VNPayController : Controller
    {
        private readonly IVnPayService _vnPayService;

        public VNPayController(IVnPayService vnPayService)
        {

            _vnPayService = vnPayService;
        }

        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }

        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return Json(response);
        }

    }
}
