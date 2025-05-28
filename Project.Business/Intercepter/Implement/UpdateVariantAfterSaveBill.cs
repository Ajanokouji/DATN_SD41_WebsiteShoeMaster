using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Common.Constants;
using Project.DbManagement;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.Intercepter.Implement
{
    public class UpdateVariantAfterSaveBill : IBillIntercepterAfterSave
    {
        private readonly IProductBusiness _productBusiness;
        private readonly IBillDetailsRepository _billDetailsRepository;
        private readonly IBillRepository _billRepository;

        public UpdateVariantAfterSaveBill(IProductBusiness productBusiness, IBillDetailsRepository billDetailsRepository, IBillRepository billRepository)
        {
            _productBusiness=productBusiness;
            _billDetailsRepository=billDetailsRepository;
            _billRepository=billRepository;
        }

        public int Order { get ; set ; }

        public async Task Intercept(BillEntity oldNodeEntity, BillEntity updatedNodeEntity)
        {
            if (oldNodeEntity!= null && updatedNodeEntity !=null&&oldNodeEntity.Status != updatedNodeEntity.Status && updatedNodeEntity.Status==BillConstants.Confirmed)
            {

                var billDetail = await _billDetailsRepository.GetBillDetailsByIdBill(updatedNodeEntity.Id);
                var productIds = billDetail.Select(x => x.IdProduct.Value).ToList();

                var listProductInBill = await _productBusiness.ListByIdsAsync(productIds);
                foreach(var item in billDetail)
                {
                    var productInBillDetail = listProductInBill.FirstOrDefault(x => x.Id == item.IdProduct);
                    var variantInBillDetail = productInBillDetail?.VariantObjs.FirstOrDefault(x => x.Sku == item.Sku);
                    var updateVariantForProduct = new Variant()
                    {
                        Group1 = variantInBillDetail?.Group1 ?? string.Empty,
                        Group2 = variantInBillDetail?.Group2 ?? string.Empty,
                        ImgUrl = variantInBillDetail?.ImgUrl ?? string.Empty,
                        Size = variantInBillDetail?.Size ?? string.Empty,
                        Sku = item.Sku,
                        Stock = variantInBillDetail?.Stock - item.Quantity ?? 0
                    };

                    await _productBusiness.PatchVariantStockBySKUAsync(productInBillDetail.Id, updateVariantForProduct);
                }
            }
         }
    }
}
