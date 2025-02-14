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
        var exist = await _iBillDetailsRepository.FindAsync(model.id_hoa_don_chi_tiet);

        if (exist == null)
        {
            throw new ArgumentException(BillConstant.BillNotFound);
        }

        var update = new BillDetails()
        {
            id_hoa_don_chi_tiet = exist.id_hoa_don_chi_tiet,
            id_hoa_don = exist.id_hoa_don,
            id_san_pham_chi_tiet = exist.id_san_pham_chi_tiet,
            ma_hoa_don_chi_tiet = exist.ma_hoa_don_chi_tiet,
            trang_thai = exist.trang_thai,
            so_luong = exist.so_luong,
            don_gia = exist.don_gia,
            ghi_chu = exist.ghi_chu
        };

        if (!string.IsNullOrWhiteSpace(model.ma_hoa_don_chi_tiet))
        {
            update.ma_hoa_don_chi_tiet = model.ma_hoa_don_chi_tiet;
        }

        if (model.id_hoa_don != null)
        {
            update.id_hoa_don = model.id_hoa_don;
        }
        
        if (model.id_san_pham_chi_tiet != null)
        {
            update.id_san_pham_chi_tiet = model.id_san_pham_chi_tiet;
        }
        
        if (model.trang_thai > 0)
        {
            update.trang_thai = model.trang_thai;
        }

        if (model.so_luong > 0)
        {
            update.so_luong = model.so_luong;
        }
        
        if (model.don_gia > 0)
        {
            update.don_gia = model.don_gia;
        }

        if (!string.IsNullOrWhiteSpace(model.ghi_chu))
        {
            update.ghi_chu = model.ghi_chu;
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