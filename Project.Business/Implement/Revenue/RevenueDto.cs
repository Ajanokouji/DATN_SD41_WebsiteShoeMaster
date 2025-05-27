using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Implement.Revenue
{
    public class RevenueDto
    {
        public DateTime Date { get; set; }          // Ngày
        public decimal TotalRevenue { get; set; }   // Doanh thu trong ngày đó
    }
    public class MonthlyRevenueDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class WeeklyRevenueDto
    {
        public int Year { get; set; }
        public int Week { get; set; }
        public decimal TotalRevenue { get; set; }
    }
    public class GeneralStatisticsDto
    {
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public double OrderSuccessRate { get; set; } // Tỷ lệ thành công (%)
        public int TotalCompletedOrders { get; set; }
        public decimal TotalRevenue { get; set; }     
    }
    public class CategorySalesDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TotalQuantitySold { get; set; }
    }

    public class SalesChannelDto
    {
        public int Source { get; set; } // 1: online, 2: offline
        public int TotalQuantitySold { get; set; }
    }


}
