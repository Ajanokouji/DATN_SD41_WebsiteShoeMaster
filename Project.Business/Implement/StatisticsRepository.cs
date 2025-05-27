using Microsoft.EntityFrameworkCore;
using Project.Business.Implement.Revenue;
using Project.DbManagement;
using Project.Business.Interface;
using Project.Common.Constants;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Project.Business.Implement.Revenue
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly ProjectDbContext _context;

        public StatisticsRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<List<RevenueDto>> GetRevenueAsync(DateTime from, DateTime to)
        {
            return await _context.Bills
                .Where(b => b.CreatedOnDate.HasValue
                            && b.CreatedOnDate.Value >= from
                            && b.CreatedOnDate.Value <= to
                            && (b.PaymentStatus == BillConstants.PaymentStatusPaid
                                || b.PaymentStatus == BillConstants.PaymentStatusRefunded))
                .GroupBy(b => b.CreatedOnDate.Value.Date)
                .Select(g => new RevenueDto
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(b =>
                        b.PaymentStatus == BillConstants.PaymentStatusPaid
                            ? (b.FinalAmount ?? 0)
                            : -(b.FinalAmount ?? 0))
                })
                .OrderBy(r => r.Date)
                .ToListAsync();
        }

        public async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync()
        {
            int year = DateTime.Now.Year;

            var revenueData = await _context.Bills
                .Where(b => b.CreatedOnDate.HasValue
                            && b.CreatedOnDate.Value.Year == year
                            && (b.Status == BillConstants.Completed || b.Status == BillConstants.Returned))
                .GroupBy(b => b.CreatedOnDate.Value.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalRevenue = g.Sum(b =>
                        b.Status == BillConstants.Completed
                            ? (b.FinalAmount ?? 0)
                            : b.Status == BillConstants.Returned
                                ? -(b.FinalAmount ?? 0)
                                : 0)
                })
                .ToListAsync();

            var fullYearRevenue = Enumerable.Range(1, 12)
                .Select(month => new MonthlyRevenueDto
                {
                    Year = year,
                    Month = month,
                    TotalRevenue = revenueData.FirstOrDefault(r => r.Month == month)?.TotalRevenue ?? 0
                })
                .ToList();

            return fullYearRevenue;
        }

        public async Task<List<WeeklyRevenueDto>> GetWeeklyRevenueAsync(DateTime from, DateTime to)
        {
            var dailyRevenue = await _context.Bills
                .Where(b => b.CreatedOnDate.HasValue
                            && b.CreatedOnDate.Value >= from
                            && b.CreatedOnDate.Value <= to
                            && (b.PaymentStatus == BillConstants.PaymentStatusPaid
                                || b.PaymentStatus == BillConstants.PaymentStatusRefunded))
                .GroupBy(b => b.CreatedOnDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(b =>
                        b.PaymentStatus == BillConstants.PaymentStatusPaid
                            ? (b.FinalAmount ?? 0)
                            : -(b.FinalAmount ?? 0))
                })
                .ToListAsync();

            var cal = CultureInfo.InvariantCulture.Calendar;
            var weeklyRevenue = dailyRevenue
                .GroupBy(d =>
                {
                    var weekNum = cal.GetWeekOfYear(d.Date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    return new { Year = d.Date.Year, Week = weekNum };
                })
                .Select(g => new WeeklyRevenueDto
                {
                    Year = g.Key.Year,
                    Week = g.Key.Week,
                    TotalRevenue = g.Sum(x => x.TotalRevenue)
                })
                .OrderBy(r => r.Year)
                .ThenBy(r => r.Week)
                .ToList();

            return weeklyRevenue;
        }

        public async Task<GeneralStatisticsDto> GetGeneralStatisticsAsync()
        {
            var totalOrders = await _context.Bills
                .CountAsync(b => b.IsDeleted == false);

            var totalCustomers = await _context.Customers
                .CountAsync(c => c.IsDeleted == false);

            var totalProducts = await _context.Products
                .CountAsync(p => p.IsDeleted == false);

            var totalCompletedOrders = await _context.Bills
                .CountAsync(b => b.IsDeleted == false && b.Status == BillConstants.Completed);

            var successRate = totalOrders > 0
                ? (double)totalCompletedOrders / totalOrders * 100
                : 0;

            var totalRevenue = await _context.Bills
          .Where(b => b.IsDeleted == false)
          .SumAsync(b =>
              b.Status == BillConstants.Completed ? (b.FinalAmount ?? 0) :
              b.Status == BillConstants.Returned ? -(b.FinalAmount ?? 0) : 0
          );


            return new GeneralStatisticsDto
            {
                TotalOrders = totalOrders,
                TotalCustomers = totalCustomers,
                TotalProducts = totalProducts,
                OrderSuccessRate = Math.Round(successRate, 2),
                TotalRevenue = totalRevenue
            };
        }

        public async Task<List<CategorySalesDto>> GetAllCategorySalesAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.IsDeleted == false)
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();

            var query = from p in _context.Products
                        join bd in _context.BillDetails on p.Id equals bd.ProductId
                        join b in _context.Bills on bd.BillId equals b.Id
                        where b.Status == BillConstants.Completed
                              && b.IsDeleted == false
                              && bd.Status == 1
                              && bd.IsDeleted == false
                        group bd by p.MainCategoryId into g
                        select new
                        {
                            CategoryId = g.Key,
                            TotalQuantitySold = g.Sum(x => x.Quantity)
                        };

            var salesByCategory = await query.ToListAsync();

            var result = categories.Select(c =>
            {
                var sale = salesByCategory.FirstOrDefault(s => s.CategoryId.HasValue && s.CategoryId.Value == c.Id);
                return new CategorySalesDto
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    TotalQuantitySold = sale != null ? sale.TotalQuantitySold : 0
                };
            }).ToList();

            var knownCategoryIds = categories.Select(c => c.Id).ToHashSet();

            var othersQuantity = salesByCategory
                .Where(s => !s.CategoryId.HasValue || !knownCategoryIds.Contains(s.CategoryId.Value))
                .Sum(s => s.TotalQuantitySold);

            if (othersQuantity > 0)
            {
                result.Add(new CategorySalesDto
                {
                    CategoryId = Guid.Empty,
                    CategoryName = "Khác",
                    TotalQuantitySold = othersQuantity
                });
            }

            return result.OrderByDescending(r => r.TotalQuantitySold).ToList();
        }

        public async Task<List<SalesChannelDto>> GetAllChannelSalesAsync()
        {
            var query = from bd in _context.BillDetails
                        join b in _context.Bills on bd.BillId equals b.Id
                        where b.Status == BillConstants.Completed
                              && b.IsDeleted == false
                              && bd.Status == 1
                              && bd.IsDeleted == false
                              && ((int)b.Source == 1 || (int)b.Source == 2)
                        group bd by b.Source into g
                        select new SalesChannelDto
                        {
                            Source = (int)g.Key,
                            TotalQuantitySold = g.Sum(x => x.Quantity)
                        };

            return await query.OrderBy(x => x.Source).ToListAsync();
        }
    }
}
