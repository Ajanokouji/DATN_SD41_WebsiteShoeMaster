using Microsoft.EntityFrameworkCore;
using Project.Business.Interface.Repositories;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Dictionary.Models;
using SERP.Framework.Common;
using SERP.Framework.Common.Extensions;
using SERP.Framework.DB.Extensions;
using System.Linq;

namespace Project.Business.Implement
{
    public class ProvinceRepository : IProvinceRepository
    {
        private readonly ProjectDbContext _context;

        public ProvinceRepository(ProjectDbContext context)
        {
            _context = context;
        }
        public async Task<Province> FindAsync(Guid id)
        {
            return await _context.Provinces.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Province>> ListAllAsync(ProvinceQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            return await query.ToListAsync();
        }

        public async Task<int> GetCountAsync(ProvinceQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            return await query.CountAsync();
        }

        public async Task<IEnumerable<Province>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.Provinces.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<Province> DeleteAsync(Guid id)
        {
            var existingProvince = await FindAsync(id);
            if (existingProvince == null) throw new Exception("Province not found");

            existingProvince.Isdeleted = true;
            _context.Provinces.Update(existingProvince);
            await _context.SaveChangesAsync();
            return existingProvince;
        }

        public async Task<Pagination<Province>> GetAllAsync(ProvinceQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                query = query.OrderByDescending((Province x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }
            return await query.GetPagedAsync(queryModel.CurrentPage??1, queryModel.PageSize??20, sortExpression);
        }

        public async Task<IEnumerable<Province>> DeleteAsync(Guid[] deleteIds)
        {
            var provinces = await ListByIdsAsync(deleteIds);
            foreach (var province in provinces)
            {
                province.Isdeleted = true;
                _context.Provinces.Update(province);
            }
            await _context.SaveChangesAsync();
            return provinces;
        }

        private IQueryable<Province> BuildQuery(ProvinceQueryModel queryModel)
        {
            var query = _context.Provinces.AsNoTracking().Where(x => !x.Isdeleted != true);

            if (!string.IsNullOrEmpty(queryModel.Name))
            {
                query = query.Where(x => x.Name.Contains(queryModel.Name));
            }

            if (!string.IsNullOrEmpty(queryModel.MaTinhThanhPho ))
            {
                query = query.Where(x => x.Code == queryModel.MaTinhThanhPho);
            }

            return query;
        }

        public async Task<Province> SaveAsync(Province province)
        {
            var existingProvince = await _context.Provinces.AsNoTracking().FirstOrDefaultAsync(x => x.Id == province.Id);

            if (existingProvince == null)
            {
                province.CreateTracking(province.Id);
                province.UpdateTracking(province.Id);
                _context.Provinces.Add(province);
            }
            else
            {
                _context.Entry(existingProvince).State = EntityState.Detached;
                existingProvince.Name = province.Name;
                existingProvince.Slug = province.Slug;
                existingProvince.Type = province.Type;
                existingProvince.NameWithType = province.NameWithType;
                existingProvince.Code = province.Code;
                existingProvince.Isdeleted = province.Isdeleted;
                existingProvince.LastModifiedByUserId = province.LastModifiedByUserId;
                existingProvince.LastModifiedOnDate = province.LastModifiedOnDate;

                province.UpdateTracking(province.Id);
                _context.Provinces.Update(existingProvince);
            }

            await _context.SaveChangesAsync();
            return province;
        }

        public async Task<IEnumerable<Province>> SaveAsync(IEnumerable<Province> provinces)
        {
            var updatedProvinces = new List<Province>();

            foreach (var province in provinces)
            {
                var existingProvince = await _context.Provinces.AsNoTracking().FirstOrDefaultAsync(x => x.Id == province.Id);

                if (existingProvince == null)
                {
                    province.CreateTracking(province.Id);
                    province.UpdateTracking(province.Id);
                    _context.Provinces.Add(province);
                    updatedProvinces.Add(province);
                }
                else
                {
                    _context.Entry(existingProvince).State = EntityState.Detached;
                    existingProvince.Name = province.Name;
                    existingProvince.Slug = province.Slug;
                    existingProvince.Type = province.Type;
                    existingProvince.NameWithType = province.NameWithType;
                    existingProvince.Code = province.Code;
                    existingProvince.Isdeleted = province.Isdeleted;
                    existingProvince.LastModifiedByUserId = province.LastModifiedByUserId;
                    existingProvince.LastModifiedOnDate = province.LastModifiedOnDate;

                    province.UpdateTracking(province.Id);
                    _context.Provinces.Update(existingProvince);
                    updatedProvinces.Add(existingProvince);
                }
            }

            await _context.SaveChangesAsync();
            return updatedProvinces;
        }

        
    }
}
