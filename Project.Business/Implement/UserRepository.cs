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
        public async Task<User> FindAsync(Guid id)
        {
            var res = await _context.Users.FindAsync(id);
            return res;
        }
        public async Task<IEnumerable<User>> ListAllAsync(UserQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var resId = await query.Select(x => x.Id).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;
        }
        public async Task<IEnumerable<User>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.Users.Where(x => ids.Contains(x.Id)).ToListAsync();
            return res;
        }
        public async Task<Pagination<User>> GetAllAsync(UserQueryModel queryModel)
        {
            UserQueryModel userQueryModel = queryModel;


            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<User> queryable = BuildQuery(queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                queryable = queryable.OrderByDescending((User x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }

            return await queryable.GetPagedOrderAsync(queryModel.CurrentPage.Value, queryModel.PageSize.Value, sortExpression);
        }

        private IQueryable<User> BuildQuery(UserQueryModel queryModel)
        {
            IQueryable<User> query = _context.Users.AsNoTracking().Where(x => x.Isdeleted != true);

            if (queryModel.Id.HasValue)
            {
                query = query.Where((User x) => x.Id == queryModel.Id.Value);
            }

            if (queryModel.ListId != null && queryModel.ListId.Any())
            {
                query = query.Where((User x) => queryModel.ListId.Contains(x.Id));
            }

            if (queryModel.ListTextSearch != null && queryModel.ListTextSearch.Any())
            {
                ExpressionStarter<User> expressionStarter = LinqKit.PredicateBuilder.New<User>();
                foreach (string ts in queryModel.ListTextSearch)
                {
                    expressionStarter = expressionStarter.Or((User p) =>
                                                                p.Email.Contains(ts.ToLower()) ||
                                                                p.PhoneNumber.Contains(ts.ToLower()));
                }

                query = query.Where(expressionStarter);
            }

            if (!string.IsNullOrWhiteSpace(queryModel.FullTextSearch))
            {
                string fullTextSearch = queryModel.FullTextSearch.ToLower();
                query = query.Where((User x) => x.Email.Contains(fullTextSearch));
            }

            if (!string.IsNullOrEmpty(queryModel.Type))
            {
                query = query.Where(x => x.Type == queryModel.Type);
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

            if (!string.IsNullOrEmpty(queryModel.Password))
            {
                query = query.Where(x => x.Password.Contains(queryModel.Password));
            }
            if (!string.IsNullOrEmpty(queryModel.UserDetailJson))
            {
                query = query.Where(x => x.UserDetailJson.Contains(queryModel.UserDetailJson));
            }
            return query;
        }
        public async Task<int> GetCountAsync(UserQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }
        public async Task<User> SaveAsync(User user)
        {
            var res = await SaveAsync(new[] { user });
            return res.FirstOrDefault();

        }
        public virtual async Task<IEnumerable<User>> SaveAsync(IEnumerable<User> users)
        {
            var updated = new List<User>();

            foreach (var user in users)
            {
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
                    exist.AvartarUrl = user.AvartarUrl;
                    exist.Password = user.Password;
                    exist.UserDetailJson = user.UserDetailJson;

                    user.UpdateTracking(user.Id);
                    _context.Users.Update(exist);
                    updated.Add(exist);
                }
            }
            await _context.SaveChangesAsync();

            return updated;
        }
        public async Task<User> DeleteAsync(Guid Id)
        {
            var exist = await FindAsync(Id);
            if (exist == null) throw new Exception(IUserRepository.MessageNoTFound);
            exist.Isdeleted = true;
            _context.Users.Update(exist);
            _context.SaveChangesAsync();
            return exist;
        }

        public Task<IEnumerable<User>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }

    }
}
