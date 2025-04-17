using Project.Business.Model;
using SERP.NewsMng.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Business.Model;
using SERP.NewsMng.Business.Models.QueryModels;
using Project.DbManagement.Entity;
namespace Project.Business.Interface.Repositories
{
    public interface IContentBaseRepository :IRepository<ContentBase, ContentBaseQueryModel>
    {
        Task<ContentBase> FindAsync(Guid id);
        Task<int> GetCountAsync(ContentBaseQueryModel query);
    }
}
