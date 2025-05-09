using Newtonsoft.Json;
using ProvinceCrawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawlProvine
{
    public class Data<T>
    {
        [JsonProperty("nItems")]
        public int NItems { get; set; }

        [JsonProperty("nPages")]
        public int NPages { get; set; }

        [JsonProperty("data")]
        public List<T> Items { get; set; }
    }
}
