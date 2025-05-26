using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsBusiness _statisticsBusiness;

    public StatisticsController(IStatisticsBusiness statisticsBusiness)
    {
        _statisticsBusiness = statisticsBusiness;
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var data = await _statisticsBusiness.GetRevenueAsync(from, to);
        return Ok(data);
    }

    [HttpGet("revenue/monthly")]
    public async Task<IActionResult> GetMonthlyRevenue()
    {
        var data = await _statisticsBusiness.GetMonthlyRevenueAsync();

        // In ra log (console hoặc file log tùy config)
        foreach (var item in data)
        {
            Console.WriteLine($"Month: {item.Month}, Revenue: {item.TotalRevenue}");
        }

        return Ok(data); // Trả về frontend để dễ test luôn
    }



[HttpGet("revenue/weekly")]
    public async Task<IActionResult> GetWeeklyRevenue([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var data = await _statisticsBusiness.GetWeeklyRevenueAsync(from, to);
        return Ok(data);
    }
    [HttpGet("general")]
    public async Task<IActionResult> GetGeneralStatistics()
    {
        var result = await _statisticsBusiness.GetGeneralStatisticsAsync();
        return Ok(new
        {
            code = 200,
            message = "Success",
            data = result
        });
    }
    [HttpGet("sales-by-category")]
    public async Task<IActionResult> GetAllCategorySales()
    {
        var data = await _statisticsBusiness.GetAllCategorySalesAsync();
        return Ok(data);
    }

    [HttpGet("sales-by-channel")]
    public async Task<IActionResult> GetAllChannelSales()
    {
        var data = await _statisticsBusiness.GetAllChannelSalesAsync();
        return Ok(data);
    }


}
