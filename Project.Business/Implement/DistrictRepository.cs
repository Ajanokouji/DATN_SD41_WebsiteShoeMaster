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
    public class DistrictRepository : IDistrictRepository
    {
        private readonly ProjectDbContext _context;

        public DistrictRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<District> FindAsync(Guid id)
        {
            return await _context.Districts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<District>> ListAllAsync(DistrictQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            return await query.ToListAsync();
        }

        public async Task<int> GetCountAsync(DistrictQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            return await query.CountAsync();
        }

        public async Task<IEnumerable<District>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            var stringIds = ids.Select(id => id);
            return await _context.Districts.Where(x => stringIds.Contains(x.Id)).ToListAsync();
        }

        public async Task<District> DeleteAsync(Guid id)
        {
            var existingDistrict = await FindAsync(id);
            if (existingDistrict == null) throw new Exception("District not found");

            existingDistrict.Isdeleted = true;
            _context.Districts.Update(existingDistrict);
            await _context.SaveChangesAsync();
            return existingDistrict;
        }

        public async Task<Pagination<District>> GetAllAsync(DistrictQueryModel queryModel)
        {
            var query = BuildQuery(queryModel);
            string sortExpression = string.Empty;
            if (string.IsNullOrWhiteSpace(queryModel.Sort) || queryModel.Sort.Equals("-LastModifiedOnDate"))
            {
                query = query.OrderByDescending((District x) => x.LastModifiedOnDate);
            }
            else
            {
                sortExpression = queryModel.Sort;
            }
            return await query.GetPagedAsync(queryModel.CurrentPage ?? 1, queryModel.PageSize ?? 20, sortExpression);
        }

        public async Task<IEnumerable<District>> DeleteAsync(Guid[] deleteIds)
        {
            var districts = await ListByIdsAsync(deleteIds);
            foreach (var district in districts)
            {
                district.Isdeleted = true;
                _context.Districts.Update(district);
            }
            await _context.SaveChangesAsync();
            return districts;
        }

        private IQueryable<District> BuildQuery(DistrictQueryModel queryModel)
        {
            var query = _context.Districts.AsNoTracking().Where(x => !x.Isdeleted != true);

            if (!string.IsNullOrEmpty(queryModel.Name))
            {
                query = query.Where(x => x.Name.Contains(queryModel.Name));
            }

            //if (!string.IsNullOrEmpty(queryModel.Code))
            //{
            //    query = query.Where(x => x.Code == queryModel.Code);
            //}

            return query;
        }

        public async Task<District> SaveAsync(District district)
        {
            var savedDistricts = await SaveAsync(new List<District> { district });
            return savedDistricts.FirstOrDefault(); // Return the first saved district
        }

        public async Task<IEnumerable<District>> SaveAsync(IEnumerable<District> districts)
        {
            var updatedDistricts = new List<District>();

            try
            {
                foreach (var district in districts)
                {
                    var existingDistrict = await FindAsync(district.Id);

                    if (existingDistrict == null)
                    {
                        district.CreateTracking(Guid.NewGuid());
                        district.UpdateTracking(Guid.NewGuid());
                        _context.Districts.Add(district);
                        updatedDistricts.Add(district);
                    }
                    else
                    {
                        _context.Entry(existingDistrict).State = EntityState.Detached;
                        existingDistrict.Name = district.Name;
                        existingDistrict.Type = district.Type;
                        existingDistrict.Slug = district.Slug;
                        existingDistrict.NameWithType = district.NameWithType;
                        existingDistrict.Path = district.Path;
                        existingDistrict.PathWithType = district.PathWithType;
                        existingDistrict.Code = district.Code;
                        existingDistrict.ParentCode = district.ParentCode;
                        existingDistrict.Isdeleted = district.Isdeleted;
                        existingDistrict.LastModifiedByUserId = district.LastModifiedByUserId;
                        existingDistrict.LastModifiedOnDate = district.LastModifiedOnDate;

                        district.UpdateTracking(Guid.NewGuid());
                        _context.Districts.Update(existingDistrict);
                        updatedDistricts.Add(existingDistrict);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("An error occurred while saving districts.", ex);
            }

            return updatedDistricts;
        }
    }
}
