using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.DbManagement.Entity;
using SERP.NewsMng.Business.Models;

namespace Project.Business.Interface
{
    public interface IContentBaseBusiness
    {
        Task<ContentBaseModel> GetContentByIdAsync(Guid id);
        Task<IEnumerable<ContentBaseModel>> GetAllContentsAsync();
        Task AddContentAsync(ContentBaseModel content);
        Task UpdateContentAsync(ContentBaseModel content);
        Task DeleteContentAsync(Guid id);
    }
}
