using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Project.Business.Interface;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using Project.MVC.Models;
using SERP.Framework.Entities.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartBusiness _cartBusiness;
        private readonly ICartDetailsBusiness _cartDetailsBusiness;
        private readonly IProductBusiness _productBusiness;

        public CartController(ICartBusiness cartBusiness, ICartDetailsBusiness cartDetailsBusiness, IProductBusiness productBusiness)
        {
            _cartBusiness = cartBusiness;
            _cartDetailsBusiness = cartDetailsBusiness;
            _productBusiness = productBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            //Kiểm tra đã có user đăng nhập chưa
            var userSessionJson = HttpContext.Session.GetString(UserConstants.UserSessionKey);
            if (!string.IsNullOrEmpty(userSessionJson))
            {
                var userSessions = JsonConvert.DeserializeObject<UserEntity>(userSessionJson);
                //Nếu userSessions khác null => đã có user đăng nhập
                if (userSessions != null)
                {
                    //Tìm cart của user
                    var lstCartFound = await _cartBusiness.LocCartTheoNhieuDK(new CartQueryModel
                    {
                        IdTaiKhoan = userSessions.Id,
                        Status = 1, //Status = đang hoạt động, Giả sử status = 1 là đang hoạt động
                    });

                    //Lấy cart chưa bị xóa
                    var cartFound = lstCartFound?.FirstOrDefault(x => x.IsDeleted == false);
                    //Ko thấy cart
                    if (cartFound == null)
                    {
                        //Trả về view rỗng
                        return View(new List<CartItemModel>());
                    }
                    else //Tìm thấy
                    {
                        //Tìm cartDetail
                        var lstCartDetailFound = await _cartDetailsBusiness.GetByCartId(cartFound.Id);

                        //Nếu cartDetail không rỗng
                        if (lstCartDetailFound != null && lstCartDetailFound.Any())
                        {
                            //Chuyển cartdetail thành cartItemModel
                            List<CartItemModel> lstCartItemModel = lstCartDetailFound.Select(x => new CartItemModel
                            {
                                ProductId = x.IdProduct,
                                Quantity = x.Quantity.Value
                            }).ToList();

                            //Lấy ra thông tin sản phẩm của từng cartItemModel
                            foreach (var cartItemModel in lstCartItemModel)
                            {
                                var productFound = await _productBusiness.FindAsync(cartItemModel.ProductId);
                                if (productFound != null)
                                {
                                    //Lấy price
                                    string? avgPrice = productFound.MetadataObj?.FirstOrDefault(m => m.FieldName == "AvgPrice")?.FieldValues;
                                    if (!string.IsNullOrWhiteSpace(avgPrice))
                                    {
                                        cartItemModel.Price = Convert.ToDecimal(avgPrice);
                                    }

             
                                    cartItemModel.Size = string.Empty;

                                    //Lấy color
                                    string? color = productFound.MetadataObj?.FirstOrDefault(m => m.FieldName == "Colorway")?.FieldValues;
                                    if (!string.IsNullOrWhiteSpace(color))
                                    {
                                        cartItemModel.Color = color;
                                    }

                                    if (!string.IsNullOrWhiteSpace(productFound.Name))
                                    {
                                        cartItemModel.ProductName = productFound.Name;
                                    }

                                    if (!string.IsNullOrWhiteSpace(productFound.ImageUrl))
                                    {
                                        cartItemModel.ProductImage = productFound.ImageUrl;
                                    }
                                }
                            }

                            return View(lstCartItemModel);
                        }
                        else //lst cartDetail rỗng
                        {
                            return View(new List<CartItemModel>());
                        }
                    }
                }
            }

            //Trường hợp chưa có user nào đăng nhập
            var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
            if (string.IsNullOrEmpty(cartSessionJson))
            {
                return View(new List<CartItemModel>());
            }

            var cartSessions = JsonConvert.DeserializeObject<List<CartItem>>(cartSessionJson);
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
        public async Task<IActionResult> AddToCart([FromBody] AddToCartViewModel productData)
        {
            var userSessionJson = HttpContext.Session.GetString(UserConstants.UserSessionKey);
            var isLoggedIn = !string.IsNullOrEmpty(userSessionJson);
            var response = new { success = false, message = "Thêm vào giỏ hàng thất bại", count = 0 };

            if (isLoggedIn)
            {
                var user = JsonConvert.DeserializeObject<UserEntity>(userSessionJson);
                if (user == null)
                    return Json(response);

                var cartDetails = new List<CartDetails>
                {
                    new CartDetails
                    {
                         Id = Guid.NewGuid(),
                        SKU = productData.SKU ?? string.Empty,
                        IdProduct = productData.ProductId ?? Guid.Empty,
                        Quantity = productData.Quantity ?? 0,
                        Size = productData.Size ?? string.Empty,
                        Color = productData.Color ?? string.Empty,
                        IsOnSale = false
                    }
                };

                var result = await _cartBusiness.AddToCartAsync(user.Id, cartDetails);
                if (!result.IsSuccess)
                    return Json(new { success = false, message = result.Message });

                var countResult = await _cartBusiness.GetCartDbCount(user);
                if (!countResult.IsSuccess)
                    return Json(new { success = true, message = result.Message, count = 0 });

                return Json(new { success = true, message = result.Message, count = countResult.Data });
            }
            else
            {
                var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
                var cartSessions = string.IsNullOrEmpty(cartSessionJson)
                    ? new List<CartItem>()
                    : JsonConvert.DeserializeObject<List<CartItem>>(cartSessionJson);

                var cartItem = new CartItem
                {

                    ProductId = productData.ProductId ?? Guid.Empty,
                    ProductName = productData.ProductName ?? string.Empty,
                    ProductImage = productData.ProductImage ?? string.Empty,
                    SKU= productData.SKU??string.Empty,
                    Price = productData.Price ?? 0m,
                    Total = productData.Total??0m,
                    Quantity = productData.Quantity ?? 0,
                    Size = productData.Size ?? string.Empty,
                    Color = productData.Color ?? string.Empty
                };

                var result = await _cartBusiness.AddToCart(cartItem, cartSessions);
                if (!result.IsSuccess)
                    return Json(new { success = false, message = result.Message, count = cartSessions.Count });

                HttpContext.Session.SetString(CartConstants.CartSessionKey, JsonConvert.SerializeObject(cartSessions));

                var countResult = await _cartBusiness.GetCartCount(cartSessions);
                if (countResult.IsSuccess)
                    HttpContext.Session.SetInt32(CartConstants.CartCountKey, countResult.Data);

                var cartItemsResult = await _cartBusiness.GetCartItems(cartSessions);
                if (cartItemsResult.IsSuccess)
                {
                    var totalResult = await _cartBusiness.CalculateCartTotal(cartItemsResult.Data);
                    if (totalResult.IsSuccess)
                        HttpContext.Session.SetString(CartConstants.CartTotalKey, totalResult.Data.ToString());
                }

                return Json(new { success = true, message = result.Message, count = cartSessions.Count });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateCart(Guid productId, int quantity, string size, string color)
        {
            //Kiểm tra đã có user đăng nhập chưa
            var userSessionJson = HttpContext.Session.GetString(UserConstants.UserSessionKey);
            if (!string.IsNullOrEmpty(userSessionJson))
            {
                var userSessions = JsonConvert.DeserializeObject<UserEntity>(userSessionJson);
                //Nếu userSessions khác null => đã có user đăng nhập
                if (userSessions != null)
                {
                    CartDetails cartDetails = new CartDetails
                    {
                        IdProduct = productId,
                        Quantity = quantity
                    };
                    List<CartDetails> lstCartDetails = new List<CartDetails>();
                    lstCartDetails.Add(cartDetails);

                    //update
                    var rs = await _cartBusiness.UpdateCartDb(userSessions, lstCartDetails);
                    if (!rs.IsSuccess)
                    {
                        return Json(new { success = false, message = rs.Message });
                    }

                    return Json(new { success = rs.IsSuccess, message = rs.Message });
                }
            }

            //Trường hợp chưa có user nào đăng nhập
            var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
            if (string.IsNullOrEmpty(cartSessionJson))
            {
                return Json(new { success = false, message = "Giỏ hàng trống" });
            }

            var cartSessions = JsonConvert.DeserializeObject<List<CartItem>>(cartSessionJson);
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
        public async Task<IActionResult> RemoveFromCart(Guid productId)
        {
            //Kiểm tra đã có user đăng nhập chưa
            var userSessionJson = HttpContext.Session.GetString(UserConstants.UserSessionKey);
            if (!string.IsNullOrEmpty(userSessionJson))
            {
                var userSessions = JsonConvert.DeserializeObject<UserEntity>(userSessionJson);
                //Nếu userSessions khác null => đã có user đăng nhập
                if (userSessions != null)
                {
                    var rs = await _cartBusiness.RemoveFromCartDb(userSessions, productId);

                    if (!rs.IsSuccess)
                    {
                        return Json(new { success = false, message = rs.Message });
                    }

                    return Json(new { success = rs.IsSuccess, message = rs.Message });
                }
            }

            //Trường hợp chưa có user nào đăng nhập
            var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
            if (string.IsNullOrEmpty(cartSessionJson))
            {
                return Json(new { success = false, message = "Giỏ hàng trống" });
            }

            var cartSessions = JsonConvert.DeserializeObject<List<CartItem>>(cartSessionJson);
            if (cartSessions == null || !cartSessions.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng trống" });
            }

            var result = await _cartBusiness.RemoveFromCart(productId, cartSessions);
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

            var cartSessions = JsonConvert.DeserializeObject<List<CartItem>>(cartSessionJson);
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

            var cartSessions = JsonConvert.DeserializeObject<List<CartItem>>(cartSessionJson);
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
