using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Interface
{
    public interface IVoucherDetailsRepository : IRepository<VoucherDetails, VoucherDetailsQueryModel>
    {
        protected const string MessageNotFound = "VoucherDetails not found";
        Task<VoucherDetails> SaveAsync(VoucherDetails article);
        Task<IEnumerable<VoucherDetails>> SaveAsync(IEnumerable<VoucherDetails> voucherDetails);
    }
}
