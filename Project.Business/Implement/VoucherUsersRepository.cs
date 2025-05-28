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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Implement
{
    public class VoucherUsersRepository : IVoucherUsersRepository
    {
        private readonly ProjectDbContext _context;

        public VoucherUsersRepository(ProjectDbContext context)
        {
                _context = context;
        }

        public async Task<VoucherUsers> FindAsync(Guid id)
        {
            var res = await _context.VoucherUsers.FindAsync(id);
            return res;
        }

        public async Task<IEnumerable<VoucherUsers>> ListAllAsync(VoucherUsersQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var resId = await query.Select(x => x.Id).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;

        }

        public async Task<IEnumerable<VoucherUsers>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.VoucherUsers.Where(x => ids.Contains(x.Id)).ToListAsync();
            return res;
        }

        public async Task<Pagination<VoucherUsers>> GetAllAsync(VoucherUsersQueryModel queryModel)
        {
            VoucherUsersQueryModel voucherUserQueryModel = queryModel;

            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<VoucherUsers> queryable = BuildQuery(queryModel);

            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                queryable = queryable.OrderByDescending((VoucherUsers x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value, sortExpression);
        }

        private IQueryable<VoucherUsers> BuildQuery(VoucherUsersQueryModel queryModel)
        {
            IQueryable<VoucherUsers> query = _context.VoucherUsers.AsNoTracking().Where(x => x.IsDeleted != true);

            if (queryModel.Id.HasValue)
            {
                query = query.Where((VoucherUsers x) => x.Id == queryModel.Id.Value);
            }

            if (queryModel.ListId != null && queryModel.ListId.Any())
            {
                query = query.Where((VoucherUsers x) => queryModel.ListId.Contains(x.Id));
            }

            if (queryModel.VoucherId.HasValue)
            {
                query = query.Where(x => x.VoucherId == queryModel.VoucherId.Value);
            }

            if (queryModel.UserId.HasValue)
            {
                query = query.Where(x => x.UserId == queryModel.UserId.Value);
            }

            if (queryModel.IsUsed.HasValue)
            {
                query = query.Where(x => x.IsUsed == queryModel.IsUsed.Value);
            }

            return query;
        }

        public async Task<VoucherUsers> SaveAsync(VoucherUsers VoucherUser)
        {
            var res = await SaveAsync(new[] { VoucherUser });
            return res.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<VoucherUsers>> SaveAsync(IEnumerable<VoucherUsers> voucherUsers)
        {
            var updated = new List<VoucherUsers>();

            foreach (var voucherUser in voucherUsers)
            {
                // Fix lỗi tracking
                var local = _context.VoucherUsers.Local.FirstOrDefault(x => x.Id == voucherUser.Id);
                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                var exist = await _context.VoucherUsers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                            x.Id == voucherUser.Id
                    );

                if (exist == null)
                {
                    voucherUser.CreateTracking(voucherUser.Id);
                    voucherUser.UpdateTracking(voucherUser.Id);
                    _context.VoucherUsers.Add(voucherUser);
                    updated.Add(voucherUser);
                }
                else
                {
                    _context.Entry(exist).State = EntityState.Detached;
                    exist.Id = voucherUser.Id;
                    exist.VoucherId = voucherUser.VoucherId;
                    exist.UserId = voucherUser.UserId;
                    exist.IsUsed = voucherUser.IsUsed;

                    exist.UpdateTracking(voucherUser.Id);
                    _context.VoucherUsers.Update(exist);
                    updated.Add(exist);
                }
            }
            await _context.SaveChangesAsync();

            return updated;
        }


        public async Task<VoucherUsers> DeleteAsync(Guid Id)
        {
            var exist = await FindAsync(Id);
            if (exist == null) throw new Exception(IVoucherUsersRepository.MessageNotFound);
            exist.IsDeleted = true;
            _context.VoucherUsers.Update(exist);
            await _context.SaveChangesAsync(); //Thêm await fix lỗi xóa
            return exist;
        }

        public Task<IEnumerable<VoucherUsers>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }


        public async Task<int> GetCountAsync(VoucherUsersQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }

        //Tìm kiếm tài khoản khách hàng linh hoạt dựa vào nhiều thuộc tính
        public async Task<Pagination<UserEntity>> SearchUser(UserQueryModel queryModel)
        {
            var searchStringNormal = RemoveDiacritics(queryModel.Username).ToLower();

            IQueryable<UserEntity> queryable = _context.Users
                .Where(p =>
                    EF.Functions.Collate(p.Username, "Latin1_General_CI_AI").ToLower().Contains(searchStringNormal) ||
                    EF.Functions.Collate(p.Name, "Latin1_General_CI_AI").ToLower().Contains(searchStringNormal) ||
                    EF.Functions.Collate(p.PhoneNumber, "Latin1_General_CI_AI").ToLower().Contains(searchStringNormal) ||
                    EF.Functions.Collate(p.Address, "Latin1_General_CI_AI").ToLower().Contains(searchStringNormal) ||
                    EF.Functions.Collate(p.Email, "Latin1_General_CI_AI").ToLower().Contains(searchStringNormal));

            queryable = queryable.Where(p => p.IsDeleted == false);
            queryable = queryable.Where(p => p.Type == UserTypeEnum.Customer);

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

        //Lấy nhiều user dựa vào nhiều userId
        public async Task<IEnumerable<UserEntity>> GetUsersByIds(Guid[] ids)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id) && x.IsDeleted == false)
                .ToListAsync();
        }

        //Tìm kiếm VoucherUser dựa trên voucherId, userId
        public async Task<IEnumerable<VoucherUsers>> FindByVidUid(IEnumerable<VoucherUsers> voucherUsers)
        {
            var voucherUsersFind = new List<VoucherUsers>();
            foreach (var voucherUser in voucherUsers)
            {
                var voucherUserFind = _context.VoucherUsers
                    .Where(v => v.VoucherId == voucherUser.VoucherId
                             && v.UserId == voucherUser.UserId
                             && v.IsDeleted == false
                             && v.IsUsed == false)
                    .FirstOrDefault();

                if (voucherUserFind != null)
                {
                    voucherUsersFind.Add(voucherUserFind);
                }
            }

            return voucherUsersFind;
        }
    }
}
