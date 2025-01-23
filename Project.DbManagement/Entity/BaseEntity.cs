using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement.Entity
{
    public class BaseEntity
    {
        public virtual Guid? CreatedByUserId { get; set; }

        public virtual Guid? LastModifiedByUserId { get; set; }

        public virtual DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;


        public virtual DateTime? CreatedOnDate { get; set; } = DateTime.Now;


        public void CreateTracking(Guid userId)
        {
            CreatedOnDate = DateTime.UtcNow;
            CreatedByUserId = userId;
        }

        public void UpdateTracking(Guid userId)
        {
            LastModifiedOnDate = DateTime.UtcNow;
            LastModifiedByUserId = userId;
        }

        public void DeleteTracking(Guid userId)
        {
            LastModifiedOnDate = DateTime.UtcNow;
            LastModifiedByUserId = userId;
        }
        public bool? Isdeleted { get; set; } =false;
    }
}
