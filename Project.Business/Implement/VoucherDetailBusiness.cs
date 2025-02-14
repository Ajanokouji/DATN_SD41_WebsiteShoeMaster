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
                id_giam_gia = exist.id_giam_gia,
                id_hoa_don = exist.id_hoa_don,
                create_on_date = exist.create_on_date,
                last_modifi_on_date = exist.last_modifi_on_date,
                Giam_Gia = exist.Giam_Gia,
                Hoa_Don = exist.Hoa_Don
            };

            if (model.id_giam_gia != Guid.Empty)
            {
                update.id_giam_gia = model.id_giam_gia;
            }
            if (model.id_hoa_don != Guid.Empty)
            {
                update.id_hoa_don = model.id_hoa_don;
            }
            if (model.create_on_date != default)
            {
                update.create_on_date = model.create_on_date;
            }
            if (model.last_modifi_on_date != default)
            {
                update.last_modifi_on_date = model.last_modifi_on_date;
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