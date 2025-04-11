using Lucene.Net.Documents;
using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using SERP.SearchService.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.SearchBusiness
{
    public interface IProductSearchService 
    {
        Task<Pagination<SearchDocumentResponseModel>> FilterAsync(ProductQueryModel model);
        Task<IEnumerable<string>> IndexAsync(IEnumerable<ProductEntity> productEntities);
        Task<string> DeleteAsync( string id);
        Task<IEnumerable<string>> DeleteManyAsync( IEnumerable<string> documentIds);

    }
}
