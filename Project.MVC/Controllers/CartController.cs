using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Nest;
using NetTopologySuite.Index.HPRtree;
using Newtonsoft.Json;
using Project.Business.Implement;
using Project.Business.Interface;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.Entity;
using Project.DbManagement.Enum;
using Project.MVC.Models;
using SERP.Framework.Common;
using SERP.Framework.Entities.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class CartController : Controller
    {
        private readonly IVoucherBusiness _voucherBusiness;
        private readonly IVoucherProductsBusiness _voucherProductsBusiness;
        private readonly IVoucherUsersBusiness _voucherUsersBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly ICartBusiness _cartBusiness;
        private readonly ICartDetailsBusiness _cartDetailsBusiness;
        private readonly IProductBusiness _productBusiness;
        private readonly string AppliedVoucher = "AppliedVoucher";

        public CartController(IVoucherProductsBusiness voucherProductsBusiness, IVoucherUsersBusiness voucherUsersBusiness, IVoucherBusiness voucherBusiness, IUserBusiness userBusiness,ICartBusiness cartBusiness, ICartDetailsBusiness cartDetailsBusiness, IProductBusiness productBusiness)
        { 
            _voucherUsersBusiness = voucherUsersBusiness;
            _voucherProductsBusiness = voucherProductsBusiness;
            _voucherBusiness = voucherBusiness;
            _userBusiness = userBusiness;
            _cartBusiness = cartBusiness;
            _cartDetailsBusiness = cartDetailsBusiness;
            _productBusiness = productBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            HttpContext.Session.Remove(AppliedVoucher);
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
                                Size= x.Size,
                                Color = x.Color,
                                SKU= x.SKU,
                                CartId= x.IdCart,
                            }).ToList();

                            //Lấy ra thông tin sản phẩm của từng cartItemModel
                            foreach (var cartItemModel in lstCartItemModel)
                            {
                                var productFound = await _productBusiness.FindAsync(cartItemModel.ProductId);
                                var variant = productFound.VariantObjs.FirstOrDefault(x => x.Sku ==cartItemModel.SKU);
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

                                    cartItemModel.IsMax = cartItemModel.Quantity==variant.Stock ? true : false;

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

            var listProductId = cartItemsResult.Data.Select(x => x.ProductId);
            var products = await _productBusiness.ListByIdsAsync(listProductId.ToList());
            foreach (var item in cartItemsResult.Data)
            {
                var product = products.FirstOrDefault(x => x.Id==item.ProductId);
                var variant = product.VariantObjs.FirstOrDefault(x => x.Sku ==item.SKU);

                item.IsMax =(item.Quantity==variant.Stock.Value)?true:false;
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
        public async Task<IActionResult> RemoveCartItem(Guid productId , string Sku )
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
            bool IsMax = false;
            if (isLoggedIn)
            {
                var user = !string.IsNullOrEmpty(userSessionJson) ? JsonConvert.DeserializeObject<UserEntity>(userSessionJson) : null;
                if (user == null) return Json(new { success = false, message = "Phiên đăng nhập không hợp lệ" });

                // 1.2 Lấy CartDetail tương ứng
                var details = await _cartBusiness.GetCartItemsByUserId(user.Id);
                var detail = details?.FirstOrDefault(d => d.ProductId == productId && d.SKU == sku);

                var maxStock = (await _productBusiness.FindAsync(detail.ProductId)).VariantObjs.FirstOrDefault(x=>x.Sku==detail.SKU).Stock;

                //var maxStock =  await _productBusiness.
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

                if (detail.Quantity==maxStock)
                {
                    IsMax=true;
                }
            }
            else
            {

                var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
                var cartSessions = string.IsNullOrEmpty(cartSessionJson)
                                        ? new List<CartItem>()
                                        : JsonConvert.DeserializeObject<List<CartItem>>(cartSessionJson) ?? new List<CartItem>();

                var item = cartSessions.FirstOrDefault(c => c.ProductId == productId && c.SKU == sku);
                var maxStock = (await _productBusiness.FindAsync(item.ProductId)).VariantObjs.FirstOrDefault(x => x.Sku==item.SKU).Stock;

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

                if (item.Quantity==maxStock)
                {
                    IsMax=true;
                }
            }

            return Json(new
            {
                success = true,
                IsMax=IsMax,
                quantity = newQuantity,
                cartCount = cartCount,
                cartTotal = cartTotal
            });
        }

        [HttpGet]
        public async Task<IActionResult> UsersVoucher()
        {
            var errAndUserEntity = await CheckUserIsLoginning();
            if (!string.IsNullOrWhiteSpace(errAndUserEntity.Err))
            {
                TempData["ErrProfileUserMs"] = errAndUserEntity.Err;
                return BadRequest(new { error = errAndUserEntity.Err });
            }

            var user = errAndUserEntity.User;
            if (user == null)
            {
                TempData["ErrProfileUserMs"] = "Phiên đăng nhập đã hết hạn hoặc người dùng chưa đăng nhập.";
                return View(new UserEntity());
            }

            var userVoucher = await _voucherUsersBusiness.ListAllByUserIdAsync(user.Id);
            var voucherIds = userVoucher?.Select(x => x.VoucherId).ToList() ?? new List<Guid>();
            var voucherList = await _voucherBusiness.ListByIdsAsync(voucherIds);
            var userCartItem = await _cartBusiness.GetCartItemsByUserId(user.Id);

            var totalAmount = userCartItem.Sum(x => x.Total);
            var today = DateTime.UtcNow;

            // --- 1. Lọc voucher còn hiệu lực ---
            var validVouchers = voucherList
                .Where(v =>
                    v.StartDate is { } sd && v.EndDate is { } ed &&
                    sd <= today && ed >= today &&
                    v.Status == 1 &&
                    v.TotalMaxUsage > v.RedeemCount)
                .ToList();

            // --- 2. Giữ lại voucher theo sản phẩm nếu VoucherType == ByProduct ---
            foreach (var voucher in validVouchers.ToList())
            {

                if (voucher.VoucherType== VocherTypeEnum.ByProduct) {
                    var voucherProducts = await _voucherProductsBusiness.ListByVoucherIdAsync(voucher.Id);

                    bool hasMatch = voucherProducts.Any(vp =>
                        userCartItem.Any(ci =>
                            ci.ProductId == vp.ProductId &&
                            ci.SKU       == vp.VarientProductId));

                    if (!hasMatch)
                        validVouchers.Remove(voucher);
                    continue;
                };

                // ---3.Validate MinAmount
                if (voucher.MinimumOrderAmount>totalAmount)
                {
                    validVouchers.Remove(voucher);
                    continue;
                }
                // --4.Validate Used Voucher
                if (userVoucher.Any(x => x.VoucherId==voucher.Id&&x.IsUsed==true))
                {
                    validVouchers.Remove(voucher);
                    continue;
                }
            }

            // --- 3. Mapping sang ViewModel ---
            var data = AutoMapperUtils.AutoMap<Voucher, VoucherViewModel>(validVouchers); // voucher hợp lệ
            var viewdata = AutoMapperUtils.AutoMap<Voucher, VoucherViewModel>(voucherList.ToList());   // tất cả voucher

    
            var validIds = new HashSet<Guid>(data.Select(d => d.Id));   // Id voucher còn hiệu lực
            foreach (var v in viewdata.Where(v => !validIds.Contains(v.Id)))
            {
                v.IsDisable = true;           
            } 

                // --- 5. Trả viewdata để hiển thị cả hai loại voucher ---
                return PartialView("_VoucherList", viewdata.OrderBy(x=>x.IsDisable));
        }

        [HttpPost]
        public async Task<JsonResult> ApplyVoucher(Guid voucherId)
        {
            try
            {
                // Giả sử bạn có phương thức lấy voucher theo id
                var voucher = await _voucherBusiness.FindAsync(voucherId);
                if (voucher == null || voucher.Status==0)
                {
                    return Json(new { success = false, message = "Mã voucher không hợp lệ hoặc đã hết hạn." });
                }

                var userSessionJson = HttpContext.Session.GetString(UserConstants.UserSessionKey);
                var isLoggedIn = !string.IsNullOrEmpty(userSessionJson);
                var response = new { success = false, message = "Thêm vào giỏ hàng thất bại", count = 0 };

                decimal subtotal = 0;
                decimal maxDiscountAmount = voucher.MaxDiscountAmount ?? 0m;
                decimal discountAmount = 0;
                if (isLoggedIn) {
                    var user = JsonConvert.DeserializeObject<UserEntity>(userSessionJson);
                    // Lấy giỏ hàng của user hiện tại (ví dụ từ session hoặc database)
                    var cart = await _cartBusiness.GetCartItemsByUserId(user.Id);
                     subtotal = cart.Sum(i => i.Total);
                }
                else
                {
                    var cartSessionJson = HttpContext.Session.GetString(CartConstants.CartSessionKey);
                    var cartSessions = string.IsNullOrEmpty(cartSessionJson)
                        ? new List<CartItem>()
                        : JsonConvert.DeserializeObject<List<CartItem>>(cartSessionJson);
                    subtotal = cartSessions.Sum(i => i.Total);
                }


                if (voucher.DiscountAmount!=null)
                {
                    var disAmount = voucher.DiscountAmount.Value;
                    if (disAmount > subtotal)
                        disAmount = subtotal;
                    discountAmount = disAmount;
                }

                if(voucher.DiscountPercentage != null)
                {
                    var disPercent = subtotal * voucher.DiscountPercentage.Value / 100m;
                    if (voucher.MaxDiscountAmount != null && disPercent > voucher.MaxDiscountAmount.Value)
                        disPercent = voucher.MaxDiscountAmount.Value;
                    discountAmount = disPercent;
                }

                if(discountAmount>maxDiscountAmount&&maxDiscountAmount !=0)
                {
                    discountAmount = maxDiscountAmount;
                }

                if(discountAmount>subtotal)
                {
                    discountAmount = subtotal;
                }


                decimal totalAfterDiscount = subtotal - discountAmount;

                // --- Lưu thông tin voucher áp dụng vào session ---
                var appliedVoucher = new AppliedVoucher()
                {
                    VoucherCode= voucher.Code,
                    VoucherId = voucherId,
                    DiscountAmount = discountAmount,
                    TotalAfterDiscount = totalAfterDiscount
                };

                var appliedVoucherJson = JsonConvert.SerializeObject(appliedVoucher);
                HttpContext.Session.SetString("AppliedVoucher", appliedVoucherJson);

                // Trả về json
                return Json(new
                {
                    success = true,
                    discountAmount = discountAmount,
                    totalAfterDiscount = totalAfterDiscount
                });
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return Json(new { success = false, message = "Có lỗi xảy ra khi áp dụng voucher." });
            }
        }


        private async Task<ErrAndUserEntity> CheckUserIsLoginning()
        {
            ErrAndUserEntity errAndUserEntity = new ErrAndUserEntity()
            {
                Err = "",
                User = new UserEntity()
            };

            string userJson = HttpContext.Session.GetString(UserConstants.UserSessionKey);

            if (!string.IsNullOrEmpty(userJson))
            {
                var user = JsonConvert.DeserializeObject<UserEntity>(userJson);
                if (user != null)
                {
                    var userFound = await _userBusiness.FindAsync(user.Id);
                    if (userFound != null)
                    {
                        if (userFound.IsDeleted == false && userFound.Type == UserTypeEnum.Customer)
                        {
                            if (userFound.IsActive == false)
                            {
                                errAndUserEntity.Err = "Tài khoản người dùng hiện đã bị tạm dừng hoạt động! Vui lòng kiểm tra lại!";
                            }
                            else
                            {
                                errAndUserEntity.Err = "";
                                errAndUserEntity.User = userFound;
                            }
                        }
                        else
                        {
                            errAndUserEntity.Err = "Không tìm thấy người dùng này! Vui lòng kiểm tra lại!";
                        }
                    }
                    else
                    {
                        errAndUserEntity.Err = "Không tìm thấy người dùng này! Vui lòng kiểm tra lại!";
                    }
                }
                else
                {
                    errAndUserEntity.Err = "Phiên đăng nhập đã hết hạn hoặc người dùng chưa đăng nhập.";
                }
            }
            else
            {
                errAndUserEntity.Err = "Phiên đăng nhập đã hết hạn hoặc người dùng chưa đăng nhập.";
            }

            return errAndUserEntity;
        }
    }
}
