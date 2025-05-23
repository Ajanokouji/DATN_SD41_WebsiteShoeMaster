using Project.Business.Model;
using SERP.NewsMng.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Business.Model;
using SERP.NewsMng.Business.Models.QueryModels;
using Project.DbManagement.Entity;
using Project.DbManagement;
namespace Project.Business.Interface.Repositories
{
    public interface IContentBaseRepository :IRepository<ContentBase, ContentBaseQueryModel>
    {
        Task<ContentBase> FindAsync(Guid id);
        Task<int> GetCountAsync(ContentBaseQueryModel query);
        Task<ContentBase> SaveAsync(ContentBase article);
        Task<IEnumerable<ContentBase>> SaveAsync(IEnumerable<ContentBase> entities);
        Task<List<ContentBase>> SearchContentBaseByKeywordsAsync(string input);
        Task<List<ContentBase>> GetByIdsAsync(IEnumerable<Guid> ids);


    }
}
