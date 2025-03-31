using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Project.Business.Interface;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
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
                    var cartFound = lstCartFound?.FirstOrDefault(x => x.Isdeleted == false);
                    //Ko thấy cart
                    if (cartFound == null)
                    {
                        //Trả về view rỗng
                        return View(new List<CartItemModel>());
                    }
                    else //Tìm thấy
                    {
                        //Tìm cartDetail
                        var lstCartDetailFound = _cartDetailsBusiness.GetByCartId(cartFound.Id);

                        //Nếu cartDetail không rỗng
                        if (lstCartDetailFound.Result != null && lstCartDetailFound.Result.Any())
                        {
                            //Chuyển cartdetail thành cartItemModel
                            List<CartItemModel> lstCartItemModel = lstCartDetailFound.Result.Select(x => new CartItemModel
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

                                    //Chưa rõ lấy size như thế nào nên để tạm thời = 1
                                    cartItemModel.Size = 1;

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
                        Id = Guid.NewGuid(),
                        IdProduct = productId,
                        Quantity = quantity,
                        IsOnSale = false
                    };

                    List<CartDetails> lstCartDetails = new List<CartDetails>();
                    lstCartDetails.Add(cartDetails);

                    var rs = await _cartBusiness.AddToCartDb(userSessions, lstCartDetails);

                    if (!rs.IsSuccess)
                    {
                        return Json(new { success = rs.IsSuccess, message = rs.Message });
                    }

                    var countResult = await _cartBusiness.GetCartDbCount(userSessions);

                    if (!countResult.IsSuccess)
                    {
                        return Json(new { success = rs.IsSuccess, message = rs.Message });
                    }

                    return Json(new { success = rs.IsSuccess, message = rs.Message, countResult.Data});
                }
            }

            //Trường hợp chưa có user nào đăng nhập
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
