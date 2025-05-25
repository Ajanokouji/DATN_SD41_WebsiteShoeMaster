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
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
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

            //Lọc theo trạng thái tổng: Tất cả, Đang diễn ra, Sắp diễn ra, Đã kết thúc, Tạm dừng
            var now = DateTime.UtcNow;
            switch (queryModel.StatusTotal)
            {
                case 0:
                    break;

                case 1: // Đang diễn ra
                    queryable = queryable.Where(x => x.StartDate <= now && x.EndDate >= now && x.Status == 1);
                    break;

                case 2: // Sắp diễn ra
                    queryable = queryable.Where(x => x.StartDate > now && x.Status == 1);
                    break;

                case 3: // Đã kết thúc
                    queryable = queryable.Where(x => x.EndDate < now);
                    break;

                case 4: // Tạm dừng: Sắp diễn ra hoặc đang diễn ra nhưng status == 0: dừng hoạt động
                    queryable = queryable.Where(x => (x.StartDate <= now && x.EndDate >= now && x.Status == 0)
                                                        || (x.StartDate > now && x.Status == 0));
                    break;

                default:
                    throw new ArgumentException("Invalid trạng thái. Allowed values: 0 (Tất cả), 1 (Đang diễn ra), 2 (Sắp diễn ra), 3 (Đã kết thúc), 4 (Tạm dừng)");
            }

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
            IQueryable<Voucher> query = _context.Vouchers.AsNoTracking().Where(x => x.IsDeleted != true);

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

            if (queryModel.loai_giam_gia != null)
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

            if (queryModel.TotalMaxUsage >= 0)
            {
                query = query.Where(x => x.TotalMaxUsage == queryModel.TotalMaxUsage);
            }

            if (queryModel.MaxUsagePerCustomer >= 0)
            {
                query = query.Where(x => x.MaxUsagePerCustomer == queryModel.MaxUsagePerCustomer);
            }

            if (queryModel.MaxDiscountAmount >= 0)
            {
                query = query.Where(x => x.MaxDiscountAmount == queryModel.MaxDiscountAmount);
            }

            if (queryModel.RedeemCount >= 0)
            {
                query = query.Where(x => x.RedeemCount == queryModel.RedeemCount);
            }

            if (queryModel.DiscountAmount != null)
            {
                query = query.Where(x => x.DiscountAmount == queryModel.DiscountAmount);
            }

            if (queryModel.DiscountPercentage != null)
            {
                query = query.Where(x => x.DiscountPercentage == queryModel.DiscountPercentage);
            }

            if (queryModel.MinimumOrderAmount != null)
            {
                query = query.Where(x => x.MinimumOrderAmount == queryModel.MinimumOrderAmount);
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
                // Fix lỗi tracking
                var local = _context.Vouchers.Local.FirstOrDefault(x => x.Id == voucher.Id);
                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

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
                    exist.Code = voucher.Code;
                    exist.DiscountAmount = voucher.DiscountAmount;
                    exist.Description = voucher.Description;
                    exist.MinimumOrderAmount = voucher.MinimumOrderAmount;
                    exist.DiscountPercentage = voucher.DiscountPercentage;
                    exist.VoucherName = voucher.VoucherName;
                    exist.VoucherType = voucher.VoucherType;
                    exist.StartDate = voucher.StartDate;
                    exist.EndDate = voucher.EndDate;
                    exist.Status = voucher.Status;
                    exist.TotalMaxUsage = voucher.TotalMaxUsage;
                    exist.MaxUsagePerCustomer = voucher.MaxUsagePerCustomer;
                    exist.MaxDiscountAmount = voucher.MaxDiscountAmount;
                    exist.RedeemCount = voucher.RedeemCount;
                    exist.ImageUrl = voucher.ImageUrl;

                    exist.UpdateTracking(voucher.Id);
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
            exist.IsDeleted = true;
            _context.Vouchers.Update(exist);
            await _context.SaveChangesAsync(); //Thêm await fix lỗi xóa
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

        public Task<Voucher> FindByCodeAsync(string code)
        {
            throw new NotImplementedException();
        }

        //Check code đã tồn tại hay chưa khi Tạo mới hoặc Cập nhật
        public async Task<bool> IsVoucherCodeExist(string code, Guid? voucherId)
        {
            //Nếu voucherId có giá trị thì nghĩa là đang muốn kiểm tra khi cập nhật
            if (voucherId.HasValue)
            {
                bool isExist = await _context.Vouchers
                    .AsNoTracking()
                    .AnyAsync(x => x.Code.Trim().ToLower() == code.Trim().ToLower() && x.Id != voucherId.Value && x.IsDeleted == false);
                return isExist;
            }
            else //Nếu voucherId ko giá trị thì nghĩa là đang muốn kiểm tra khi tạo mới
            {
                bool isExist = await _context.Vouchers
                    .AsNoTracking()
                    .AnyAsync(x => x.Code.Trim().ToLower() == code.Trim().ToLower() && x.IsDeleted == false);
                return isExist;
            }
        }

        //Gợi ý voucher toàn shop
        public async Task<List<Voucher>> RecommendVoucherAllShop()
        {
            IQueryable<Voucher> query = _context.Vouchers.AsNoTracking().Where(x => x.IsDeleted == false
                                                                                && x.VoucherType == DbManagement.Enum.VocherTypeEnum.AllShop
                                                                                && x.RedeemCount < x.TotalMaxUsage);

            //Đang diễn ra
            var now = DateTime.UtcNow;
            query = query.Where(x => x.StartDate <= now && x.EndDate >= now && x.Status == 1);

            // Lấy toàn bộ danh sách thỏa điều kiện
            var filteredList = await query.ToListAsync();

            // Nếu ít hơn hoặc bằng 4 thì trả về nguyên danh sách
            if (filteredList.Count <= 4)
                return filteredList;

            // Random 4 phần tử
            var random = new Random();
            var randomTop4 = filteredList
                .OrderBy(x => random.Next())
                .Take(4)
                .ToList();

            return randomTop4;
        }

        //Gợi ý voucher sản phẩm
        public async Task<List<Voucher>> RecommendVoucherProduct(Guid productId)
        {
            // Bước 1: Lấy tất cả VoucherProduct theo productId và chưa bị xóa
            var voucherUsers = await _context.VoucherProducts
                .AsNoTracking()
                .Where(x => x.ProductId == productId && x.IsDeleted == false)
                .ToListAsync();

            // Bước 2: Kiểm tra có phần tử không
            if (voucherUsers.Count == 0)
                return new List<Voucher>(); // Trả về danh sách rỗng

            // Bước 3: Lấy danh sách voucherId không trùng
            var distinctVoucherIds = voucherUsers
                .Select(x => x.VoucherId)
                .Distinct()
                .ToList();

            var now = DateTime.UtcNow;

            // Bước 4: Truy vấn Voucher theo các điều kiện
            var validVouchers = await _context.Vouchers
                .AsNoTracking()
                .Where(x => distinctVoucherIds.Contains(x.Id)
                            && x.IsDeleted == false
                            && x.VoucherType == DbManagement.Enum.VocherTypeEnum.ByProduct
                            && x.RedeemCount < x.TotalMaxUsage
                            && x.StartDate <= now
                            && x.EndDate >= now
                            && x.Status == 1)
                .ToListAsync();

            return validVouchers;
        }
    }
}
