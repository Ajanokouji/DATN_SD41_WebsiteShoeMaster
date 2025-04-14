using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.AdminSell.Models;
using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;

namespace Project.AdminSell.Controllers;

public class SellOffController : Controller
{
    private readonly ILogger<SellOffController> _logger;
    private readonly IBillBusiness _billBusiness;
    private readonly IProductBusiness _productBusiness;

    public SellOffController(ILogger<SellOffController> logger, IBillBusiness billBusiness, IProductBusiness productBusiness)
    {
        _logger = logger;
        _billBusiness = billBusiness;
        _productBusiness = productBusiness;
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
    
    // public async Task<IActionResult> ShowProductDetail(Guid idprd)
    // {
    //     var product = await _productBusiness.GetProductDetailById(idprd);
    //     return Json(new
    //     {
    //         data = product,
    //         status = true,
    //     });
    // }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}