using Project.Business.Model;
using Project.DbManagement.Entity;

using SERP.Framework.Common;

namespace Project.Business.Interface
{
    public interface IProvinceBusiness
    {
        Task<Pagination<Province>> GetAllAsync(ProvinceQueryModel queryModel);
        Task<IEnumerable<Province>> ListAllAsync(ProvinceQueryModel queryModel);
        Task<int> GetCountAsync(ProvinceQueryModel queryModel);
        Task<IEnumerable<Province>> ListByIdsAsync(IEnumerable<Guid> ids);
        Task<Province> FindAsync(Guid id);
        Task<Province> DeleteAsync(Guid id);
        Task<IEnumerable<Province>> DeleteAsync(Guid[] deleteIds);
        Task<Province> SaveAsync(Province province);
        Task<IEnumerable<Province>> SaveAsync(IEnumerable<Province> provinces);
    }
}