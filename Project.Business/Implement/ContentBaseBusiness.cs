using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.DbManagement.Entity;
using SERP.NewsMng.Business.Models;

namespace Project.Business.Implement
{
    public class ContentBaseBusiness : IContentBaseBusiness
    {
        private readonly IContentBaseRepository _contentBaseRepository;

        public ContentBaseBusiness(IContentBaseRepository contentBaseRepository)
        {
            _contentBaseRepository = contentBaseRepository;
        }

        public Task AddContentAsync(ContentBaseModel content)
        {
            throw new NotImplementedException();
        }

        public Task DeleteContentAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ContentBaseModel>> GetAllContentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ContentBaseModel> GetContentByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateContentAsync(ContentBaseModel content)
        {
            throw new NotImplementedException();
        }
    }
}
