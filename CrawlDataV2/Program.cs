using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project.Business.Implement;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using SERP.Framework.Entities.Enums;
using SERP.Framework.Entities.Metadata;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    private static readonly string apiUrlTemplate = "https://api.sneakersapi.dev/api/v3/stockx/products?display%5Btraits%5D=true&display%5Bvariants%5D=true&display%5Bsizes%5D=true&page={0}&limit=20";
    private static readonly string apiKey = "sd_EWDfvBlXkZjIUoESlAhGO8qXaB6tbu5b";
    private static readonly int maxLimit = 300;

    static async Task Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<ProjectDbContext>(options => options.UseSqlServer(@"Server=LAPTOP-IHE70EQ6\SQLEXPRESS; Database=DATN_SHOEMASTER;User Id=DBSET;Password=123123;MultipleActiveResultSets=true;TrustServerCertificate=True"))
            .AddSingleton<IProductRepository, ProductRepository>() // Assuming ProductRepository is implemented
            .AddSingleton<IProductBusiness, ProductBusiness>()
            .BuildServiceProvider();

        var productBusiness = serviceProvider.GetService<IProductBusiness>();

        int page = 1;


        while (page <= maxLimit)
        {
            string apiUrl = string.Format(apiUrlTemplate, page);

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", apiKey);
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
            
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            page++;

        }
    }

 

  
}
