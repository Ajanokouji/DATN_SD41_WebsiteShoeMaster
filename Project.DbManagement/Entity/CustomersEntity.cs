using Newtonsoft.Json;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.DbManagement
{
    public class CustomersEntity : BaseEntity
    {
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Code { get; set; }

        public Guid? TTTHIDMain { get; set; }

        //[NotMapped]
        //public List<Guid> TTLHRelatedIds { get; set; }

        //[Column(TypeName = "nvarchar(max)")]
        //public string? TTLHRelateIdsJson
        //{
        //    get
        //    {
        //        return TTLHRelatedIds == null ? null : JsonConvert.SerializeObject(TTLHRelatedIds);
        //    }
        //    set
        //    {
        //        if (string.IsNullOrWhiteSpace(value))
        //        {
        //            TTLHRelatedIds = null;
        //            return;
        //        }
        //        try
        //        {
        //            TTLHRelatedIds = JsonConvert.DeserializeObject<List<Guid>>(value);
        //        }
        //        catch (JsonReaderException ex)
        //        {
        //            // Handle the exception or log the error
        //            TTLHRelatedIds = null;
        //        }
        //    }
        //}

        [Column(TypeName = "nvarchar(256)")]
        public string? Name { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public string? PhoneNumber { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string? Email { get; set; }

        [Column(TypeName = "nvarchar(512)")]
        public string? Address { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Description { get; set; }

        [Column(TypeName = "nvarchar(128)")]
        public string? UserName { get; set; }
        public bool? IsAnonymous { get; set; } = true;

        [NotMapped]
        public int TotalOrders { get; set; }

        [NotMapped]
        public decimal TotalSpent { get; set; }

        [NotMapped]
        public DateTime? LastOrderDate { get; set; }
    }
}
