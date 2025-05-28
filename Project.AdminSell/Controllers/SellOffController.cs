using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.AdminSell.Models;
using Project.Business.Interface;
using Project.Business.Libraries;
using Project.Business.Model;
using Project.Common.Constants;
using Project.DbManagement;
using Project.DbManagement.Entity;
using Project.DbManagement.ViewModels;

namespace Project.AdminSell.Controllers;

public class SellOffController : Controller
{
    private readonly ILogger<SellOffController> _logger;
    private readonly IBillBusiness _billBusiness;
    private readonly IProductBusiness _productBusiness;
    private readonly IBillDetailsBusiness _billDetailsBusiness;
    private readonly ICustomerBusiness _customerBusiness;
    private const string vnp_TmnCode = "BG41PLRJ"; // ví dụ: ABCDEF01
    private const string vnp_HashSecret = "Z2KAB0MGR42X4UMOQU3MKEU2IHVOP3H3"; // ví dụ: 1a2b3c...

    private const string
        vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"; // Dùng sandbox thì đổi thành sandbox

    public SellOffController(ILogger<SellOffController> logger, IBillBusiness billBusiness,
        IProductBusiness productBusiness, IBillDetailsBusiness billDetailsBusiness, ICustomerBusiness customerBusiness)
    {
        _logger = logger;
        _billBusiness = billBusiness;
        _productBusiness = productBusiness;
        _billDetailsBusiness = billDetailsBusiness;
        _customerBusiness = customerBusiness;
    }

    [HttpGet]
    public IActionResult Sell()
    {
        if (HttpContext.Session.GetString("LoginInfor") == null)
        {
            TempData["Alert"] = "Bạn phải đăng nhập để thực hiện thao tác này.";
            return RedirectToAction("Login", "Login");
        }

        var lstBill = _billBusiness.GetAllPendingBill();
        ViewData["lstBill"] = lstBill;
        return View();
    }

    [HttpGet]
    public IActionResult GetAllPDBill()
    {
        var lstBill = _billBusiness.GetAllPendingBill();
        return Ok(lstBill);
    }

    [HttpPost]
    [Route("SellOff/CreateBill/{idEmployee}")]
    public bool CreateBill(Guid idEmployee)
    {
        return _billBusiness.CreatePendingBill(idEmployee);
    }

    [HttpDelete]
    [Route("SellOff/DeleteBill/{idBill}")]
    public bool DeleteBill(Guid idBill)
    {
        return _billBusiness.DeletePendingBill(idBill);
    }

    [HttpGet]
    public async Task<IActionResult> LoadProduct(int page, int pagesize)
    {
        var listProduct = await _productBusiness.GetAllProduct();
        var model = listProduct.Skip((page - 1) * pagesize).Take(pagesize).ToList();
        int totalRow = listProduct.Count;
        return Json(new
        {
            data = model,
            total = totalRow,
            status = true,
        });
    }

    [HttpGet]
    [Route("SellOff/ShowProductDetail/{idprd}")]
    public async Task<IActionResult> ShowProductDetail(Guid idprd)
    {
        var product = await _productBusiness.GetProductDetailsById(idprd);
        return PartialView("_ProductDetails", product);
    }

    [HttpGet]
    [Route("SellOff/ListProductDetail/{idprd}")]
    public async Task<IActionResult> ListProductDetail(Guid idprd)
    {
        var product = await _productBusiness.ListProductDetailsById(idprd);
        return Json(new { data = product });
    }
    
    [HttpPost]
    public async Task<IActionResult> FilterProductDetails(FilterProductDetailsViewModel filter)
    {
        var lstProductDetails = await _productBusiness.ListProductDetailsById(filter.ProductId);
        //Lọc màu
        if(filter.lstColor != null)
        {
            lstProductDetails = lstProductDetails.Where(c => filter.lstColor.Contains(c.Color)).ToList();
        }
        //Lọc kích thước
        if(filter.lstSize != null)
        {
            lstProductDetails = lstProductDetails.Where(c => filter.lstSize.Contains(c.Size)).ToList();
        }
        return Json(new { data = lstProductDetails });
    }

    [HttpGet("/SellOff/GetPDBill/{id}")]
    public IActionResult GetPDBill(Guid id)
    {
        var bill = _billBusiness.GetPDBillById(id);
        return PartialView("_Cart", bill);
    }

    [HttpPost]
    public async Task<ActionResult> AddProductToCart(BillDetailsRequest request)
    {
        try
        {
            BillDetailsRequest billDetails = new BillDetailsRequest()
            {
                Id = new Guid(),
                IdProduct = request.IdProduct,
                IdBill = request.IdBill,
                IdEmployee = request.IdEmployee,
                Quantity = request.Quantity,
                // createbyid, lasmodifiedbyid, createdondate, lastmodifiedondate, productimg, productname, 
                Color = request.Color,
                Size = request.Size,
                //DonGia = request.DonGia,//Thanh toán rồi mới lưu
            };
            var response = await _billDetailsBusiness.SaveBillDetails(billDetails);
            if (response) return Json(new { success = true });
            return Json(new { success = false });
        }
        catch
        {
            return Json(new { success = false });
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(Guid idbilldeltails, int quantity)
    {
        try
        {
            var response = await _billDetailsBusiness.UpdateQuantity(idbilldeltails, quantity);
            return Json(new { success = true, data = response });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpDelete]
    [Route("SellOff/DeleteBillDetails/{idbilldeltails}")]
    public async Task<IActionResult> DeleteBillDetails(Guid idbilldeltails)
    {
        var response = await _billDetailsBusiness.DeleteBillDetails(idbilldeltails);
        if (response == true)
        {
            return Json(new { success = true, message = "Xóa thành công" });
        }
        else
            return Json(new { success = false, message = "Xóa thất bại" });
    }

    [HttpGet]
    public async Task<IActionResult> SearchProduct(int page, int pagesize, string keyword)
    {
        var listProduct = await _productBusiness.SearchProduct(keyword);
        var model = listProduct.Skip((page - 1) * pagesize).Take(pagesize).ToList();
        int totalRow = listProduct.Count;
        return Json(new
        {
            data = model,
            total = totalRow,
            status = true,
        });
    }

    [HttpGet("/SellOff/ViewPayment/{id}")]
    public async Task<IActionResult> ViewPayment(Guid id)
    {
        var bill = _billBusiness.GetPDBillById(id);
        var lstBillDetails = await _billDetailsBusiness.GetBillDetailsByIdBill(id);
        //Kiểm tra là hóa đơn của khách có tài khoản không?
        var client = "Customer";
        var loginInfor = new UserEntity();
        string? session = HttpContext.Session.GetString("LoginInfor");
        if (session != null)
        {
            loginInfor = JsonConvert.DeserializeObject<UserEntity>(session);
        }

        var quantity = lstBillDetails.Sum(c => c.Quantity);
        var totalPrice = lstBillDetails.Sum(c => c.Quantity * c.Price);
        //ViewData["lstPttt"] = listpttt;
        var payBill = new PaySellOffViewModel()
        {
            Id = bill.Id,
            BillCode = bill.BillCode,
            Client = client,
            Employee = loginInfor.Name,
            PaymentDate = DateTime.Now,
            TotalQuantity = quantity,
            TotalPrice = totalPrice,
            BillDetails = lstBillDetails
        };
        return PartialView("_Pay", payBill);
    }

    public async Task<IActionResult> ThanhToan(PaymentBillRequest request)
    {
        var billrequest = new PaymentBillRequest()
        {
            Id = request.Id,
            IdEmployee = request.IdEmployee,
            IdCustomer = request.IdCustomer,
            PaymentDate = DateTime.Now,
            PaymentMethod = request.PaymentMethod,
            TotalPrice = request.TotalPrice,
            status = BillConstants.Completed,
        };
        var response = _billBusiness.PaymentBill(billrequest);
        if (response == true)
        {
            return Json(new { success = true, message = "Payment Success" });
        }
        else
            return Json(new { success = false, message = "Payment Fails" });
    }

    [HttpGet]
    public async Task<IActionResult> SearchCustomers(string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return Json(new List<CustomerViewModel>());

            var result = await _customerBusiness.GetCustomerByPhoneNumber(query);

            return Json(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Lỗi: " + ex.Message);
        }
    }

    [HttpPost]
    public IActionResult AddCustomers(CustomerViewModel request)
    {
        try
        {
            var response = _customerBusiness.AddCustomerSell(request);
            return Json(new { success = response });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> InvoicePreview(Guid id)
    {
        var bill = _billBusiness.GetPDBillById(id);
        var lstBillDetails = await _billDetailsBusiness.ListAllByIdBill(id);
        //Kiểm tra là hóa đơn của khách có tài khoản không?
        var client = "Customer";
        var loginInfor = new UserEntity();
        string? session = HttpContext.Session.GetString("LoginInfor");
        if (session != null)
        {
            loginInfor = JsonConvert.DeserializeObject<UserEntity>(session);
        }

        var quantity = lstBillDetails.Sum(c => c.Quantity);
        var totalPrice = lstBillDetails.Sum(c => c.Quantity * c.Price);
        // Chuyển đổi BillDetailsEntity sang BillDetailsViewModel
        var billDetailsViewModels = new List<BillDetailsViewModel>();
        foreach (var detail in lstBillDetails)
        {
            billDetailsViewModels.Add(await ConvertToViewModel(detail));
        }

        var payBill = new PaySellOffViewModel()
        {
            Id = bill.Id,
            BillCode = bill.BillCode,
            Client = client,
            Employee = loginInfor.Name,
            PaymentDate = DateTime.Now,
            TotalQuantity = quantity,
            TotalPrice = totalPrice,
            BillDetails = billDetailsViewModels
        };
        return PartialView("_BillPreview", payBill);
    }

    private async Task<BillDetailsViewModel> ConvertToViewModel(BillDetailsEntity billDetailsEntity)
    {
        var res = new BillDetailsViewModel()
        {
            Color = billDetailsEntity.Color,
            Size = billDetailsEntity.Size,
            Id = billDetailsEntity.Id,
            IdBill = billDetailsEntity.BillId,
            IdProduct = billDetailsEntity.ProductId,
            Image = billDetailsEntity.ProductImage,
            Name = billDetailsEntity.ProductName,
            Quantity = billDetailsEntity.Quantity,
            Price = billDetailsEntity.Price
        };
        return res;
    }

    [HttpGet]
    public IActionResult CreateVnPayQr(string orderId, decimal amount)
    {
        string returnUrl = Url.Action("VnPayReturn", "SellOff", null, Request.Scheme);

        var vnPay = new VnPayLibrary();
        vnPay.AddRequestData("vnp_Version", "2.1.0");
        vnPay.AddRequestData("vnp_Command", "pay");
        vnPay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        vnPay.AddRequestData("vnp_Amount", ((long)amount * 100).ToString()); // VNPay nhân 100
        vnPay.AddRequestData("vnp_CurrCode", "VND");
        vnPay.AddRequestData("vnp_TxnRef", orderId);
        vnPay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn hàng {orderId}");
        vnPay.AddRequestData("vnp_OrderType", "billpayment");
        vnPay.AddRequestData("vnp_Locale", "vn");
        vnPay.AddRequestData("vnp_ReturnUrl", returnUrl);
        vnPay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString());
        vnPay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

        string paymentUrl = vnPay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        return Json(new { paymentUrl = paymentUrl });
    }

    [HttpGet]
    public IActionResult VnPayReturn()
    {
        // // Ở đây bạn có thể lấy các tham số query string trả về từ VNPay như:
        // var vnpAmount = Request.Query["vnp_Amount"];
        // var vnpTxnRef = Request.Query["vnp_TxnRef"];
        // var vnpResponseCode = Request.Query["vnp_ResponseCode"];
        // // ... xử lý logic thanh toán, xác thực secure hash, cập nhật trạng thái đơn hàng...
        //
        // // Tạm thời trả về thông báo đơn giản
        // return Content($"Thanh toán VNPay trả về: Amount={vnpAmount}, TxnRef={vnpTxnRef}, ResponseCode={vnpResponseCode}");
        var vnp_Amount = Request.Query["vnp_Amount"].ToString();
        var vnp_TxnRef = Request.Query["vnp_TxnRef"].ToString();
        var vnp_ResponseCode = Request.Query["vnp_ResponseCode"].ToString();
        var vnp_TransactionStatus = Request.Query["vnp_TransactionStatus"].ToString();

        var status = (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00") ? "success" : "fail";

        // Truyền dữ liệu trạng thái thanh toán về view Sell.cshtml qua TempData
        TempData["VnPayStatus"] = status;
        TempData["VnPayAmount"] = vnp_Amount;
        TempData["VnPayTxnRef"] = vnp_TxnRef;

        return RedirectToAction("Sell", "SellOff"); // giả sử đây là action của view Sell.cshtml
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}