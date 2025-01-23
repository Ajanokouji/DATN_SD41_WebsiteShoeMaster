namespace Project.DbManagement.Extension
{
    public class RelatedObj
    {
        public string ObjectId { get; set; }

        public string ObjectCode { get; set; }

        public string ObjectName { get; set; }

        public string ObjectType { get; set; }

        public virtual string ObjectKeyword { get; set; }

        public virtual string ObjectContent { get; set; }

        public RelatedObj()
        {
        }

        public RelatedObj(string objectId, string objectType)
        {
            ObjectId = objectId;
            ObjectType = objectType;
        }

        public RelatedObj(string objectId, string objectCode, string objectName, string objectType)
        {
            ObjectId = objectId;
            ObjectCode = objectCode;
            ObjectName = objectName;
            ObjectType = objectType;
        }

        public RelatedObj(string objectId, string objectCode, string objectName, string objectType, string objectKeyword)
        {
            ObjectId = objectId;
            ObjectCode = objectCode;
            ObjectName = objectName;
            ObjectType = objectType;
            ObjectKeyword = objectKeyword;
        }
    }
}
