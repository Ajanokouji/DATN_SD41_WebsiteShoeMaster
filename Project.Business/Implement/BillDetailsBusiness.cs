using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using SERP.Framework.Common;

namespace Project.Business.Implement;

public class BillDetailsBusiness : IBillDetailsBusiness
{
    private readonly IBillDetailsRepository _iBillDetailsRepository;

    public BillDetailsBusiness(IBillDetailsRepository iBillDetailsRepository)
    {
        _iBillDetailsRepository = iBillDetailsRepository;
    }

    public async Task<BillDetails> DeleteAsync(Guid contentId)
    {
        return await _iBillDetailsRepository.DeleteAsync(contentId);
    }

    public async Task<IEnumerable<BillDetails>> DeleteAsync(Guid[] deleteIds)
    {
        return await _iBillDetailsRepository.DeleteAsync(deleteIds);
    }

    public async Task<BillDetails> FindAsync(Guid contentId)
    {
        return await _iBillDetailsRepository.FindAsync(contentId);
    }

    public async Task<Pagination<BillDetails>> GetAllAsync(BillDetailsQueryModel queryModel)
    {
        return await _iBillDetailsRepository.GetAllAsync(queryModel);
    }

    public async Task<int> GetCountAsync(BillDetailsQueryModel queryModel)
    {
        return await _iBillDetailsRepository.GetCountAsync(queryModel);
    }

    public async Task<IEnumerable<BillDetails>> ListAllAsync(BillDetailsQueryModel queryModel)
    {
        return await _iBillDetailsRepository.ListAllAsync(queryModel);
    }

    public async Task<IEnumerable<BillDetails>> ListByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _iBillDetailsRepository.ListByIdsAsync(ids);
    }

    public async Task<BillDetails> PatchAsync(BillDetails model)
    {
        var exist = await _iBillDetailsRepository.FindAsync(model.Id);

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
        return await SaveAsync(new[] { billDetails });
    }

    public Task<BillDetails> SaveAsync(IEnumerable<BillDetails> billDetailsEntities)
    {
        throw new NotImplementedException();
    }
}