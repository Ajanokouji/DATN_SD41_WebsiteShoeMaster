using Nest;
using Project.Business.Interface;
using Project.DbManagement;
using SERP.Framework.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Intercepter.Implement
{

    public class SaveCustomerInforAfterBill : IBillIntercepterAfterSave
    {
        private readonly IBillBusiness _billBusiness;
        private readonly ICustomerBusiness _customerBusiness;

        public SaveCustomerInforAfterBill(IBillBusiness billBusiness,ICustomerBusiness customerBusiness)
        {
            _billBusiness = billBusiness;
            _customerBusiness=customerBusiness;
        }

        public int Order { get ; set ; } =101;

        public async Task Intercept(BillEntity oldBillEntity , BillEntity updateBillEntity)
        {

            if ( updateBillEntity.CustomerId !=null )
            {
                var res = await _customerBusiness.FindByPhoneNumberAsync(updateBillEntity.RecipientPhone);
                if(res ==null)
                {
                    var newCustomer = new CustomersEntity
                    {

                        Email = updateBillEntity.RecipientEmail,
                        PhoneNumber = updateBillEntity.RecipientPhone,
                        Address = updateBillEntity.RecipientAddress,
                        Name = updateBillEntity.RecipientName,
                        Description = updateBillEntity.Note,
                        UserName = updateBillEntity.RecipientPhone,

                    };
                    _customerBusiness.SaveAsync(newCustomer);
                }
            }
        }
    }
}
