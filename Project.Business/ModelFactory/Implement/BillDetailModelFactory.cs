using Microsoft.VisualBasic;
using Project.Business.Model;
using Project.DbManagement;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.ModelFactory.Implement
{
    class BillDetailModelFactory : IBillDetailModelFactory
    {
        public async Task<List<BillDetailsEntity>> ConvertEntities(IEnumerable<BillDetailModel> models)
        {
             var res = new List<BillDetailsEntity>();
            foreach (var model in models)
            {
                res.Add(await ConvertEntity(model) );
            }
            return res;
        }

        public async Task<BillDetailsEntity> ConvertEntity(BillDetailModel model)
        {
           var res = new BillDetailsEntity() { 
            Id = model.Id,
            ProductId= model.ProductId,
            BillDetailCode =  model.BillDetailCode,
            BillId = model.BillId,
            Color = model.Color,
            CreatedByUserId = model.CreatedByUserId,
            CreatedOnDate= model.CreatedOnDate,
            LastModifiedOnDate = model.LastModifiedOnDate,
            Notes = model.Notes,
            SKU=model.SKU,
            Status = model.Status,
            Size = model.Size,
            LastModifiedByUserId = model.LastModifiedByUserId,
            Price= model .Price,
            ProductImage = model.ProductImage,
            IsDeleted =model.IsDeleted,
            ProductName=     model.ProductName,
            Quantity = model.Quantity,
            TotalPrice = model.TotalPrice
           };
            return res;
        }

        public async Task<BillDetailModel> CreateModel(BillDetailsEntity entity)
        {
            var res = new BillDetailModel() {
                Id = entity.Id,
                ProductId= entity.ProductId,
                BillDetailCode =  entity.BillDetailCode,
                BillId = entity.BillId,
                Color = entity.Color,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedOnDate= entity.CreatedOnDate,
                LastModifiedOnDate = entity.LastModifiedOnDate,
                Notes = entity.Notes,
                Status = entity.Status,
                Size = entity.Size,
                LastModifiedByUserId = entity.LastModifiedByUserId,
                Price= entity.Price,
                ProductImage = entity.ProductImage,
                IsDeleted =entity.IsDeleted,
                ProductName=     entity.ProductName,
                Quantity = entity.Quantity,
                TotalPrice = entity.TotalPrice,
                SKU= entity.SKU
            };
            return res;
        }

        public async Task<List<BillDetailModel>> CreateModels(IEnumerable<BillDetailsEntity> entities)
        {
            var res = new List<BillDetailModel>();
            foreach (var entity in entities)
            {
                res.Add(await CreateModel(entity));
            }
            return res;
        }
    }
}
