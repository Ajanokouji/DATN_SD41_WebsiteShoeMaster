using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business
{
    public interface IPaymentMethodsRepository : IRepository<PaymentMethods, PaymentMethodsQueryModel>
    {
        protected const string MessageNoTFound = "Product not found";
        Task<PaymentMethods> SaveAsync(PaymentMethods article);
        Task<IEnumerable<PaymentMethods>> SaveAsync(IEnumerable<PaymentMethods> paymentMethods);
    }
}
