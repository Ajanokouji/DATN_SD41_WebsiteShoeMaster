using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement.Entity;

using SERP.Framework.Common;

namespace Project.Business.Implementation
{
    public class ProvinceBusiness : IProvinceBusiness
    {
        private readonly IProvinceRepository _provinceRepository;

        public ProvinceBusiness(IProvinceRepository provinceRepository)
        {
            _provinceRepository = provinceRepository;
        }

        public async Task<Pagination<Province>> GetAllAsync(ProvinceQueryModel queryModel)
        {
            return await _provinceRepository.GetAllAsync(queryModel);
        }

        public async Task<IEnumerable<Province>> ListAllAsync(ProvinceQueryModel queryModel)
        {
            return await _provinceRepository.ListAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(ProvinceQueryModel queryModel)
        {
            return await _provinceRepository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<Province>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _provinceRepository.ListByIdsAsync(ids);
        }

        public async Task<Province> FindAsync(Guid id)
        {
            return await _provinceRepository.FindAsync(id);
        }

        public async Task<Province> DeleteAsync(Guid id)
        {
            var province = await _provinceRepository.DeleteAsync(id);
            return province;
        }

        public async Task<IEnumerable<Province>> DeleteAsync(Guid[] deleteIds)
        {
            var provinces = await _provinceRepository.DeleteAsync(deleteIds);

            return provinces;
        }

        public async Task<Province> SaveAsync(Province province)
        {
            var res = await _provinceRepository.SaveAsync(province);
            return res;
        }

        public async Task<IEnumerable<Province>> SaveAsync(IEnumerable<Province> provinces)
        {
           var res = await _provinceRepository.SaveAsync(provinces);
            return res;
        }
    }
}
