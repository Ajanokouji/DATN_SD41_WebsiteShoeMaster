using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Project.Business.Implement;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.DbManagement;
using Project.DbManagement.Entity;
using Project.DbManagement.Migrations;
using SERP.Framework.Common;
using SERP.Framework.DB.Extensions;
using SERP.Framework.Entities.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    private static readonly string apiUrlTemplate = "https://api.sneakersapi.dev/api/v3/stockx/products/";
    private static readonly string apiKey = "sd_EWDfvBlXkZjIUoESlAhGO8qXaB6tbu5b";
    private static readonly string constr = "Server=LAPTOP-IHE70EQ6\\SQLEXPRESS; Database=DATN_SHOEMASTER;User Id=sa;Password=123123;MultipleActiveResultSets=true;TrustServerCertificate=True;Connection Timeout=60;";
    private static readonly int PageSize = 50;
    private static readonly int MinPage = 1;
    private static readonly int MaxPage = 300;
    private static  int index = 1;

    static async Task Main()
    {
        var serviceProvider = new ServiceCollection()
    .AddDbContext<ProjectDbContext>(options => options.UseSqlServer(constr))
    .AddSingleton<IProductRepository, ProductRepository>()
    .AddMemoryCache() // Đăng ký đúng
    .AddSingleton<IProductBusiness, ProductBusiness>()
    .BuildServiceProvider();
        var productBusiness = serviceProvider.GetService<IProductBusiness>();
        using (var conn = new SqlConnection(constr))
        {

            var totalRecord = await conn.GetCountAsync("[Products]", "", new Dictionary<string, object>());
            var totalPage = (int)Math.Ceiling(totalRecord / (double)PageSize);
            var minPage = MinPage <= totalPage ? MinPage : totalPage + 1;
            var maxPage = MaxPage <= totalPage ? MaxPage : totalPage;


            for (int currentPage = minPage; currentPage <= maxPage; currentPage++)
            {
                var products = await conn.GetPagedAsync<ProductEntity>(
                    "Products",
                    "*",
                    "",
                    null,
                    currentPage,
                    PageSize,
                    "-Id");

                foreach (var product in products.Content)
                {
                    try
                    {

                        using (HttpClient client = new HttpClient())
                        {
                            string apiUrl = apiUrlTemplate+product.Id;
                            client.DefaultRequestHeaders.Add("Authorization", apiKey);
                            HttpResponseMessage response = await client.GetAsync(apiUrl);

                            if (response.IsSuccessStatusCode)
                            {
                                var jsonResponse = await response.Content.ReadAsStringAsync();
                                var apiData = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
                                var productDetail = apiData.Data;
                                await UpdateProduct(productDetail, product, productBusiness);
                                // Xử lý dữ liệu từ API (nếu cần)
                                Console.WriteLine($"Fetched data for Product ID: {product.Id} {index++}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching data for Product ID: {product.Id} - {ex.Message}");
                    }
                }
            }
        }
    }

    private static async Task UpdateProduct(ProductData respone, ProductEntity entity, IProductBusiness productBusiness)
    {
        entity.MetadataObj.Add(new MetaField()
        {
            FieldName ="Category",
            FieldValues=respone.Category??string.Empty
        });

        entity.MetadataObj.Add(new MetaField()
        {
            FieldName ="SecondaryCategory",
            FieldValues=respone.SecondaryCategory??string.Empty
        });

        if ((respone.gallery_360!=null&&respone.gallery_360.Count>0)||(respone.Gallery!=null&&respone.Gallery.Count>0)) {
            entity.MediaObjs = new List<string>();
            foreach (var item in (respone.gallery_360?.Count > 0 ? respone.gallery_360 : respone.gallery_360))
            {
                entity.MediaObjs?.Add(item);
            }
            ;
        }
        entity.VariantObjs = new List<Project.DbManagement.Entity.Variant>();
        if (respone.variants!=null &&respone.variants.Count > 0)
        {
            
            foreach (var variant in respone.variants)
            {
                entity.VariantObjs.Add(new Project.DbManagement.Entity.Variant()
                {
                    Id = variant.Id,
                    ProductId= variant.ProductId,
                    Size = variant.Size,
                    SizeType = variant.SizeType,
                });
            }
        }

         await productBusiness.SaveAsync(entity);
    }
}

public class ApiResponse
{
    public string Status { get; set; }
    public QueryData Query { get; set; }
    public ProductData Data { get; set; }
}

public class QueryData
{
    public string Country { get; set; }
    public string Currency { get; set; }
}

public class ProductData
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string Sku { get; set; }
    public string Slug { get; set; }
    public string Category { get; set; }
    public string SecondaryCategory { get; set; }
    public List<string> Gallery { get; set; }
    public List<string> gallery_360 { get; set; }
    public decimal MinPrice { get; set; }
    public decimal AvgPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public int Rank { get; set; }
    public int WeeklyOrders { get; set; }
    public List<Trait> Traits { get; set; }
    public List<Variant> variants { get; set; }
}

public class MarketData
{
    public SalesInformation SalesInformation { get; set; }
    public MarketState State { get; set; }
}

public class SalesInformation
{
    public int LastSale { get; set; }
    public int SalesLast72Hours { get; set; }
}

public class MarketState
{
    public PriceData HighestBid { get; set; }
    public PriceData LowestAsk { get; set; }
    public int NumberOfAsks { get; set; }
    public int NumberOfBids { get; set; }
}

public class PriceData
{
    public int? Amount { get; set; }
}

public class MediaData
{
    public List<string> All360Images { get; set; }
    public List<string> Gallery { get; set; }
    public string ThumbUrl { get; set; }
}

public class Trait
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class Variant
{
    public string Id { get; set; }
    public string ProductId { get; set; }
    public string Size { get; set; }
    public string SizeType { get; set; }
    public SubtotalPrices Subtotal { get; set; }
    public List<SizeInfo> Sizes { get; set; }
}

public class SubtotalPrices
{
    public decimal EXPRESS_EXPEDITED { get; set; }
    public decimal EXPRESS_STANDARD { get; set; }
    public decimal STANDARD { get; set; }
}

public class SizeInfo
{
    public string VariantId { get; set; }
    public string Size { get; set; }
    public string SizeType { get; set; }
}

public class Ask
{
    public string VariantId { get; set; }
    public decimal Amount { get; set; }
    public int Count { get; set; }
    public DateTime CreatedAt { get; set; }
}