using Microsoft.Extensions.Caching.Memory;
using Nest;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using Serilog;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Business.Implement
{
    public class CommentsBusiness : ICommentsBusiness
    {
        private readonly ICommentsRepository _commentsRepository;
        private readonly ILogger _logger;

        public CommentsBusiness(ICommentsRepository commentsRepository, ILogger logger)
        {
            _commentsRepository=commentsRepository;
            _logger=logger;
        }

        public async Task<CommentsEntity> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _commentsRepository.DeleteAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting comment {CommentId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<CommentsEntity>> DeleteAsync(Guid[] deleteIds)
        {
            try
            {
                var result = await _commentsRepository.DeleteAsync(deleteIds);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting multiple comments: {CommentIds}", string.Join(", ", deleteIds));
                throw;
            }
        }

        public async Task<CommentsEntity> FindAsync(Guid id)
        {
            try
            {
                var comment = await _commentsRepository.FindAsync(id);
                if (comment == null)
                {
                    _logger.Warning("Comment {CommentId} not found", id);
                }
                return comment;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error finding comment {CommentId}", id);
                throw;
            }
        }

        public async Task<Pagination<CommentsEntity>> GetAllAsync(CommentsModel queryModel)
        {
            try
            {

                return await _commentsRepository.GetAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting all comments");
                throw;
            }
        }

        public async Task<int> GetCountAsync(CommentsModel queryModel)
        {
            try
            {
                return await _commentsRepository.GetCountAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting comment count");
                throw;
            }
        }

        public async Task<IEnumerable<CommentsEntity>> ListAllAsync(CommentsModel queryModel)
        {
            try
            {
                return await _commentsRepository.ListAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing all comments");
                throw;
            }
        }

        public async Task<IEnumerable<CommentsEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            try
            {
                return await _commentsRepository.ListByIdsAsync(ids);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing comments by ids: {CommentIds}", string.Join(", ", ids));
                throw;
            }
        }

        public async Task<IEnumerable<CommentsEntity>> ListByProductIdAsync(Guid id)
        {
            var res = await _commentsRepository.ListAllAsync(new CommentsModel()
            {
                PageSize=4,
                ObjectId = id,
            });
            return res;
        }

        public async Task<CommentsEntity> PatchAsync(CommentsEntity model)
        {
            var exist = await _commentsRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException("Comment not found");
            }

            var update = new CommentsEntity
            {
                Id = exist.Id,
                ObjectId = exist.ObjectId,
                Ref = exist.Ref,
                Message = exist.Message,
                Status = exist.Status,
                UserId = exist.UserId,
                Username = exist.Username,
                CreatedByUserId = exist.CreatedByUserId,
                LastModifiedByUserId = exist.LastModifiedByUserId,
                CreatedOnDate = exist.CreatedOnDate,
                LastModifiedOnDate = DateTime.UtcNow,
                IsPublish = exist.IsPublish,
                ParentId = exist.ParentId,
                TolalReply = exist.TolalReply
            };

            // Update fields if new values are provided
            if (!string.IsNullOrWhiteSpace(model.Message))
                update.Message = model.Message;
            if (model.Status != exist.Status)
                update.Status = model.Status;
            if (model.LastModifiedByUserId != null)
                update.LastModifiedByUserId = model.LastModifiedByUserId;

            return await SaveAsync(update);
        }

        public async Task<CommentsEntity> SaveAsync(CommentsEntity comment)
        {
            var res = await SaveAsync(new[] { comment });
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<CommentsEntity>> SaveAsync(IEnumerable<CommentsEntity> comments)
        {
            return await _commentsRepository.SaveAsync(comments);
        }
    }
}
