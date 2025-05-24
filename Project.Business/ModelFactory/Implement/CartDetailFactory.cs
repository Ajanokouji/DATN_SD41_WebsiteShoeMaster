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
            if (cartDetails == null)
                throw new ArgumentNullException(nameof(cartDetails));

            var listProduct = await _productRepository.ListByIdsAsync(cartDetails.Select(x => x.IdProduct));

            var modelList = new List<CartItemModel>();
            foreach (var cartDetail in cartDetails)
            {
                var product = listProduct.FirstOrDefault(x => x.Id == cartDetail.IdProduct);

                // Safely get price as string, handle nulls
                string price = "0";
                if (product?.VariantObjs != null && !string.IsNullOrEmpty(cartDetail.SKU))
                {
                    var variant = product.VariantObjs.FirstOrDefault(x => x.Sku == cartDetail.SKU);
                    price = variant?.Price ?? "0";
                }

                // Safely get metadata values
                string brand = string.Empty;
                string category = string.Empty;
                if (product?.MetadataObj != null)
                {
                    brand = MetadataUtil.GetMetadatavalue(product.MetadataObj, "Brand") ?? string.Empty;
                    category = MetadataUtil.GetMetadatavalue(product.MetadataObj, "Category") ?? string.Empty;
                }

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
                    CartId = cartDetail.IdCart,
                    Price =  decimal.TryParse(price, out var originalPrice) ? originalPrice : 0m,
                    Id = cartDetail.Id,
                    ProductName = product?.Name ?? string.Empty,
                    ProductImage = product?.ImageUrl ?? string.Empty,
                    Brand = brand,
                    Category = category,
                    Description = product?.Description ?? string.Empty,
                };
                modelList.Add(model);
            }

            return modelList;
        }

    }
}
