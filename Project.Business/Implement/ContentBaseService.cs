using AutoMapper;
using Project.Business.Interface.Repositories;
using Project.Business.Interface.Services;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using SERP.NewsMng.Business.Models;
using SERP.NewsMng.Business.Models.QueryModels;

namespace Project.Business.Implement
{
    internal class ContentBaseService : IContentBaseService
    {
        private readonly IRepository<ContentBase, ContentBaseQueryModel> _repository;
        private readonly IMapper _mapper;
        private readonly ProjectDbContext _context;

        public ContentBaseService(IRepository<ContentBase, ContentBaseQueryModel> repository, IMapper mapper,ProjectDbContext context)
        {
            _context = context;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ContentBaseModel> GetByIdAsync(Guid id)
        {
            var entity = await _repository.FindAsync(id);
            return _mapper.Map<ContentBaseModel>(entity);
        }

        public async Task<IEnumerable<ContentBaseModel>> GetAllAsync()
        {
            var entities = await _repository.ListAllAsync(new ContentBaseQueryModel());
            return _mapper.Map<IEnumerable<ContentBaseModel>>(entities);
        }


        public async Task<ContentBaseModel> CreateAsync(ContentBaseModel model)
        {
            var entity = _mapper.Map<ContentBase>(model);
            entity.Id = Guid.NewGuid();
            entity.CreatedOnDate = DateTime.Now;
            entity.LastModifiedOnDate = DateTime.Now;
            entity.IsDeleted = false;

            _context.ContentBases.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<ContentBaseModel>(entity);
        }

        public async Task<ContentBaseModel> UpdateAsync(ContentBaseModel model)
        {
            var existing = await _repository.FindAsync(model.Id);
            if (existing == null) throw new Exception("Không tìm thấy nội dung cần cập nhật");

            existing = _mapper.Map(model, existing);
            existing.LastModifiedOnDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return _mapper.Map<ContentBaseModel>(existing);
        }

        public async Task<ContentBaseModel> DeleteAsync(Guid id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return _mapper.Map<ContentBaseModel>(deleted);
        }
       

    }
}
