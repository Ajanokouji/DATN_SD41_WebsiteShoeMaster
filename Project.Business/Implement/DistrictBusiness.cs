using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.DbManagement.Entity;
using Project.DbManagement.ViewModels;
using SERP.Dictionary.Models;
using SERP.Framework.Common;

namespace Project.Business.Implementation
{
    public class DistrictBusiness : IDistrictBusiness
    {
        private readonly IDistrictRepository _districtRepository;

        public DistrictBusiness(IDistrictRepository districtRepository)
        {
            _districtRepository = districtRepository;
        }

        public async Task<Pagination<District>> GetAllAsync(DistrictQueryModel queryModel)
        {
            return await _districtRepository.GetAllAsync(queryModel);
        }

        public async Task<IEnumerable<District>> ListAllAsync(DistrictQueryModel queryModel)
        {
            return await _districtRepository.ListAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(DistrictQueryModel queryModel)
        {
            return await _districtRepository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<District>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _districtRepository.ListByIdsAsync(ids);
        }

        public async Task<District> FindAsync(Guid id)
        {
            return await _districtRepository.FindAsync(id);
        }

        public async Task<District> DeleteAsync(Guid id)
        {
            var district = await _districtRepository.DeleteAsync(id);
            return district;
        }

        public async Task<IEnumerable<District>> DeleteAsync(Guid[] deleteIds)
        {
            var districts = await _districtRepository.DeleteAsync(deleteIds);
            return districts;
        }

        public async Task<District> SaveAsync(District district)
        {
            var res = await _districtRepository.SaveAsync(district);
            return res;
        }

        public async Task<IEnumerable<District>> SaveAsync(IEnumerable<District> districts)
        {
            var res = await _districtRepository.SaveAsync(districts);
            return res;
        }
    }
}
