using Microsoft.EntityFrameworkCore;
using Nest;
using Project.Business.Interface.Repositories;
using Project.Business.Interface.Services;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Business;
using SERP.Framework.Common;
using SERP.Framework.Common.Extensions;
using SERP.Framework.DB.Extensions;
using SERP.NewsMng.Business.Models.QueryModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Project.Business.Implement
{
    public class ContentBaseRepository : IContentBaseRepository
    {
        private readonly ProjectDbContext _context;

        public ContentBaseRepository(ProjectDbContext context)
        {
            _context = context;
        }
        public async Task<List<ContentBase>> SearchContentBaseByKeywordsAsync(string currentTitle)
        {
            if (string.IsNullOrWhiteSpace(currentTitle))
                return new List<ContentBase>();

            string NormalizeString(string str)
            {
                str = str.ToLowerInvariant();
                var normalizedString = str.Normalize(NormalizationForm.FormD);
                var sb = new StringBuilder();
                foreach (var c in normalizedString)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString().Normalize(NormalizationForm.FormC);
            }

            var keywords = NormalizeString(currentTitle)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct()
                .ToList();

            if (!keywords.Any())
                return new List<ContentBase>();

            var allArticles = await _context.ContentBases.AsNoTracking().ToListAsync();

            var matched = allArticles
                .Where(article =>
                {
                    var normTitle = NormalizeString(article.Title ?? "");
                    var normSeoTitle = NormalizeString(article.SeoTitle ?? "");
                    var normContent = NormalizeString(article.Content ?? "");

                    return keywords.Any(k =>
                        normTitle.Contains(k) ||
                        normSeoTitle.Contains(k) ||
                        normContent.Contains(k));
                })
                .OrderByDescending(a => a.PublishStartDate ?? DateTime.MinValue)
                .Take(6)
                .ToList();

            return matched;
        }


        public async Task<ContentBase> FindAsync(Guid id)
        {
            return await _context.ContentBases.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ContentBase>> ListAllAsync(ContentBaseQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var ids = await query.Select(x => x.Id).ToListAsync();
            return await ListByIdsAsync(ids);
        }

        public async Task<IEnumerable<ContentBase>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.ContentBases.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<Pagination<ContentBase>> GetAllAsync(ContentBaseQueryModel queryModel)
        {
            ContentBaseQueryModel contentBaseQueryModel = queryModel;

            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<ContentBase> queryable = BuildQuery(queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                queryable = queryable.OrderByDescending((ContentBase x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value, sortExpression);
        }


        public async Task<int> GetCountAsync(ContentBaseQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            return await query.CountAsync();
        }

        private IQueryable<ContentBase> BuildQuery(ContentBaseQueryModel queryModel)
        {

            var query = _context.ContentBases.AsNoTracking().Where(x => x.IsDeleted == false);

            if (queryModel.Id.HasValue)
            {
                query = query.Where(x => x.Id == queryModel.Id.Value);
            }

            if (!string.IsNullOrEmpty(queryModel.Title))
            {
                query = query.Where(x => x.Title.Contains(queryModel.Title));
            }

            if (queryModel.IsPublish.HasValue)
            {
                query = query.Where(x => x.IsPublish == queryModel.IsPublish.Value);
            }

            if (queryModel.PublishStartDate.HasValue)
            {
                query = query.Where(x => x.PublishStartDate >= queryModel.PublishStartDate.Value);
            }

            if (queryModel.PublishEndDate.HasValue)
            {
                query = query.Where(x => x.PublishEndDate <= queryModel.PublishEndDate.Value);
            }
            if (!string.IsNullOrWhiteSpace(queryModel.SeoUri))
            {
                query = query.Where(x => x.SeoUri.ToLower().Contains(queryModel.SeoUri.ToLower()));
            }
            return query;
        }

        public async Task<ContentBase> SaveAsync(ContentBase entity)
        {
            var result = await SaveAsync(new[] { entity });
            return result.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<ContentBase>> SaveAsync(IEnumerable<ContentBase> entities)
        {
            var updated = new List<ContentBase>();

            foreach (var entity in entities)
            {
                var exist = await _context.ContentBases
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == entity.Id);

                if (exist == null)
                {
                    // Đảm bảo rằng việc tạo mới đối tượng đã được xử lý đúng cách
                    entity.CreateTracking(entity.Id);
                    entity.UpdateTracking(entity.Id);
                    _context.ContentBases.Add(entity);
                    updated.Add(entity);
                }
                else
                {
                    // Xử lý cập nhật nếu đối tượng đã tồn tại
                    _context.Entry(exist).State = EntityState.Detached;

                    exist.Title = entity.Title;
                    exist.SeoUri = entity.SeoUri;
                    exist.SeoTitle = entity.SeoTitle;
                    exist.SeoDescription = entity.SeoDescription;
                    exist.SeoKeywords = entity.SeoKeywords;
                    exist.PublishStartDate = entity.PublishStartDate;
                    exist.PublishEndDate = entity.PublishEndDate;
                    exist.IsPublish = entity.IsPublish;
                    exist.Content = entity.Content;
                    exist.UpdateTracking(entity.Id);

                    _context.ContentBases.Update(exist);
                    updated.Add(exist);
                }
            }

            await _context.SaveChangesAsync();
            return updated;
        }


        public async Task<ContentBase> DeleteAsync(Guid id)
        {
            var exist = await FindAsync(id);
            if (exist == null) throw new Exception("Content not found.");
            exist.IsDeleted = true;
            _context.ContentBases.Update(exist);
            await _context.SaveChangesAsync();
            return exist;
        }

        public async Task<IEnumerable<ContentBase>> DeleteAsync(Guid[] deleteIds)
        {
           throw new NotImplementedException();
        }
        public async Task<List<ContentBase>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.ContentBases
                                   .Where(x => ids.Contains(x.Id))
                                   .ToListAsync();
        }

    }
}
