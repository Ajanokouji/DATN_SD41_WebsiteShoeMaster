using Newtonsoft.Json;
using ProvinceCrawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace crawlProvine
{
    public class ApiResponse<T>
    {
        [JsonProperty("exitcode")]
        public int ExitCode { get; set; }

        [JsonProperty("data")]
        public Data<T> Data { get; set; }
    }
}
