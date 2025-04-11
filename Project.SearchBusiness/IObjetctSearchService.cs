using Lucene.Net.Documents;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.SearchBusiness
{
    public  interface IObjetctSearchService
    {
        Task<Document> ConvertToObject(ProductEntity productEntity);
        Task<ProductEntity> ConvertToEntity(Document document);
    }
}
