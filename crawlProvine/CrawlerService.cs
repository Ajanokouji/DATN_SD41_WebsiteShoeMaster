using System;
using System.Net.Http;
using System.Threading.Tasks;
using crawlProvine;
using Newtonsoft.Json;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.DbManagement.Entity;
using Project.DbManagement.Enum;
using SERP.Framework.Common;

public class CrawlerService : ISourceCrawler
{
    private readonly HttpClient _httpClient;
    private readonly IProvinceRepository  _provinceRepository;
    public CrawlerService(HttpClient httpClient, IProvinceRepository provinceRepository)
    {
        _httpClient = httpClient;
        _provinceRepository = provinceRepository;
    }

    public async Task CrawlAsync()
    {
        string url = "https://vn-public-apis.fpo.vn/districts/getAll?limit=-1";
        var response = await _httpClient.GetStringAsync(url);
        var result = JsonConvert.DeserializeObject<ApiResponse<DataDTO>>(response);

        if (result?.ExitCode == 1 && result.Data?.Items != null)
        {
            Console.WriteLine($"Đã lấy được {result.Data.Items.Count} quận/huyện:");
            foreach (var data in result.Data.Items)
            {
                Console.WriteLine($"- {data.NameWithType} (Mã: {data.Code}) - Tỉnh mã: {data.ParentCode}");
                try
                {
                    await  _provinceRepository.SaveAsync(new Province
                    {
                        Id = GuidUtils.GenerateGuidFromString(data.Id),
                        Name = data.Name,
                        Slug = data.Slug,
                        Type = data.Type,
                        NameWithType = data.NameWithType,
                        Code = data.Code,
                        Path = data.Path,
                        PathWithType = data.PathWithType,
                        ParentCode = data.ParentCode,
                        AddressType = AddressType.District
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving district {data.NameWithType}: {ex.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine("Không lấy được dữ liệu quận/huyện.");
        }
    }
}
