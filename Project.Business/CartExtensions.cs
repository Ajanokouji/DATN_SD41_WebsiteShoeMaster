using Project.Business.Model;
using System.Collections.Generic;
using System.Linq;

namespace Project.Common.Extensions
{
    public static class CartExtensions
    {
        public static List<CartItemModel> ToCartItemModels(this List<CartItem> cartSessions)
        {
            if (cartSessions == null || !cartSessions.Any())
            {
                return new List<CartItemModel>();
            }

            return cartSessions.Select(item => new CartItemModel
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductImage = item.ProductImage,
                Price = item.Price,
                Quantity = item.Quantity,
                Size = string.Empty, // Mặc định, cần cập nhật nếu có thông tin size
                Color = string.Empty // Mặc định, cần cập nhật nếu có thông tin màu
            }).ToList();
        }
    }
} 