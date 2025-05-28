using AutoMapper.Configuration;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.FileManagementService.Business;
using SERP.FileManagementService.Entities;
using SERP.Framework.Business;
using SERP.Framework.Common;
using SERP.Framework.Common.Extensions;
using SERP.Framework.DB.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;
namespace Project.Business.Implement

{
    public class UserRepository : IUserRepository
    {
        private readonly ProjectDbContext _context;
        public UserRepository(ProjectDbContext context)
        {
            _context = context;
        }
        public async Task<UserEntity> FindAsync(Guid id)
        {
            var res = await _context.Users.FindAsync(id);
            return res;
        }
        public async Task<IEnumerable<UserEntity>> ListAllAsync(UserQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var resId = await query.Select(x => x.Id).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;
        }
        public async Task<IEnumerable<UserEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.Users.Where(x => ids.Contains(x.Id)).ToListAsync();
            return res;
        }
        public async Task<Pagination<UserEntity>> GetAllAsync(UserQueryModel queryModel)
        {
            UserQueryModel userQueryModel = queryModel;


            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<UserEntity> queryable = BuildQuery(queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                queryable = queryable.OrderByDescending((UserEntity x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value, sortExpression);
        }

        private IQueryable<UserEntity> BuildQuery(UserQueryModel queryModel)
        {
            IQueryable<UserEntity> query = _context.Users.AsNoTracking().Where(x => x.IsDeleted != true);

            if (queryModel.Id.HasValue)
            {
                query = query.Where((UserEntity x) => x.Id == queryModel.Id.Value);
            }

            if (queryModel.ListId != null && queryModel.ListId.Any())
            {
                query = query.Where((UserEntity x) => queryModel.ListId.Contains(x.Id));
            }

            if (queryModel.ListTextSearch != null && queryModel.ListTextSearch.Any())
            {
                ExpressionStarter<UserEntity> expressionStarter = LinqKit.PredicateBuilder.New<UserEntity>();
                foreach (string ts in queryModel.ListTextSearch)
                {
                    expressionStarter = expressionStarter.Or((UserEntity p) =>
                                                                p.Email.Contains(ts.ToLower()) ||
                                                                p.PhoneNumber.Contains(ts.ToLower()));
                }

                query = query.Where(expressionStarter);
            }

            if (!string.IsNullOrWhiteSpace(queryModel.FullTextSearch))
            {
                string fullTextSearch = queryModel.FullTextSearch.ToLower();
                query = query.Where((UserEntity x) => x.Email.Contains(fullTextSearch));
            }

            if (queryModel.Type.HasValue)
            {
                query = query.Where(x => x.Type == queryModel.Type.Value);
            }

            if (!string.IsNullOrEmpty(queryModel.Name))
            {
                query = query.Where(x => x.Name == queryModel.Name);
            }

            if (!string.IsNullOrEmpty(queryModel.PhoneNumber))
            {
                query = query.Where(x => x.PhoneNumber == queryModel.PhoneNumber);
            }

            if (!string.IsNullOrEmpty(queryModel.Username))
            {
                query = query.Where(x => x.Username == queryModel.Username);
            }

            if (!string.IsNullOrEmpty(queryModel.AvartarUrl))
            {
                query = query.Where(x => x.AvartarUrl.Contains(queryModel.AvartarUrl));
            }

            return query;
        }
        public async Task<int> GetCountAsync(UserQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }
        public async Task<UserEntity> SaveAsync(UserEntity user)
        {
            var res = await SaveAsync(new[] { user });
            return res.FirstOrDefault();

        }
        public virtual async Task<IEnumerable<UserEntity>> SaveAsync(IEnumerable<UserEntity> users)
        {
            var updated = new List<UserEntity>();

            foreach (var user in users)
            {
                // Fix lỗi tracking
                var local = _context.Users.Local.FirstOrDefault(x => x.Id == user.Id);
                if (local != null)
                {
                    _context.Entry(local).State = EntityState.Detached;
                }

                var exist = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                            x.Id == user.Id
                    );

                if (exist == null)
                {
                    user.CreateTracking(user.Id);
                    user.UpdateTracking(user.Id);
                    _context.Users.Add(user);
                    updated.Add(user);
                }
                else
                {

                    _context.Entry(exist).State = EntityState.Detached;
                    exist.Id = user.Id;
                    exist.Type = user.Type;
                    exist.Username = user.Username;
                    exist.Name = user.Name;
                    exist.PhoneNumber = user.PhoneNumber;
                    exist.Email = user.Email;
                    exist.Address = user.Address;
                    exist.AvartarUrl = user.AvartarUrl;
                    exist.Password = user.Password;
                    exist.UserDetailJson = user.UserDetailJson;
                    exist.IsActive = user.IsActive;

                    user.UpdateTracking(user.Id);
                    _context.Users.Update(exist);
                    updated.Add(exist);
                }
            }
            await _context.SaveChangesAsync();

            return updated;
        }
        public async Task<UserEntity> DeleteAsync(Guid Id)
        {
            var exist = await FindAsync(Id);
            if (exist == null) throw new Exception(IUserRepository.MessageNoTFound);
            exist.IsDeleted = true;
            _context.Users.Update(exist);
            await _context.SaveChangesAsync();
            return exist;
        }

        public Task<IEnumerable<UserEntity>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserEntity>> LocUserTheoNhieuDK(UserQueryModel userQueryModel)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userQueryModel.Username))
            {
                query = query.Where(u => u.Username.ToLower() == userQueryModel.Username.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(userQueryModel.Name))
            {
                query = query.Where(u => u.Name.Contains(userQueryModel.Name));
            }

            if (!string.IsNullOrWhiteSpace(userQueryModel.Email))
            {
                query = query.Where(u => u.Email.ToLower() == userQueryModel.Email.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(userQueryModel.AvartarUrl))
            {
                query = query.Where(u => u.AvartarUrl.Contains(userQueryModel.AvartarUrl));
            }

            if (!string.IsNullOrWhiteSpace(userQueryModel.PhoneNumber))
            {
                query = query.Where(u => u.PhoneNumber.ToLower() == userQueryModel.PhoneNumber.ToLower());
            }

            if (userQueryModel.Type.HasValue)
            {
                query = query.Where(x => x.Type == userQueryModel.Type.Value);
            }

            return await query.ToListAsync();
        }

        public UserEntity UserLogin(string username, string password)
        {
            var user = _context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Username == username && u.Password == password);
            return user;
        }
    }
}
