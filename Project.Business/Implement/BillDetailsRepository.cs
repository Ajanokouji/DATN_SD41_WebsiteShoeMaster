using Project.Business.Model;
using Project.DbManagement;
using Microsoft.EntityFrameworkCore;
using SERP.Framework.Common;
using SERP.Framework.Business;
using SERP.Framework.DB.Extensions;
using LinqKit;
using Project.Business.Interface.Repositories;

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
            var resId = await query.Select(x => x.Id).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;
        }

        public async Task<IEnumerable<BillDetails>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.BillDetails.Where(x => ids.Contains(x.Id)).ToListAsync();
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
                query = query.Where((BillDetails x) => x.Id == queryModel.Id.Value);
            }

            if (queryModel.ListId != null && queryModel.ListId.Any())
            {
                query = query.Where((BillDetails x) => queryModel.ListId.Contains(x.Id));
            }

            if (queryModel.ListTextSearch != null && queryModel.ListTextSearch.Any())
            {
                ExpressionStarter<BillDetails> expressionStarter = LinqKit.PredicateBuilder.New<BillDetails>();
                foreach (string ts in queryModel.ListTextSearch)
                {
                    expressionStarter = expressionStarter.Or((BillDetails p) => 
                                                                p.BillDetailCode.Contains(ts.ToLower()) ||
                                                                p.Notes.Contains(ts.ToLower()));
                }

                query = query.Where(expressionStarter);
            }
            
            if (queryModel.id_hoa_don_chi_tiet.HasValue)
            {
                query = query.Where(x => x.Id == queryModel.id_hoa_don_chi_tiet.Value);
            }
            
            if (queryModel.id_hoa_don.HasValue)
            {
                query = query.Where(x => x.BillId == queryModel.id_hoa_don.Value);
            }
            
            if (queryModel.id_san_pham_chi_tiet.HasValue)
            {
                query = query.Where(x => x.ProductDetailId == queryModel.id_san_pham_chi_tiet.Value);
            }
            
            if (!string.IsNullOrEmpty(queryModel.ma_hoa_don_chi_tiet))
            {
                query = query.Where(x => x.BillDetailCode==queryModel.ma_hoa_don_chi_tiet);
            }
            
            if (!string.IsNullOrEmpty(queryModel.ghi_chu))
            {
                query = query.Where(x => x.Notes==queryModel.ghi_chu);
            }
            
            if (queryModel.don_gia > 0)
            {
                query = query.Where(x => x.Price==queryModel.don_gia);
            }
            
            if (queryModel.trang_thai >= 0)
            {
                query = query.Where(x => x.Status==queryModel.trang_thai);
            }
            
            if (queryModel.so_luong >= 0)
            {
                query = query.Where(x => x.Quantity==queryModel.so_luong);
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
                            x.Id==billDetail.Id
                    );

                if (exist == null)
                {
                    billDetail.CreateTracking(billDetail.Id);
                    billDetail.UpdateTracking(billDetail.Id);
                    _context.BillDetails.Add(billDetail);
                    updated.Add(billDetail);
                }
                else
                {
                    _context.Entry(exist).State = EntityState.Detached;
                    exist.Id = billDetail.Id;
                    exist.BillId = billDetail.BillId;  
                    exist.ProductDetailId = billDetail.ProductDetailId;
                    exist.BillDetailCode = billDetail.BillDetailCode;
                    exist.Status = billDetail.Status;
                    exist.Quantity = billDetail.Quantity;
                    exist.Price = billDetail.Price;
                    exist.Notes = billDetail.Notes;

                    billDetail.UpdateTracking(billDetail.Id);
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