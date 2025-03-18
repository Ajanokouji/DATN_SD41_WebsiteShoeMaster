using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    private static readonly string apiUrlTemplate = "https://api.sneakersapi.dev/api/v3/realtime/stockx/products/";
    private static readonly string apiKey = "sd_EWDfvBlXkZjIUoESlAhGO8qXaB6tbu5b";
    private static readonly string constr = "Server=LAPTOP-IHE70EQ6\\SQLEXPRESS; Database=DATN_SHOEMASTER;User Id=DBSET;Password=123123;MultipleActiveResultSets=true;TrustServerCertificate=True";
    private static readonly int PageSize = 50;
    private static readonly int MinPage = 1;
    private static readonly int MaxPage = 300;

    static async Task Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<ProjectDbContext>(options => options.UseSqlServer(constr))
            .AddSingleton<IProductRepository, ProductRepository>()
            .AddSingleton<IProductBusiness, ProductBusiness>()
            .BuildServiceProvider();
        var productBusiness = serviceProvider.GetService<IProductBusiness>();
        using (var conn = new SqlConnection(constr))
        {

            var totalRecord = await conn.GetCountAsync("[Products]", "", new Dictionary<string, object>());
            var totalPage = (int)Math.Ceiling(totalRecord / (double)PageSize);
            var minPage = MinPage <= totalPage ? MinPage : totalPage + 1;
            var maxPage = MaxPage <= totalPage ? MaxPage : totalPage;

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            for (int currentPage = minPage; currentPage <= maxPage; currentPage++)
            {
                var products = await conn.GetPagedAsync<ProductEntity>(
                    "Products",
                    "*",
                    "",
                    null,
                    currentPage,
                    PageSize,
                    "-MATB");

                foreach (var product in products.Content)
                {
                    try
                    {
                        string apiUrl = apiUrlTemplate + product.Id;
                        var response = await httpClient.GetAsync(apiUrl);
                        response.EnsureSuccessStatusCode();

                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var apiData = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
                        var productDetail = apiData.Data;
                        UpdateProduct(productDetail, product, productBusiness);
                        // Xử lý dữ liệu từ API (nếu cần)
                        Console.WriteLine($"Fetched data for Product ID: {product.Id}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching data for Product ID: {product.Id} - {ex.Message}");
                    }
                }
            }
        }
    }

    private static void UpdateProduct(ProductData respone, ProductEntity entity, IProductBusiness productBusiness)
    {
        entity.MainCategoryId=GuidUtils.GenerateGuidFromString(respone.ProductCategory);
        entity.MetadataObj.Add(new MetaField()
        {
            FieldName ="ProductCategory",
            FieldValues=respone.ProductCategory
        });

        entity.MetadataObj.Add(new MetaField()
        {
            FieldName ="SecondaryTitle",
            FieldValues=respone.SecondaryTitle
        });

        entity.MetadataObj.Add(new MetaField()
        {
            FieldName ="ThumbUrl",
            FieldValues=respone.Media.ThumbUrl
        });

        foreach (var item in respone.Media.All360Images) {
            entity.MediaObjs.Add(item);
        };

        productBusiness.SaveAsync(entity);
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
    public string Brand { get; set; }
    public string Id { get; set; }
    public string ListingType { get; set; }
    public MarketData Market { get; set; }
    public MediaData Media { get; set; }
    public string PrimaryTitle { get; set; }
    public string ProductCategory { get; set; }
    public string SecondaryTitle { get; set; }
    public string Title { get; set; }
    public List<Trait> Traits { get; set; }
    public string UrlKey { get; set; }
    public List<Variant> Variants { get; set; }
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
    public List<Gtin> Gtins { get; set; }
    public VariantMarket Market { get; set; }
    public VariantTraits Traits { get; set; }
}

public class Gtin
{
    public string Identifier { get; set; }
    public string Type { get; set; }
}

public class VariantMarket
{
    public MarketState State { get; set; }
}

public class VariantTraits
{
    public string Size { get; set; }
}
