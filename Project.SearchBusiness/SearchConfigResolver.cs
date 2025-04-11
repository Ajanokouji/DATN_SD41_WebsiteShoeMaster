using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.SearchBusiness
{
    public class SearchConfigResolver : ISearchConfigResolver
    {
        private readonly IConfiguration _configuration;

        public SearchConfigResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<SearchSetting> GetSetting()
        {
            var setting = new SearchSetting();

            setting.IndexPath = _configuration["SearchSettings:IndexPath"];
            setting.IndexName = _configuration["SearchSettings:IndexName"];

            return setting;
        }
    }
}
