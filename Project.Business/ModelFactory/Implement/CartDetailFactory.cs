using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Business.ModelFactory.Implement
{
    public class CartDetailFactory : ICartDetailFactory
    {
        private readonly IProductRepository _productRepository;
        public CartDetailFactory(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<CartItemModel> ConvertToModel(CartDetails cartDetail)
        {
            var res = await ConvertToModels(new List<CartDetails> { cartDetail });
            return res.FirstOrDefault();
        }


        public async Task<IEnumerable<CartItemModel>> ConvertToModels(IEnumerable<CartDetails> cartDetails)
        {
            var listProduct = await _productRepository.ListByIdsAsync(cartDetails.Select(x=>x.IdProduct));
            if (cartDetails == null)
                throw new ArgumentNullException(nameof(cartDetails));

            var modelList = new List<CartItemModel>();
            foreach (var cartDetail in cartDetails)
            {
                var product = listProduct.FirstOrDefault(x => x.Id == cartDetail.IdProduct);
                var model = new CartItemModel
                {
                    ProductId = cartDetail.IdProduct,
                    ProductCode = cartDetail.Code ?? string.Empty,
                    Size = cartDetail.Size ?? string.Empty,
                    Color = cartDetail.Color ?? string.Empty,
                    Quantity = cartDetail.Quantity ?? 0,
                    SKU = cartDetail.SKU ?? string.Empty,
                    CreatedDate = cartDetail.CreatedOnDate ?? DateTime.MinValue,
                    LastModifiedDate = cartDetail.LastModifiedOnDate,
                    // Các trường dưới đây sẽ cần xử lý thêm nếu có dữ liệu từ entity Product


                    ProductName = product.Name ??string.Empty,
                    ProductImage = product.ImageUrl?? string.Empty,
                    Brand = product.MetadataObj.GetMetadatavalue("Brand")??string.Empty,
                    Category =  product.MetadataObj.GetMetadatavalue("Category")??string.Empty,
                    Description =product.Description??string.Empty,               
                };
                modelList.Add(model);
            }

            return modelList;
        }

    }
}
