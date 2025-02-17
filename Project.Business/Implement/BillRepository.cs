using LinqKit;
using Microsoft.EntityFrameworkCore;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement;
using SERP.Framework.Business;
using SERP.Framework.Common;
using SERP.Framework.DB.Extensions;

namespace Project.Business.Implement;

public class BillRepository : IBillRepository
{
    private readonly ProjectDbContext _context;

    public BillRepository(ProjectDbContext context)
    {
        _context = context;
    }
    public async Task<BillEntity> FindAsync(Guid id)
        {
            var res = await _context.Bills.FindAsync(id);
            return res;
        }
        public async Task<IEnumerable<BillEntity>> ListAllAsync(BillQueryModel queryModel)
        {
            var query = BuildQuery( queryModel);
            var resId = await query.Select(x => x.Id).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;

        }

        public async Task<IEnumerable<BillEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.Bills.Where(x => ids.Contains(x.Id)).ToListAsync();
            return res;
        }


    public async Task<Pagination<BillEntity>> GetAllAsync(BillQueryModel queryModel)
        {
            BillQueryModel billQueryModel = queryModel;


            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<BillEntity> queryable = BuildQuery( queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                queryable = queryable.OrderByDescending((BillEntity x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value, sortExpression);
        }

        private IQueryable<BillEntity> BuildQuery( BillQueryModel queryModel)
        {
            IQueryable<BillEntity> query = _context.Bills.AsNoTracking().Where(x => x.Isdeleted!=true);

            if (queryModel.Id.HasValue)
            {
                query = query.Where((BillEntity x) => x.Id == queryModel.Id.Value);
            }

            if (queryModel.ListId != null && queryModel.ListId.Any())
            {
                query = query.Where((BillEntity x) => queryModel.ListId.Contains(x.Id));
            }

            if (queryModel.ListTextSearch != null && queryModel.ListTextSearch.Any())
            {
                ExpressionStarter<BillEntity> expressionStarter = LinqKit.PredicateBuilder.New<BillEntity>();
                foreach (string ts in queryModel.ListTextSearch)
                {
                    expressionStarter = expressionStarter.Or((BillEntity p) => 
                                                                p.BillCode.Contains(ts.ToLower()) ||
                                                                p.RecipientName.Contains(ts.ToLower()));
                }

                query = query.Where(expressionStarter);
            }

            if (!string.IsNullOrWhiteSpace(queryModel.FullTextSearch))
            {
                string fullTextSearch = queryModel.FullTextSearch.ToLower();
                query = query.Where((BillEntity x) => x.RecipientName.Contains(fullTextSearch));
            }
            
            if (queryModel.id_hoa_don.HasValue)
            {
                query = query.Where(x => x.Id == queryModel.id_hoa_don.Value);
            }
            
            if (queryModel.id_nhan_vien.HasValue)
            {
                query = query.Where(x => x.EmployeeId == queryModel.id_nhan_vien.Value);
            }
            
            if (queryModel.id_khach_hang.HasValue)
            {
                query = query.Where(x => x.CustomerId == queryModel.id_khach_hang.Value);
            }
            
            if (queryModel.id_don_hang.HasValue)
            {
                query = query.Where(x => x.OrderId == queryModel.id_don_hang.Value);
            }
            
            if (queryModel.id_phuong_thuc_thanh_toan.HasValue)
            {
                query = query.Where(x => x.PaymentMethodId == queryModel.id_phuong_thuc_thanh_toan.Value);
            }
            
            if (!string.IsNullOrEmpty(queryModel.ma_hoa_don))
            {
                query = query.Where(x => x.BillCode==queryModel.ma_hoa_don);
            }
            
            if (!string.IsNullOrEmpty(queryModel.ten_khach_nhan))
            {
                query = query.Where(x => x.RecipientName == queryModel.ten_khach_nhan);
            }
            
            if (!string.IsNullOrEmpty(queryModel.email_khach_nhan))
            {
                query = query.Where(x => x.RecipientEmail == queryModel.email_khach_nhan);
            }
            
            if (!string.IsNullOrEmpty(queryModel.so_dien_thoai_khach_nhan))
            {
                query = query.Where(x => x.RecipientPhone == queryModel.so_dien_thoai_khach_nhan);
            }
            
            if (!string.IsNullOrEmpty(queryModel.dia_chi_nhan))
            {
                query = query.Where(x => x.RecipientAddress == queryModel.dia_chi_nhan);
            }
            
            if (queryModel.tong_tien > 0)
            {
                query = query.Where(x => x.TotalAmount == queryModel.tong_tien);
            }
            
            if (queryModel.tong_tien_khuyen_mai > 0)
            {
                query = query.Where(x => x.DiscountAmount == queryModel.tong_tien_khuyen_mai);
            }
            
            
            if (queryModel.tong_tien_sau_khuyen_mai > 0)
            {
                query = query.Where(x => x.AmountAfterDiscount == queryModel.tong_tien_sau_khuyen_mai);
            }
            
            if (queryModel.tong_tien_phai_thanh_toan > 0)
            {
                query = query.Where(x => x.AmountToPay == queryModel.tong_tien_phai_thanh_toan);
            }
            
            if (queryModel.trang_thai >= 0)
            {
                query = query.Where(x => x.Status == queryModel.trang_thai);
            }
            
            if (queryModel.trang_thai_thanh_toan >= 0)
            {
                query = query.Where(x => x.PaymentStatus == queryModel.trang_thai_thanh_toan);
            }
            
            if (queryModel.create_on_date != null)
            {
                query = query.Where(x => x.CreatedOnDate == queryModel.create_on_date);
            }
            
            
            if (queryModel.last_modifi_on_date != null)
            {
                query = query.Where(x => x.LastModifiedOnDate == queryModel.last_modifi_on_date);
            }
            
            if (!string.IsNullOrEmpty(queryModel.update_by))
            {
                query = query.Where(x => x.UpdateBy == queryModel.update_by);
            }
            
            if (!string.IsNullOrEmpty(queryModel.ghi_chu))
            {
                query = query.Where(x => x.Notes == queryModel.ghi_chu);
            }
            
            return query;
        }

        public async Task<int> GetCountAsync(BillQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }

        public async Task<BillEntity> SaveAsync(BillEntity bills)
        {
            var res = await SaveAsync(new [] { bills });
            return res.FirstOrDefault();

        }

        public virtual async Task<IEnumerable<BillEntity>> SaveAsync( IEnumerable<BillEntity>  bills)
        {
            var updated = new List<BillEntity>();

            foreach (var bill in bills)
            {
                var exist = await _context.Bills
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                            x.Id==bill.Id
                    );

                if (exist == null)
                {
                    bill.CreateTracking(bill.Id);
                    bill.UpdateTracking(bill.Id);
                    _context.Bills.Add(bill);
                    updated.Add(bill);
                }
                else
                {
                    _context.Entry(exist).State = EntityState.Detached;
                    exist.Id = bill.Id;
                    exist.EmployeeId = bill.EmployeeId;  
                    exist.CustomerId = bill.CustomerId;
                    exist.OrderId = bill.OrderId;
                    exist.PaymentMethodId = bill.PaymentMethodId;
                    exist.BillCode = bill.BillCode;
                    exist.RecipientName = bill.RecipientName;
                    exist.RecipientEmail = bill.RecipientEmail;
                    exist.RecipientPhone = bill.RecipientPhone;
                    exist.RecipientAddress = bill.RecipientAddress;
                    exist.TotalAmount = bill.TotalAmount;
                    exist.DiscountAmount = bill.DiscountAmount;
                    exist.AmountAfterDiscount = bill.AmountAfterDiscount;
                    exist.AmountToPay = bill.AmountToPay;
                    exist.Status = bill.Status;
                    exist.PaymentStatus = bill.PaymentStatus;
                    exist.CreatedOnDate = bill.CreatedOnDate;
                    exist.RecipientEmail = bill.RecipientEmail;
                    exist.LastModifiedOnDate = bill.LastModifiedOnDate;
                    exist.UpdateBy = bill.UpdateBy;
                    exist.Notes = bill.Notes;
                    exist.LastModifiedByUserId=bill.LastModifiedByUserId;

                    bill.UpdateTracking(bill.Id);
                    _context.Bills.Update(exist);
                    updated.Add(exist);
                }
            }
            await _context.SaveChangesAsync();

            return updated;
        }


        public async Task<BillEntity> DeleteAsync(Guid Id)
        {
            var exist = await FindAsync(Id);
            if (exist==null) throw new Exception(IBillRepository.MessageNotFound);
            exist.Isdeleted=true;
            _context.Bills.Update(exist);
            _context.SaveChangesAsync();
            return exist;
        }

        public Task<IEnumerable<BillEntity>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }

   
}