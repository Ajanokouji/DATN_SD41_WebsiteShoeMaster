using Project.Business.Model;
using Project.DbManagement;
using Microsoft.EntityFrameworkCore;
using SERP.Framework.Common;
using SERP.Framework.Business;
using SERP.Framework.DB.Extensions;
using LinqKit;
using Nest;
using Newtonsoft.Json;
using Project.Business.Interface.Repositories;
using Project.Common;
using Project.DbManagement.Entity;
using Project.DbManagement.ViewModels;
using SERP.Framework.Entities.Metadata.Extensions;

namespace Project.Business.Implement;

public class BillDetailsRepository : IBillDetailsRepository
{
    private readonly ProjectDbContext _context;

    public BillDetailsRepository(ProjectDbContext context)
    {
        _context = context;
    }

    public async Task<BillDetailsEntity> FindAsync(Guid id)
    {
        var res = await _context.BillDetails.FindAsync(id);
        return res;
    }

    public async Task<IEnumerable<BillDetailsEntity>> ListAllAsync(BillDetailsQueryModel queryModel)
    {
        var query = BuildQuery(queryModel);
        var resId = await query.Select(x => x.Id).ToListAsync();
        var res = await ListByIdsAsync(resId);
        return res;
    }

    public async Task<IEnumerable<BillDetailsEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
    {
        var res = await _context.BillDetails.Where(x => ids.Contains(x.Id)).ToListAsync();
        return res;
    }

    public async Task<Pagination<BillDetailsEntity>> GetAllAsync(BillDetailsQueryModel queryModel)
    {
        BillDetailsQueryModel billDetailsQueryModel = queryModel;


        queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
        IQueryable<BillDetailsEntity> queryable = BuildQuery(queryModel);
        string sortExpression = string.Empty;
        if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
        {
            queryable = queryable.OrderByDescending((BillDetailsEntity x) => x.LastModifiedOnDate);
        }
        else
        {
            sortExpression = queryModel.Sort;
        }

        return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value,
            sortExpression);
    }

    private IQueryable<BillDetailsEntity> BuildQuery(BillDetailsQueryModel queryModel)
    {
        IQueryable<BillDetailsEntity> query = _context.BillDetails.AsNoTracking().Where(x => x.IsDeleted != true);

        if (queryModel.Id.HasValue)
        {
            query = query.Where((BillDetailsEntity x) => x.Id == queryModel.Id.Value);
        }

        if (queryModel.BillIds != null && queryModel.BillIds.Any())
        {
            query = query.Where((BillDetailsEntity x) => queryModel.BillIds.Contains(x.BillId.Value));
        }

        if (queryModel.ListId != null && queryModel.ListId.Any())
        {
            query = query.Where((BillDetailsEntity x) => queryModel.ListId.Contains(x.Id));
        }

        if (queryModel.ListTextSearch != null && queryModel.ListTextSearch.Any())
        {
            ExpressionStarter<BillDetailsEntity> expressionStarter = LinqKit.PredicateBuilder.New<BillDetailsEntity>();
            foreach (string ts in queryModel.ListTextSearch)
            {
                expressionStarter = expressionStarter.Or((BillDetailsEntity p) =>
                    p.BillDetailCode.Contains(ts.ToLower()) ||
                    p.Notes.Contains(ts.ToLower()));
            }

            query = query.Where(expressionStarter);
        }

        if (queryModel.id_hoa_don_chi_tiet.HasValue)
        {
            query = query.Where(x => x.Id == queryModel.id_hoa_don_chi_tiet.Value);
        }

        if (queryModel.BillId.HasValue)
        {
            query = query.Where(x => x.BillId == queryModel.BillId.Value);
        }

        if (queryModel.id_san_pham_chi_tiet.HasValue)
        {
            query = query.Where(x => x.ProductId == queryModel.id_san_pham_chi_tiet.Value);
        }

        if (!string.IsNullOrEmpty(queryModel.ma_hoa_don_chi_tiet))
        {
            query = query.Where(x => x.BillDetailCode == queryModel.ma_hoa_don_chi_tiet);
        }

        if (!string.IsNullOrEmpty(queryModel.ghi_chu))
        {
            query = query.Where(x => x.Notes == queryModel.ghi_chu);
        }

        if (queryModel.don_gia > 0)
        {
            query = query.Where(x => x.Price == queryModel.don_gia);
        }

        if (queryModel.trang_thai >= 0)
        {
            query = query.Where(x => x.Status == queryModel.trang_thai);
        }

        if (queryModel.so_luong >= 0)
        {
            query = query.Where(x => x.Quantity == queryModel.so_luong);
        }


        return query;
    }

    public async Task<int> GetCountAsync(BillDetailsQueryModel queryModel)
    {
        var query = BuildQuery(queryModel);
        var res = await query.CountAsync();
        return res;
    }

    public async Task<BillDetailsEntity> SaveAsync(BillDetailsEntity billDetails)
    {
        var res = await SaveAsync(new[] { billDetails });
        return res.FirstOrDefault();
    }

    public virtual async Task<IEnumerable<BillDetailsEntity>> SaveAsync(IEnumerable<BillDetailsEntity> billDetails)
    {
        var updated = new List<BillDetailsEntity>();

        foreach (var billDetail in billDetails)
        {
            var exist = await _context.BillDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == billDetail.Id
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
                exist.ProductId = billDetail.ProductId;
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


    public async Task<BillDetailsEntity> DeleteAsync(Guid Id)
    {
        var exist = await FindAsync(Id);
        if (exist == null) throw new Exception(IBillDetailsRepository.MessageNotFound);
        exist.IsDeleted = true;
        _context.BillDetails.Update(exist);
        _context.SaveChangesAsync();
        return exist;
    }

    public Task<IEnumerable<BillDetailsEntity>> DeleteAsync(Guid[] deleteIds)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveBillDetails(BillDetailsRequest request)
    {
        try
        {
            var product = _context.Products.Find(request.IdProduct);
            // Kiểm tra sản phẩm đã tồn tại trong hóa đơn chưa
            var prdDtExist = _context.BillDetails
                .Any(b => b.BillId == request.IdBill 
                          && b.ProductId == request.IdProduct 
                          && b.Color == request.Color 
                          && b.Size == request.Size);

            if (product == null || product.VariantObjs == null)
                return false;

            // Tìm variant tương ứng với màu sắc và kích cỡ
            var variant = product.VariantObjs
                .FirstOrDefault(v => v.Group1 == request.Color && v.Group2 == request.Size);

            if (variant == null || variant.Stock < request.Quantity)
                return false;

            // Thêm mới chi tiết hóa đơn nếu chưa có
            if (!prdDtExist)
            {
                var guid = Guid.NewGuid();
                var billDetails = new BillDetailsEntity()
                {
                    Id = guid,
                    BillDetailCode = "BILLDETAILS" + guid.ToString().Substring(0, 8).ToUpper(),
                    BillId = request.IdBill,
                    ProductId = request.IdProduct,
                    CreatedByUserId = request.IdEmployee,
                    Quantity = request.Quantity,
                    Size = request.Size,
                    Color = request.Color,
                    ProductImage = !string.IsNullOrEmpty(variant.ImgUrl) ? variant.ImgUrl : product.ImageUrl,
                    ProductName = product.Name,
                    Price = Convert.ToDecimal(variant.Price),
                    TotalPrice = Convert.ToDecimal(variant.Price) * request.Quantity,
                    Status = 0,
                };

                await _context.BillDetails.AddAsync(billDetails);
                await _context.SaveChangesAsync();

                //Trừ số lượng product
                variant.Stock -= request.Quantity;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return true;
            }
            else // Nếu đã có, cập nhật số lượng và tổng giá
            {
                var exist = _context.BillDetails
                    .FirstOrDefault(c => c.ProductId == request.IdProduct && c.BillId == request.IdBill
                                                                          && c.Color == request.Color &&
                                                                          c.Size == request.Size);

                if (exist == null)
                    return false;

                exist.Quantity += request.Quantity;
                exist.TotalPrice = exist.Quantity * Convert.ToDecimal(variant.Price);
                _context.BillDetails.Update(exist);
                await _context.SaveChangesAsync();
                //Trừ số lượng product
                variant.Stock -= request.Quantity;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<BillDetailsEntity> UpdateQuantity(Guid idBillDetails, int quantity)
    {
        try
        {
            var billDetails = _context.BillDetails.Find(idBillDetails);
            var product = _context.Products.Find(billDetails.ProductId);
            var variant = product.VariantObjs
                .FirstOrDefault(v => v.Group1 == billDetails.Color && v.Group2 == billDetails.Size);
            int CurrentQuantity = Convert.ToInt32(variant.Stock);
            int returnQuantity = billDetails.Quantity - quantity;
            int value = CurrentQuantity += returnQuantity;
            if (value < 0) throw new Exception("Số lượng sản phẩm không đủ");
            variant.Stock = value;
            billDetails.Quantity = quantity;
            _context.Products.Update(product);
            _context.BillDetails.Update(billDetails);
            await _context.SaveChangesAsync();
            billDetails.TotalPrice = billDetails.Quantity * billDetails.Price;
            _context.BillDetails.Update(billDetails);
            await _context.SaveChangesAsync();
            return billDetails;
        }
        catch (Exception e)
        {
            throw new Exception("Lỗi cập nhật số lượng hóa đơn chi tiết", e);
        }
    }

    public async Task<bool> DeleteBillDetails(Guid idBillDetails)
    {
        try
        {
            var billDetails = await _context.BillDetails.FindAsync(idBillDetails);
            if (billDetails == null) throw new Exception("Not Found");
            int billDetailsQuantity = billDetails.Quantity;
            var product = await _context.Products.FindAsync(billDetails.ProductId);
            var variant = product.VariantObjs
                .FirstOrDefault(v => v.Group1 == billDetails.Color && v.Group2 == billDetails.Size);
            int productQuantity = Convert.ToInt32(variant.Stock);
            int updateQuantity = productQuantity + billDetailsQuantity;
            variant.Stock = updateQuantity;
            _context.Products.Update(product);
            _context.BillDetails.Remove(billDetails);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            throw new Exception("Delete Failed");
        }
    }

    public async Task<List<BillDetailsViewModel>> GetBillDetailsByIdBill(Guid idBill)
    {
        List<BillDetailsViewModel> lstBillDetails = await (from billDetails in _context.BillDetails
            join product in _context.Products on billDetails.ProductId equals product.Id
            where billDetails.BillId == idBill
            select new BillDetailsViewModel()
            {
                Id = billDetails.Id,
                IdBill = billDetails.BillId,
                IdProduct = billDetails.ProductEntity.Id,
                Sku= billDetails.SKU,
                Name = product.Name,
                Color = billDetails.Color,
                Size = billDetails.Size,
                Quantity = billDetails.Quantity,
                Price = billDetails.Price,
            }).ToListAsync();
        return lstBillDetails;
    }
}