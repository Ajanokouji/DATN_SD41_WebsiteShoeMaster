using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Project.AdminSell.Models;

namespace Project.AdminSell.Controllers;

public class SellOffController : Controller
{
    private readonly ILogger<SellOffController> _logger;

    public SellOffController(ILogger<SellOffController> logger)
    {
        _logger = logger;
    }

    public IActionResult Sell()
    {
        return View();
    }
    
    [HttpGet("getAll")]
        public async Task<IActionResult> GetAllSanPham()
        {
            return Ok();
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetSanPhamById(Guid id)
        {
            return Ok();
        }
        [HttpGet("getByIdLsp/{idLsp}")]
        public async Task<IActionResult> GetSanPhamByIdDanhMuc(Guid idLsp)
        {
            return Ok();
        }

        [HttpPost("AddSanPham")]
        public async Task<IActionResult> CreateSanPham()
        {
            return Ok();
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteSanPham(Guid id)
        {
            return Ok();
        }
        [HttpPost("AddAnh")]
        public async Task<IActionResult> AddAnhToSanPham()
        {
            return Ok();
        }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
