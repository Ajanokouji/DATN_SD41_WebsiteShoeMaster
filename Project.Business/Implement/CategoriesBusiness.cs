using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using SERP.Framework.Common;

namespace Project.Business.Implement
{
    public class CategoriesBusiness : ICategoriesBusiness
    {
        private readonly ICategoriesRepository _categoriesRepository;

        public CategoriesBusiness(ICategoriesRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }

        public async Task<Pagination<CategoriesEntity>> GetAllAsync(CategoriesQueryModel queryModel)
        {
            return await _categoriesRepository.GetAllAsync(queryModel);
        }

        public async Task<IEnumerable<CategoriesEntity>> ListAllAsync(CategoriesQueryModel queryModel)
        {
            return await _categoriesRepository.ListAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(CategoriesQueryModel queryModel)
        {
            return await _categoriesRepository.GetCountAsync(queryModel);
        }


        public async Task<IEnumerable<CategoriesEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _categoriesRepository.ListByIdsAsync(ids);
            return res;
        }

        public async Task<CategoriesEntity> FindAsync(Guid categoryId)
        {
            return await _categoriesRepository.FindAsync(categoryId);
        }

        public async Task<CategoriesEntity> DeleteAsync(Guid categoryId)
        {
            return await _categoriesRepository.DeleteAsync(categoryId);
        }

        public async Task<IEnumerable<CategoriesEntity>> DeleteAsync(Guid[] deleteIds)
        {
            return await _categoriesRepository.DeleteAsync(deleteIds);
        }

        public async Task<CategoriesEntity> SaveAsync(CategoriesEntity categoryEntity)
        {
            var res = await SaveAsync(new[] { categoryEntity });
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<CategoriesEntity>> SaveAsync(IEnumerable<CategoriesEntity> categoryEntities)
        {
            return await _categoriesRepository.SaveAsync(categoryEntities);
        }

        public async Task<CategoriesEntity> PatchAsync(CategoriesEntity model)
        {
            var exist = await _categoriesRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException(ICategoriesRepository.MessageNoTFound);
            }

            var update = new CategoriesEntity
            {
                Id = exist.Id,
                Name = exist.Name,
                Description = exist.Description,
                ParentId = exist.ParentId,
                SortOrder = exist.SortOrder,
                Type = exist.Type,
                ParentPath = exist.ParentPath,
                Code = exist.Code,
                CompleteCode = exist.CompleteCode,
                CompleteName = exist.CompleteName,
                CompletePath = exist.CompletePath,
                CreatedByUserId = exist.CreatedByUserId,
                CreatedOnDate = exist.CreatedOnDate,
                LastModifiedByUserId = exist.LastModifiedByUserId,
                LastModifiedOnDate = exist.LastModifiedOnDate,
                MetadataObj = exist.MetadataObj,
                MetadataJson = exist.MetadataJson,
                Isdeleted = exist.Isdeleted
            };

            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                update.Name = model.Name;
            }
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                update.Description = model.Description;
            }
            if (model.ParentId != Guid.Empty)
            {
                update.ParentId = model.ParentId;
            }
            if (model.SortOrder != 0)
            {
                update.SortOrder = model.SortOrder;
            }
            if (!string.IsNullOrWhiteSpace(model.Type))
            {
                update.Type = model.Type;
            }
            if (!string.IsNullOrWhiteSpace(model.ParentPath))
            {
                update.ParentPath = model.ParentPath;
            }
            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                update.Code = model.Code;
            }
            if (!string.IsNullOrWhiteSpace(model.CompleteCode))
            {
                update.CompleteCode = model.CompleteCode;
            }
            if (!string.IsNullOrWhiteSpace(model.CompleteName))
            {
                update.CompleteName = model.CompleteName;
            }
            if (!string.IsNullOrWhiteSpace(model.CompletePath))
            {
                update.CompletePath = model.CompletePath;
            }
            if (model.MetadataObj != null && model.MetadataObj.Any())
            {
                update.MetadataObj = model.MetadataObj;
            }
            if (!string.IsNullOrWhiteSpace(model.MetadataJson))
            {
                update.MetadataJson = model.MetadataJson;
            }

            return await SaveAsync(update);
        }

    }
}
