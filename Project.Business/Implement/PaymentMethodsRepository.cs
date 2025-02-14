﻿using System;
using AutoMapper.Configuration;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.FileManagementService.Business;
using SERP.FileManagementService.Entities;
using SERP.Framework.Business;
using SERP.Framework.Common;
using SERP.Framework.Common.Extensions;
using SERP.Framework.DB.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Project.Business.Implement
{
    public class PaymentMethodsRepository : IPaymentMethodsRepository
    {
        private readonly  ProjectDbContext _context;

        public PaymentMethodsRepository(ProjectDbContext context)
        {
            _context = context;
        }
        public async Task<PaymentMethods> FindAsync(Guid id)
        {
            var res = await _context.PaymentMethods.FindAsync(id);
            return res;
        }
        public async Task<IEnumerable<PaymentMethods>> ListAllAsync(PaymentMethodsQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var resId = await query.Select(x => x.id_phuong_thuc_thanh_toan).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;
        }
        public async Task<IEnumerable<PaymentMethods>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.PaymentMethods.Where(x => ids.Contains(x.id_phuong_thuc_thanh_toan)).ToListAsync();
            return res;
        }

        public async Task<Pagination<PaymentMethods>> GetAllAsync(PaymentMethodsQueryModel queryModel)
        {
            PaymentMethodsQueryModel paymentMethodsQueryModel = queryModel;


            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<PaymentMethods> queryable = BuildQuery(queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                queryable = queryable.OrderByDescending((PaymentMethods x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value, sortExpression);
        }

        private IQueryable<PaymentMethods> BuildQuery(PaymentMethodsQueryModel queryModel)
        {
            IQueryable<PaymentMethods> query = _context.PaymentMethods.AsNoTracking().Where(x => x.Isdeleted != true);

            if (queryModel.Id.HasValue)
            {
                query = query.Where((PaymentMethods x) => x.id_phuong_thuc_thanh_toan == queryModel.Id.Value);
            }

            if (queryModel.ListId != null && queryModel.ListId.Any())
            {
                query = query.Where((PaymentMethods x) => queryModel.ListId.Contains(x.id_phuong_thuc_thanh_toan));
            }

            if (queryModel.ListTextSearch != null && queryModel.ListTextSearch.Any())
            {
                ExpressionStarter<PaymentMethods> expressionStarter = LinqKit.PredicateBuilder.New<PaymentMethods>();
                foreach (string ts in queryModel.ListTextSearch)
                {
                    expressionStarter = expressionStarter.Or((PaymentMethods p) =>
                                                                p.ma_phuong_thuc_thanh_toan.Contains(ts.ToLower()) ||
                                                                p.ten_phuong_thuc_thanh_toan.Contains(ts.ToLower()));
                }

                query = query.Where(expressionStarter);
            }

            if (!string.IsNullOrWhiteSpace(queryModel.FullTextSearch))
            {
                string fullTextSearch = queryModel.FullTextSearch.ToLower();
                query = query.Where((PaymentMethods x) => x.ma_phuong_thuc_thanh_toan.Contains(fullTextSearch));
            }

            if (!string.IsNullOrEmpty(queryModel.ma_phuong_thuc_thanh_toan))
            {
                query = query.Where(x => x.ma_phuong_thuc_thanh_toan == queryModel.ma_phuong_thuc_thanh_toan);
            }

            if (!string.IsNullOrEmpty(queryModel.ten_phuong_thuc_thanh_toan))
            {
                query = query.Where(x => x.ten_phuong_thuc_thanh_toan == queryModel.ten_phuong_thuc_thanh_toan);
            }

            if (queryModel.trang_thai >= 0)
            {
                query = query.Where(x => x.trang_thai == queryModel.trang_thai);
            }

            if (!string.IsNullOrEmpty(queryModel.create_by))
            {
                query = query.Where(x => x.create_by == queryModel.create_by);
            }

            if (!string.IsNullOrEmpty(queryModel.update_by))
            {
                query = query.Where(x => x.update_by.Contains(queryModel.update_by));
            }

            if (queryModel.create_on_date != null)
            {
                query = query.Where(x => x.create_on_date == queryModel.create_on_date);
            }


            if (queryModel.last_modifi_on_date != null)
            {
                query = query.Where(x => x.last_modifi_on_date == queryModel.last_modifi_on_date);
            }
            return query;
        }

        public async Task<int> GetCountAsync(PaymentMethodsQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }

        public async Task<PaymentMethods> SaveAsync(PaymentMethods p)
        {
            var res = await SaveAsync(new[] { p });
            return res.FirstOrDefault();

        }

        public virtual async Task<IEnumerable<PaymentMethods>> SaveAsync(IEnumerable<PaymentMethods> paymentMethods)
        {
            var updated = new List<PaymentMethods>();

            foreach (var p in paymentMethods)
            {
                var exist = await _context.PaymentMethods
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                            x.id_phuong_thuc_thanh_toan == p.id_phuong_thuc_thanh_toan
                    );

                if (exist == null)
                {
                    p.CreateTracking(p.id_phuong_thuc_thanh_toan);
                    p.UpdateTracking(p.id_phuong_thuc_thanh_toan);
                    _context.PaymentMethods.Add(p);
                    updated.Add(p);
                }
                else
                {
                    _context.Entry(exist).State = EntityState.Detached;
                    exist.ma_phuong_thuc_thanh_toan = p.ma_phuong_thuc_thanh_toan;
                    exist.ten_phuong_thuc_thanh_toan = p.ten_phuong_thuc_thanh_toan;
                    exist.trang_thai = p.trang_thai;
                    exist.create_by = p.create_by;
                    exist.update_by = p.update_by;
                    exist.create_on_date = p.create_on_date;
                    exist.last_modifi_on_date = p.last_modifi_on_date;

                    p.UpdateTracking(p.id_phuong_thuc_thanh_toan);
                    _context.PaymentMethods.Update(exist);
                    updated.Add(exist);
                }
            }
            await _context.SaveChangesAsync();

            return updated;
        }

        public async Task<PaymentMethods> DeleteAsync(Guid Id)
        {
            var exist = await FindAsync(Id);
            if (exist == null) throw new Exception(IPaymentMethodsRepository.MessageNoTFound);
            exist.Isdeleted = true;
            _context.PaymentMethods.Update(exist);
            _context.SaveChangesAsync();
            return exist;
        }

        public Task<IEnumerable<PaymentMethods>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }
    }
}


