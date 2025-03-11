using SERP.Framework.Entities.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Common
{
    public static  class MetadataUtil
    {
        public static string FindMetadatavalue(this List<MetaField> metadataContentObjs, string fieldName)
        {
            var value = metadataContentObjs?.Find(x => x.FieldName == fieldName)?.FieldValues;
            if (value != null) return value;
            else return string.Empty;
        }
    }
}
