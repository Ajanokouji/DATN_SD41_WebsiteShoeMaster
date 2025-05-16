using LinqKit;
using Microsoft.EntityFrameworkCore;
using Nest;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Business;
using SERP.Framework.Common;
using SERP.Framework.DB.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Implement
{
    public class VoucherProductRepository : IVoucherProductsRepository
    {
        private readonly ProjectDbContext _context;

        public VoucherProductRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<VoucherProducts> FindAsync(Guid id)
        {
            var res = await _context.VoucherProducts.FindAsync(id);
            return res;
        }

        public async Task<IEnumerable<VoucherProducts>> ListAllAsync(VoucherProductsQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var resId = await query.Select(x => x.Id).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;
        }

        public async Task<IEnumerable<VoucherProducts>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.VoucherProducts.Where(x => ids.Contains(x.Id)).ToListAsync();
            return res;
        }

        public async Task<Pagination<VoucherProducts>> GetAllAsync(VoucherProductsQueryModel queryModel)
        {
            VoucherProductsQueryModel voucherPrQueryModel = queryModel;

            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<VoucherProducts> queryable = BuildQuery(queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                queryable = queryable.OrderByDescending((VoucherProducts x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value, sortExpression);
        }

        private IQueryable<VoucherProducts> BuildQuery(VoucherProductsQueryModel queryModel)
        {
            IQueryable<VoucherProducts> query = _context.VoucherProducts.AsNoTracking().Where(x => x.IsDeleted != true);

            if (queryModel.Id.HasValue)
            {
                query = query.Where((VoucherProducts x) => x.Id == queryModel.Id.Value);
            }

            if (queryModel.ListId != null && queryModel.ListId.Any())
            {
                query = query.Where((VoucherProducts x) => queryModel.ListId.Contains(x.Id));
            }

            if (queryModel.VoucherId.HasValue)
            {
                query = query.Where(x => x.VoucherId == queryModel.VoucherId.Value);
            }

            if (queryModel.ProductId.HasValue)
            {
                query = query.Where(x => x.ProductId == queryModel.ProductId.Value);
            }

            if (!String.IsNullOrWhiteSpace(queryModel.VarientProductId))
            {
                query = query.Where(x => x.VarientProductId == queryModel.VarientProductId);
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

        public async Task<VoucherProducts> SaveAsync(VoucherProducts VoucherProducts)
        {
            var res = await SaveAsync(new[] { VoucherProducts });
            return res.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<VoucherProducts>> SaveAsync(IEnumerable<VoucherProducts> VoucherProducts)
        {
            var updated = new List<VoucherProducts>();

            foreach (var voucherPr in VoucherProducts)
            {
                // Fix lỗi tracking
                var local = _context.Vouchers.Local.FirstOrDefault(x => x.Id == voucherPr.Id);
                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                var exist = await _context.VoucherProducts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                            x.Id == voucherPr.Id
                    );

                if (exist == null)
                {
                    voucherPr.CreateTracking(voucherPr.Id);
                    voucherPr.UpdateTracking(voucherPr.Id);
                    _context.VoucherProducts.Add(voucherPr);
                    updated.Add(voucherPr);
                }
                else
                {
                    _context.Entry(exist).State = EntityState.Detached;
                    exist.Id = voucherPr.Id;
                    exist.VoucherId = voucherPr.VoucherId;
                    exist.ProductId = voucherPr.ProductId;
                    exist.VarientProductId = voucherPr.VarientProductId;

                    exist.UpdateTracking(voucherPr.Id);
                    _context.VoucherProducts.Update(exist);
                    updated.Add(exist);
                }
            }
            await _context.SaveChangesAsync();

            return updated;
        }


        public async Task<VoucherProducts> DeleteAsync(Guid Id)
        {
            var exist = await FindAsync(Id);
            if (exist == null) throw new Exception(IVoucherProductsRepository.MessageNotFound);
            exist.IsDeleted = true;
            _context.VoucherProducts.Update(exist);
            await _context.SaveChangesAsync();
            return exist;
        }

        public Task<IEnumerable<VoucherProducts>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }


        public async Task<int> GetCountAsync(VoucherProductsQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }

        //Lấy nhiều Product dựa vào nhiều productId: Làm ở đây vì sợ conflic với product
        public async Task<IEnumerable<ProductEntity>> GetProductsByIds(Guid[] ids)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id) && x.IsDeleted == false)
                .ToListAsync();
        }

        //Tìm kiếm sản phẩm linh hoạt dựa vào tên, mã và mô tả
        public async Task<Pagination<ProductEntity>> SearchProduct(ProductQueryModel queryModel)
        {
            var searchStringNormal = RemoveDiacritics(queryModel.TenSanPham).ToLower();

            IQueryable<ProductEntity> queryable = _context.Products
                .Where(p =>
                    EF.Functions.Collate(p.Name, "Latin1_General_CI_AI").ToLower().Contains(searchStringNormal) ||
                    EF.Functions.Collate(p.Code, "Latin1_General_CI_AI").ToLower().Contains(searchStringNormal) ||
                    EF.Functions.Collate(p.Description, "Latin1_General_CI_AI").ToLower().Contains(searchStringNormal));

            queryable = queryable.Where(p => p.IsDeleted == false);

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value, string.Empty);
        }

        //Hàm loại bỏ dấu của chuỗi
        public static string RemoveDiacritics(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string normalized = input.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in normalized)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        //Tìm kiếm voucherProduct dựa trên voucherId, productId và VariantId
        public async Task<IEnumerable<VoucherProducts>> FindByVcidPidVaid(IEnumerable<VoucherProducts> voucherProducts)
        {
            var voucherProductsFind = new List<VoucherProducts>();
            foreach (var voucherProduct in voucherProducts)
            {
                var voucherProductFind = _context.VoucherProducts
                    .Where(v => v.VoucherId == voucherProduct.VoucherId
                             && v.ProductId == voucherProduct.ProductId
                             && v.VarientProductId == voucherProduct.VarientProductId
                             && v.IsDeleted == false)
                    .FirstOrDefault();

                if (voucherProductFind != null)
                {
                    voucherProductsFind.Add(voucherProductFind);
                }
            }

            return voucherProductsFind;
        }
    }
}