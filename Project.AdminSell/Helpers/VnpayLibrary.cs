using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Project.AdminSell.Helpers;

public class VnpayLibrary
{
    private readonly SortedList<string, string> requestData = new SortedList<string, string>();

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            requestData.Add(key, value);
        }
    }

    public string CreateRequestUrl(string baseUrl, string hashSecret)
    {
        StringBuilder data = new StringBuilder();
        foreach (var kv in requestData)
        {
            data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
        }

        string queryString = data.ToString().TrimEnd('&');
        string signData = string.Join("&", requestData.Select(kv => $"{kv.Key}={kv.Value}"));
        string vnp_SecureHash = HmacSHA512(hashSecret, signData);
        return baseUrl + "?" + queryString + "&vnp_SecureHash=" + vnp_SecureHash;
    }

    private string HmacSHA512(string key, string inputData)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}