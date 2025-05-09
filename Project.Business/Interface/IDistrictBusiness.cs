using Project.DbManagement.Entity;
using Project.DbManagement.ViewModels;
using SERP.Dictionary.Models;
using SERP.Framework.Common;

namespace Project.Business.Interface
{
    public interface IDistrictBusiness
    {
        Task<Pagination<District>> GetAllAsync(DistrictQueryModel queryModel);
        Task<IEnumerable<District>> ListAllAsync(DistrictQueryModel queryModel);
        Task<int> GetCountAsync(DistrictQueryModel queryModel);
        Task<IEnumerable<District>> ListByIdsAsync(IEnumerable<Guid> ids);
        Task<District> FindAsync(Guid id);
        Task<District> DeleteAsync(Guid id);
        Task<IEnumerable<District>> DeleteAsync(Guid[] deleteIds);
        Task<District> SaveAsync(District district);
        Task<IEnumerable<District>> SaveAsync(IEnumerable<District> districts);
    }
}
