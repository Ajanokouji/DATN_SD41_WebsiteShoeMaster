using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Project.MVC.ImageExtractor  // đổi ProjectName thành tên thật của bạn
{
    public static class HtmlImageExtractor
    {
        public static List<string> ExtractImageSources(string html, int maxImages = 5)
        {
            var imgSrcs = new List<string>();
            if (string.IsNullOrEmpty(html)) return imgSrcs;

            var matches = Regex.Matches(html, "<img[^>]+src=[\"']([^\"']+)[\"'][^>]*>", RegexOptions.IgnoreCase);
            foreach (Match match in matches)
            {
                if (imgSrcs.Count >= maxImages) break;
                imgSrcs.Add(match.Groups[1].Value);
            }
            return imgSrcs;
        }
    }
}
