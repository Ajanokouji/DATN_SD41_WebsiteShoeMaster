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
                        string responseData = await response.Content.ReadAsStringAsync();
                        ApiResponse? apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        Console.WriteLine("Fetched Products:");
                        foreach (var product in apiResponse?.Data ?? new List<Product>())
                        {
                            await SaveEntity(product, productBusiness);
                        }
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

            page ++;

        }
    }

    private static async Task SaveEntity(Product product, IProductBusiness productBusiness)
    {
        var saveEntity = new ProductEntity()
        {
            Id = product.Id,
            Code = product.Sku,
            Name = product.Title,
            ImageUrl = product.Image,
            MainCategoryId = GuidUtils.GenerateGuidFromString(product.Category),
            Status = (StatusEnum.PublishedApproved).ToString(),
            CreatedOnDate = product.Created_at,
            LastModifiedOnDate = product.Updated_at,
            MetadataObj = new List<MetaField>()
        };
        saveEntity.MetadataObj.Add(new MetaField()
        {
            FieldName = "MinPrice",
            FieldValues = product?.min_price.ToString() ?? string.Empty,
        });
        saveEntity.MetadataObj.Add(new MetaField()
        {
            FieldName = "MaxPrice",
            FieldValues = product?.max_price.ToString() ?? string.Empty,
        });
        saveEntity.MetadataObj.Add(new MetaField()
        {
            FieldName = "AvgPrice",
            FieldValues = product?.avg_price.ToString() ?? string.Empty,
        });

        saveEntity.MetadataObj.Add(new MetaField()
        {
            FieldName = "Brand",
            FieldValues = product?.Brand ?? string.Empty
        });

        saveEntity.MetadataObj.Add(new MetaField()
        {
            FieldName = "Model",
            FieldValues = product?.Model ?? string.Empty
        });

        saveEntity.MetadataObj.Add(new MetaField()
        {
            FieldName = "Colorway",
            FieldValues = product.Traits?.FirstOrDefault(x => x.Trait == "Colorway")?.Value.ToString() ?? string.Empty
        });

        saveEntity.MetadataObj.Add(new MetaField()
        {
            FieldName = "Featured",
            FieldValues = product.Traits?.FirstOrDefault(x => x.Trait == "Featured")?.Value.ToString() ?? string.Empty
        });

        saveEntity.MetadataObj.Add(new MetaField()
        {
            FieldName = "Release Date",
            FieldValues = product.Traits?.FirstOrDefault(x => x.Trait == "Release Date")?.Value.ToString() ?? string.Empty
        });

        await productBusiness.SaveAsync(saveEntity);
    }



    public class ApiResponse
    {
        public string Status { get; set; } = string.Empty;
        public Query Query { get; set; } = new Query();
        public List<Product> Data { get; set; } = new List<Product>();
        public Meta Meta { get; set; } = new Meta();
    }

    public class Query
    {
        public string DisplayTraits { get; set; } = string.Empty;
        public string DisplayVariants { get; set; } = string.Empty;
        public string QueryText { get; set; } = string.Empty;
    }

    public class Product
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Brand { get; set; } = string.Empty;
        public string? Model { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Category { get;  set; }
        public string Product_type { get; set; } = string.Empty;
        public float min_price { get; set; }
        public float max_price { get; set; }
        public float avg_price { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        public string SecondaryCategory { get; set; } = string.Empty;
        public List<TraitModel> Traits { get; set; } = new List<TraitModel>();
        public List<Variant> Variants { get; set; } = new List<Variant>();
    }

    public class TraitModel
    {
        public string ProductId { get; set; } = string.Empty;
        public string Trait { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class Variant
    {
        public string Id { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string SizeType { get; set; } = string.Empty;
        public decimal LowestAsk { get; set; }
        public int TotalAsks { get; set; }
    }

    public class Meta
    {
        public int CurrentPage { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
    }
}
