using Lucene.Net.Documents;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.SearchBusiness
{
    public class ObjetctSearchService : IObjetctSearchService
    {
        private readonly string KeyWord = "keyword";
        public async Task<Document> ConvertToObject(ProductEntity productEntity)
        {
            var res = new Document();

            // Map basic fields  
            res.Add(new StringField(nameof(productEntity.Id), productEntity.Id.ToString(), Field.Store.YES));
            res.Add(new StringField(nameof(productEntity.Code), productEntity.Code ?? string.Empty, Field.Store.YES));
            res.Add(new StringField(nameof(productEntity.Name), productEntity.Name ?? string.Empty, Field.Store.YES));
            res.Add(new StringField(nameof(productEntity.Status), productEntity.Status ?? string.Empty, Field.Store.YES));
            res.Add(new StringField(nameof(productEntity.ImageUrl), productEntity.ImageUrl ?? string.Empty, Field.Store.YES));
            res.Add(new StringField(nameof(productEntity.SortOrder), productEntity.SortOrder ?? string.Empty, Field.Store.YES));
            res.Add(new TextField(nameof(productEntity.Description), productEntity.Description ?? string.Empty, Field.Store.YES));
            res.Add(new StringField(nameof(productEntity.MainCategoryId), productEntity.MainCategoryId?.ToString() ?? string.Empty, Field.Store.YES));
            res.Add(new StringField(nameof(productEntity.WorkFlowStates), productEntity.WorkFlowStates ?? string.Empty, Field.Store.YES));

            // Map JSON fields  
            res.Add(new StoredField(nameof(productEntity.VariantJson), productEntity.VariantJson ?? string.Empty));
            res.Add(new StoredField(nameof(productEntity.MediasJson), productEntity.MediasJson ?? string.Empty));
            res.Add(new StoredField(nameof(productEntity.RelatedIds), productEntity.RelatedIds ?? string.Empty));
            res.Add(new StoredField(nameof(productEntity.MetadataJson), productEntity.MetadataJson ?? string.Empty));
            res.Add(new StoredField(nameof(productEntity.LabelsJson), productEntity.LabelsJson ?? string.Empty));

            // Map additional fields  
            res.Add(new StringField(nameof(productEntity.CompleteName), productEntity.CompleteName ?? string.Empty, Field.Store.YES));
            res.Add(new StringField(nameof(productEntity.CompletePath), productEntity.CompletePath ?? string.Empty, Field.Store.YES));
            res.Add(new StringField(nameof(productEntity.CompleteCode), productEntity.CompleteCode ?? string.Empty, Field.Store.YES));

            // Map date fields  
            long publicOnDateTicksValues = productEntity.PublicOnDate.Value.Ticks;
            res.Add(new Int64Field($"{nameof(productEntity.PublicOnDate)}.{KeyWord}",publicOnDateTicksValues, Field.Store.YES));
            res.Add(new StoredField(nameof(productEntity.PublicOnDate), productEntity.PublicOnDate.Value.ToString("yyyy-MM-dd")));

            // Map base entity fields  
            res.Add(new StringField(nameof(productEntity.CreatedByUserId), productEntity.CreatedByUserId?.ToString() ?? string.Empty, Field.Store.YES));
            res.Add(new StringField(nameof(productEntity.LastModifiedByUserId), productEntity.LastModifiedByUserId?.ToString() ?? string.Empty, Field.Store.YES));
            res.Add(new StringField(nameof(productEntity.IsDeleted), productEntity.IsDeleted?.ToString() ?? string.Empty, Field.Store.YES));


            long createdOnDateTicksValues = productEntity.CreatedOnDate.Value.Ticks;
            res.Add(new Int64Field($"{nameof(productEntity.PublicOnDate)}.{KeyWord}", createdOnDateTicksValues, Field.Store.YES));
            res.Add(new StoredField(nameof(productEntity.CreatedOnDate), productEntity.CreatedOnDate?.ToString("o") ?? string.Empty));

            long lastModifiedOnDateTicksValues = productEntity.LastModifiedOnDate.Value.Ticks;
            res.Add(new Int64Field($"{nameof(productEntity.PublicOnDate)}.{KeyWord}", lastModifiedOnDateTicksValues, Field.Store.YES));
            res.Add(new StoredField(nameof(productEntity.LastModifiedOnDate), productEntity.LastModifiedOnDate?.ToString("o") ?? string.Empty));

            return res;
        }
        public async Task<ProductEntity> ConvertToEntity(Document document)
        {
            var res = new ProductEntity();

            // Map basic fields
            res.Id = Guid.Parse(document.Get(nameof(ProductEntity.Id)));
            res.Code = document.Get(nameof(ProductEntity.Code));
            res.Name = document.Get(nameof(ProductEntity.Name));
            res.Status = document.Get(nameof(ProductEntity.Status));
            res.ImageUrl = document.Get(nameof(ProductEntity.ImageUrl));
            res.SortOrder = document.Get(nameof(ProductEntity.SortOrder));
            res.Description = document.Get(nameof(ProductEntity.Description));
            res.MainCategoryId = Guid.TryParse(document.Get(nameof(ProductEntity.MainCategoryId)), out var mainCategoryId) ? mainCategoryId : null;
            res.WorkFlowStates = document.Get(nameof(ProductEntity.WorkFlowStates));

            // Map JSON fields
            res.VariantJson = document.Get(nameof(ProductEntity.VariantJson));
            res.MediasJson = document.Get(nameof(ProductEntity.MediasJson));
            res.RelatedIds = document.Get(nameof(ProductEntity.RelatedIds));
            res.MetadataJson = document.Get(nameof(ProductEntity.MetadataJson));
            res.LabelsJson = document.Get(nameof(ProductEntity.LabelsJson));

            // Map additional fields
            res.CompleteName = document.Get(nameof(ProductEntity.CompleteName));
            res.CompletePath = document.Get(nameof(ProductEntity.CompletePath));
            res.CompleteCode = document.Get(nameof(ProductEntity.CompleteCode));

            // Map date fields
            if (long.TryParse(document.Get($"{nameof(ProductEntity.PublicOnDate)}.{KeyWord}"), out var publicOnDateTicks))
            {
                res.PublicOnDate = new DateTime(publicOnDateTicks);
            }

            if (long.TryParse(document.Get($"{nameof(ProductEntity.CreatedOnDate)}.{KeyWord}"), out var createdOnDateTicks))
            {
                res.CreatedOnDate = new DateTime(createdOnDateTicks);
            }

            if (long.TryParse(document.Get($"{nameof(ProductEntity.LastModifiedOnDate)}.{KeyWord}"), out var lastModifiedOnDateTicks))
            {
                res.LastModifiedOnDate = new DateTime(lastModifiedOnDateTicks);
            }

            // Map base entity fields
            res.CreatedByUserId = Guid.TryParse(document.Get(nameof(ProductEntity.CreatedByUserId)), out var createdByUserId) ? createdByUserId : null;
            res.LastModifiedByUserId = Guid.TryParse(document.Get(nameof(ProductEntity.LastModifiedByUserId)), out var lastModifiedByUserId) ? lastModifiedByUserId : null;
            res.IsDeleted = bool.TryParse(document.Get(nameof(ProductEntity.IsDeleted)), out var isDeleted) ? isDeleted : null;

            return res;
        }

    }
}
