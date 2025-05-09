using System;
using System.Net.Http;
using System.Threading.Tasks;
using crawlProvine;
using Newtonsoft.Json;
using Project.Business.Interface;
using SERP.Framework.Common;

public class DistrictCrawlerService : ISourceCrawler
{
    private readonly HttpClient _httpClient;
    private readonly IDistrictBusiness _districtBusiness;
    public DistrictCrawlerService(HttpClient httpClient, IDistrictBusiness districtBusiness)
    {
        _httpClient = httpClient;
        _districtBusiness = districtBusiness;
    }

    public async Task CrawlAsync()
    {
        string url = "https://vn-public-apis.fpo.vn/districts/getAll?limit=-1";
        var response = await _httpClient.GetStringAsync(url);
        var result = JsonConvert.DeserializeObject<ApiResponse<DistrictDTO>>(response);

        if (result?.ExitCode == 1 && result.Data?.Items != null)
        {
            Console.WriteLine($"Đã lấy được {result.Data.Items.Count} quận/huyện:");
            foreach (var district in result.Data.Items)
            {
                Console.WriteLine($"- {district.NameWithType} (Mã: {district.Code}) - Tỉnh mã: {district.ParentCode}");
                try
                {
                    await  _districtBusiness.SaveAsync(new District
                    {
                        Id = GuidUtils.GenerateGuidFromString(district.Id),
                        Name = district.Name,
                        Slug = district.Slug,
                        Type = district.Type,
                        NameWithType = district.NameWithType,
                        Code = district.Code,
                        Path = district.Path,
                        PathWithType = district.PathWithType,
                        ParentCode = district.ParentCode
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving district {district.NameWithType}: {ex.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine("Không lấy được dữ liệu quận/huyện.");
        }
    }
}
