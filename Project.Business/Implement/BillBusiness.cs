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
        var exist = await _billRepository.FindAsync(model.id_hoa_don);

        if (exist == null)
        {
            throw new ArgumentException(BillConstant.BillNotFound);
        }

        var update = new BillEntity()
        {
            id_hoa_don = exist.id_hoa_don,
            id_nhan_vien = exist.id_nhan_vien,
            id_khach_hang = exist.id_khach_hang,
            id_don_hang = exist.id_don_hang,
            id_phuong_thuc_thanh_toan = exist.id_phuong_thuc_thanh_toan,
            ma_hoa_don = exist.ma_hoa_don,
            ten_khach_nhan = exist.ten_khach_nhan,
            so_dien_thoai_khach_nhan = exist.so_dien_thoai_khach_nhan,
            dia_chi_nhan = exist.dia_chi_nhan,
            tong_tien = exist.tong_tien,
            tong_tien_khuyen_mai = exist.tong_tien_khuyen_mai,
            tong_tien_sau_khuyen_mai = exist.tong_tien_sau_khuyen_mai,
            tong_tien_phai_thanh_toan = exist.tong_tien_phai_thanh_toan,
            trang_thai = exist.trang_thai,
            trang_thai_thanh_toan = exist.trang_thai_thanh_toan,
            create_on_date = exist.create_on_date,
            email_khach_nhan = exist.email_khach_nhan,
            last_modifi_on_date = exist.last_modifi_on_date,
            update_by = exist.update_by,
            ghi_chu = exist.ghi_chu,
            LastModifiedByUserId = exist.LastModifiedByUserId
        };

        if (!string.IsNullOrWhiteSpace(model.ma_hoa_don))
        {
            update.ma_hoa_don = model.ma_hoa_don;
        }

        if (model.id_nhan_vien != null)
        {
            update.id_nhan_vien = model.id_nhan_vien;
        }
        
        if (model.id_khach_hang != null)
        {
            update.id_nhan_vien = model.id_nhan_vien;
        }
        
        if (model.id_don_hang != null)
        {
            update.id_nhan_vien = model.id_nhan_vien;
        }
        
        if (model.id_phuong_thuc_thanh_toan != null)
        {
            update.id_nhan_vien = model.id_nhan_vien;
        }

        if (!string.IsNullOrWhiteSpace(model.ten_khach_nhan))
        {
            update.ten_khach_nhan = model.ten_khach_nhan;
        }

        if (!string.IsNullOrWhiteSpace(model.so_dien_thoai_khach_nhan))
        {
            update.so_dien_thoai_khach_nhan = model.so_dien_thoai_khach_nhan;
        }

        if (!string.IsNullOrWhiteSpace(model.email_khach_nhan))
        {
            update.email_khach_nhan = model.email_khach_nhan;
        }

        if (!string.IsNullOrWhiteSpace(model.dia_chi_nhan))
        {
            update.dia_chi_nhan = model.dia_chi_nhan;
        }

        if (model.tong_tien > 0)
        {
            update.tong_tien = model.tong_tien;
        }

        if (model.tong_tien_khuyen_mai > 0)
        {
            update.tong_tien_khuyen_mai = model.tong_tien_khuyen_mai;
        }  

        if (model.tong_tien_sau_khuyen_mai > 0)
        {
            update.tong_tien_sau_khuyen_mai = model.tong_tien_sau_khuyen_mai;
        }

        if (model.tong_tien_sau_khuyen_mai > 0)
        {
            update.tong_tien_sau_khuyen_mai = model.tong_tien_sau_khuyen_mai;
        }
        
        if (model.tong_tien_phai_thanh_toan > 0)
        {
            update.tong_tien_phai_thanh_toan = model.tong_tien_phai_thanh_toan;
        }

        if (model.trang_thai > 0)
        {
            update.trang_thai = model.trang_thai;
        }

        if (model.trang_thai_thanh_toan > 0)
        {
            update.trang_thai_thanh_toan = model.trang_thai_thanh_toan;
        }

        if (model.create_on_date != null)
        {
            update.create_on_date = model.create_on_date;
        }

        if (model.last_modifi_on_date != null)
        {
            update.last_modifi_on_date = model.last_modifi_on_date;
        }

        if (!string.IsNullOrWhiteSpace(model.update_by))
        {
            update.update_by = model.update_by;
        }

        if (!string.IsNullOrWhiteSpace(model.ghi_chu))
        {
            update.ghi_chu = model.ghi_chu;
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