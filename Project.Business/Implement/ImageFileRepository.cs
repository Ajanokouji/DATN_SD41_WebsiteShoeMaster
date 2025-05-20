using Project.Business.Interface.Project.Business.Interface.Repositories;
using Project.DbManagement.Entity;
using Project.DbManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SERP.Framework.Common;
using Project.Business.Model;
using SERP.Framework.DB.Extensions;
using Project.Business.Interface.Repositories;

namespace Project.Business.Implement
{
    public class ImageFileRepository : IImageFileRepository
    {
        private readonly ProjectDbContext _context;

        public ImageFileRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<ImageFileEntity> FindAsync(Guid id)
        {
            return await _context.ImageFiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted ==false);
        }

        public async Task<ImageFileEntity> SaveAsync(ImageFileEntity imageFile)
        {
            var exist = await _context.ImageFiles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == imageFile.Id);
            if (exist == null)
            {
                _context.ImageFiles.Add(imageFile);
            }
            else
            {
                exist.FileName = imageFile.FileName;
                exist.CompleteFilePath = imageFile.CompleteFilePath;
                exist.CreatedOnDate=  imageFile.CreatedOnDate;
                exist.LastModifiedOnDate=  imageFile.LastModifiedOnDate;
                exist.ContentType=  imageFile.ContentType;
                exist.FilePath = imageFile.FilePath;
                exist.FileSize = imageFile.FileSize;
                exist.UploadedBy = imageFile.UploadedBy;
                _context.Entry(exist).State = EntityState.Detached;
                _context.ImageFiles.Update(imageFile);
            }
            await _context.SaveChangesAsync();
            return imageFile;
        }

        public async Task<ImageFileEntity> DeleteAsync(Guid id)
        {
            var exist = await FindAsync(id);
            if (exist == null) throw new Exception("ImageFile not found");
            exist.IsDeleted = true;
            _context.ImageFiles.Update(exist);
            await _context.SaveChangesAsync();
            return exist;
        }

        public async Task<ImageFileEntity> FindByCompletePathAsync(string completePath)
        {
            var res = await _context.ImageFiles.AsNoTracking().FirstOrDefaultAsync(x => x.CompleteFilePath == completePath);
            return res;
        }

        private  IQueryable<ImageFileEntity> BuildQuery(ImageFileQueryModel imageFileQueryModel)
        {
            IQueryable<ImageFileEntity> query = _context.ImageFiles.AsNoTracking().Where(x=>x.IsDeleted==false);

            if (imageFileQueryModel.Id != Guid.Empty)
            {
                query = query.Where(x => x.Id == imageFileQueryModel.Id);
            }

            if (!string.IsNullOrEmpty(imageFileQueryModel.FileName))
            {
                query = query.Where(x => x.FileName.Contains(imageFileQueryModel.FileName));
            }

            if (!string.IsNullOrEmpty(imageFileQueryModel.FilePath))
            {
                query = query.Where(x => x.FilePath.Contains(imageFileQueryModel.FilePath));
            }

            if (!string.IsNullOrEmpty(imageFileQueryModel.CompleteFilePath))
            {
                query = query.Where(x => x.CompleteFilePath.Contains(imageFileQueryModel.CompleteFilePath));
            }

            if (imageFileQueryModel.FileSize.HasValue)
            {
                query = query.Where(x => x.FileSize == imageFileQueryModel.FileSize);
            }

            if (!string.IsNullOrEmpty(imageFileQueryModel.UploadedBy))
            {
                query = query.Where(x => x.UploadedBy.Contains(imageFileQueryModel.UploadedBy));
            }

            return query;
        }

        public async Task<Pagination<ImageFileEntity>> GetAllAsync(ImageFileQueryModel imageFileQueryModel)
        {
            var query = BuildQuery(imageFileQueryModel);

            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(imageFileQueryModel.Sort) || imageFileQueryModel.Sort.Equals("-CreatedOnDate"))
            {
                query = query.OrderByDescending(x => x.CreatedOnDate);
            }
            else
            {
                sortExpression = imageFileQueryModel.Sort;
            }

            return await query.GetPagedAsync(imageFileQueryModel.CurrentPage.Value, imageFileQueryModel.PageSize.Value, sortExpression);
        }

        public async Task<IEnumerable<ImageFileEntity>> ListAllAsync(ImageFileQueryModel queryModel)
        {
           var query = BuildQuery(queryModel);
            return await query.ToListAsync();
        }

        public async Task<int> GetCountAsync(ImageFileQueryModel queryModel)
        {

            var query = BuildQuery(queryModel);
            return await query.CountAsync();
        }

        public async Task<IEnumerable<ImageFileEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var query = _context.ImageFiles.AsNoTracking().Where(x => ids.Contains(x.Id)&& x.IsDeleted==false);
            return await query.ToListAsync();
        }

        Task<IEnumerable<ImageFileEntity>> IRepository<ImageFileEntity, ImageFileQueryModel>.DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }
    }
}
