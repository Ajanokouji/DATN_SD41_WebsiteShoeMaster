using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Project.Business.Implement;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using SERP.SearchService.Entities;
using System.Data;

namespace Project.SearchBusiness
{
    public class ProductSearchService : ProductDapperRepository, IProductSearchService, IProductRepository
    {
        private readonly ISearchConfigResolver _searchConfigResolver;
        private readonly IObjetctSearchService _objetctSearchService;

        public ProductSearchService(IDbConnection dbConnection, ProjectDbContext context) : base(dbConnection, context)
        {
        }

        private async Task<FSDirectory> GetIndexDirectoryAsync()
        {
            var setting = await _searchConfigResolver.GetSetting();
            return FSDirectory.Open(new DirectoryInfo(setting.IndexPath));
        }

        public async Task<Pagination<SearchDocumentResponseModel>> FilterAsync(ProductQueryModel model)
        {

            var indexDir = await GetIndexDirectoryAsync();
            var analyzer = new NonUnicodeVietnameseAnalyzer(LuceneVersion.LUCENE_48);
            var reader = DirectoryReader.Open(indexDir);


            var searcher = new IndexSearcher(reader);

            var query = new TermQuery(new Term("phrase", "old"));
            TopDocs topDocs = searcher.Search(query, n: 3);

            int hits = topDocs.TotalHits;
            Console.WriteLine($"Matching results: {hits}");

            foreach (var sdoc in topDocs.ScoreDocs)
            {
                Document mdoc = searcher.Doc(sdoc.Doc);
                Console.WriteLine(mdoc.Get("phrase"));
            }

            return null;
        }

        public async Task<Query> BuildQuery(ProductQueryModel queryModel )
        {
            var res = new BooleanQuery();

            if(!string.IsNullOrEmpty(queryModel.MaSanPham))
            {
                var termQuery = new TermQuery(new Term("Code", queryModel.MaSanPham));
                res.Add(termQuery, Occur.MUST);
            }

            if (!string.IsNullOrEmpty(queryModel.TenSanPham))
            {
                var termQuery = new TermQuery(new Term("Name", queryModel.TenSanPham));
                res.Add(termQuery, Occur.MUST);
            }

            if (!string.IsNullOrEmpty(queryModel.Status))
            {
                var termQuery = new TermQuery(new Term("Status", queryModel.Status));
                res.Add(termQuery, Occur.MUST);
            }

            if (!string.IsNullOrEmpty(queryModel.Type))
            {
                var termQuery = new TermQuery(new Term("Type", queryModel.Type));
                res.Add(termQuery, Occur.MUST);
            }

            if (queryModel.MainCategoryId.HasValue)
            {
                var termQuery = new TermQuery(new Term("MainCategoryId", queryModel.MainCategoryId.ToString()));
                res.Add(termQuery, Occur.MUST);
            }

            if (!string.IsNullOrEmpty(queryModel.WorkFlowStates))
            {
                var termQuery = new TermQuery(new Term("WorkFlowStates", queryModel.WorkFlowStates));
                res.Add(termQuery, Occur.MUST);
            }

            if (queryModel.PublicOnDate.HasValue)
            {
                var termQuery = new TermQuery(new Term("PublicOnDate", queryModel.PublicOnDate.Value.ToString("yyyy-MM-dd")));
                res.Add(termQuery, Occur.MUST);
            }

            if (!string.IsNullOrEmpty(queryModel.Description))
            {
                var termQuery = new TermQuery(new Term("Description", queryModel.Description));
                res.Add(termQuery, Occur.MUST);
            }

            if (!string.IsNullOrEmpty(queryModel.Status))
            {
                var termQuery = new TermQuery(new Term("Status", queryModel.Status));
                res.Add(termQuery, Occur.MUST);
            }

            if (queryModel.PropertySearch!= null&&queryModel.PropertySearch.Any())
            {
                
            }

            return res;
        }
        public async Task<IEnumerable<string>> IndexAsync(IEnumerable<ProductEntity>  productEntities)
        {

            var indexDir = await GetIndexDirectoryAsync();
            const LuceneVersion ver = LuceneVersion.LUCENE_48;
            using var analyzer = new NonUnicodeVietnameseAnalyzer(ver);

            // to do IndexService
            var idxCfg = new IndexWriterConfig(ver, analyzer);
            idxCfg.OpenMode = OpenMode.CREATE;
            using var writer = new IndexWriter(indexDir, idxCfg);

            foreach(var prod in productEntities)
            {
                // to do convert to object
                var searchDoc = await _objetctSearchService.ConvertToObject(prod);
                writer.AddDocument(searchDoc);
            }
            writer.Commit();


            throw new NotImplementedException();
        }


        public Task<string> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> DeleteManyAsync(IEnumerable<string> documentIds)
        {
            throw new NotImplementedException();
        }
    }
}
