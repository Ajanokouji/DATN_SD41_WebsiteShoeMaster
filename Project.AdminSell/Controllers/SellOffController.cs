using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.AdminSell.Models;
using Project.Business.Interface;
using Project.Business.Model;
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

    public SellOffController(ILogger<SellOffController> logger, IBillBusiness billBusiness,
        IProductBusiness productBusiness, IBillDetailsBusiness billDetailsBusiness)
    {
        _logger = logger;
        _billBusiness = billBusiness;
        _productBusiness = productBusiness;
        _billDetailsBusiness = billDetailsBusiness;
    }

    [HttpGet]
    public IActionResult Sell()
    {
        var lstBill = _billBusiness.GetAllPendingBill();
        ViewData["lstBill"] = lstBill;
        UserEntity user = new UserEntity()
        {
            Id = Guid.Parse("ab68f918-2da3-4674-8b14-6f3c25579145"),
            Email = "admin@gmail.com",
            Name = "Admin",
            PhoneNumber = "0123456789"
        };
        var response = JsonConvert.SerializeObject(user);
        HttpContext.Session.SetString("LoginInfor", response);
        return View();
    }

    [HttpGet]
    public IActionResult GetAllPDBill()
    {
        var lstBill = _billBusiness.GetAllPendingBill();
        return Ok(lstBill);
    }

    [HttpPost]
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
                Quantity = request.Quantity,
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
        };
        return PartialView("_Pay", payBill);
    }

    public async Task<IActionResult> ThanhToan(PaymentBillRequest request)
    {
        var billrequest = new PaymentBillRequest()
        {
            Id = request.Id,
            IdEmployee = request.IdEmployee,
            PaymentDate = DateTime.Now,
            PaymentMethod = request.PaymentMethod,
            TotalPrice = request.TotalPrice,
            status = "complete",
        };
        var response = _billBusiness.PaymentBill(billrequest);
        if (response == true)
        {
            return Json(new { success = true, message = "Payment Success" });
        }
        else
            return Json(new { success = false, message = "Payment Fails" });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}