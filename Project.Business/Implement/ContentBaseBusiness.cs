using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using SERP.NewsMng.Business.Models.QueryModels;

namespace Project.Business.Implement
{
    public class ContentBaseBusiness : IContentBaseBusiness
    {
        private readonly IContentBaseRepository _contentBaseRepository;

        public ContentBaseBusiness(IContentBaseRepository contentBaseRepository)
        {
            _contentBaseRepository = contentBaseRepository;
        }

        public async Task<ContentBase> DeleteAsync(Guid contentId)
        {
            return await _contentBaseRepository.DeleteAsync(contentId);
        }

        public async Task<IEnumerable<ContentBase>> DeleteAsync(Guid[] deleteIds)
        {
            return await _contentBaseRepository.DeleteAsync(deleteIds);
        }

        public async Task<ContentBase> FindAsync(Guid contentId)
        {
            return await _contentBaseRepository.FindAsync(contentId);
        }

        public async Task<Pagination<ContentBase>> GetAllAsync(ContentBaseQueryModel queryModel)
        {
            return await _contentBaseRepository.GetAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(ContentBaseQueryModel queryModel)
        {
            return await _contentBaseRepository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<ContentBase>> ListAllAsync(ContentBaseQueryModel queryModel)
        {
            return await _contentBaseRepository.ListAllAsync(queryModel);
        }

        public async Task<ContentBase> PatchAsync(ContentBase contentBase)
        {
            var existingContent = await _contentBaseRepository.FindAsync(contentBase.Id);

            if (existingContent == null)
            {
                throw new ArgumentException("Content not found");
            }

            // Cập nhật thông tin của contentBase
            existingContent.Title = contentBase.Title ?? existingContent.Title;
            existingContent.SeoUri = contentBase.SeoUri ?? existingContent.SeoUri;
            existingContent.SeoTitle = contentBase.SeoTitle ?? existingContent.SeoTitle;
            existingContent.SeoDescription = contentBase.SeoDescription ?? existingContent.SeoDescription;
            existingContent.SeoKeywords = contentBase.SeoKeywords ?? existingContent.SeoKeywords;
            existingContent.PublishStartDate = contentBase.PublishStartDate == default ? existingContent.PublishStartDate : contentBase.PublishStartDate;
            existingContent.PublishEndDate = contentBase.PublishEndDate == default ? existingContent.PublishEndDate : contentBase.PublishEndDate;
            existingContent.IsPublish = contentBase.IsPublish;
            existingContent.Content = contentBase.Content;
            //existingContent.IsDeleted = contentBase.IsDeleted;
            existingContent.LastModifiedOnDate = DateTime.Now;

            return await SaveAsync(existingContent);
        }

        public async Task<ContentBase> SaveAsync(ContentBase contentBase)
        {
            return await _contentBaseRepository.SaveAsync(contentBase);
        }

        public async Task<IEnumerable<ContentBase>> SaveAsync(IEnumerable<ContentBase> contentBases)
        {
             return await _contentBaseRepository.SaveAsync(contentBases);
        }
        public async Task<List<ContentBase>> SearchContentBaseByKeywordsAsync(string input)
        {
            return await _contentBaseRepository.SearchContentBaseByKeywordsAsync(input);
        }
        public async Task<List<ContentBase>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _contentBaseRepository.GetByIdsAsync(ids);
        }



    }
}
