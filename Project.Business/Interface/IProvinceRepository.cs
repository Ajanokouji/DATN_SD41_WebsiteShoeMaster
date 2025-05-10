using Project.Business.Model;
using Project.DbManagement.Entity;
using Project.DbManagement.ViewModels;

using SERP.Framework.Common;

namespace Project.Business.Interface.Repositories
{
    public interface IProvinceRepository : IRepository<Province, ProvinceQueryModel>
    {
        Task<Province> SaveAsync(Province province);
        Task<IEnumerable<Province>> SaveAsync(IEnumerable<Province> provinces);
    }
}
