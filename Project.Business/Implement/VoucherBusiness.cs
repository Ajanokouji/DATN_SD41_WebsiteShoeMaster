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
            var exist = await _voucherRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException("Voucher not found");
            }
            var update = new Voucher
            {
                Id = exist.Id,
                VoucherName = exist.VoucherName,
                VoucherType = exist.VoucherType,
                StartDate = exist.StartDate,
                EndDate = exist.EndDate,
                Status = exist.Status,
                CreatedOnDate = exist.CreatedOnDate,
                LastModifiedOnDate = exist.LastModifiedOnDate
            };

            if (!string.IsNullOrWhiteSpace(model.VoucherName))
            {
                update.VoucherName = model.VoucherName;
            }
            if (model.VoucherType >= 0)
            {
                update.VoucherType = model.VoucherType;
            }
            if (model.StartDate != null)
            {
                update.StartDate = model.StartDate;
            }
            if (model.EndDate != null)
            {
                update.EndDate = model.EndDate;
            }
            if (model.Status >= 0)
            {
                update.Status = model.Status;
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