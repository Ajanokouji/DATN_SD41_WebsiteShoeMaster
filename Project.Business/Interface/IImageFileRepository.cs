using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.Common;

namespace Project.Business.Interface
{
    namespace Project.Business.Interface.Repositories
    {
        public interface IImageFileRepository : IRepository<ImageFileEntity,ImageFileQueryModel>
        {
            Task<ImageFileEntity> FindByCompletePathAsync(string completePath);
            Task<ImageFileEntity> SaveAsync(ImageFileEntity imageFile);
        }
    }
}
