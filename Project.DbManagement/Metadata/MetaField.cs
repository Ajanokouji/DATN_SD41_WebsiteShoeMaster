using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DbManagement
{
    public class MetaField :MetadataValue
    {
        public virtual Guid Id { get; set; }
        public virtual Guid? ParentId { get; set; }

        public virtual List<string> RelatedFieldNames { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual string Placeholder { get; set; }

        public virtual string DefaultValue { get; set; }

        public virtual bool AllowNulls { get; set; }

        public virtual int SortOrder { get; set; }

        public virtual bool AllowSearch { get; set; }

        public virtual bool AllowToView { get; set; }

        public virtual bool AllowToModify { get; set; }

        public virtual bool IsMultipleLanguage { get; set; }

        public MetaField()
        {
        }

        public MetaField(string fieldName, string fieldValues)
            : base(fieldName, fieldValues)
        {
        }

        public MetaField(Guid id, string fieldName, string fieldValues)
            : base(id, fieldName, fieldValues)
        {
        }

        public MetaField(string fieldName, string fieldValues, MetafieldTypeEnum fieldType)
            : base(fieldName, fieldValues, fieldType)
        {
        }

        public MetaField(Guid id, string fieldName, string fieldValues, MetafieldTypeEnum fieldType)
            : base(id, fieldName, fieldValues, fieldType)
        {
        }

        public MetaField(Guid id, string fieldName, string fieldValues, string fieldValueTexts, MetafieldTypeEnum fieldType)
            : base(id, fieldName, fieldValues, fieldValueTexts, fieldType)
        {
        }
    }
}
