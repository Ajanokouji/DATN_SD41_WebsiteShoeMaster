using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Common;

namespace Project.Business.Implement
{
    public class VoucherBusiness : IVoucherBusiness
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherBusiness(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<Voucher> DeleteAsync(Guid id)
        {
            return await _voucherRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Voucher>> DeleteAsync(Guid[] deleteIds)
        {
            return await _voucherRepository.DeleteAsync(deleteIds);
        }

        public async Task<Voucher> FindAsync(Guid id)
        {
            return await _voucherRepository.FindAsync(id);
        }

        public async Task<Pagination<Voucher>> GetAllAsync(VoucherQueryModel queryModel)
        {
            return await _voucherRepository.GetAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(VoucherQueryModel queryModel)
        {
            return await _voucherRepository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<Voucher>> ListAllAsync(VoucherQueryModel queryModel)
        {
            return await _voucherRepository.ListAllAsync(queryModel);
        }

        public async Task<IEnumerable<Voucher>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _voucherRepository.ListByIdsAsync(ids);
        }

        public async Task<Voucher> PatchAsync(Voucher model)
        {
            var exist = await _voucherRepository.FindAsync(model.id_giam_gia);

            if (exist == null)
            {
                throw new ArgumentException("Voucher not found");
            }
            var update = new Voucher
            {
                id_giam_gia = exist.id_giam_gia,
                ten_giam_gia = exist.ten_giam_gia,
                loai_giam_gia = exist.loai_giam_gia,
                thoi_gian_bat_dau = exist.thoi_gian_bat_dau,
                thoi_gian_ket_thuc = exist.thoi_gian_ket_thuc,
                trang_thai = exist.trang_thai,
                create_on_date = exist.create_on_date,
                last_modifi_on_date = exist.last_modifi_on_date
            };

            if (!string.IsNullOrWhiteSpace(model.ten_giam_gia))
            {
                update.ten_giam_gia = model.ten_giam_gia;
            }
            if (model.loai_giam_gia >= 0)
            {
                update.loai_giam_gia = model.loai_giam_gia;
            }
            if (model.thoi_gian_bat_dau != null)
            {
                update.thoi_gian_bat_dau = model.thoi_gian_bat_dau;
            }
            if (model.thoi_gian_ket_thuc != null)
            {
                update.thoi_gian_ket_thuc = model.thoi_gian_ket_thuc;
            }
            if (model.trang_thai >= 0)
            {
                update.trang_thai = model.trang_thai;
            }
            return await SaveAsync(update);
        }

        public async Task<Voucher> SaveAsync(Voucher voucher)
        {
            var res = await SaveAsync(new[] { voucher });
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<Voucher>> SaveAsync(IEnumerable<Voucher> vouchers)
        {
            return await _voucherRepository.SaveAsync(vouchers);
        }
    }
}