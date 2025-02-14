using Project.Business.Model;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Interface.Repositories
{
    public interface ICategoriesRepository : IRepository<CategoriesEntity, CategoriesQueryModel>
    {
        protected const string MessageNoTFound = "Cart not found";
        Task<CategoriesEntity> SaveAsync(CategoriesEntity categories);
        Task<IEnumerable<CategoriesEntity>> SaveAsync(IEnumerable<CategoriesEntity> categories);
    }
}
