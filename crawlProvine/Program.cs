using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using crawlProvine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Project.Business.Implement;
using Project.Business.Implementation;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Common;

namespace ProvinceCrawler
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Đăng ký DI container
            var services = new ServiceCollection();
            services.AddHttpClient();

            services.AddDbContext<ProjectDbContext>(options =>
       options.UseSqlServer("Server=.\\SQLEXPRESS; Database=DATN_SHOEMASTER;MultipleActiveResultSets=true;TrustServerCertificate=True;Integrated Security=True;Connection Timeout=360;"));

            services.AddScoped<IProvinceRepository, ProvinceRepository>();
            services.AddScoped<IProvinceBusiness, ProvinceBusiness>();

            services.AddTransient<ProvinceCrawlerService>();
            services.AddTransient<CrawlerService>();

            var serviceProvider = services.BuildServiceProvider();

            //ISourceCrawler crawler = serviceProvider.GetRequiredService<ProvinceCrawlerService>();
            ISourceCrawler crawler = serviceProvider.GetRequiredService<CrawlerService>();



            if (crawler != null)
            {
                await crawler.CrawlAsync();
            }
            else
            {
                Console.WriteLine("Không hợp lệ. Chọn 'province' hoặc 'district'.");
            }
        }
    }
}
