using Project.DbManagement.Entity;
using Project.DbManagement.ViewModels;
using SERP.Dictionary.Models;
using SERP.Framework.Common;

namespace Project.Business.Interface.Repositories
{
    public interface IDistrictRepository : IRepository<District, DistrictQueryModel>
    {
        Task<District> SaveAsync(District district);
        Task<IEnumerable<District>> SaveAsync(IEnumerable<District> districts);
    }
}
