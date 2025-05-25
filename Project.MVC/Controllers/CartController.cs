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
                                Quantity = x.Quantity.Value,
                                Color = x.Color,
                                SKU= x.SKU,
                                CartId= x.IdCart,
                            }).ToList();

                            //Lấy ra thông tin sản phẩm của từng cartItemModel
                            foreach (var cartItemModel in lstCartItemModel)
                            {
                                var productFound = await _productBusiness.FindAsync(cartItemModel.ProductId);
                                if (productFound != null)
                                {
                                    //Lấy price
                                    string? avgPrice = productFound.VariantObjs?.FirstOrDefault(m => m.Sku == cartItemModel.SKU).Price;
                                    if (!string.IsNullOrWhiteSpace(avgPrice))
                                    {
                                        cartItemModel.Price = Convert.ToDecimal(avgPrice);
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

                var countResult = await _cartBusiness.GetCartDbCount(user.Id);
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
        public async Task<IActionResult> RemoveFromCart(Guid productId , string Sku )
        {
            //Kiểm tra đã có user đăng nhập chưa
            var userSessionJson = HttpContext.Session.GetString(UserConstants.UserSessionKey??string.Empty);
            if (!string.IsNullOrEmpty(userSessionJson))
            {
                var user = JsonConvert.DeserializeObject<UserEntity>(userSessionJson);
                //Nếu userSessions khác null => đã có user đăng nhập
                if (user != null)
                {
                    var rs = await _cartBusiness.RemoveFromCartDb(user.Id, productId,Sku);

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

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(Guid productId, string sku)
        {
            return await ChangeQuantity(productId, sku, +1);
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(Guid productId, string sku)
        {
            return await ChangeQuantity(productId, sku, -1);
        }


        private async Task<IActionResult> ChangeQuantity(Guid productId, string sku, int delta)
        {

            var userSessionJson = HttpContext.Session.GetString(UserConstants.UserSessionKey);
            var isLoggedIn = !string.IsNullOrEmpty(userSessionJson);

            int newQuantity = 0;
            int cartCount = 0;
            decimal cartTotal = 0;

            if (isLoggedIn)
            {
                var user = !string.IsNullOrEmpty(userSessionJson) ? JsonConvert.DeserializeObject<UserEntity>(userSessionJson) : null;
                if (user == null) return Json(new { success = false, message = "Phiên đăng nhập không hợp lệ" });

                // 1.2 Lấy CartDetail tương ứng
                var details = await _cartBusiness.GetCartItemsByUserId(user.Id);
                var detail = details?.FirstOrDefault(d => d.ProductId == productId && d.SKU == sku);
                if (detail == null) return Json(new { success = false, message = "Sản phẩm không tồn tại" });

                // 1.3 Cập nhật số lượng
                detail.Quantity = detail.Quantity + delta;
                if (detail.Quantity <= 0)
                {
                    // Xoá khỏi DB
                    await _cartDetailsBusiness.DeleteAsync(detail.Id);
                    newQuantity = 0;
                }
                else
                {
                    // Update DB
                    var rs = await _cartBusiness.UpdateCartDb(user, new List<CartDetails>
                    {
                        new CartDetails { IdProduct = productId, SKU = sku, Quantity = detail.Quantity }
                    });
                    if (!rs.IsSuccess) return Json(new { success = false, message = rs.Message });
                    newQuantity = detail.Quantity;
                }

                // 1.4 Lấy lại count & total cho giỏ DB
                var countRs = await _cartBusiness.GetCartDbCount(user.Id);
                cartCount = countRs.IsSuccess ? countRs.Data : 0;

                // Không có CalculateCartDbTotal, dùng GetCartItemsByUserId + CalculateCartTotal
                var items = await _cartBusiness.GetCartItemsByUserId(user.Id);
                var totalRs = await _cartBusiness.CalculateCartTotal(items);
                cartTotal = totalRs.IsSuccess ? totalRs.Data : 0;

                HttpContext.Session.SetInt32(CartConstants.CartCountKey, cartCount);
                HttpContext.Session.SetString(CartConstants.CartTotalKey, cartTotal.ToString("N0"));
            }
            else
            {

                var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
                var cartSessions = string.IsNullOrEmpty(cartSessionJson)
                                        ? new List<CartItem>()
                                        : JsonConvert.DeserializeObject<List<CartItem>>(cartSessionJson) ?? new List<CartItem>();

                var item = cartSessions.FirstOrDefault(c => c.ProductId == productId && c.SKU == sku);
                if (item == null) return Json(new { success = false, message = "Sản phẩm không tồn tại" });

                item.Quantity += delta;
                if (item.Quantity <= 0)
                    cartSessions.Remove(item);

                newQuantity = Math.Max(item.Quantity, 0);

                // Lưu lại
                HttpContext.Session.SetString(CartConstants.CartSessionKey, JsonConvert.SerializeObject(cartSessions));

                // Đếm & tính tiền
                var countRs = await _cartBusiness.GetCartCount(cartSessions);
                if (countRs.IsSuccess) cartCount = countRs.Data;

                var itemsRs = await _cartBusiness.GetCartItems(cartSessions);
                if (itemsRs.IsSuccess)
                {
                    var totalRs = await _cartBusiness.CalculateCartTotal(itemsRs.Data);
                    if (totalRs.IsSuccess) cartTotal = totalRs.Data;
                }

                HttpContext.Session.SetInt32(CartConstants.CartCountKey, cartCount);
                HttpContext.Session.SetString(CartConstants.CartTotalKey, cartTotal.ToString("N0"));
            }

            return Json(new
            {
                success = true,
                quantity = newQuantity,
                cartCount = cartCount,
                cartTotal = cartTotal
            });
        }
    }
}
