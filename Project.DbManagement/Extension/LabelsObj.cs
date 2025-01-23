using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement.Extension
{
    public class LabelsObj
    {
        public string ObjectId { get; set; }

        public string ObjectCode { get; set; }

        public string ObjectName { get; set; }

        public string Color { get; set; }

        public LabelsObj()
        {
        }

        public LabelsObj(string objectId, string objectCode, string objectName)
        {
            ObjectId = objectId;
            ObjectCode = objectCode;
            ObjectName = objectName;
        }

        public LabelsObj(string objectId, string objectCode, string objectName, string color)
        {
            ObjectId = objectId;
            ObjectCode = objectCode;
            ObjectName = objectName;
            Color = color;
        }
    }
}
