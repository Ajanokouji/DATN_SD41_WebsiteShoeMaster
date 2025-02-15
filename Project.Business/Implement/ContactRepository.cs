using AutoMapper.Configuration;
using LinqKit;
using Microsoft.EntityFrameworkCore;
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
    public class ContactRepository : IContactRepository
    {
        private readonly ProjectDbContext _context;
        public ContactRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<Contacts> FindAsync(Guid id)
        {
            var res = await _context.Contacts.FindAsync(id);
            return res;
        }

        public async Task<IEnumerable<Contacts>> ListAllAsync(ContactQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var resId = await query.Select(x => x.Id).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;
        }

        public async Task<IEnumerable<Contacts>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.Contacts.Where(x => ids.Contains(x.Id)).ToListAsync();
            return res;
        }

        public async Task<Pagination<Contacts>> GetAllAsync(ContactQueryModel queryModel)
        {
            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<Contacts> queryable = BuildQuery(queryModel);
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

        private IQueryable<Contacts> BuildQuery(ContactQueryModel queryModel)
        {
            IQueryable<Contacts> query = _context.Contacts.AsNoTracking().Where(x => x.Isdeleted != true);

            if (queryModel.Id.HasValue)
            {
                query = query.Where(x => x.Id == queryModel.Id.Value);
            }

            if (!string.IsNullOrEmpty(queryModel.Ten))
            {
                query = query.Where(x => x.Name.Contains(queryModel.Ten));
            }

            if (!string.IsNullOrEmpty(queryModel.DiaChi))
            {
                query = query.Where(x => x.Address.Contains(queryModel.DiaChi));
            }

            if (!string.IsNullOrEmpty(queryModel.Email))
            {
                query = query.Where(x => x.Email.Contains(queryModel.Email));
            }

            if (!string.IsNullOrEmpty(queryModel.SoDienThoai))
            {
                query = query.Where(x => x.PhoneNumber == queryModel.SoDienThoai);
            }

            return query;
        }

        public async Task<int> GetCountAsync(ContactQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }

        public async Task<Contacts> SaveAsync(Contacts contact)
        {
            var res = await SaveAsync(new[] { contact });
            return res.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<Contacts>> SaveAsync(IEnumerable<Contacts> contactEntities)
        {
            var updated = new List<Contacts>();

            foreach (var contact in contactEntities)
            {
                var exist = await _context.Contacts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == contact.Id);

                if (exist == null)
                {
                    contact.CreateTracking(contact.Id);
                    contact.UpdateTracking(contact.Id);
                    _context.Contacts.Add(contact);
                    updated.Add(contact);
                }
                else
                {
                    _context.Entry(exist).State = EntityState.Detached;
                    exist.Name = contact.Name;
                    exist.FullName = contact.FullName;
                    exist.Address = contact.Address;
                    exist.DateOfBirth = contact.DateOfBirth;
                    exist.Email = contact.Email;
                    exist.PhoneNumber = contact.PhoneNumber;
                    exist.Content = contact.Content;
                    exist.ImageUrl = contact.ImageUrl;
                    contact.UpdateTracking(contact.Id);
                    _context.Contacts.Update(exist);
                    updated.Add(exist);
                }
            }
            await _context.SaveChangesAsync();
            return updated;
        }

        public async Task<Contacts> DeleteAsync(Guid id)
        {
            var exist = await FindAsync(id);
            if (exist == null) throw new Exception(IContactRepository.MessageNoTFound);
            exist.Isdeleted = true;
            _context.Contacts.Update(exist);
            await _context.SaveChangesAsync();
            return exist;
        }

        public Task<IEnumerable<Contacts>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }
    }
}
