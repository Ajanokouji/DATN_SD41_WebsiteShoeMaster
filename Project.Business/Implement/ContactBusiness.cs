using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Business.Implement
{
    public class ContactBusiness : IContactBusiness
    {
        private readonly IContactRepository _contactRepository;

        public ContactBusiness(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<Contacts> DeleteAsync(Guid contactId)
        {
            return await _contactRepository.DeleteAsync(contactId);
        }

        public async Task<IEnumerable<Contacts>> DeleteAsync(Guid[] contactIds)
        {
            return await _contactRepository.DeleteAsync(contactIds);
        }

        public async Task<Contacts> FindAsync(Guid contactId)
        {
            return await _contactRepository.FindAsync(contactId);
        }

        public async Task<Pagination<Contacts>> GetAllAsync(ContactQueryModel queryModel)
        {
            return await _contactRepository.GetAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(ContactQueryModel queryModel)
        {
            return await _contactRepository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<Contacts>> ListAllAsync(ContactQueryModel queryModel)
        {
            return await _contactRepository.ListAllAsync(queryModel);
        }

        public async Task<IEnumerable<Contacts>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _contactRepository.ListByIdsAsync(ids);
        }

        public async Task<Contacts> PatchAsync(Contacts model)
        {
            var exist = await _contactRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException(ContactConstant.ContactNotFound);
            }

            var update = new Contacts
            {
                Id = exist.Id,
                Ten = exist.Ten,
                TenDayDu = exist.TenDayDu,
                DiaChi = exist.DiaChi,
                NgaySinh = exist.NgaySinh,
                ImageUrl = exist.ImageUrl,
                Email = exist.Email,
                SoDienThoai = exist.SoDienThoai,
                NoiDung = exist.NoiDung
            };

            if (!string.IsNullOrWhiteSpace(model.Ten))
            {
                update.Ten = model.Ten;
            }
            if (!string.IsNullOrWhiteSpace(model.TenDayDu))
            {
                update.TenDayDu = model.TenDayDu;
            }
            if (!string.IsNullOrWhiteSpace(model.DiaChi))
            {
                update.DiaChi = model.DiaChi;
            }
            if (!string.IsNullOrWhiteSpace(model.NgaySinh))
            {
                update.NgaySinh = model.NgaySinh;
            }
            if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                update.ImageUrl = model.ImageUrl;
            }
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                update.Email = model.Email;
            }
            if (!string.IsNullOrWhiteSpace(model.SoDienThoai))
            {
                update.SoDienThoai = model.SoDienThoai;
            }
            if (!string.IsNullOrWhiteSpace(model.NoiDung))
            {
                update.NoiDung = model.NoiDung;
            }

            return await SaveAsync(update);
        }

        public async Task<Contacts> SaveAsync(Contacts contactEntity)
        {
            var res = await SaveAsync(new[] { contactEntity });
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<Contacts>> SaveAsync(IEnumerable<Contacts> contactEntities)
        {
            return await _contactRepository.SaveAsync(contactEntities);
        }
    }
}
