using LinqKit;
using Microsoft.EntityFrameworkCore;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.Common.Constants;
using Project.DbManagement;
using Project.DbManagement.Entity;
using Project.DbManagement.ViewModels;
using SERP.Framework.Business;
using SERP.Framework.Common;
using SERP.Framework.DB.Extensions;
using SERP.Framework.Entities.Metadata.Extensions;

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
        var res = await _context.Bills.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id);
        return res;
    }

    public async Task<IEnumerable<BillEntity>> ListAllAsync(BillQueryModel queryModel)
    {
        var query = BuildQuery(queryModel);
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
        IQueryable<BillEntity> queryable = BuildQuery(queryModel);
        string sortExpression = string.Empty;
        if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
        {
            queryable = queryable.OrderByDescending((BillEntity x) => x.LastModifiedOnDate);
        }
        else
        {
            sortExpression = queryModel.Sort;
        }

        return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value,
            sortExpression);
    }

    private IQueryable<BillEntity> BuildQuery(BillQueryModel queryModel)
    {
        IQueryable<BillEntity> query = _context.Bills.AsNoTracking().Where(x => x.IsDeleted != true);

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

        if (queryModel.CustomerId.HasValue)
        {
            query = query.Where(x => x.CustomerId == queryModel.CustomerId.Value);
        }

        if (queryModel.id_don_hang.HasValue)
        {
            query = query.Where(x => x.OrderId == queryModel.id_don_hang.Value);
        }

        if (queryModel.id_phuong_thuc_thanh_toan.HasValue)
        {
            query = query.Where(x => x.PaymentMethodId == queryModel.id_phuong_thuc_thanh_toan.Value);
        }

        if (!string.IsNullOrEmpty(queryModel.BillCode))
        {
            query = query.Where(x => x.BillCode == queryModel.BillCode);
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

        if (!string.IsNullOrEmpty(queryModel.Status))
        {
            query = query.Where(x => x.Status == queryModel.Status);
        }

        if (!string.IsNullOrEmpty(queryModel.PaymentStatus))
        {
            query = query.Where(x => x.PaymentStatus == queryModel.PaymentStatus);
        }

        if (queryModel.create_on_date != null)
        {
            query = query.Where(x => x.CreatedOnDate == queryModel.create_on_date);
        }


        if (queryModel.last_modifi_on_date != null)
        {
            query = query.Where(x => x.LastModifiedOnDate == queryModel.last_modifi_on_date);
        }


        if (!string.IsNullOrEmpty(queryModel.ghi_chu))
        {
            query = query.Where(x => x.Note == queryModel.ghi_chu);
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
        var res = await SaveAsync(new[] { bills });
        return res.FirstOrDefault();
    }

    public virtual async Task<IEnumerable<BillEntity>> SaveAsync(IEnumerable<BillEntity> bills)
    {
        var updated = new List<BillEntity>();

        foreach (var bill in bills)
        {
            var exist = await _context.Bills
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == bill.Id
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
                exist.FinalAmount = bill.FinalAmount;
                exist.VoucherCode = bill.VoucherCode;
                exist.VoucherId = bill.VoucherId;
                exist.PaymentMethod = bill.PaymentMethod;
                exist.Source = bill.Source;
                exist.PaymentStatus = bill.PaymentStatus;
                exist.CreatedOnDate = bill.CreatedOnDate;
                exist.RecipientEmail = bill.RecipientEmail;
                exist.Note = bill.Note;

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
        if (exist == null) throw new Exception(IBillRepository.MessageNotFound);
        exist.IsDeleted = true;
        _context.Bills.Update(exist);
        _context.SaveChangesAsync();
        return exist;
    }


    public Task<IEnumerable<BillEntity>> DeleteAsync(Guid[] deleteIds)
    {
        throw new NotImplementedException();
    }

    public List<BillEntity> GetAllPendingBill()
    {
        return _context.Bills.Where(hd => hd.Status == BillConstants.PendingConfirmation && hd.Source == Source.OffLine).OrderBy(hd => hd.CreatedOnDate).ToList();
    }

    public bool CreatePendingBill(Guid idEmployee)
    {
        try
        {
            BillEntity bill = new BillEntity();
            bill.Id = Guid.NewGuid();
            bill.BillCode = "BILL" + (bill.Id).ToString().Substring(0, 8).ToUpper();
            bill.EmployeeId = idEmployee;
            bill.CreatedByUserId = idEmployee;
            bill.CreatedOnDate = DateTime.UtcNow;
            bill.DiscountAmount = 0;
            bill.AmountAfterDiscount = 0;
            bill.AmountToPay = 0;
            bill.Source = Source.OffLine;
            bill.Status = BillConstants.PendingConfirmation;
            _context.Bills.Add(bill);
            _context.SaveChanges();
            return true;

            // var listBills = _context.Bills.Where(b => b.Status == "Pending").ToList();
            // if (listBills.Count() < 20)
            // {
            //     BillEntity bill = new BillEntity();
            //     bill.Id = Guid.NewGuid();
            //     bill.BillCode = "BILL" + (bill.Id).ToString().Substring(0, 8).ToUpper();
            //     bill.EmployeeId = idEmployee;
            //     bill.CreatedByUserId = idEmployee;
            //     bill.CreatedOnDate = DateTime.Now;
            //     bill.Status = "Pending";
            //     _context.Bills.Add(bill);
            //     _context.SaveChanges();
            //     return true;
            // }
            // else
            // {
            //     return false;
            // }
        }
        catch
        {
            return false;
        }
    }

    public bool DeletePendingBill(Guid idBill)
    {
        try
        {
            var bill = _context.Bills.SingleOrDefault(b => b.Id == idBill);
            if (bill == null) return false;

            var listBilldetails = _context.BillDetails.Where(bd => bd.BillId == idBill).ToList();

            foreach (var billDetails in listBilldetails)
            {
                var product = _context.Products.SingleOrDefault(p => p.Id == billDetails.ProductId);
                var variant = product.VariantObjs.FirstOrDefault(v => v.Group1 == billDetails.Color && v.Group2 == billDetails.Size);
                if (product == null) continue;

                variant.Stock += billDetails.Quantity;
                _context.Products.Update(product);
            }

            _context.BillDetails.RemoveRange(listBilldetails);
            _context.Bills.Remove(bill);
            _context.SaveChanges();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public BillViewModel GetPDBillById(Guid idBill)
    {
        // List<BillDetailsViewModel> listBillDetails = (from bdt in _context.BillDetails
        //     join prd in _context.Products on bdt.ProductId equals prd.Id
        //     where bdt.BillId == idBill
        //     select new BillDetailsViewModel()
        //     {
        //         Id = bdt.Id,
        //         IdBill = bdt.BillId,
        //         IdProduct = prd.Id,
        //         Name = prd.MetadataObj.GetMetadatavalue("Name"),
        //         Color = prd.MetadataObj.GetMetadatavalue("Color"),
        //         Size = prd.MetadataObj.GetMetadatavalue("Size"),
        //         Quantity = bdt.Quantity,
        //         Price = bdt.Price,
        //     }).AsEnumerable().Reverse().ToList();
        // var result = (from bill in _context.Bills.ToList() 
        //     join customers in _context.Customers.ToList() on bill.CustomerId equals customers.Id
        //     where bill.Id == idBill
        //     select new BillViewModel()
        //     {
        //         Id = bill.Id,
        //         BillCode = bill.BillCode,
        //         IdClient = customers?.Id,
        //         NameClient = customers?.Name,
        //         listBillDetails = listBillDetails,
        //         Note = bill.Note == null ? "" : bill.Note,
        //     }).FirstOrDefault();
        // return result;
        List<BillDetailsViewModel> listBillDetails = (from bdt in _context.BillDetails
            join prd in _context.Products on bdt.ProductId equals prd.Id
            where bdt.BillId == idBill
            orderby bdt.CreatedOnDate descending
            select new BillDetailsViewModel()
            {
                Id = bdt.Id,
                IdBill = bdt.BillId,
                IdProduct = prd.Id,
                Image = bdt.ProductImage,
                Name = prd.Name,
                Color = bdt.Color,
                Size = bdt.Size,
                Quantity = bdt.Quantity,
                Price = bdt.Price,
            }).AsEnumerable().Reverse().ToList();
        var result = (from bill in _context.Bills.ToList()
            where bill.Id == idBill
            select new BillViewModel
            {
                Id = idBill,
                BillCode = bill.BillCode,
                listBillDetails = listBillDetails
            }).FirstOrDefault();
        return result;
    }

    public bool PaymentBill(PaymentBillRequest bill)
    {
        var customer = _context.Customers.FirstOrDefault(c => c.Id == bill.IdCustomer);
        var update = _context.Bills.FirstOrDefault(p => p.Id == bill.Id);
        // if (update.Source == Source.Website) return false;
        //Lưu tiền vào HDCT
        var lstBillDetails = _context.BillDetails.Where(c => c.BillId == bill.Id).ToList();
        //Xóa lsthdct có số lượng = 0
        var delete = lstBillDetails.Where(c => c.Quantity == 0).ToList();
        _context.BillDetails.RemoveRange(delete);
        _context.SaveChanges();
        foreach (var item in lstBillDetails)
        {
            var totalPrice = item.Quantity * item.Price;
            item.TotalPrice = totalPrice;
            item.LastModifiedByUserId = bill.IdEmployee;

            _context.BillDetails.Update(item);
            _context.SaveChanges();
        }

        // Update bill
        update.EmployeeId = bill.IdEmployee;
        update.LastModifiedByUserId = bill.IdEmployee;
        //update.PaymentDate = bill.PaymentDate;
        update.Status = bill.status;
        update.TotalAmount = bill.TotalPrice;
        update.PaymentMethod = bill.PaymentMethod;
        update.CustomerId = bill.IdCustomer;
        update.RecipientName = customer.Name;
        update.RecipientEmail = customer.Email;
        update.RecipientPhone = customer.PhoneNumber;
        update.RecipientAddress = customer.Address;
        update.AmountAfterDiscount = bill.TotalPrice;
        update.AmountToPay = bill.TotalPrice;
        _context.Bills.Update(update);
        _context.SaveChanges();
        return true;
    }

    public async Task<BillEntity> FindByCodeAsync(string billCode)
    {
        var res = await _context.Bills.FirstOrDefaultAsync(x => x.BillCode==billCode &&x.IsDeleted != true);
        return res;
    }
}