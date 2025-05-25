using SERP.Framework.Entities.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project.DbManagement.Entity
{
    public class CategoriesEntity : BaseEntity
    {
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(1024)")]
        public string Description { get; set; }

        public Guid? ParentId { get; set; }

        public int SortOrder { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Type { get; set; }

        [Column(TypeName = "nvarchar(512)")]
        public string ParentPath { get; set; }

        [Column(TypeName = "nvarchar(128)")]
        public string Code { get; set; }

        [Column(TypeName = "nvarchar(128)")]
        public string CompleteCode { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string CompleteName { get; set; }

        [Column(TypeName = "nvarchar(512)")]
        public string CompletePath { get; set; }
    }
}
