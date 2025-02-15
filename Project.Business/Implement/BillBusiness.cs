using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using SERP.Framework.Common;

namespace Project.Business.Implement;

public class BillBusiness : IBillBusiness
{
    private readonly IBillRepository _billRepository;

    public BillBusiness(IBillRepository billRepository)
    {
        _billRepository = billRepository;
    }

    public async Task<BillEntity> DeleteAsync(Guid contentId)
    {
        return await _billRepository.DeleteAsync(contentId);
    }

    public async Task<IEnumerable<BillEntity>> DeleteAsync(Guid[] deleteIds)
    {
        return await _billRepository.DeleteAsync(deleteIds);
    }

    public async Task<BillEntity> FindAsync(Guid contentId)
    {
        return await _billRepository.FindAsync(contentId);
    }

    public async Task<Pagination<BillEntity>> GetAllAsync(BillQueryModel queryModel)
    {
        return await _billRepository.GetAllAsync(queryModel);
    }

    public async Task<int> GetCountAsync(BillQueryModel queryModel)
    {
        return await _billRepository.GetCountAsync(queryModel);
    }

    public async Task<IEnumerable<BillEntity>> ListAllAsync(BillQueryModel queryModel)
    {
        return await _billRepository.ListAllAsync(queryModel);
    }

    public async Task<IEnumerable<BillEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _billRepository.ListByIdsAsync(ids);
    }

    public async Task<BillEntity> PatchAsync(BillEntity model)
    {
        var exist = await _billRepository.FindAsync(model.Id);

        if (exist == null)
        {
            throw new ArgumentException(BillConstant.BillNotFound);
        }

        var update = new BillEntity()
        {
            Id = exist.Id,
            EmployeeId = exist.EmployeeId,
            CustomerId = exist.CustomerId,
            OrderId = exist.OrderId,
            PaymentMethodId = exist.PaymentMethodId,
            BillCode = exist.BillCode,
            RecipientName = exist.RecipientName,
            RecipientPhone = exist.RecipientPhone,
            RecipientAddress = exist.RecipientAddress,
            TotalAmount = exist.TotalAmount,
            DiscountAmount = exist.DiscountAmount,
            AmountAfterDiscount = exist.AmountAfterDiscount,
            AmountToPay = exist.AmountToPay,
            Status = exist.Status,
            PaymentStatus = exist.PaymentStatus,
            CreatedOnDate = exist.CreatedOnDate,
            RecipientEmail = exist.RecipientEmail,
            LastModifiedOnDate = exist.LastModifiedOnDate,
            UpdateBy = exist.UpdateBy,
            Notes = exist.Notes,
            LastModifiedByUserId = exist.LastModifiedByUserId
        };

        if (!string.IsNullOrWhiteSpace(model.BillCode))
        {
            update.BillCode = model.BillCode;
        }

        if (model.EmployeeId != null)
        {
            update.EmployeeId = model.EmployeeId;
        }
        
        if (model.CustomerId != null)
        {
            update.CustomerId = model.CustomerId;
        }
        
        if (model.OrderId != null)
        {
            update.OrderId = model.OrderId;
        }
        
        if (model.PaymentMethodId != null)
        {
            update.PaymentMethodId = model.PaymentMethodId;
        }

        if (!string.IsNullOrWhiteSpace(model.RecipientName))
        {
            update.RecipientName = model.RecipientName;
        }

        if (!string.IsNullOrWhiteSpace(model.RecipientPhone))
        {
            update.RecipientPhone = model.RecipientPhone;
        }

        if (!string.IsNullOrWhiteSpace(model.RecipientEmail))
        {
            update.RecipientEmail = model.RecipientEmail;
        }

        if (!string.IsNullOrWhiteSpace(model.RecipientAddress))
        {
            update.RecipientAddress = model.RecipientAddress;
        }

        if (model.TotalAmount > 0)
        {
            update.TotalAmount = model.TotalAmount;
        }

        if (model.DiscountAmount > 0)
        {
            update.DiscountAmount = model.DiscountAmount;
        }  

        if (model.AmountAfterDiscount > 0)
        {
            update.AmountAfterDiscount = model.AmountAfterDiscount;
        }

        if (model.AmountAfterDiscount > 0)
        {
            update.AmountAfterDiscount = model.AmountAfterDiscount;
        }
        
        if (model.AmountToPay > 0)
        {
            update.AmountToPay = model.AmountToPay;
        }

        if (model.Status > 0)
        {
            update.Status = model.Status;
        }

        if (model.PaymentStatus > 0)
        {
            update.PaymentStatus = model.PaymentStatus;
        }

        if (model.CreatedOnDate != null)
        {
            update.CreatedOnDate = model.CreatedOnDate;
        }

        if (model.LastModifiedOnDate != null)
        {
            update.LastModifiedOnDate = model.LastModifiedOnDate;
        }

        if (!string.IsNullOrWhiteSpace(model.UpdateBy))
        {
            update.UpdateBy = model.UpdateBy;
        }

        if (!string.IsNullOrWhiteSpace(model.Notes))
        {
            update.Notes = model.Notes;
        }
        
        return await SaveAsync(update);
    }

    public async Task<BillEntity> SaveAsync(BillEntity billEntity)
    {
        return await SaveAsync(new[] { billEntity });
    }

    public Task<BillEntity> SaveAsync(IEnumerable<BillEntity> billEntities)
    {
        throw new NotImplementedException();
    }
}