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
using Project.DbManagement.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace Project.Business.Implement
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ProjectDbContext _context;
        public CustomerRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<CustomersEntity> FindAsync(Guid id)
        {
            var res = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return res;
        }

        public async Task<IEnumerable<CustomersEntity>> ListAllAsync(CustomerQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var resId = await query.Select(x => x.Id).ToListAsync();
            var res = await ListByIdsAsync(resId);
            return res;
        }

        public async Task<IEnumerable<CustomersEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var res = await _context.Customers.Where(x => ids.Contains(x.Id)).ToListAsync();
            return res;
        }

        public async Task<Pagination<CustomersEntity>> GetAllAsync(CustomerQueryModel queryModel)
        {
            queryModel.Sort = QueryUtils.FormatSortInput(queryModel.Sort);
            IQueryable<CustomersEntity> queryable = BuildQuery(queryModel);
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

        private IQueryable<CustomersEntity> BuildQuery(CustomerQueryModel queryModel)
        {
            IQueryable<CustomersEntity> query = _context.Customers.AsNoTracking().Where(x => x.IsDeleted != true);

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

            if (!string.IsNullOrEmpty(queryModel.PhoneNumber))
            {
                query = query.Where(x => x.PhoneNumber == queryModel.PhoneNumber);
            }
            //if (queryModel.MetaDataQueries != null && queryModel.MetaDataQueries.Any())
            //{
            //    string text = queryModel.BuildMetadataQuery<NodeEntity>(conditionParameters);
            //    if (!string.IsNullOrEmpty(text17))
            //    {
            //        text = text + " AND (" + text17.Replace("MetadataEntity", MetadataEntityTableName) + ")";
            //    }
            //}

            //if (query.RelationQuery != null)
            //{
            //    string text18 = BuildRelationQueryExpression(query.RelationQuery, conditionParameters);
            //    if (!string.IsNullOrEmpty(text18))
            //    {
            //        text = text + " AND " + text18;
            //    }
            //}
            return query;
        }

        public async Task<int> GetCountAsync(CustomerQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            var res = await query.CountAsync();
            return res;
        }

        public async Task<CustomersEntity> SaveAsync(CustomersEntity customer)
        {
            var res = await SaveAsync(new[] { customer });
            return res.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<CustomersEntity>> SaveAsync(IEnumerable<CustomersEntity> customerEntities)
        {
            var updated = new List<CustomersEntity>();

            foreach (var customer in customerEntities)
            {
                var exist = await _context.Customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == customer.Id);

                if (exist == null)
                {
                    customer.CreateTracking(customer.Id);
                    customer.UpdateTracking(customer.Id);
                    _context.Customers.Add(customer);
                    updated.Add(customer);
                }
                else
                {
                    _context.Entry(exist).State = EntityState.Detached;
                    exist.Name = customer.Name;
                    exist.PhoneNumber = customer.PhoneNumber;
                    exist.Address = customer.Address;
                    exist.Email = customer.Email;
                    exist.Description = customer.Description;
                    exist.UserName = customer.UserName;
                    customer.UpdateTracking(customer.Id);
                    _context.Customers.Update(exist);
                    updated.Add(exist);
                }
            }
            await _context.SaveChangesAsync();
            return updated;
        }

        public async Task<List<CustomerViewModel>> GetCustomerByPhoneNumber(string phoneNumber)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);

            if (customer == null) return new List<CustomerViewModel>();

            var result = new List<CustomerViewModel>
            {
                new CustomerViewModel
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    PhoneNumber = customer.PhoneNumber
                }
            };

            return result;
        }

        public bool AddCustomerSell(CustomerViewModel customer)
        {
            if (customer == null) return false;

            var existingCustomer = _context.Customers
                .FirstOrDefault(c => c.PhoneNumber == customer.PhoneNumber || c.Email == customer.Email);

            if (existingCustomer != null)
            {
                if (existingCustomer.PhoneNumber == customer.PhoneNumber)
                    throw new Exception("Số điện thoại đã tồn tại!");
                if (existingCustomer.Email == customer.Email)
                    throw new Exception("Email đã tồn tại!");
            }
            
            customer.Id = Guid.NewGuid();
            CustomersEntity customers = new CustomersEntity();
            customers.Id = customer.Id;
            customers.Name = customer.Name;
            customers.PhoneNumber = customer.PhoneNumber;
            customers.Email = customer.Email;
            customers.Address = customer.Address;
            customers.IsAnonymous = customer.IsAnonymous;
            customers.CreatedByUserId = customer.CreatedByUserId = null;
            customers.LastModifiedByUserId = customer.LastModifiedByUserId = null;
            customers.CreatedOnDate = customer.CreatedOnDate = DateTime.Now;
            customers.LastModifiedOnDate = customer.LastModifiedOnDate = DateTime.Now;
            _context.Customers.Add(customers);
            _context.SaveChanges();
            return true;
        }


        public async Task<CustomersEntity> DeleteAsync(Guid id)
        {
            var exist = await FindAsync(id);
            if (exist == null) throw new Exception(ICustomerRepository.MessageNoTFound);
            exist.IsDeleted = true;
            _context.Customers.Update(exist);
            await _context.SaveChangesAsync();
            return exist;
        }

        public Task<IEnumerable<CustomersEntity>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }
    }
}
