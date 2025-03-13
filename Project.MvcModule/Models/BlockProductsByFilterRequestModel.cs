using Project.Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.MvcModule.Models
{
    public class BlockProductsByFilterRequestModel
    {
        public ProductQueryModel QueryModel { get; set; }
        public string? ViewName { get; set; }
    }
}
