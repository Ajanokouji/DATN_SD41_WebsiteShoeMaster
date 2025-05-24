using Dapper;
using Microsoft.Data.SqlClient;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.FileManagementService.Entities;
using SERP.Framework.Business;
using SERP.Framework.Common;
using SERP.Framework.Common.Extensions;
using SERP.Framework.DB.Extensions;
using System.Data;


namespace Project.Business.Implement
{
    public class ProductDapperRepository : ProductRepository,IProductRepository
    {
        private readonly IDbConnection _dbConnection;

        public ProductDapperRepository(IDbConnection dbConnection,ProjectDbContext context) : base(context)
        {
            _dbConnection=dbConnection;
        }

        public override async Task<ProductEntity> FindAsync(Guid id)
        {
            var query = "SELECT * FROM Products WHERE Id = @Id AND IsDeleted = 0";
            var product = await _dbConnection.QueryFirstOrDefaultAsync<ProductEntity>(query, new { Id = id });
            return product;
        }

        public override async Task<IEnumerable<ProductEntity>> ListAllAsync(ProductQueryModel queryModel)
        {
            string sortExpression = BuildSortExpression(queryModel);

            var baseIdEntities = await ListIdObjectsAsync(queryModel);
            var res = await ListByIdsAsync( baseIdEntities.Select(x => x.Id));
            res = res.AsQueryable().ApplySorting(sortExpression);
            return res;
        }
        public async Task<IEnumerable<ProductEntity>> ListIdObjectsAsync(ProductQueryModel query)
        {
            query.PageSize ??= 20;
            query.CurrentPage ??= 1;

            var conditionParameters = new Dictionary<string, object>();
            var sqlConditions = await BuildQuery(query, conditionParameters);
            string sortExpression = BuildSortExpression(query);
            var table = $"ProductS n";

            using (var conn =_dbConnection)
            {
                var listNodeObjectIds = await conn.GetListAsync<ProductEntity>(table,
                    $"n.Id ",
                    sqlConditions, conditionParameters,
                    query.CurrentPage.Value, query.PageSize.Value, sortExpression);

                return listNodeObjectIds;
            }
        }
        public override async Task<IEnumerable<ProductEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var query = "SELECT * FROM Products WHERE Id IN @Ids AND IsDeleted = 0";
            var products = await _dbConnection.QueryAsync<ProductEntity>(query, new { Ids = ids });
            return products;
        }

        public override async Task<Pagination<ProductEntity>> GetAllAsync(ProductQueryModel queryModel)
        {
            var conditionParameters = new Dictionary<string, object>();
            var sqlConditions = await BuildQuery(queryModel, conditionParameters);
            string sortExpression = BuildSortExpression(queryModel);
            var table = "Products n";

            var content = await _dbConnection.GetPagedAsync<ProductEntity>(
                table, "*", sqlConditions, conditionParameters,
                queryModel.CurrentPage.Value, queryModel.PageSize.Value, sortExpression
            );

            return content;
        }

        protected  string BuildSortExpression(PaginationRequest queryModel)
        {
            string sortExpression;
            if (string.IsNullOrWhiteSpace(queryModel.Sort))
            {
                sortExpression = $"-{nameof(NodeEntity.LastModifiedOnDate)}";
            }
            else
            {
                sortExpression = QueryUtils.FormatSortInput(queryModel.Sort);
            }

            return sortExpression;
        }



        private async Task<string> BuildQuery(ProductQueryModel queryModel,Dictionary<string,object> conditionParameters)
        {
            var query = "IsDeleted = 0";
            if (queryModel.Id.HasValue)
            {
                query += " AND Id = @Id";
                conditionParameters.TryAdd($"@Id", queryModel.Id);
            }

            if (queryModel.ListId != null && queryModel.ListId.Any())
            {
                query += " AND Id IN @ListId";
                conditionParameters.TryAdd($"@ListId", queryModel.ListId);
            }

            if (queryModel.ListTextSearch != null && queryModel.ListTextSearch.Any())
            {
                var searchConditions = queryModel.ListTextSearch.Select(ts => $"(LOWER(Name) LIKE '%{ts.ToLower()}%' OR LOWER(Description) LIKE '%{ts.ToLower()}%')");
                query += " AND (" + string.Join(" OR ", searchConditions) + ")";
            }

            if (!string.IsNullOrWhiteSpace(queryModel.FullTextSearch))
            {
                query += " AND LOWER(Name) LIKE '%@FullTextSearch%'";
            }

            if (!string.IsNullOrEmpty(queryModel.MaSanPham))
            {
                query += " AND Code = @MaSanPham";
                conditionParameters.TryAdd($"@MaSanPham", queryModel.MaSanPham);
            }

            if (!string.IsNullOrEmpty(queryModel.WorkFlowStates))
            {
                query += " AND WorkFlowStates = @WorkFlowStates";
                conditionParameters.TryAdd($"@WorkFlowStates", queryModel.WorkFlowStates);
            }

            if (queryModel.MainCategoryId.HasValue)
            {
                query += " AND MainCategoryId = @MainCategoryId";
                conditionParameters.TryAdd($"@MainCategoryId", queryModel.MainCategoryId);
            }

            if (!string.IsNullOrEmpty(queryModel.Status))
            {
                query += " AND Status = @Status";
                conditionParameters.TryAdd($"@Status", queryModel.Status);

            }

            if (!string.IsNullOrEmpty(queryModel.TenSanPham))
            {
                query += " AND Name LIKE '%@TenSanPham%'";
                conditionParameters.TryAdd($"@TenSanPham", queryModel.TenSanPham);
            }

            if (!string.IsNullOrEmpty(queryModel.Description))
            {
                query += " AND Description LIKE '%@Description%'";
                conditionParameters.TryAdd($"@Description", queryModel.Description);
            }

        //    if (queryModel.MetaDataQueries != null && queryModel.MetaDataQueries.Any())
        //{
        //    var metadataQuery = queryModel.BuildMetadataQuery<NodeEntity>(conditionParameters);
        //    if (!string.IsNullOrEmpty(metadataQuery))
        //            query += $" AND ({metadataQuery.Replace(nameof(MetadataEntity), MetadataEntityTableName)})";
        //}
            return query;
        }

        public override async Task<int> GetCountAsync(ProductQueryModel queryModel)
        {
            var conditionParameters = new Dictionary<string, object>();
            var query =  BuildQuery(queryModel, conditionParameters);
            var count = await _dbConnection.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM ({query}) AS CountQuery");
            return count;
        }


        public override async Task<ProductEntity> DeleteAsync(Guid id)
        {
            var query = "UPDATE Products SET IsDeleted = 1 WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(query, new { Id = id });
            return await FindAsync(id);
        }

        public async Task<IEnumerable<ProductEntity>> DeleteAsync(Guid[] deleteIds)
        {
            var query = "UPDATE Products SET IsDeleted = 1 WHERE Id IN @Ids";
            await _dbConnection.ExecuteAsync(query, new { Ids = deleteIds });
            return await ListByIdsAsync(deleteIds);
        }
    }
}
