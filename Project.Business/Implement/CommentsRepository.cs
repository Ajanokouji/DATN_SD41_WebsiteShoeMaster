using Project.Business.Interface;
using Project.DbManagement.Entity;
using Project.DbManagement;
using SERP.Framework.Business;
using SERP.Framework.Common;
using SERP.Framework.DB.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Business.Model;
using System.Data.Entity;

namespace Project.Business.Implement
{
    public class CommentsRepository : ICommentsRepository
    {
        private readonly ProjectDbContext _context;

        public CommentsRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public virtual async Task<CommentsEntity> FindAsync(Guid id)
        {
            return await _context.Comments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public virtual async Task<IEnumerable<CommentsEntity>> ListAllAsync(CommentsModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var resId = await query.Select(x => x.Id).ToListAsync();
            return await ListByIdsAsync(resId);
        }

        public virtual async Task<IEnumerable<CommentsEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.Comments.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public virtual async Task<Pagination<CommentsEntity>> GetAllAsync(CommentsModel queryModel)
        {
            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort ?? "-CreatedOnDate");
            IQueryable<CommentsEntity> queryable = BuildQuery(queryModel);

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

        private IQueryable<CommentsEntity> BuildQuery(CommentsModel queryModel)
        {
            var query = _context.Comments.AsNoTracking().Where(x => x.IsDeleted != true);

            if (queryModel.Id.HasValue)
            {
                query = query.Where(x => x.Id == queryModel.Id.Value);
            }

            if (queryModel.ObjectId.HasValue)
            {
                query = query.Where(x => x.ObjectId == queryModel.ObjectId.Value);
            }

            if (!string.IsNullOrWhiteSpace(queryModel.Ref))
            {
                query = query.Where(x => x.Ref.Contains(queryModel.Ref));
            }

            if (!string.IsNullOrWhiteSpace(queryModel.Message))
            {
                query = query.Where(x => x.Message.Contains(queryModel.Message));
            }

            if (queryModel.Status.HasValue)
            {
                query = query.Where(x => x.Status == queryModel.Status.Value);
            }

            if (queryModel.UserId.HasValue)
            {
                query = query.Where(x => x.UserId == queryModel.UserId.Value);
            }

            if (!string.IsNullOrWhiteSpace(queryModel.Username))
            {
                query = query.Where(x => x.Username.Contains(queryModel.Username));
            }

            if (queryModel.IsPublish.HasValue)
            {
                query = query.Where(x => x.IsPublish == queryModel.IsPublish.Value);
            }

            if (queryModel.ParentId.HasValue)
            {
                query = query.Where(x => x.ParentId == queryModel.ParentId.Value);
            }

            return query;
        }


        public virtual async Task<int> GetCountAsync(CommentsModel queryModel)
        {
            var query = BuildQuery(queryModel);
            return await query.CountAsync();
        }

        public virtual async Task<CommentsEntity> SaveAsync(CommentsEntity comment)
        {
            var result = await SaveAsync(new[] { comment });
            return result.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<CommentsEntity>> SaveAsync(IEnumerable<CommentsEntity> comments)
        {
            var updated = new List<CommentsEntity>();

            foreach (var comment in comments)
            {
                var exist = await _context.Comments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == comment.Id);

                if (exist == null)
                {
                    comment.CreateTracking(comment.CreatedByUserId ?? Guid.Empty);
                    comment.UpdateTracking(comment.LastModifiedByUserId ?? Guid.Empty);
                    _context.Comments.Add(comment);
                    updated.Add(comment);
                }
                else
                {
                    // Detach entity to avoid tracking conflict
                    _context.Entry(exist).State = Microsoft.EntityFrameworkCore.EntityState.Detached;


                    // Update fields (chỉ giữ những field có thật trong CommentsEntity)
                    exist.Message = comment.Message;
                    exist.Status = comment.Status;
                    exist.Username = comment.Username;
                    exist.LastModifiedOnDate = comment.LastModifiedOnDate;
                    exist.LastModifiedByUserId = comment.LastModifiedByUserId;
                    exist.ObjectId = comment.ObjectId;
                    exist.Ref = comment.Ref;
                    exist.IsPublish = comment.IsPublish;
                    exist.ParentId = comment.ParentId;

                    exist.UpdateTracking(comment.LastModifiedByUserId ?? Guid.Empty); // Sửa chỗ này

                    _context.Comments.Update(exist);
                    updated.Add(exist);
                }
            }

            await _context.SaveChangesAsync();
            return updated;
        }



        public virtual async Task<CommentsEntity> DeleteAsync(Guid id)
        {
            var exist = await FindAsync(id);
            if (exist == null) throw new Exception("Comment not found");
            exist.IsDeleted = true;
            _context.Comments.Update(exist);
            await _context.SaveChangesAsync();
            return exist;
        }

        public virtual async Task<IEnumerable<CommentsEntity>> DeleteAsync(Guid[] deleteIds)
        {
            var commentsToDelete = await _context.Comments.Where(x => deleteIds.Contains(x.Id)).ToListAsync();
            if (!commentsToDelete.Any()) throw new Exception("No comments found to delete");

            foreach (var comment in commentsToDelete)
            {
                comment.IsDeleted = true;
                _context.Comments.Update(comment);
            }

            await _context.SaveChangesAsync();
            return commentsToDelete;
        }
    }

}
