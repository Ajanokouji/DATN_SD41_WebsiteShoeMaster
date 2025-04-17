using Project.Business.Model;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Interface
{
    namespace Project.Business.Interface
    {
        public interface IImageFileBusiness
        {
            Task<ImageFile> FindAsync(Guid id);
            Task<Pagination<ImageFile>> GetAllAsync(ImageFileQueryModel imageFileQueryModel);
            Task<ImageFile> FindByCompletePathAsync(string  completePath);
            Task<IEnumerable<ImageFile>> ListAllAsync();
            Task<ImageFile> SaveAsync(ImageFile imageFile);
            Task<ImageFile> DeleteAsync(Guid id);
        }
    }

}
