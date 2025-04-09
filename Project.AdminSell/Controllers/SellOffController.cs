using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.AdminSell.IServices;
using Project.AdminSell.Models;
using Project.DbManagement.Entity;

namespace Project.AdminSell.Controllers;

public class SellOffController : Controller
{
    private readonly ILogger<SellOffController> _logger;
    private readonly ISellOffService _sellOffService;

    public SellOffController(ILogger<SellOffController> logger, ISellOffService sellOffService)
    {
        _logger = logger;
        _sellOffService = sellOffService;
    }

    [HttpGet]
    public IActionResult Sell()
    {
        var lstBill = _sellOffService.GetAllPendingBill();
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
        var lstBill = _sellOffService.GetAllPendingBill();
        return Ok(lstBill);
        // return Json(new { data = lstBill });
    }

    [HttpPost]
    public bool CreateBill(Guid idEmployee)
    {
        return _sellOffService.CreatePendingBill(idEmployee);
    }

    [HttpDelete]
    [Route("SellOff/DeleteBill/{idBill}")]
    public bool DeleteBill(Guid idBill)
    {
        return _sellOffService.DeletePendingBill(idBill);
    }

    // Sản phẩm
    [HttpGet]
    public async Task<IActionResult> LoadSp(int page, int pagesize)
    {
        // var listsanPham = await _httpClient.GetFromJsonAsync<List<SanPhamBanHang>>("SanPham/getAllSPBanHang");
        // var model = listsanPham.Skip((page - 1) * pagesize).Take(pagesize).ToList();
        // int totalRow = listsanPham.Count;
        // return Json(new
        // {
        //     data = model,
        //     total = totalRow,
        //     status = true,
        // });
        return Ok();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}