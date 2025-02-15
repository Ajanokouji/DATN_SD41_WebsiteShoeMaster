using LinqKit;
using Microsoft.EntityFrameworkCore;
using Nest;
using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Business;
using SERP.Framework.Common;
using SERP.Framework.DB.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Implement
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly ProjectDbContext _context;

        public VoucherRepository(ProjectDbContext context)
        {
                _context = context;
        }

        public async Task<Voucher> FindAsync(Guid id)
        {
            var res = await _context.Vouchers.FindAsync(id);
            return res;
        }

        public async Task<IEnumerable<Voucher>> ListAllAsync(VoucherQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var resId = await query.Select(x => x.Id).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;

        }

        public async Task<IEnumerable<Voucher>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.Vouchers.Where(x => ids.Contains(x.Id)).ToListAsync();
            return res;
        }

        public async Task<Pagination<Voucher>> GetAllAsync(VoucherQueryModel queryModel)
        {
            VoucherQueryModel voucherQueryModel = queryModel;


            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<Voucher> queryable = BuildQuery(queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                queryable = queryable.OrderByDescending((Voucher x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value, sortExpression);
        }

        private IQueryable<Voucher> BuildQuery(VoucherQueryModel queryModel)
        {
            IQueryable<Voucher> query = _context.Vouchers.AsNoTracking().Where(x => x.Isdeleted != true);

            if (queryModel.Id.HasValue)
            {
                query = query.Where((Voucher x) => x.Id == queryModel.Id.Value);
            }

            if (queryModel.ListId != null && queryModel.ListId.Any())
            {
                query = query.Where((Voucher x) => queryModel.ListId.Contains(x.Id));
            }

            if (queryModel.ListTextSearch != null && queryModel.ListTextSearch.Any())
            {
                ExpressionStarter<Voucher> expressionStarter = LinqKit.PredicateBuilder.New<Voucher>();
                foreach (string ts in queryModel.ListTextSearch)
                {
                    expressionStarter = expressionStarter.Or((Voucher p) => p.VoucherName.Contains(ts.ToLower()));
                }

                query = query.Where(expressionStarter);
            }

            if (queryModel.id_giam_gia.HasValue)
            {
                query = query.Where(x => x.Id == queryModel.id_giam_gia.Value);
            }

            if (!string.IsNullOrEmpty(queryModel.ten_giam_gia))
            {
                query = query.Where(x => x.VoucherName.Contains(queryModel.ten_giam_gia));
            }

            if (queryModel.loai_giam_gia >= 0)
            {
                query = query.Where(x => x.VoucherType == queryModel.loai_giam_gia);
            }

            if (queryModel.thoi_gian_bat_dau != null)
            {
                query = query.Where(x => x.StartDate == queryModel.thoi_gian_bat_dau);
            }

            if (queryModel.thoi_gian_ket_thuc != null)
            {
                query = query.Where(x => x.StartDate == queryModel.thoi_gian_ket_thuc);
            }

            if (queryModel.trang_thai >= 0)
            {
                query = query.Where(x => x.Status == queryModel.trang_thai);
            }

            if (queryModel.create_on_date != null)
            {
                query = query.Where(x => x.CreatedOnDate == queryModel.create_on_date);
            }

            if (queryModel.last_modifi_on_date != null)
            {
                query = query.Where(x => x.LastModifiedOnDate == queryModel.last_modifi_on_date);
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




        public async Task<Voucher> SaveAsync(Voucher Vouchers)
        {
            var res = await SaveAsync(new[] { Vouchers });
            return res.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<Voucher>> SaveAsync(IEnumerable<Voucher> Vouchers)
        {
            var updated = new List<Voucher>();

            foreach (var voucher in Vouchers)
            {
                var exist = await _context.Vouchers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                            x.Id == voucher.Id
                    );

                if (exist == null)
                {
                    voucher.CreateTracking(voucher.Id);
                    voucher.UpdateTracking(voucher.Id);
                    _context.Vouchers.Add(voucher);
                    updated.Add(voucher);
                }
                else
                {
                    _context.Entry(exist).State = EntityState.Detached;
                    exist.Id = voucher.Id;
                    exist.VoucherName = voucher.VoucherName;
                    exist.VoucherType = voucher.VoucherType;
                    exist.StartDate = voucher.StartDate;
                    exist.EndDate = voucher.EndDate;
                    exist.Status = voucher.Status;
                    exist.CreatedOnDate = voucher.CreatedOnDate;
                    exist.LastModifiedOnDate = voucher.LastModifiedOnDate;

                    voucher.UpdateTracking(voucher.Id);
                    _context.Vouchers.Update(exist);
                    updated.Add(exist);
                }
            }
            await _context.SaveChangesAsync();

            return updated;
        }


        public async Task<Voucher> DeleteAsync(Guid Id)
        {
            var exist = await FindAsync(Id);
            if (exist == null) throw new Exception(IVoucherRepository.MessageNotFound);
            exist.Isdeleted = true;
            _context.Vouchers.Update(exist);
            _context.SaveChangesAsync();
            return exist;
        }

        public Task<IEnumerable<Voucher>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }


        public async Task<int> GetCountAsync(VoucherQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }
    }
}
