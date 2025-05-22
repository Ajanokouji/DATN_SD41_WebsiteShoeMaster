using Project.Business.Implement;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.DbManagement;
using SERP.Framework.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.ModelFactory.Implement
{
    public class BillModelFactory : IBillModelFactory
    {
        private readonly IBillDetailsBusiness _billDetailsBusiness;

        public BillModelFactory(IBillDetailsBusiness   billDetailsBusiness)
        {
            _billDetailsBusiness=billDetailsBusiness;
        }

        public async Task<List<BillEntity>> ConvertEntities(IEnumerable<BillModel> entities)
        {
            var res = new List<BillEntity>();
            {
                foreach (var model in entities)
                {
                    res.Add(await ConvertEntity(model));
                }
            }
            return res;
        }

        public async Task<BillEntity> ConvertEntity(BillModel model)
        {
            var res = new BillEntity()
            {
                Id = model.Id,
                BillCode = model.BillCode,
                CreatedByUserId = model.CreatedByUserId,
                CreatedOnDate = model.CreatedOnDate,
                LastModifiedByUserId = model.LastModifiedByUserId,
                LastModifiedOnDate = model.LastModifiedOnDate,
                Status = model.Status,
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = model.PaymentStatus,
                TotalAmount = model.TotalAmount,
                CustomerId = model.CustomerId,
                VoucherCode = model.VoucherCode,
                 AmountAfterDiscount = model.AmountAfterDiscount,
                AmountToPay = model.AmountToPay,
                FinalAmount = model.FinalAmount,
                DiscountAmount = model.DiscountAmount,
                RecipientAddress = model.RecipientAddress,
                EmployeeId = model.EmployeeId,
                IsDeleted = model.IsDeleted,
                Note = model.Note,
                OrderId = model.OrderId,
                PaymentMethodId = model.PaymentMethodId,
                RecipientEmail = model.RecipientEmail,
                RecipientName = model.RecipientName,
                RecipientPhone = model.RecipientPhone,
                VoucherId = model.VoucherId
            };
            return res;
        }

        public async Task<BillModel> CreateModel(BillEntity enitity)
        {
            var res = new BillModel()
            {
                Id = enitity.Id,
                BillCode = enitity.BillCode,
                CreatedByUserId = enitity.CreatedByUserId,
                CreatedOnDate = enitity.CreatedOnDate,
                LastModifiedByUserId = enitity.LastModifiedByUserId,
                LastModifiedOnDate = enitity.LastModifiedOnDate,
                Status = enitity.Status,
                PaymentMethod = enitity.PaymentMethod,
                PaymentStatus = enitity.PaymentStatus,
                TotalAmount = enitity.TotalAmount,
                CustomerId = enitity.CustomerId,
                VoucherCode = enitity.VoucherCode,
                AmountAfterDiscount = enitity.AmountAfterDiscount,
                AmountToPay = enitity.AmountToPay,
                FinalAmount = enitity.FinalAmount,
                DiscountAmount = enitity.DiscountAmount,
                RecipientAddress = enitity.RecipientAddress,
                EmployeeId = enitity.EmployeeId,
                IsDeleted = enitity.IsDeleted,
                Note = enitity.Note,
                CustomerAddress = enitity.RecipientAddress,
                CustomerEmail = enitity.RecipientEmail,
                CustomerName = enitity.RecipientName,
                CustomerPhone = enitity.RecipientPhone,
                Source = enitity.Source,
                OrderId = enitity.OrderId,
                PaymentMethodId = enitity.PaymentMethodId,
                RecipientEmail = enitity.RecipientEmail,
                RecipientName = enitity.RecipientName,
                RecipientPhone = enitity.RecipientPhone,
                VoucherId = enitity.VoucherId
            };
            return res;
        }

        public async Task<BillModel> CreateModel(BillEntity entity, bool getBillDetail)
        {
            var res = await CreateModel(entity);
            if (getBillDetail)
            {
                res.BillDetails = await _billDetailsBusiness.GetBillDetailsByBillId(entity.Id);
            }
            return res;
        }

        public async Task<List<BillModel>> CreateModels(IEnumerable<BillEntity> entities)
        {
            var res = new List<BillModel>();
            {
                foreach (var entity in entities)
                {
                    res.Add( await CreateModel(entity));
                }
            }
            return res;
        }

        public async Task<List<BillModel>> CreateModels(IEnumerable<BillEntity> entities, bool getBillDetails)
        {
            var res = new List<BillModel>();
            {
                foreach (var entity in entities)
                {
                    res.Add(await CreateModel(entity,getBillDetails));
                }
            }
            return res;
        }
    }
}
