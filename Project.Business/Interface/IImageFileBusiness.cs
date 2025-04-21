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
            /// <summary>
            /// Finds an image file by its ID.
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            Task<ImageFileEntity> FindAsync(Guid id);
            Task<Pagination<ImageFileEntity>> GetAllAsync(ImageFileQueryModel imageFileQueryModel);
            /// <summary>
            /// Finds an image file by its complete path.
            /// </summary>
            /// <param name="completePath"></param>
            /// <returns></returns>
            Task<ImageFileEntity> FindByCompletePathAsync(string  completePath);
            /// <summary>
            /// Lists all image files based on the provided query model.
            /// </summary>
            /// <param name="imageFileQueryModel"></param>
            /// <returns></returns>
            Task<IEnumerable<ImageFileEntity>> ListAllAsync(ImageFileQueryModel imageFileQueryModel);
            /// <summary>
            /// Saves the image file entity to the database.
            /// </summary>
            /// <param name="imageFile"></param>
            /// <returns></returns>
            Task<ImageFileEntity> SaveAsync(ImageFileEntity imageFile);
            Task<ImageFileEntity> DeleteAsync(Guid id);
        }
    }

}
