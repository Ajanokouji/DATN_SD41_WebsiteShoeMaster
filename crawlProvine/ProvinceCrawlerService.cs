using System;
using System.Net.Http;
using System.Threading.Tasks;
using crawlProvine;
using Newtonsoft.Json;

public class ProvinceCrawlerService : ISourceCrawler
{
    private readonly HttpClient _httpClient;

    public ProvinceCrawlerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task CrawlAsync()
    {
        string url = "https://vn-public-apis.fpo.vn/provinces/getAll?limit=-1";
        var response = await _httpClient.GetStringAsync(url);
        var result = JsonConvert.DeserializeObject<ApiResponse<ProvinceDTO>>(response);

        if (result?.ExitCode == 1 && result.Data?.Items != null)
        {
            Console.WriteLine($"Đã lấy được {result.Data.Items.Count} tỉnh/thành:");
            foreach (var province in result.Data.Items)
            {
                Console.WriteLine($"- {province.NameWithType} (Mã: {province.Code})");
            }
        }
        else
        {
            Console.WriteLine("Không lấy được dữ liệu tỉnh/thành.");
        }
    }
}
