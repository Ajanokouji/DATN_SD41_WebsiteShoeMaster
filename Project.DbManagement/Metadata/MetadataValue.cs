using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement
{
    public class MetadataValue
    {
        public virtual Guid Id { get; set; }

        public virtual string FieldName { get; set; }

        public virtual string FieldDisplayName { get; set; }

        public virtual MetafieldTypeEnum FieldType { get; set; }

        public virtual string FieldValues { get; set; }

        public virtual string FieldValueTexts { get; set; }

        public virtual string FieldValueType { get; set; }

        public virtual List<MetaKeyValue> FieldSelectionValues { get; set; }

        public virtual string PrefixValue { get; set; }

        public virtual string SuffixValue { get; set; }

        public MetadataValue()
        {
        }

        public MetadataValue(string fieldName, string fieldValues)
            : this(Guid.NewGuid(), fieldName, fieldValues, null, MetafieldTypeEnum.Text)
        {
        }

        public MetadataValue(Guid id, string fieldName, string fieldValues)
            : this(id, fieldName, fieldValues, null, MetafieldTypeEnum.Text)
        {
        }

        public MetadataValue(string fieldName, string fieldValues, MetafieldTypeEnum fieldType)
            : this(Guid.NewGuid(), fieldName, fieldValues, null, fieldType)
        {
        }

        public MetadataValue(Guid id, string fieldName, string fieldValues, MetafieldTypeEnum fieldType)
            : this(id, fieldName, fieldValues, null, fieldType)
        {
        }

        public MetadataValue(Guid id, string fieldName, string fieldValues, string fieldValueTexts, MetafieldTypeEnum fieldType)
        {
            Id = id;
            FieldName = fieldName;
            FieldValues = fieldValues;
            FieldValueTexts = fieldValueTexts;
            FieldType = fieldType;
        }
    }
}
