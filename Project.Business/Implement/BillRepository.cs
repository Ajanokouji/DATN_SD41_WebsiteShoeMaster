using Microsoft.EntityFrameworkCore;
using Project.Business.Model;
using Project.DbManagement;

namespace Project.Business.Implement;

public class BillRepository : IBillRepository
{
    private readonly ProjectDbContext _context;

    public BillRepository(ProjectDbContext context)
    {
        _context = context;
    }
    public async Task<Bill> FindAsync(Guid id)
        {
            var res = await _context.Bills.FindAsync(id);
            return res;
        }
        public async Task<IEnumerable<Bill>> ListAllAsync(BillQueryModel queryModel)
        {
            var query = BuildQuery( queryModel);
            var resId = await query.Select(x => x.id_hoa_don).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;

        }

        public async Task<IEnumerable<Bill>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.Bills.Where(x => ids.Contains(x.id_hoa_don)).ToListAsync();
            return res;
        }

        public async Task<Pagination<Bill>> GetAllAsync(BillQueryModel queryModel)
        {
            BillQueryModel billQueryModel = queryModel;


            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<Bill> queryable = BuildQuery( queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                queryable = queryable.OrderByDescending((Bill x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value, sortExpression);
        }

        private IQueryable<Bill> BuildQuery( BillQueryModel queryModel)
        {
            IQueryable<Bill> query = _context.Bills.AsNoTracking().Where(x => x.Isdeleted!=true);

            if (queryModel.Id.HasValue)
            {
                query = query.Where((Bill x) => x.Id == queryModel.Id.Value);
            }

            if (queryModel.ListId != null && queryModel.ListId.Any())
            {
                query = query.Where((Bill x) => queryModel.ListId.Contains(x.Id));
            }

            if (queryModel.ListTextSearch != null && queryModel.ListTextSearch.Any())
            {
                ExpressionStarter<Bill> expressionStarter = LinqKit.PredicateBuilder.New<Bill>();
                foreach (string ts in queryModel.ListTextSearch)
                {
                    expressionStarter = expressionStarter.Or((Bill p) => 
                                                                p.ma_hoa_don.Contains(ts.ToLower()) ||
                                                                p.ten_khach_nhan.Contains(ts.ToLower()));
                }

                query = query.Where(expressionStarter);
            }

            if (!string.IsNullOrWhiteSpace(queryModel.FullTextSearch))
            {
                string fullTextSearch = queryModel.FullTextSearch.ToLower();
                query = query.Where((Bill x) => x.ten_khach_nhan.Contains(fullTextSearch));
            }
            
            if (queryModel.id_hoa_don.HasValue)
            {
                query = query.Where(x => x.id_hoa_don == queryModel.id_hoa_don.Value);
            }
            
            if (queryModel.id_nhan_vien.HasValue)
            {
                query = query.Where(x => x.id_nhan_vien == queryModel.id_nhan_vien.Value);
            }
            
            if (queryModel.id_khach_hang.HasValue)
            {
                query = query.Where(x => x.id_khach_hang == queryModel.id_khach_hang.Value);
            }
            
            if (queryModel.id_don_hang.HasValue)
            {
                query = query.Where(x => x.id_don_hang == queryModel.id_don_hang.Value);
            }
            
            if (queryModel.id_phuong_thuc_thanh_toan.HasValue)
            {
                query = query.Where(x => x.id_phuong_thuc_thanh_toan == queryModel.id_phuong_thuc_thanh_toan.Value);
            }
            
            if (!string.IsNullOrEmpty(queryModel.ma_hoa_don))
            {
                query = query.Where(x => x.ma_hoa_don==queryModel.ma_hoa_don);
            }
            
            if (!string.IsNullOrEmpty(queryModel.ten_khach_nhan))
            {
                query = query.Where(x => x.ten_khach_nhan==queryModel.ten_khach_nhan);
            }
            
            if (!string.IsNullOrEmpty(queryModel.email_khach_nhan))
            {
                query = query.Where(x => x.email_khach_nhan==queryModel.email_khach_nhan);
            }
            
            if (!string.IsNullOrEmpty(queryModel.so_dien_thoai_khach_nhan))
            {
                query = query.Where(x => x.so_dien_thoai_khach_nhan==queryModel.so_dien_thoai_khach_nhan);
            }
            
            if (!string.IsNullOrEmpty(queryModel.dia_chi_nhan))
            {
                query = query.Where(x => x.dia_chi_nhan==queryModel.dia_chi_nhan);
            }
            
            if (queryModel.tong_tien > 0)
            {
                query = query.Where(x => x.tong_tien==queryModel.tong_tien);
            }
            
            if (queryModel.tong_tien_khuyen_mai > 0)
            {
                query = query.Where(x => x.tong_tien_khuyen_mai==queryModel.tong_tien_khuyen_mai);
            }
            
            
            if (queryModel.tong_tien_sau_khuyen_mai > 0)
            {
                query = query.Where(x => x.tong_tien_sau_khuyen_mai==queryModel.tong_tien_sau_khuyen_mai);
            }
            
            if (queryModel.tong_tien_phai_thanh_toan > 0)
            {
                query = query.Where(x => x.tong_tien_phai_thanh_toan==queryModel.tong_tien_phai_thanh_toan);
            }
            
            if (queryModel.trang_thai >= 0)
            {
                query = query.Where(x => x.trang_thai==queryModel.trang_thai);
            }
            
            if (queryModel.trang_thai_thanh_toan >= 0)
            {
                query = query.Where(x => x.trang_thai_thanh_toan==queryModel.trang_thai_thanh_toan);
            }
            
            if (queryModel.create_on_date != null)
            {
                query = query.Where(x => x.create_on_date==queryModel.create_on_date);
            }
            
            
            if (queryModel.last_modifi_on_date != null)
            {
                query = query.Where(x => x.last_modifi_on_date==queryModel.last_modifi_on_date);
            }
            
            if (!string.IsNullOrEmpty(queryModel.update_by))
            {
                query = query.Where(x => x.update_by==queryModel.update_by);
            }
            
            if (!string.IsNullOrEmpty(queryModel.ghi_chu))
            {
                query = query.Where(x => x.ghi_chu==queryModel.ghi_chu);
            }
            
            return query;
        }

        public async Task<int> GetCountAsync(BillQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }

        public async Task<Bill> saveAsync(Bill bills)
        {
            var res = await saveAsync(new [] { bills });
            return res.FirstOrDefault();

        }

        public virtual async Task<IEnumerable<Bill>> saveAsync( IEnumerable<Bill>  bills)
        {
            var updated = new List<Bill>();

            foreach (var bill in bills)
            {
                var exist = await _context.Bills
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                            x.id_hoa_don==bill.id_hoa_don
                    );

                if (exist == null)
                {
                    bill.CreateTracking(bill.id_hoa_don);
                    bill.UpdateTracking(bill.id_hoa_don);
                    _context.Bills.Add(bill);
                    updated.Add(bill);
                }
                else
                {
                    _context.Entry(exist).State = EntityState.Detached;
                    exist.id_hoa_don = bill.id_hoa_don;
                    exist.id_nhan_vien = bill.id_nhan_vien;  
                    exist.id_khach_hang = bill.id_khach_hang;
                    exist.id_don_hang = bill.id_don_hang;
                    exist.id_phuong_thuc_thanh_toan = bill.id_phuong_thuc_thanh_toan;
                    exist.ma_hoa_don = bill.ma_hoa_don;
                    exist.ten_khach_nhan = bill.ten_khach_nhan;
                    exist.email_khach_nhan = bill.email_khach_nhan;
                    exist.so_dien_thoai_khach_nhan = bill.so_dien_thoai_khach_nhan;
                    exist.dia_chi_nhan = bill.dia_chi_nhan;
                    exist.tong_tien = bill.tong_tien;
                    exist.tong_tien_khuyen_mai = bill.tong_tien_khuyen_mai;
                    exist.tong_tien_sau_khuyen_mai = bill.tong_tien_sau_khuyen_mai;
                    exist.tong_tien_phai_thanh_toan = bill.tong_tien_phai_thanh_toan;
                    exist.trang_thai = bill.trang_thai;
                    exist.trang_thai_thanh_toan = bill.trang_thai_thanh_toan;
                    exist.create_on_date = bill.create_on_date;
                    exist.email_khach_nhan = bill.email_khach_nhan;
                    exist.last_modifi_on_date = bill.last_modifi_on_date;
                    exist.update_by = bill.update_by;
                    exist.ghi_chu = bill.ghi_chu;
                    exist.LastModifiedByUserId=bill.LastModifiedByUserId;

                    bill.UpdateTracking(bill.id_hoa_don);
                    _context.Bills.Update(exist);
                    updated.Add(exist);
                }
            }
            await _context.SaveChangesAsync();

            return updated;
        }



        public async Task<Bill> DeleteAsync(Guid Id)
        {
            var exist = await FindAsync(Id);
            if (exist==null) throw new Exception(IBillRepository.MessageNotFound);
            exist.Isdeleted=true;
            _context.Bills.Update(exist);
            _context.SaveChangesAsync();
            return exist;
        }

        public Task<IEnumerable<Bill>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }
}