using Microsoft.EntityFrameworkCore;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Business;
using SERP.Framework.Common;
using SERP.Framework.DB.Extensions;

namespace Project.Business.Implement
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly ProjectDbContext _context;

        public CategoriesRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<CategoriesEntity> FindAsync(Guid id)
        {
            var res = await _context.Categories.FindAsync(id);
            return res;
        }

        public async Task<IEnumerable<CategoriesEntity>> ListAllAsync(CategoriesQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var resId = await query.Select(x => x.Id).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;
        }

        public async Task<IEnumerable<CategoriesEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.Categories.Where(x => ids.Contains(x.Id)).ToListAsync();
            return res;
        }

        public async Task<Pagination<CategoriesEntity>> GetAllAsync(CategoriesQueryModel queryModel)
        {
            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<CategoriesEntity> queryable = BuildQuery(queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                queryable = queryable.OrderByDescending(x => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value, sortExpression);
        }

        private IQueryable<CategoriesEntity> BuildQuery(CategoriesQueryModel queryModel)
        {
            IQueryable<CategoriesEntity> query = _context.Categories.AsNoTracking().Where(x => x.Isdeleted != true);

            if (queryModel.Id.HasValue)
            {
                query = query.Where(x => x.Id == queryModel.Id.Value);
            }

            if (!string.IsNullOrEmpty(queryModel.Code))
            {
                query = query.Where(x => x.Code == queryModel.Code);
            }

            if (!string.IsNullOrEmpty(queryModel.Name))
            {
                query = query.Where(x => x.Name.Contains(queryModel.Name));
            }

            if (!string.IsNullOrEmpty(queryModel.Description))
            {
                query = query.Where(x => x.Description.Contains(queryModel.Description));
            }

            if (queryModel.ParentId != Guid.Empty)
            {
                query = query.Where(x => x.ParentId == queryModel.ParentId);
            }

            if (!string.IsNullOrEmpty(queryModel.Type))
            {
                query = query.Where(x => x.Type == queryModel.Type);
            }

            return query;
        }

        public async Task<int> GetCountAsync(CategoriesQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }

        public async Task<CategoriesEntity> SaveAsync(CategoriesEntity category)
        {
            var res = await SaveAsync(new[] { category });
            return res.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<CategoriesEntity>> SaveAsync(IEnumerable<CategoriesEntity> categories)
        {
            var updated = new List<CategoriesEntity>();

            foreach (var category in categories)
            {
                var exist = await _context.Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == category.Id);

                if (exist == null)
                {
                    category.CreateTracking(category.Id);
                    category.UpdateTracking(category.Id);
                    _context.Categories.Add(category);
                    updated.Add(category);
                }
                else
                {
                    _context.Entry(exist).State = EntityState.Detached;
                    exist.Name = category.Name;
                    exist.Description = category.Description;
                    exist.ParentId = category.ParentId;
                    exist.SortOrder = category.SortOrder;
                    exist.Type = category.Type;
                    exist.ParentPath = category.ParentPath;
                    exist.Code = category.Code;
                    exist.CompleteCode = category.CompleteCode;
                    exist.CompleteName = category.CompleteName;
                    exist.CompletePath = category.CompletePath;
                    exist.MetadataObj = category.MetadataObj;
                    exist.LastModifiedOnDate = category.LastModifiedOnDate;
                    exist.CreatedOnDate = category.CreatedOnDate;

                    category.UpdateTracking(category.Id);
                    _context.Categories.Update(exist);
                    updated.Add(exist);
                }
            }
            await _context.SaveChangesAsync();

            return updated;
        }

        public async Task<CategoriesEntity> DeleteAsync(Guid id)
        {
            var exist = await FindAsync(id);
            if (exist == null) throw new Exception(ICategoriesRepository.MessageNoTFound);
            exist.Isdeleted = true;
            _context.Categories.Update(exist);
            await _context.SaveChangesAsync();
            return exist;
        }

        public Task<IEnumerable<CategoriesEntity>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }
    }
}
