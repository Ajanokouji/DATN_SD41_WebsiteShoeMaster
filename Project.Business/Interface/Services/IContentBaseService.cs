using SERP.NewsMng.Business.Models;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Common;
using SERP.NewsMng.Business.Models.QueryModels;

namespace Project.Business.Interface.Services
{
    public interface IContentBaseService
    {
        Task<ContentBaseModel> GetByIdAsync(Guid id);
        Task<IEnumerable<ContentBaseModel>> GetAllAsync();

        Task<ContentBaseModel> CreateAsync(ContentBaseModel model);
        Task<ContentBaseModel> UpdateAsync(ContentBaseModel model);
        Task<ContentBaseModel> DeleteAsync(Guid id);
    }
}
