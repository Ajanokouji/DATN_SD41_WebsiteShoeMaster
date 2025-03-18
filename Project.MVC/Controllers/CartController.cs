using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.Business.Interface;
using Project.Business.Model;
using Project.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartBusiness _cartBusiness;

        public CartController(ICartBusiness cartBusiness)
        {
            _cartBusiness = cartBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
            if (string.IsNullOrEmpty(cartSessionJson))
            {
                return View(new List<CartItemModel>());
            }

            var cartSessions = JsonConvert.DeserializeObject<List<CartSession>>(cartSessionJson);
            if (cartSessions == null || !cartSessions.Any())
            {
                return View(new List<CartItemModel>());
            }

            var cartItemsResult = await _cartBusiness.GetCartItems(cartSessions);
            if (!cartItemsResult.IsSuccess)
            {
                return View(new List<CartItemModel>());
            }

            return View(cartItemsResult.Data);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid productId, string productName, string productImage, decimal price, int quantity, int size, string color)
        {
            var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
            var cartSessions = string.IsNullOrEmpty(cartSessionJson)
                ? new List<CartSession>()
                : JsonConvert.DeserializeObject<List<CartSession>>(cartSessionJson);

            var cartItem = new CartSession
            {
                ProductId = productId,
                ProductName = productName,
                ProductImage = productImage,
                Price = price,
                Quantity = quantity,
                Size = size,
                Color = color
            };

            var result = await _cartBusiness.AddToCart(cartItem, cartSessions);
            if (result.IsSuccess)
            {
                HttpContext.Session.SetString(CartConstants.CartSessionKey, JsonConvert.SerializeObject(cartSessions));
                
                // Cập nhật số lượng sản phẩm trong giỏ hàng
                var countResult = await _cartBusiness.GetCartCount(cartSessions);
                if (countResult.IsSuccess)
                {
                    HttpContext.Session.SetInt32(CartConstants.CartCountKey, countResult.Data);
                }

                // Cập nhật tổng tiền giỏ hàng
                var cartItemsResult = await _cartBusiness.GetCartItems(cartSessions);
                if (cartItemsResult.IsSuccess)
                {
                    var totalResult = await _cartBusiness.CalculateCartTotal(cartItemsResult.Data);
                    if (totalResult.IsSuccess)
                    {
                        HttpContext.Session.SetString(CartConstants.CartTotalKey, totalResult.Data.ToString());
                    }
                }
            }

            return Json(new { success = result.IsSuccess, message = result.Message, count = cartSessions.Count });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(Guid productId, int quantity, int size, string color)
        {
            var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
            if (string.IsNullOrEmpty(cartSessionJson))
            {
                return Json(new { success = false, message = "Giỏ hàng trống" });
            }

            var cartSessions = JsonConvert.DeserializeObject<List<CartSession>>(cartSessionJson);
            if (cartSessions == null || !cartSessions.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng trống" });
            }

            var cartItem = cartSessions.FirstOrDefault(x => x.ProductId == productId && x.Size == size && x.Color == color);
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
            }

            cartItem.Quantity = quantity;
            var result = await _cartBusiness.UpdateCartItem(cartItem, cartSessions);
            if (result.IsSuccess)
            {
                HttpContext.Session.SetString(CartConstants.CartSessionKey, JsonConvert.SerializeObject(cartSessions));
                
                // Cập nhật số lượng sản phẩm trong giỏ hàng
                var countResult = await _cartBusiness.GetCartCount(cartSessions);
                if (countResult.IsSuccess)
                {
                    HttpContext.Session.SetInt32(CartConstants.CartCountKey, countResult.Data);
                }

                // Cập nhật tổng tiền giỏ hàng
                var cartItemsResult = await _cartBusiness.GetCartItems(cartSessions);
                if (cartItemsResult.IsSuccess)
                {
                    var totalResult = await _cartBusiness.CalculateCartTotal(cartItemsResult.Data);
                    if (totalResult.IsSuccess)
                    {
                        HttpContext.Session.SetString(CartConstants.CartTotalKey, totalResult.Data.ToString());
                    }
                }
            }

            return Json(new { success = result.IsSuccess, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(Guid productId, int size)
        {
            var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
            if (string.IsNullOrEmpty(cartSessionJson))
            {
                return Json(new { success = false, message = "Giỏ hàng trống" });
            }

            var cartSessions = JsonConvert.DeserializeObject<List<CartSession>>(cartSessionJson);
            if (cartSessions == null || !cartSessions.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng trống" });
            }

            var result = await _cartBusiness.RemoveFromCart(productId, size, cartSessions);
            if (result.IsSuccess)
            {
                HttpContext.Session.SetString(CartConstants.CartSessionKey, JsonConvert.SerializeObject(cartSessions));
                
                // Cập nhật số lượng sản phẩm trong giỏ hàng
                var countResult = await _cartBusiness.GetCartCount(cartSessions);
                if (countResult.IsSuccess)
                {
                    HttpContext.Session.SetInt32(CartConstants.CartCountKey, countResult.Data);
                }

                // Cập nhật tổng tiền giỏ hàng
                var cartItemsResult = await _cartBusiness.GetCartItems(cartSessions);
                if (cartItemsResult.IsSuccess)
                {
                    var totalResult = await _cartBusiness.CalculateCartTotal(cartItemsResult.Data);
                    if (totalResult.IsSuccess)
                    {
                        HttpContext.Session.SetString(CartConstants.CartTotalKey, totalResult.Data.ToString());
                    }
                }
            }

            return Json(new { success = result.IsSuccess, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _cartBusiness.ClearCart();
            if (result.IsSuccess)
            {
                HttpContext.Session.Remove(CartConstants.CartSessionKey);
                HttpContext.Session.Remove(CartConstants.CartCountKey);
                HttpContext.Session.Remove(CartConstants.CartTotalKey);
            }

            return Json(new { success = result.IsSuccess, message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
            if (string.IsNullOrEmpty(cartSessionJson))
            {
                return Json(new { count = 0 });
            }

            var cartSessions = JsonConvert.DeserializeObject<List<CartSession>>(cartSessionJson);
            if (cartSessions == null || !cartSessions.Any())
            {
                return Json(new { count = 0 });
            }

            var result = await _cartBusiness.GetCartCount(cartSessions);
            return Json(new { count = result.IsSuccess ? result.Data : 0 });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartTotal()
        {
            var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
            if (string.IsNullOrEmpty(cartSessionJson))
            {
                return Json(new { total = 0 });
            }

            var cartSessions = JsonConvert.DeserializeObject<List<CartSession>>(cartSessionJson);
            if (cartSessions == null || !cartSessions.Any())
            {
                return Json(new { total = 0 });
            }

            var cartItemsResult = await _cartBusiness.GetCartItems(cartSessions);
            if (!cartItemsResult.IsSuccess)
            {
                return Json(new { total = 0 });
            }

            var result = await _cartBusiness.CalculateCartTotal(cartItemsResult.Data);
            return Json(new { total = result.IsSuccess ? result.Data : 0 });
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return RedirectToAction("Index", "Checkout");
        }
    }
}
