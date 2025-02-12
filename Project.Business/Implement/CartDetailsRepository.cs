using AutoMapper.Configuration;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Project.Business.Interface;
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
    public class CartDetailsRepository : ICartDetailsRepository
    {
        private readonly ProjectDbContext _context;

        public CartDetailsRepository(ProjectDbContext context)
        {
            _context = context;
        }
        public async Task<CartDetails> FindAsync(Guid id)
        {
            var res = await _context.CartDetails.FindAsync(id);
            return res;
        }
        public async Task<IEnumerable<CartDetails>> ListAllAsync(CartDetailsQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var resId = await query.Select(x => x.Id).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;
        }

        public async Task<IEnumerable<CartDetails>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.CartDetails.Where(x => ids.Contains(x.Id)).ToListAsync();
            return res;
        }

        public async Task<Pagination<CartDetails>> GetAllAsync(CartDetailsQueryModel queryModel)
        {
            CartDetailsQueryModel cartDetailsQueryModel = queryModel;


            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<CartDetails> queryable = BuildQuery(queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                queryable = queryable.OrderByDescending((CartDetails x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value, sortExpression);
        }

        private IQueryable<CartDetails> BuildQuery(CartDetailsQueryModel queryModel)
        {
            IQueryable<CartDetails> query = _context.CartDetails.AsNoTracking().Where(x => x.Isdeleted != true);

            if (queryModel.Id.HasValue)
            {
                query = query.Where((CartDetails x) => x.Id == queryModel.Id.Value);
            }

            if (queryModel.ListId != null && queryModel.ListId.Any())
            {
                query = query.Where((CartDetails x) => queryModel.ListId.Contains(x.Id));
            }

            if (queryModel.ListTextSearch != null && queryModel.ListTextSearch.Any())
            {
                ExpressionStarter<CartDetails> expressionStarter = LinqKit.PredicateBuilder.New<CartDetails>();
                foreach (string ts in queryModel.ListTextSearch)
                {
                    expressionStarter = expressionStarter.Or((CartDetails p) => p.Code.Contains(ts.ToLower()));
                }

                query = query.Where(expressionStarter);
            }

            if (queryModel.IdGioHang.HasValue)
            {
                query = query.Where(x => x.IdGioHang == queryModel.IdGioHang.Value);
            }

            if (queryModel.IdSanPham.HasValue)
            {
                query = query.Where(x => x.IdSanPham == queryModel.IdSanPham.Value);
            }

            if (queryModel.Quantity.HasValue)
            {
                query = query.Where(x => x.Quantity == queryModel.Quantity);
            }

            if (queryModel.IsOnSale.HasValue)
            {
                query = query.Where(x => x.IsOnSale == queryModel.IsOnSale);
            }

            if (!string.IsNullOrEmpty(queryModel.Code))
            {
                query = query.Where(x => x.Code.Contains(queryModel.Code));
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

        public async Task<int> GetCountAsync(CartDetailsQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }

        public async Task<CartDetails> SaveAsync(CartDetails cartDetails)
        {
            var res = await SaveAsync(new[] { cartDetails });
            return res.FirstOrDefault();

        }

        public virtual async Task<IEnumerable<CartDetails>> SaveAsync(IEnumerable<CartDetails> cartDetailses)
        {
            var updated = new List<CartDetails>();

            foreach (var cartDetails in cartDetailses)
            {
                var exist = await _context.CartDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                            x.Id == cartDetails.Id
                    );

                if (exist == null)
                {
                    cartDetails.CreateTracking(cartDetails.Id);
                    cartDetails.UpdateTracking(cartDetails.Id);
                    _context.CartDetails.Add(cartDetails);
                    updated.Add(cartDetails);
                }
                else
                {
                    _context.Entry(exist).State = EntityState.Detached;
                    exist.IdGioHang = cartDetails.IdGioHang;
                    exist.IdSanPham = cartDetails.IdSanPham;
                    exist.Quantity = cartDetails.Quantity;
                    exist.IsOnSale = cartDetails.IsOnSale;
                    exist.Code = cartDetails.Code;

                    cartDetails.UpdateTracking(cartDetails.Id);
                    _context.CartDetails.Update(exist);
                    updated.Add(exist);
                }
            }
            await _context.SaveChangesAsync();

            return updated;
        }



        public async Task<CartDetails> DeleteAsync(Guid Id)
        {
            var exist = await FindAsync(Id);
            if (exist == null) throw new Exception(ICartDetailsRepository.MessageNoTFound);
            exist.Isdeleted = true;
            _context.CartDetails.Update(exist);
            _context.SaveChangesAsync();
            return exist;
        }

        public Task<IEnumerable<CartDetails>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }
    }
}
