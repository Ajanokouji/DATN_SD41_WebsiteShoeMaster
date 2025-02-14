using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Business.Implement
{
    public class CustomerBusiness : ICustomerBusiness
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerBusiness(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Customers> DeleteAsync(Guid customerId)
        {
            return await _customerRepository.DeleteAsync(customerId);
        }

        public async Task<IEnumerable<Customers>> DeleteAsync(Guid[] deleteIds)
        {
            return await _customerRepository.DeleteAsync(deleteIds);
        }

        public async Task<Customers> FindAsync(Guid customerId)
        {
            return await _customerRepository.FindAsync(customerId);
        }

        public async Task<Pagination<Customers>> GetAllAsync(CustomerQueryModel queryModel)
        {
            return await _customerRepository.GetAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(CustomerQueryModel queryModel)
        {
            return await _customerRepository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<Customers>> ListAllAsync(CustomerQueryModel queryModel)
        {
            return await _customerRepository.ListAllAsync(queryModel);
        }

        public async Task<IEnumerable<Customers>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _customerRepository.ListByIdsAsync(ids);
        }

        public async Task<Customers> PatchAsync(Customers model)
        {
            var exist = await _customerRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException("Customer not found.");
            }

            var update = new Customers
            {
                Id = exist.Id,
                TTTHIDMain = exist.TTTHIDMain,
                TTLHRelatedIds = exist.TTLHRelatedIds,
                Code = exist.Code,
                CreatedOnDate = exist.CreatedOnDate,
                Description = exist.Description,
                DiaChi = exist.DiaChi,
                Email = exist.Email,
                PhoneNumber = exist.PhoneNumber,
                Ten = exist.Ten,
                UserNameTaiKhoan = exist.UserNameTaiKhoan
            };

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                update.Code = model.Code;
            }
            if (!string.IsNullOrWhiteSpace(model.Ten))
            {
                update.Ten = model.Ten;
            }
            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                update.PhoneNumber = model.PhoneNumber;
            }
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                update.Email = model.Email;
            }
            if (!string.IsNullOrWhiteSpace(model.DiaChi))
            {
                update.DiaChi = model.DiaChi;
            }
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                update.Description = model.Description;
            }
            if (!string.IsNullOrWhiteSpace(model.UserNameTaiKhoan))
            {
                update.UserNameTaiKhoan = model.UserNameTaiKhoan;
            }
            if (model.TTTHIDMain.HasValue)
            {
                update.TTTHIDMain = model.TTTHIDMain;
            }
            if (model.TTLHRelatedIds != null && model.TTLHRelatedIds.Any())
            {
                update.TTLHRelatedIds = model.TTLHRelatedIds;
            }

            return await SaveAsync(update);
        }

        public async Task<Customers> SaveAsync(Customers Customers)
        {
            var res = await SaveAsync(new[] { Customers });
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<Customers>> SaveAsync(IEnumerable<Customers> customerEntities)
        {
            return await _customerRepository.SaveAsync(customerEntities);
        }
    }
}
