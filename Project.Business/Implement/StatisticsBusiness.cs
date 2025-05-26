using Project.Business.Implement.Revenue;
using Project.Business.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Business.Implement.Revenue
{
    public class StatisticsBusiness : IStatisticsBusiness
    {
        private readonly IStatisticsRepository _statisticsRepository;

        public StatisticsBusiness(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }

        public async Task<List<RevenueDto>> GetRevenueAsync(DateTime from, DateTime to)
        {
            // Logic nghiệp vụ nếu có, ví dụ validate tham số
            if (from > to)
            {
                throw new ArgumentException("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc");
            }
            return await _statisticsRepository.GetRevenueAsync(from, to);
        }

        public async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync()
        {
            return await _statisticsRepository.GetMonthlyRevenueAsync();
        }

        public async Task<List<WeeklyRevenueDto>> GetWeeklyRevenueAsync(DateTime from, DateTime to)
        {
            if (from > to)
            {
                throw new ArgumentException("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc");
            }
            return await _statisticsRepository.GetWeeklyRevenueAsync(from, to);
        }

        public async Task<GeneralStatisticsDto> GetGeneralStatisticsAsync()
        {
            return await _statisticsRepository.GetGeneralStatisticsAsync();
        }

        public Task<List<CategorySalesDto>> GetAllCategorySalesAsync()
        {
            return _statisticsRepository.GetAllCategorySalesAsync();
        }

        public Task<List<SalesChannelDto>> GetAllChannelSalesAsync()
        {
            return _statisticsRepository.GetAllChannelSalesAsync();
        }

    }
}
