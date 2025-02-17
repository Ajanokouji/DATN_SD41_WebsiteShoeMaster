using AutoMapper.Configuration;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.FileManagementService.Business;
using SERP.FileManagementService.Entities;
using SERP.Framework.Business;
using SERP.Framework.Common;
using SERP.Framework.Common.Extensions;
using SERP.Framework.DB.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace Project.Business.Implement
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProjectDbContext _context;

        public ProductRepository(ProjectDbContext context)
        {
            _context=context;
        }
        public async Task<ProductEntity> FindAsync(Guid id)
        {
            var res = await _context.Products.FindAsync(id);
            return res;
        }
        public async Task<IEnumerable<ProductEntity>> ListAllAsync(ProductQueryModel queryModel)
        {
            var query = BuildQuery( queryModel);
            var resId = await query.Select(x => x.Id).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;
        }

        public async Task<IEnumerable<ProductEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.Products.Where(x => ids.Contains(x.Id)).ToListAsync();
            return res;
        }

        public async Task<Pagination<ProductEntity>> GetAllAsync(ProductQueryModel queryModel)
        {
            ProductQueryModel productQueryModel = queryModel;


            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<ProductEntity> queryable = BuildQuery( queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                queryable = queryable.OrderByDescending((ProductEntity x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value, sortExpression);
        }

        private IQueryable<ProductEntity> BuildQuery( ProductQueryModel queryModel)
        {
            IQueryable<ProductEntity> query = _context.Products.AsNoTracking().Where(x => x.Isdeleted!=true);

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
                query = query.Where(x => x.Code==queryModel.MaSanPham);
            }

            if (!string.IsNullOrEmpty(queryModel.WorkFlowStates))
            {
                query = query.Where(x => x.WorkFlowStates==queryModel.WorkFlowStates);
            }

            if (queryModel.DanhMucId.HasValue)
            {
                query = query.Where(x => x.MainCategoryId == queryModel.DanhMucId.Value);
            }

            if (!string.IsNullOrEmpty(queryModel.Status))
            {
                query = query.Where(x => x.Status==queryModel.Status);
            }

            if (!string.IsNullOrEmpty(queryModel.TenSanPham))
            {
                query = query.Where(x => x.Name.Contains(queryModel.TenSanPham));
            }

            if (!string.IsNullOrEmpty(queryModel.Description))
            {
                query = query.Where(x => x.Description.Contains(queryModel.Description));
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

        public async Task<int> GetCountAsync(ProductQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }

        public async Task<ProductEntity> SaveAsync(ProductEntity product)
        {
            var res = await SaveAsync(new [] { product });
            return res.FirstOrDefault();

        }

        public virtual async Task<IEnumerable<ProductEntity>> SaveAsync( IEnumerable<ProductEntity>  productEntities)
        {
            var updated = new List<ProductEntity>();

            foreach (var product in productEntities)
            {
                var exist = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                            x.Id==product.Id
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
                    exist.ImageUrl= product.ImageUrl;
                    exist.Name = product.Name;  
                    exist.Code=product.Code;
                    exist.MainCategoryId=product.MainCategoryId;
                    exist.CompletePath=product.CompletePath;
                    exist.CompleteName=product.CompleteName;
                    exist.CompleteCode=product.CompleteCode;
                    exist.CreatedByUserId=product.CreatedByUserId;
                    exist.LastModifiedByUserId=product.LastModifiedByUserId;
                    exist.RelatedObjectIds=product.RelatedObjectIds;
                    exist.MetadataObj=product.MetadataObj;
                    exist.SortOrder=product.SortOrder;
                    exist.LabelsObjs=product.LabelsObjs;
                    exist.CreatedOnDate=exist.CreatedOnDate;
                    exist.PublicOnDate=product.PublicOnDate;
                    exist.Status=product.Status;
                    exist.LastModifiedOnDate=product.LastModifiedOnDate;
                    exist.WorkFlowStates=product.WorkFlowStates;
                    exist.CreatedOnDate=product.CreatedOnDate;
                   exist.Description = product.Description;

                    product.UpdateTracking(product.Id);
                    _context.Products.Update(exist);
                    updated.Add(exist);
                }
            }
            await _context.SaveChangesAsync();

            return updated;
        }



        public async Task<ProductEntity> DeleteAsync(Guid Id)
        {
            var exist = await FindAsync(Id);
            if (exist==null) throw new Exception(IProductRepository.MessageNoTFound);
            exist.Isdeleted=true;
            _context.Products.Update(exist);
            _context.SaveChangesAsync();
            return exist;
        }

        public Task<IEnumerable<ProductEntity>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }

       
    }
}
