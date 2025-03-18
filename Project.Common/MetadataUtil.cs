using SERP.Framework.Entities.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Common
{
    public static class MetadataUtil
    {
        public static string GetMetadatavalue(this List<MetaField> metadataContentObjs, string fieldName)
        {
            var value = metadataContentObjs?.Find(x => x.FieldName == fieldName)?.FieldValues;
            if (value != null) return value;
            else return string.Empty;
        }

        public static string TryGetMetadatavalue(this List<MetaField> metadataContentObjs, string fieldName, out bool success)
        {
            try
            {
                var value = metadataContentObjs?.Find(x => x.FieldName == fieldName)?.FieldValues;
                success = value != null;
                return value ?? string.Empty;
            }
            catch
            {
                success = false;
                return string.Empty;
            }
        }
    }
}
