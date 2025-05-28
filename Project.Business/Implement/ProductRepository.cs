using LinqKit;
using Microsoft.EntityFrameworkCore;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Business;
using SERP.Framework.Common;
using SERP.Framework.Common.Extensions;
using SERP.Framework.DB.Extensions;
using Microsoft.EntityFrameworkCore.SqlServer;
using Newtonsoft.Json;
using Project.Common;
using Project.DbManagement.ViewModels;

namespace Project.Business.Implement
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProjectDbContext _context;

        public ProductRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public virtual async Task<ProductEntity> FindAsync(Guid id)
        {
            var res = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return res;
        }

        public virtual async Task<IEnumerable<ProductEntity>> ListAllAsync(ProductQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var resId = await query.Select(x => x.Id).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;
        }

        public virtual async Task<IEnumerable<ProductEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.Products.Where(x => ids.Contains(x.Id)).ToListAsync();
            return res;
        }

        public virtual async Task<Pagination<ProductEntity>> GetAllAsync(ProductQueryModel queryModel)
        {
            ProductQueryModel productQueryModel = queryModel;


            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort ?? "-CreatedOnDate");
            IQueryable<ProductEntity> queryable = BuildQuery(queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                queryable = queryable.OrderByDescending((ProductEntity x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }

            //select columns
            if (queryModel.IsSelectMetadata)
            {
                queryable.Select(x => new
                    {
                        x.Id,
                        x.Code,
                        x.Name,
                        x.Status,
                        x.ImageUrl,
                        x.SortOrder,
                        x.Description,
                        x.MainCategoryId,
                        x.RelatedObjectIds,
                        x.RelatedIds,
                        x.WorkFlowStates,
                        x.PublicOnDate,
                        x.CompleteName,
                        x.CompleteCode,
                        x.CompletePath,
                        x.LabelsJson,
                        x.LabelsObjs,
                        x.CreatedByUserId,
                        x.CreatedOnDate,
                        x.LastModifiedByUserId,
                        x.LastModifiedOnDate
                    }
                );
            }

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value,
                sortExpression);
        }

        private IQueryable<ProductEntity> BuildQuery(ProductQueryModel queryModel)
        {
            IQueryable<ProductEntity> query = _context.Products.AsNoTracking().Where(x => x.IsDeleted!=true);

            if (queryModel.Id.HasValue)
            {
                query = query.Where((ProductEntity x) => x.Id == queryModel.Id.Value);
            }

            if (queryModel.ListId != null && queryModel.ListId.Any())
            {
                query = query.Where((ProductEntity x) => queryModel.ListId.Contains(x.Id));
            }

            if (queryModel.ListTextSearch != null && queryModel.ListTextSearch.Any())
            {
                ExpressionStarter<ProductEntity> expressionStarter = LinqKit.PredicateBuilder.New<ProductEntity>();
                foreach (string ts in queryModel.ListTextSearch)
                {
                    expressionStarter = expressionStarter.Or((ProductEntity p) => 
                                                                p.Name.Contains(ts.ToLower()) ||
                                                                p.Description.Contains(ts.ToLower()));
                }

                query = query.Where(expressionStarter);
            }

            if (!string.IsNullOrWhiteSpace(queryModel.FullTextSearch))
            {
                string fullTextSearch = queryModel.FullTextSearch.ToLower();
                query = query.Where((ProductEntity x) => x.Name.Contains(fullTextSearch));
            }

            if (!string.IsNullOrEmpty(queryModel.MaSanPham))
            {
                query = query.Where(x => x.Code == queryModel.MaSanPham);
            }

            if (!string.IsNullOrEmpty(queryModel.WorkFlowStates))
            {
                query = query.Where(x => x.WorkFlowStates == queryModel.WorkFlowStates);
            }

            if (queryModel.MainCategoryId.HasValue)
            {
                query = query.Where(x => x.MainCategoryId == queryModel.MainCategoryId.Value);
            }

            if (!string.IsNullOrEmpty(queryModel.Status))
            {
                query = query.Where(x => x.Status == queryModel.Status);
            }

            if (!string.IsNullOrEmpty(queryModel.TenSanPham))
            {
                query = query.Where(x => x.Name.Contains(queryModel.TenSanPham));
            }

            if (!string.IsNullOrEmpty(queryModel.Description))
            {
                query = query.Where(x => x.Description.Contains(queryModel.Description));
            }

            if (true)
            {
                query =  query.Where(x => x.MetadataObj != null && x.MetadataObj.Any(m => m.FieldName == "Brand" && m.FieldValues == "Nike"));
            }

            //if (queryModel.MetaDataQueries != null && queryModel.MetaDataQueries.Any())
            //{
            //    string text = queryModel.BuildMetadataQuery<NodeEntity>(conditionParameters);
            //    if (!string.IsNullOrEmpty(text17))
            //    {
            //        text = text + " AND (" + text17.Replace("MetadataEntity", MetadataEntityTableName) + ")";
            //    }
            //}

            //if (query.RelationQuery != null)
            //{
            //    string text18 = BuildRelationQueryExpression(query.RelationQuery, conditionParameters);
            //    if (!string.IsNullOrEmpty(text18))
            //    {
            //        text = text + " AND " + text18;
            //    }
            //}


            return query;
        }

        public virtual async Task<int> GetCountAsync(ProductQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }

        public virtual async Task<ProductEntity> SaveAsync(ProductEntity product)
        {
            var res = await SaveAsync(new[] { product });
            return res.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<ProductEntity>> SaveAsync(IEnumerable<ProductEntity> productEntities)
        {
            var updated = new List<ProductEntity>();
            try
            {
                var x = _context.Database.GetDbConnection();
                foreach (var product in productEntities)
                {
                    var local = _context.Products.Local.FirstOrDefault(x => x.Id == product.Id);
                    if (local != null)
                    {
                        _context.Entry(local).State = EntityState.Detached;
                    }
                    var exist = await _context.Products
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.Id == product.Id
                        );

                    if (exist == null)
                    {
                        product.CreateTracking(product.Id);
                        product.UpdateTracking(product.Id);
                        _context.Products.Add(product);
                        updated.Add(product);
                    }
                    else
                    {
                        _context.Entry(exist).State = EntityState.Detached;
                        exist.ImageUrl = product.ImageUrl;
                        exist.Name = product.Name;
                        exist.Code = product.Code;
                        exist.MainCategoryId = product.MainCategoryId;
                        exist.CompletePath = product.CompletePath;
                        exist.CompleteName = product.CompleteName;
                        exist.CompleteCode = product.CompleteCode;
                        exist.CreatedByUserId = product.CreatedByUserId;
                        exist.LastModifiedByUserId = product.LastModifiedByUserId;
                        exist.RelatedObjectIds = product.RelatedObjectIds;
                        exist.MetadataObj = product.MetadataObj;
                        exist.SortOrder = product.SortOrder;
                        exist.LabelsObjs = product.LabelsObjs;
                        exist.CreatedOnDate = exist.CreatedOnDate;
                        exist.PublicOnDate = product.PublicOnDate;
                        exist.Status = product.Status;
                        exist.LastModifiedOnDate = product.LastModifiedOnDate;
                        exist.WorkFlowStates = product.WorkFlowStates;
                        exist.CreatedOnDate = product.CreatedOnDate;
                        exist.Description = product.Description;
                        exist.MediaObjs = product.MediaObjs;
                        exist.MediasJson = product.MediasJson;
                        exist.VariantJson = product.VariantJson;
                        exist.VariantObjs = product.VariantObjs;

                        product.UpdateTracking(product.Id);
                        _context.Products.Update(exist);
                        updated.Add(exist);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message, ex);
            }

            return updated;
        }


        public virtual async Task<ProductEntity> DeleteAsync(Guid id)
        {
            var exist = await FindAsync(id);
            if (exist == null)
                throw new Exception(IProductRepository.MessageNoTFound);

            exist.IsDeleted = true;

            _context.Products.Update(exist); // đảm bảo EF tracking lại entity để update

            await _context.SaveChangesAsync(); // cần await

            return exist;
        }

        public virtual Task<IEnumerable<ProductEntity>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ListProductSellViewModel>> GetAllProduct()
        {
            var products = await _context.Products.AsNoTracking().ToListAsync();

            var result = products.Select(product =>
            {
                List<Variant>? variants = null;

                try
                {
                    if (!string.IsNullOrWhiteSpace(product.VariantJson))
                        variants = JsonConvert.DeserializeObject<List<Variant>>(product.VariantJson);
                }
                catch
                {
                    variants = null;
                }

                string priceRange = "0ᴠɴᴅ";

                if (variants != null && variants.Count > 0)
                {
                    var prices = variants
                        .Select(v => decimal.TryParse(v.Price, out var p) ? p : (decimal?)null)
                        .Where(p => p.HasValue)
                        .Select(p => p.Value)
                        .ToList();

                    if (prices.Count > 0)
                    {
                        var min = prices.Min();
                        var max = prices.Max();

                        priceRange = min == max 
                            ? $"{min:N0}ᴠɴᴅ"
                            : $"{min:N0}ᴠɴᴅ ~ {max:N0}ᴠɴᴅ";
                    }
                }

                return new ListProductSellViewModel
                {
                    Id = product.Id,
                    ProductCode = product.Code,
                    Image = product.ImageUrl,
                    Name = product.Name,
                    Price = priceRange
                };
            }).ToList();

            return result;
        }

        public async Task<ProductDetailsViewModel> GetProductDetailsById(Guid idprd)
        {
            var products = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == idprd);
            var lstColor = products.VariantObjs?
                .Where(x => x.Stock != 0)
                .Select(y => y.Group1)
                .Distinct()
                .ToList() ?? new List<string>();

            var lstSize = products.VariantObjs?
                .Where(x => x.Stock != 0)
                .Select(y => y.Group2)
                .Distinct()
                .ToList() ?? new List<string>();

            var result = await (from product in _context.Products.AsNoTracking()
                where product.Id == idprd
                select new ProductDetailsViewModel()
                {
                    Id = product.Id,
                    Name = product.Name,
                    lstColor = lstColor,
                    lstSize = lstSize,
                }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<ListProductDetailsViewModel>> ListProductDetailsById(Guid idprd)
        {
            // var result = await (from product in _context.Products.AsNoTracking()
            //     where product.Id == idprd
            //     select new ListProductDetailsViewModel()
            //     {
            //         Id = product.Id,
            //         Name = product.Name,
            //         Color = product.MetadataObj.GetMetadatavalue("Colorway"),
            //         Size = product.MetadataObj.GetMetadatavalue("Size"),
            //         Quantity = product.MetadataObj.GetMetadatavalue("Quantity"),
            //         Image = product.ImageUrl,
            //         Price = product.MetadataObj.GetMetadatavalue("MaxPrice")
            //     }).ToListAsync();

            var product = await _context.Products.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == idprd);

            if (product == null || product.VariantObjs == null || !product.VariantObjs.Any())
                return new List<ListProductDetailsViewModel>();

            var result = product.VariantObjs.Select(v => new ListProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Color = v.Group1,
                Size = v.Group2,
                Quantity = v.Stock?.ToString() ?? "0",
                Image = !string.IsNullOrEmpty(v.ImgUrl) ? v.ImgUrl : product.ImageUrl,
                Price = v.Price
            }).ToList();
            return result;
        }

        public async Task<List<ListProductSellViewModel>> SearchProduct(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await GetAllProduct();
            }

            keyword = keyword.ToLower();

            var products = await _context.Products
                .AsNoTracking()
                .Where(p =>
                    (p.Name.ToLower().Contains(keyword)) ||
                    (p.Code.ToLower().Contains(keyword)))
                .ToListAsync();

            var result = products.Select(product =>
            {
                List<Variant>? variants = null;

                try
                {
                    if (!string.IsNullOrWhiteSpace(product.VariantJson))
                        variants = JsonConvert.DeserializeObject<List<Variant>>(product.VariantJson);
                }
                catch
                {
                    variants = null;
                }

                string priceRange = "0ᴠɴᴅ";

                if (variants != null && variants.Count > 0)
                {
                    var prices = variants
                        .Select(v => decimal.TryParse(v.Price, out var p) ? p : (decimal?)null)
                        .Where(p => p.HasValue)
                        .Select(p => p.Value)
                        .ToList();

                    if (prices.Count > 0)
                    {
                        var min = prices.Min();
                        var max = prices.Max();

                        priceRange = min == max
                            ? $"{min:N0}ᴠɴᴅ"
                            : $"{min:N0}ᴠɴᴅ ~ {max:N0}ᴠɴᴅ";
                    }
                }

                return new ListProductSellViewModel
                {
                    Id = product.Id,
                    ProductCode = product.Code,
                    Image = product.ImageUrl,
                    Name = product.Name,
                    Price = priceRange
                };
            }).ToList();
            return result;
        }
    }
}