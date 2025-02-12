using Project.Business.Interface;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Implement
{
    public class ProductBusiness : IProductBusiness
    {
        private readonly IProductRepository _productRepository;

        public ProductBusiness(IProductRepository productRepository)
        {
            _productRepository=productRepository;
        }

        public async Task<ProductEntity> DeleteAsync(Guid contentId)
        {
            return  await _productRepository.DeleteAsync(contentId);
        }

        public async Task<IEnumerable<ProductEntity>> DeleteAsync(Guid[] deleteIds)
        {
            return await _productRepository.DeleteAsync(deleteIds);
        }

        public async Task<ProductEntity> FindAsync(Guid contentId)
        {
            return await _productRepository.FindAsync(contentId);
        }

        public async Task<Pagination<ProductEntity>> GetAllAsync(ProductQueryModel queryModel)
        {
            return await _productRepository.GetAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(ProductQueryModel queryModel)
        {
           return await _productRepository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<ProductEntity>> ListAllAsync(ProductQueryModel queryModel)
        {
         return await _productRepository.ListAllAsync(queryModel);
        }

        public async Task<IEnumerable<ProductEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
           return await _productRepository.ListByIdsAsync(ids);
        }

        public async Task<ProductEntity> PatchAsync(ProductEntity  model)
        {
            var exist = await _productRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException(ProductConstant.ProductNotFound);
            }
            var update = new ProductEntity
            {
                Id = exist.Id,
                DanhMucId = exist.DanhMucId,
                CompleteCode = exist.CompleteCode,
                CompleteName = exist.CompleteName,
                CompletePath = exist.CompletePath,
                CreatedByUserId = exist.CreatedByUserId,
                CreatedOnDate = exist.CreatedOnDate,
                Description = exist.Description,
                ImageUrl = exist.ImageUrl,
                Isdeleted = exist.Isdeleted,
                LabelsJson = exist.LabelsJson,
                LabelsObjs = exist.LabelsObjs,
                LastModifiedByUserId = exist.LastModifiedByUserId,
                LastModifiedOnDate = exist.LastModifiedOnDate,
                MaSanPham = exist.MaSanPham,
                MetadataObj = exist.MetadataObj,
                PublicOnDate = exist.PublicOnDate,
                RelatedIds = exist.RelatedIds,
                RelatedObjectIds = exist.RelatedObjectIds,
                SortOrder = exist.SortOrder,
                Status = exist.Status,
                TenSanPham = exist.TenSanPham,
                WorkFlowStates = exist.WorkFlowStates
            };

            if (!string.IsNullOrWhiteSpace(model.MaSanPham))
            {
                update.MaSanPham = model.MaSanPham;
            }
            if (!string.IsNullOrWhiteSpace(model.TenSanPham))
            {
                update.TenSanPham = model.TenSanPham;
            }
            if (!string.IsNullOrWhiteSpace(model.Status))
            {
                update.Status = model.Status;
            }
            if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                update.ImageUrl = model.ImageUrl;
            }
            if (!string.IsNullOrWhiteSpace(model.SortOrder))
            {
                update.SortOrder = model.SortOrder;
            }
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                update.Description = model.Description;
            }
            if (model.DanhMucId.HasValue)
            {
                update.DanhMucId = model.DanhMucId;
            }
            if (model.RelatedObjectIds != null && model.RelatedObjectIds.Any())
            {
                update.RelatedObjectIds = model.RelatedObjectIds;
            }
            if (!string.IsNullOrWhiteSpace(model.RelatedIds))
            {
                update.RelatedIds = model.RelatedIds;
            }
            if (!string.IsNullOrWhiteSpace(model.WorkFlowStates))
            {
                update.WorkFlowStates = model.WorkFlowStates;
            }
            if (model.PublicOnDate.HasValue)
            {
                update.PublicOnDate = model.PublicOnDate;
            }
            if (model.MetadataObj != null && model.MetadataObj.Any())
            {
                update.MetadataObj = model.MetadataObj;
            }
            if (!string.IsNullOrWhiteSpace(model.MetadataJson))
            {
                update.MetadataJson = model.MetadataJson;
            }
            if (!string.IsNullOrWhiteSpace(model.CompleteName))
            {
                update.CompleteName = model.CompleteName;
            }
            if (!string.IsNullOrWhiteSpace(model.CompletePath))
            {
                update.CompletePath = model.CompletePath;
            }
            if (!string.IsNullOrWhiteSpace(model.CompleteCode))
            {
                update.CompleteCode = model.CompleteCode;
            }
            if (model.LabelsObjs != null && model.LabelsObjs.Any())
            {
                update.LabelsObjs = model.LabelsObjs;
            }
            if (!string.IsNullOrWhiteSpace(model.LabelsJson))
            {
                update.LabelsJson = model.LabelsJson;
            }
            return await SaveAsync(update);
        }

        public async Task<ProductEntity> SaveAsync(ProductEntity productEntity)
        {
            return  await SaveAsync(new[] {productEntity} )
        }

        public Task<ProductEntity> SaveAsync(IEnumerable<ProductEntity> productEntities)
        {
            throw new NotImplementedException();
        }
    }
}
