using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using SERP.Framework.Common;

namespace Project.Business.Implement;

public class BillDetailsBusiness : IBillDetailsBusiness
{
    private readonly IBillDetailsRepository _billDetailsRepository;

    public BillDetailsBusiness(IBillDetailsRepository iBillDetailsRepository)
    {
        _billDetailsRepository = iBillDetailsRepository;
    }

    public async Task<BillDetails> DeleteAsync(Guid contentId)
    {
        return await _billDetailsRepository.DeleteAsync(contentId);
    }

    public async Task<IEnumerable<BillDetails>> DeleteAsync(Guid[] deleteIds)
    {
        return await _billDetailsRepository.DeleteAsync(deleteIds);
    }

    public async Task<BillDetails> FindAsync(Guid contentId)
    {
        return await _billDetailsRepository.FindAsync(contentId);
    }

    public async Task<Pagination<BillDetails>> GetAllAsync(BillDetailsQueryModel queryModel)
    {
        return await _billDetailsRepository.GetAllAsync(queryModel);
    }

    public async Task<int> GetCountAsync(BillDetailsQueryModel queryModel)
    {
        return await _billDetailsRepository.GetCountAsync(queryModel);
    }

    public async Task<IEnumerable<BillDetails>> ListAllAsync(BillDetailsQueryModel queryModel)
    {
        return await _billDetailsRepository.ListAllAsync(queryModel);
    }

    public async Task<IEnumerable<BillDetails>> ListByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _billDetailsRepository.ListByIdsAsync(ids);
    }

    public async Task<BillDetails> PatchAsync(BillDetails model)
    {
        var exist = await _billDetailsRepository.FindAsync(model.Id);

        if (exist == null)
        {
            throw new ArgumentException(BillConstant.BillNotFound);
        }

        var update = new BillDetails()
        {
            Id = exist.Id,
            BillId = exist.BillId,
            ProductDetailId = exist.ProductDetailId,
            BillDetailCode = exist.BillDetailCode,
            Status = exist.Status,
            Quantity = exist.Quantity,
            Price = exist.Price,
            Notes = exist.Notes
        };

        if (!string.IsNullOrWhiteSpace(model.BillDetailCode))
        {
            update.BillDetailCode = model.BillDetailCode;
        }

        if (model.BillId != null)
        {
            update.BillId = model.BillId;
        }
        
        if (model.ProductDetailId != null)
        {
            update.ProductDetailId = model.ProductDetailId;
        }
        
        if (model.Status > 0)
        {
            update.Status = model.Status;
        }

        if (model.Quantity > 0)
        {
            update.Quantity = model.Quantity;
        }
        
        if (model.Price > 0)
        {
            update.Price = model.Price;
        }

        if (!string.IsNullOrWhiteSpace(model.Notes))
        {
            update.Notes = model.Notes;
        }
        
        return await SaveAsync(update);
    }

    public async Task<BillDetails> SaveAsync(BillDetails billDetails)
    {
        var res = await SaveAsync(new[] { billDetails });
        return res.FirstOrDefault();
    }

    public async Task<IEnumerable<BillDetails>> SaveAsync(IEnumerable<BillDetails> billDetailsEntities)
    {
        return await _billDetailsRepository.SaveAsync(billDetailsEntities);
    }
}