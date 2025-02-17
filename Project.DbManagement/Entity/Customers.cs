using Newtonsoft.Json;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement
{
    public class Customers: BaseEntity
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public Guid? TTTHIDMain { get; set; }
        [NotMapped]
        public List<Guid> TTLHRelatedIds { get; set; }

        public string? TTLHRelateIdsJson {  
            get
            {
                return TTLHRelatedIds == null ? null : string.Join(",", TTLHRelatedIds);
            }
            set { 
                  if(string.IsNullOrWhiteSpace(value))
                {
                    TTLHRelatedIds = null;
                    return;
                }
                  this.TTLHRelatedIds= JsonConvert.DeserializeObject<List<Guid>>(value);
            } }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? UserName { get; set; }
    }
}
