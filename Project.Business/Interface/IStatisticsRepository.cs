using Project.Business.Implement.Revenue;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Business.Interface
{
    public interface IStatisticsRepository
    {
        Task<List<RevenueDto>> GetRevenueAsync(DateTime from, DateTime to);
        Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync();
        Task<List<WeeklyRevenueDto>> GetWeeklyRevenueAsync(DateTime from, DateTime to);
        Task<GeneralStatisticsDto> GetGeneralStatisticsAsync();
        Task<List<CategorySalesDto>> GetAllCategorySalesAsync();
        Task<List<SalesChannelDto>> GetAllChannelSalesAsync();

    }
}
