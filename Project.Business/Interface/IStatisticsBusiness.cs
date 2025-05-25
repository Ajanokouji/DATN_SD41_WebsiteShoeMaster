using Project.Business.Implement.Revenue;

public interface IStatisticsBusiness
{
    Task<List<RevenueDto>> GetRevenueAsync(DateTime from, DateTime to);
    Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync();
    Task<List<WeeklyRevenueDto>> GetWeeklyRevenueAsync(DateTime from, DateTime to);
    Task<GeneralStatisticsDto> GetGeneralStatisticsAsync();
    Task<List<CategorySalesDto>> GetAllCategorySalesAsync();
    Task<List<SalesChannelDto>> GetAllChannelSalesAsync();





}
