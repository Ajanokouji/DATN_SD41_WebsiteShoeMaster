using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Implement
{
    public class VoucherDetailsRepository : IVoucherDetailsRepository
    {
        private readonly ProjectDbContext _context;

        public VoucherDetailsRepository(ProjectDbContext context)
        {
            _context = context;
        }
        public Task<VoucherDetails> DeleteAsync(Guid contentId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VoucherDetails>> DeleteAsync(Guid[] deleteIds)
        {
            throw new NotImplementedException();
        }

        public Task<VoucherDetails> FindAsync(Guid contentId)
        {
            throw new NotImplementedException();
        }

        public Task<Pagination<VoucherDetails>> GetAllAsync(VoucherDetailsQueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCountAsync(VoucherDetailsQueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VoucherDetails>> ListAllAsync(VoucherDetailsQueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VoucherDetails>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public Task<VoucherDetails> SaveAsync(VoucherDetails article)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VoucherDetails>> SaveAsync(IEnumerable<VoucherDetails> voucherDetails)
        {
            throw new NotImplementedException();
        }
    }
}
