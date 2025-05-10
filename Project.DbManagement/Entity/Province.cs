using Newtonsoft.Json;
using Project.DbManagement.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement.Entity
{
    public class Province : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Slug { get; set; }

        public string? Type { get; set; }

        public string? ParentCode { get; set;}

        public string? NameWithType { get; set; }

        public string? Code { get; set; }

        public string? Path { get; set; }

        public string? PathWithType { get; set; }

        public AddressType? AddressType { get; set; }

    }
}

