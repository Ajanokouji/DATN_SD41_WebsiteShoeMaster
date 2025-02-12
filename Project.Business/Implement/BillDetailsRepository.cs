using Project.Business.Model;
using Project.DbManagement;
using Microsoft.EntityFrameworkCore;
using Project.Business.Interface;
using SERP.Framework.Common;
using SERP.Framework.Business;
using SERP.Framework.DB.Extensions;
using LinqKit;

namespace Project.Business.Implement;

public class BillDetailsRepository : IBillDetailsRepository
{
    private readonly ProjectDbContext _context;

    public BillDetailsRepository(ProjectDbContext context)
    {
        _context = context;
    }
    public async Task<BillDetails> FindAsync(Guid id)
        {
            var res = await _context.BillDetails.FindAsync(id);
            return res;
        }
        public async Task<IEnumerable<BillDetails>> ListAllAsync(BillDetailsQueryModel queryModel)
        {
            var query = BuildQuery( queryModel);
            var resId = await query.Select(x => x.id_hoa_don_chi_tiet).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;

        }

        public async Task<IEnumerable<BillDetails>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.BillDetails.Where(x => ids.Contains(x.id_hoa_don_chi_tiet)).ToListAsync();
            return res;
        }

        public async Task<Pagination<BillDetails>> GetAllAsync(BillDetailsQueryModel queryModel)
        {
            BillDetailsQueryModel billDetailsQueryModel = queryModel;


            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<BillDetails> queryable = BuildQuery( queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                queryable = queryable.OrderByDescending((BillDetails x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value, sortExpression);
        }

        private IQueryable<BillDetails> BuildQuery( BillDetailsQueryModel queryModel)
        {
            IQueryable<BillDetails> query = _context.BillDetails.AsNoTracking().Where(x => x.Isdeleted!=true);

            if (queryModel.Id.HasValue)
            {
                query = query.Where((BillDetails x) => x.id_hoa_don_chi_tiet == queryModel.Id.Value);
            }

            if (queryModel.ListId != null && queryModel.ListId.Any())
            {
                query = query.Where((BillDetails x) => queryModel.ListId.Contains(x.id_hoa_don_chi_tiet));
            }

            if (queryModel.ListTextSearch != null && queryModel.ListTextSearch.Any())
            {
                ExpressionStarter<BillDetails> expressionStarter = LinqKit.PredicateBuilder.New<BillDetails>();
                foreach (string ts in queryModel.ListTextSearch)
                {
                    expressionStarter = expressionStarter.Or((BillDetails p) => 
                                                                p.ma_hoa_don_chi_tiet.Contains(ts.ToLower()) ||
                                                                p.ghi_chu.Contains(ts.ToLower()));
                }

                query = query.Where(expressionStarter);
            }

            if (!string.IsNullOrWhiteSpace(queryModel.FullTextSearch))
            {
                string fullTextSearch = queryModel.FullTextSearch.ToLower();
                query = query.Where((BillDetails x) => x.update_by.Contains(fullTextSearch));
            }
            
            if (queryModel.id_hoa_don_chi_tiet.HasValue)
            {
                query = query.Where(x => x.id_hoa_don_chi_tiet == queryModel.id_hoa_don_chi_tiet.Value);
            }
            
            if (queryModel.id_hoa_don.HasValue)
            {
                query = query.Where(x => x.id_hoa_don == queryModel.id_hoa_don.Value);
            }
            
            if (queryModel.id_san_pham_chi_tiet.HasValue)
            {
                query = query.Where(x => x.id_san_pham_chi_tiet == queryModel.id_san_pham_chi_tiet.Value);
            }
            
            if (!string.IsNullOrEmpty(queryModel.ma_hoa_don_chi_tiet))
            {
                query = query.Where(x => x.ma_hoa_don_chi_tiet==queryModel.ma_hoa_don_chi_tiet);
            }
            
            if (!string.IsNullOrEmpty(queryModel.create_by))
            {
                query = query.Where(x => x.create_by==queryModel.create_by);
            }
            
            if (!string.IsNullOrEmpty(queryModel.update_by))
            {
                query = query.Where(x => x.update_by==queryModel.update_by);
            }
            
            if (!string.IsNullOrEmpty(queryModel.ghi_chu))
            {
                query = query.Where(x => x.ghi_chu==queryModel.ghi_chu);
            }
            
            if (queryModel.don_gia > 0)
            {
                query = query.Where(x => x.don_gia==queryModel.don_gia);
            }
            
            if (queryModel.trang_thai >= 0)
            {
                query = query.Where(x => x.trang_thai==queryModel.trang_thai);
            }
            
            if (queryModel.so_luong >= 0)
            {
                query = query.Where(x => x.so_luong==queryModel.so_luong);
            }
            
            if (queryModel.create_on_date != null)
            {
                query = query.Where(x => x.create_on_date==queryModel.create_on_date);
            }
            
            
            if (queryModel.last_modifi_on_date != null)
            {
                query = query.Where(x => x.last_modifi_on_date==queryModel.last_modifi_on_date);
            }
            
            return query;
        }

        public async Task<int> GetCountAsync(BillDetailsQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }

        public async Task<BillDetails> saveAsync(BillDetails billDetails)
        {
            var res = await saveAsync(new [] { billDetails });
            return res.FirstOrDefault();

        }

        public virtual async Task<IEnumerable<BillDetails>> saveAsync( IEnumerable<BillDetails>  billDetails)
        {
            var updated = new List<BillDetails>();

            foreach (var billDetail in billDetails)
            {
                var exist = await _context.BillDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                            x.id_hoa_don_chi_tiet==billDetail.id_hoa_don
                    );

                if (exist == null)
                {
                    billDetail.CreateTracking(billDetail.id_hoa_don_chi_tiet);
                    billDetail.UpdateTracking(billDetail.id_hoa_don_chi_tiet);
                    _context.BillDetails.Add(billDetail);
                    updated.Add(billDetail);
                }
                else
                {
                    _context.Entry(exist).State = EntityState.Detached;
                    exist.id_hoa_don_chi_tiet = billDetail.id_hoa_don_chi_tiet;
                    exist.id_hoa_don = billDetail.id_hoa_don;  
                    exist.id_san_pham_chi_tiet = billDetail.id_san_pham_chi_tiet;
                    exist.ma_hoa_don_chi_tiet = billDetail.ma_hoa_don_chi_tiet;
                    exist.trang_thai = billDetail.trang_thai;
                    exist.so_luong = billDetail.so_luong;
                    exist.don_gia = billDetail.don_gia;
                    exist.create_by = billDetail.create_by;
                    exist.update_by = billDetail.update_by;
                    exist.create_on_date = billDetail.create_on_date;
                    exist.last_modifi_on_date = billDetail.last_modifi_on_date;
                    exist.ghi_chu = billDetail.ghi_chu;

                    billDetail.UpdateTracking(billDetail.id_hoa_don_chi_tiet);
                    _context.BillDetails.Update(exist);
                    updated.Add(exist);
                }
            }
            await _context.SaveChangesAsync();

            return updated;
        }



        public async Task<BillDetails> DeleteAsync(Guid Id)
        {
            var exist = await FindAsync(Id);
            if (exist==null) throw new Exception(IBillDetailsRepository.MessageNotFound);
            exist.Isdeleted=true;
            _context.BillDetails.Update(exist);
            _context.SaveChangesAsync();
            return exist;
        }

        public Task<IEnumerable<BillDetails>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }
}