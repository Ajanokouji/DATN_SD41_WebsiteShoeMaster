using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Common;

namespace Project.Business.Implement
{
    public class VoucherDetailsBusiness : IVoucherDetailsBusiness
    {
        private readonly IVoucherDetailsRepository _voucherDetailsRepository;

        public VoucherDetailsBusiness(IVoucherDetailsRepository voucherDetailsRepository)
        {
            _voucherDetailsRepository = voucherDetailsRepository;
        }

        public async Task<VoucherDetails> DeleteAsync(Guid contentId)
        {
            return await _voucherDetailsRepository.DeleteAsync(contentId);
        }

        public async Task<IEnumerable<VoucherDetails>> DeleteAsync(Guid[] deleteIds)
        {
            return await _voucherDetailsRepository.DeleteAsync(deleteIds);
        }

        public async Task<VoucherDetails> FindAsync(Guid contentId)
        {
            return await _voucherDetailsRepository.FindAsync(contentId);
        }

        public async Task<Pagination<VoucherDetails>> GetAllAsync(VoucherDetailsQueryModel queryModel)
        {
            return await _voucherDetailsRepository.GetAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(VoucherDetailsQueryModel queryModel)
        {
            return await _voucherDetailsRepository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<VoucherDetails>> ListAllAsync(VoucherDetailsQueryModel queryModel)
        {
            return await _voucherDetailsRepository.ListAllAsync(queryModel);
        }

        public async Task<IEnumerable<VoucherDetails>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _voucherDetailsRepository.ListByIdsAsync(ids);
        }

        public async Task<VoucherDetails> PatchAsync(VoucherDetails model)
        {
            var exist = await _voucherDetailsRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException("VoucherDetails not found");
            }
            var update = new VoucherDetails
            {
                Id = exist.Id,
                VoucherId = exist.VoucherId,
                BillId = exist.BillId,
                CreatedOnDate = exist.CreatedOnDate,
                LastModifiedOnDate = exist.LastModifiedOnDate
            };

            if (model.VoucherId != Guid.Empty)
            {
                update.VoucherId = model.VoucherId;
            }
            if (model.BillId != Guid.Empty)
            {
                update.BillId = model.BillId;
            }
            if (model.CreatedOnDate != default)
            {
                update.CreatedOnDate = model.CreatedOnDate;
            }
            if (model.LastModifiedOnDate != default)
            {
                update.LastModifiedOnDate = model.LastModifiedOnDate;
            }
            return await SaveAsync(update);
        }

        public async Task<VoucherDetails> SaveAsync(VoucherDetails voucherDetails)
        {
            var res = await SaveAsync(new[] { voucherDetails });
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<VoucherDetails>> SaveAsync(IEnumerable<VoucherDetails> voucherDetails)
        {
            return await _voucherDetailsRepository.SaveAsync(voucherDetails);
        }
    }
}