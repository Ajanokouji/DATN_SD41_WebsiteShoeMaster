using Microsoft.Extensions.Caching.Memory;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using Serilog;
using SERP.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Project.Business.Implement
{
    public class CartBusiness : ICartBusiness
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartDetailsRepository _cartDetailsRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private const string CartListCacheKey = "CartList";
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CartBusiness(ICartRepository cartRepository, 
            ICartDetailsRepository cartDetailsRepository, IContactRepository contactRepository, IMemoryCache cache)
        {
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _cartDetailsRepository = cartDetailsRepository ?? throw new ArgumentNullException(nameof(cartDetailsRepository));
            _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = Log.ForContext<CartBusiness>();
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
        }

        public async Task<Cart> DeleteAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Invalid cart ID", nameof(id));
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await _cartRepository.DeleteAsync(id);
                    if (result != null)
                    {
                        _cache.Remove(CartListCacheKey);
                        _logger.Information("Cart {CartId} deleted successfully", id);
                    }
                    else
                    {
                        _logger.Warning("Cart {CartId} not found for deletion", id);
                    }

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting cart {CartId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Cart>> DeleteAsync(Guid[] deleteIds)
        {
            try
            {
                if (deleteIds == null || !deleteIds.Any())
                {
                    throw new ArgumentException("Delete IDs cannot be null or empty", nameof(deleteIds));
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await _cartRepository.DeleteAsync(deleteIds);
                    if (result != null && result.Any())
                    {
                        _cache.Remove(CartListCacheKey);
                        _logger.Information("Multiple carts deleted successfully: {CartIds}", string.Join(", ", deleteIds));
                    }

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting multiple carts: {CartIds}", string.Join(", ", deleteIds));
                throw;
            }
        }

        public async Task<Cart> FindAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Invalid cart ID", nameof(id));
                }

                var cart = await _cartRepository.FindAsync(id);
                if (cart == null)
                {
                    _logger.Warning("Cart {CartId} not found", id);
                }
                return cart;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error finding cart {CartId}", id);
                throw;
            }
        }

        public async Task<Pagination<Cart>> GetAllAsync(CartQueryModel queryModel)
        {
            try
            {
                if (queryModel == null)
                {
                    throw new ArgumentNullException(nameof(queryModel));
                }

                if (queryModel.PageSize == 0 && queryModel.CurrentPage == 0)
                {
                    if (_cache.TryGetValue(CartListCacheKey, out Pagination<Cart> cachedCarts))
                    {
                        _logger.Debug("Retrieved carts from cache");
                        return cachedCarts;
                    }

                    var carts = await _cartRepository.GetAllAsync(queryModel);
                    _cache.Set(CartListCacheKey, carts, _cacheOptions);
                    _logger.Debug("Carts cached successfully");
                    return carts;
                }

                return await _cartRepository.GetAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting all carts");
                throw;
            }
        }

        public async Task<int> GetCountAsync(CartQueryModel queryModel)
        {
            try
            {
                if (queryModel == null)
                {
                    throw new ArgumentNullException(nameof(queryModel));
                }

                return await _cartRepository.GetCountAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting cart count");
                throw;
            }
        }

        public async Task<IEnumerable<Cart>> ListAllAsync(CartQueryModel queryModel)
        {
            try
            {
                if (queryModel == null)
                {
                    throw new ArgumentNullException(nameof(queryModel));
                }

                return await _cartRepository.ListAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing all carts");
                throw;
            }
        }

        public async Task<IEnumerable<Cart>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    throw new ArgumentException("IDs cannot be null or empty", nameof(ids));
                }

                return await _cartRepository.ListByIdsAsync(ids);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing carts by ids: {CartIds}", string.Join(", ", ids));
                throw;
            }
        }

        public async Task<Cart> PatchAsync(Cart model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }

                var exist = await _cartRepository.FindAsync(model.Id);
                if (exist == null)
                {
                    _logger.Warning("Cart {CartId} not found for update", model.Id);
                    throw new ArgumentException("Cart not found");
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var update = new Cart
                    {
                        Id = exist.Id,
                        IdUser = model.IdUser != Guid.Empty ? model.IdUser : exist.IdUser,
                        IdContact = model.IdContact != Guid.Empty ? model.IdContact : exist.IdContact,
                        Status = model.Status != 0 ? model.Status : exist.Status,
                        Description = !string.IsNullOrWhiteSpace(model.Description) ? model.Description : exist.Description,
                        CreatedByUserId = exist.CreatedByUserId,
                        CreatedOnDate = exist.CreatedOnDate,
                        LastModifiedByUserId = exist.LastModifiedByUserId,
                        LastModifiedOnDate = DateTime.UtcNow,
                        Isdeleted = exist.Isdeleted
                    };

                    var result = await SaveAsync(update);
                    if (result != null)
                    {
                        _logger.Information("Cart {CartId} updated successfully", model.Id);
                    }

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating cart {CartId}", model?.Id);
                throw;
            }
        }

        public async Task<Cart> SaveAsync(Cart cart)
        {
            try
            {
                if (cart == null)
                {
                    throw new ArgumentNullException(nameof(cart));
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await _cartRepository.SaveAsync(cart);
                    if (result != null)
                    {
                        _cache.Remove(CartListCacheKey);
                        _logger.Information("Cart {CartId} saved successfully", cart.Id);
                    }

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error saving cart {CartId}", cart?.Id);
                throw;
            }
        }

        public async Task<IEnumerable<Cart>> SaveAsync(IEnumerable<Cart> carts)
        {
            try
            {
                if (carts == null || !carts.Any())
                {
                    throw new ArgumentException("Carts cannot be null or empty", nameof(carts));
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await _cartRepository.SaveAsync(carts);
                    if (result != null && result.Any())
                    {
                        _cache.Remove(CartListCacheKey);
                        _logger.Information("Multiple carts saved successfully: {CartIds}", 
                            string.Join(", ", result.Select(c => c.Id)));
                    }

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error saving multiple carts");
                throw;
            }
        }

        public async Task<Cart> UpdateCartAsync(Cart cart)
        {
            try
            {
                if (cart == null)
                {
                    throw new ArgumentNullException(nameof(cart));
                }

                var exist = await _cartRepository.FindAsync(cart.Id);
                if (exist == null)
                {
                    _logger.Warning("Cart {CartId} not found for update", cart.Id);
                    throw new ArgumentException("Cart not found");
                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await SaveAsync(cart);
                    _logger.Information("Cart {CartId} updated successfully", cart.Id);

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating cart {CartId}", cart?.Id);
                throw;
            }
        }

        public async Task<Cart> GetCartByUserId(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("Invalid user ID", nameof(userId));
                }

                var cart = await _cartRepository.GetCartByUserId(userId);
                if (cart == null)
                {
                    _logger.Debug("No active cart found for user {UserId}", userId);
                }
                return cart;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting cart for user {UserId}", userId);
                throw;
            }
        }

        public async Task<ServiceResult<List<CartItemModel>>> GetCartItems(List<CartItem> cartSessions)
        {
            try
            {
                if (cartSessions == null || !cartSessions.Any())
                {
                    return new ServiceResult<List<CartItemModel>>
                    {
                        IsSuccess = false,
                        Message = "Giỏ hàng trống",
                        Data = new List<CartItemModel>()
                    };
                }

                // Giả lập việc lấy thông tin sản phẩm từ database
                var cartItems = cartSessions.Select(item => new CartItemModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductImage = item.ProductImage,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Size = item.Size,
                    Color = item.Color
                }).ToList();

                return new ServiceResult<List<CartItemModel>>
                {
                    IsSuccess = true,
                    Data = cartItems,
                    Message = "Lấy thông tin giỏ hàng thành công"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<CartItemModel>>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lấy thông tin giỏ hàng: {ex.Message}",
                    Data = new List<CartItemModel>()
                };
            }
        }

        public async Task<ServiceResult<decimal>> CalculateCartTotal(List<CartItemModel> cartItems)
        {
            try
            {
                if (cartItems == null || !cartItems.Any())
                {
                    return new ServiceResult<decimal>
                    {
                        IsSuccess = false,
                        Message = "Giỏ hàng trống",
                        Data = 0
                    };
                }

                decimal total = cartItems.Sum(item => item.Price * item.Quantity);

                return new ServiceResult<decimal>
                {
                    IsSuccess = true,
                    Data = total,
                    Message = "Tính tổng giỏ hàng thành công"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<decimal>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tính tổng giỏ hàng: {ex.Message}",
                    Data = 0
                };
            }
        }

        public async Task<ServiceResult<int>> GetCartCount(List<CartItem> cartSessions)
        {
            try
            {
                if (cartSessions == null || !cartSessions.Any())
                {
                    return new ServiceResult<int>
                    {
                        IsSuccess = true,
                        Data = 0,
                        Message = "Giỏ hàng trống"
                    };
                }

                int count = cartSessions.Sum(item => item.Quantity);

                return new ServiceResult<int>
                {
                    IsSuccess = true,
                    Data = count,
                    Message = "Lấy số lượng sản phẩm trong giỏ hàng thành công"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<int>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi lấy số lượng sản phẩm trong giỏ hàng: {ex.Message}",
                    Data = 0
                };
            }
        }

        public async Task<ServiceResult<bool>> AddToCart(CartItem cartItem, List<CartItem> currentCart)
        {
            try
            {
                if (cartItem == null)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = "Thông tin sản phẩm không hợp lệ",
                        Data = false
                    };
                }

                if (currentCart == null)
                {
                    currentCart = new List<CartItem>();
                }

                // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
                var existingItem = currentCart.FirstOrDefault(x => 
                    x.ProductId == cartItem.ProductId && 
                    x.Size == cartItem.Size && 
                    x.Color == cartItem.Color);

                if (existingItem != null)
                {
                    // Nếu đã có, tăng số lượng
                    existingItem.Quantity += cartItem.Quantity;
                    existingItem.Total = existingItem.Price * existingItem.Quantity;
                }
                else
                {
                    // Nếu chưa có, thêm mới
                    cartItem.Total = cartItem.Price * cartItem.Quantity;
                    currentCart.Add(cartItem);
                }

                return new ServiceResult<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Thêm sản phẩm vào giỏ hàng thành công"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi thêm sản phẩm vào giỏ hàng: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ServiceResult<bool>> UpdateCartItem(CartItem cartItem, List<CartItem> currentCart)
        {
            try
            {
                if (cartItem == null || currentCart == null || !currentCart.Any())
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = "Thông tin giỏ hàng không hợp lệ",
                        Data = false
                    };
                }

                // Tìm sản phẩm trong giỏ hàng
                var existingItem = currentCart.FirstOrDefault(x => 
                    x.ProductId == cartItem.ProductId && 
                    x.Size == cartItem.Size && 
                    x.Color == cartItem.Color);

                if (existingItem == null)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy sản phẩm trong giỏ hàng",
                        Data = false
                    };
                }

                // Cập nhật số lượng
                existingItem.Quantity = cartItem.Quantity;
                existingItem.Total = existingItem.Price * existingItem.Quantity;

                return new ServiceResult<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Cập nhật giỏ hàng thành công"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi cập nhật giỏ hàng: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ServiceResult<bool>> RemoveFromCart(Guid productId , List<CartItem> currentCart)
        {
            try
            {
                if (currentCart == null || !currentCart.Any())
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = "Giỏ hàng trống",
                        Data = false
                    };
                }

                // Tìm sản phẩm trong giỏ hàng
                var existingItem = currentCart.FirstOrDefault(x => 
                    x.ProductId == productId);

                if (existingItem == null)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy sản phẩm trong giỏ hàng",
                        Data = false
                    };
                }

                // Xóa sản phẩm khỏi giỏ hàng
                currentCart.Remove(existingItem);

                return new ServiceResult<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Xóa sản phẩm khỏi giỏ hàng thành công"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi xóa sản phẩm khỏi giỏ hàng: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ServiceResult<bool>> ClearCart()
        {
            try
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Xóa giỏ hàng thành công"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi xóa giỏ hàng: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ServiceResult<bool>> AddCartSessionToCartDb(UserEntity user, List<CartItemModel> lstCartItemModel)
        {
            //Tìm cart dựa vào userID
            Cart cart = new Cart();
            try
            {
                cart = _cartRepository.GetCartByUserId(user.Id).Result;
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = false
                };
            }

            //Nếu không tìm thấy cart
            if (cart == null)
            {
                //Tạo contact cho user
                Contacts newContact = new Contacts
                {
                    Id = Guid.NewGuid(),
                    Name = user.Name,
                    FullName = null,
                    Address = user.Address,
                    DateOfBirth = null,
                    ImageUrl = null,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Content = null,
                    CreatedByUserId = user.Id,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = user.Id,
                    LastModifiedOnDate = DateTime.Now,
                    Isdeleted = false
                };

                //Tạo mới cart cho user
                Cart newCart = new Cart
                {
                    Id = Guid.NewGuid(),
                    IdUser = user.Id,
                    IdContact = newContact.Id,
                    Status = 1,
                    Description = "string.Empty",
                    CreatedByUserId = user.Id,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = user.Id,
                    LastModifiedOnDate = DateTime.Now,
                    Isdeleted = false
                };

                try
                {
                    await _contactRepository.SaveAsync(newContact);
                }
                catch (Exception ex)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi khi thêm thông tin contact cho người dùng: {ex.Message}",
                        Data = false
                    };
                }

                try
                {
                    await _cartRepository.SaveAsync(newCart);
                }
                catch (Exception ex)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi khi tạo mới giỏ hàng cho người dùng: {ex.Message}",
                        Data = false
                    };
                }

                cart = newCart;
            }

            //Chuyển đổi từ CartItemModel sang CartDetails
            List<CartDetails> lstCartDtsNeedAdd = lstCartItemModel.Select(x => new CartDetails
            {
                Id = Guid.NewGuid(),
                IdCart = cart.Id,
                IdProduct = x.ProductId,
                Quantity = x.Quantity,
                Code = x.ProductCode,
                CreatedByUserId = user.Id,
                IsOnSale = false, //Để tạm là false
                CreatedOnDate = DateTime.Now,
                LastModifiedByUserId = user.Id,
                LastModifiedOnDate = DateTime.Now,
                Isdeleted = false
            }).ToList();

            //Tìm các cartDetail
            List<CartDetails> lstCartDetailFound = _cartDetailsRepository.GetByCartId(cart.Id).Result.ToList();

            //Nếu không tìm thấy cartDetail nào
            if (lstCartDetailFound == null || !lstCartDetailFound.Any())
            {
                //Thêm các cartDetail mới vào db
                try
                {
                    await _cartDetailsRepository.SaveAsync(lstCartDtsNeedAdd);
                }
                catch (Exception ex)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi khi thêm cart session item vào giỏ hàng người dùng: {ex.Message}",
                        Data = false
                    };
                }

                return new ServiceResult<bool>
                {
                    IsSuccess = true,
                    Message = $"Thêm cart session vào cart user thành công",
                    Data = false
                };
            }
            else //Nếu tìm thấy
            {
                foreach (var cartDetail in lstCartDtsNeedAdd)
                {
                    //Check xem cartDetail đã tồn tại trong cart của user hay chưa
                    bool cartDetailExist = lstCartDetailFound.Any(x => x.IdProduct == cartDetail.IdProduct);

                    //Nếu cartDetail chưa tồn tại
                    if (!cartDetailExist)
                    {
                        try
                        {
                            //Thêm cartDetail vào db
                            await _cartDetailsRepository.SaveAsync(cartDetail);
                        }
                        catch (Exception ex)
                        {
                            return new ServiceResult<bool>
                            {
                                IsSuccess = false,
                                Message = $"Lỗi khi tiến hành thêm CartDetail: {ex.Message}",
                                Data = false
                            };
                        }
                    }
                    else //Nếu cartDetail đã tồn tại
                    {
                        //Cộng số lượng rồi update cartDetail
                        var cartDetailFound = lstCartDetailFound.FirstOrDefault(x => x.IdProduct == cartDetail.IdProduct);
                        cartDetailFound.Quantity += cartDetail.Quantity;
                        cartDetailFound.LastModifiedByUserId = user.Id;
                        cartDetailFound.LastModifiedOnDate = DateTime.Now;

                        try
                        {
                            //update
                            await _cartDetailsRepository.SaveAsync(cartDetailFound);
                        }
                        catch (Exception ex)
                        {
                            return new ServiceResult<bool>
                            {
                                IsSuccess = false,
                                Message = $"Lỗi khi tiến hành thêm CartDetail: {ex.Message}",
                                Data = false
                            };
                        }
                    }
                }

                return new ServiceResult<bool>
                {
                    IsSuccess = true,
                    Message = $"Thêm cart session vào cart user thành công",
                    Data = false
                };
            }
        }

        public async Task<ServiceResult<bool>> AddToCartDb(UserEntity user, List<CartDetails> lstCartDetails)
        {
            //Tìm cart dựa vào userID
            Cart cart = new Cart();
            try
            {
                cart = _cartRepository.GetCartByUserId(user.Id).Result;
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = false
                };
            }

            //Nếu không tìm thấy cart
            if (cart == null)
            {
                //Tạo contact cho user
                Contacts newContact = new Contacts
                {
                    Id = Guid.NewGuid(),
                    Name = user.Name,
                    FullName = null,
                    Address = user.Address,
                    DateOfBirth = null,
                    ImageUrl = null,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Content = null,
                    CreatedByUserId = user.Id,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = user.Id,
                    LastModifiedOnDate = DateTime.Now,
                    Isdeleted = false
                };

                //Tạo mới cart cho user
                Cart newCart = new Cart
                {
                    Id = Guid.NewGuid(),
                    IdUser = user.Id,
                    IdContact = newContact.Id,
                    Status = 1,
                    Description = "string.Empty",
                    CreatedByUserId = user.Id,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = user.Id,
                    LastModifiedOnDate = DateTime.Now,
                    Isdeleted = false
                };

                try
                {
                    await _contactRepository.SaveAsync(newContact);
                }
                catch (Exception ex)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi khi thêm thông tin contact cho người dùng: {ex.Message}",
                        Data = false
                    };
                }

                try
                {
                    await _cartRepository.SaveAsync(newCart);
                }
                catch (Exception ex)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi khi tạo mới giỏ hàng cho người dùng: {ex.Message}",
                        Data = false
                    };
                }

                cart = newCart;
            }

            List<CartDetails> lstCartDtsNeedAdd = lstCartDetails;
            foreach (var cartDetails in lstCartDetails)
            {
                cartDetails.Id = Guid.NewGuid();
                cartDetails.IdCart = cart.Id;
            }

            //Tìm các cartDetail
            List<CartDetails> lstCartDetailFound = _cartDetailsRepository.GetByCartId(cart.Id).Result.ToList();

            //Nếu không tìm thấy cartDetail nào
            if (lstCartDetailFound == null || !lstCartDetailFound.Any())
            {
                //Thêm các cartDetail mới vào db
                try
                {
                    await _cartDetailsRepository.SaveAsync(lstCartDtsNeedAdd);
                }
                catch (Exception ex)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi khi thêm sản phẩm vào giỏ hàng người dùng: {ex.Message}",
                        Data = false
                    };
                }

                return new ServiceResult<bool>
                {
                    IsSuccess = true,
                    Message = $"Thêm sản phẩm vào giỏ hàng người dùng thành công",
                    Data = false
                };
            }
            else //Nếu tìm thấy
            {
                foreach (var cartDetail in lstCartDtsNeedAdd)
                {
                    //Check xem cartDetail đã tồn tại trong cart của user hay chưa
                    bool cartDetailExist = lstCartDetailFound.Any(x => x.IdProduct == cartDetail.IdProduct);

                    //Nếu cartDetail chưa tồn tại
                    if (!cartDetailExist)
                    {
                        try
                        {
                            //Thêm cartDetail vào db
                            await _cartDetailsRepository.SaveAsync(cartDetail);
                        }
                        catch (Exception ex)
                        {
                            return new ServiceResult<bool>
                            {
                                IsSuccess = false,
                                Message = $"Lỗi khi tiến hành thêm sản phẩm vào giỏ hàng người dùng: {ex.Message}",
                                Data = false
                            };
                        }
                    }
                    else //Nếu cartDetail đã tồn tại
                    {
                        //Cộng số lượng rồi update cartDetail
                        var cartDetailFound = lstCartDetailFound.FirstOrDefault(x => x.IdProduct == cartDetail.IdProduct);
                        cartDetailFound.Quantity += cartDetail.Quantity;
                        cartDetailFound.LastModifiedByUserId = user.Id;
                        cartDetailFound.LastModifiedOnDate = DateTime.Now;

                        try
                        {
                            //update
                            await _cartDetailsRepository.SaveAsync(cartDetailFound);
                        }
                        catch (Exception ex)
                        {
                            return new ServiceResult<bool>
                            {
                                IsSuccess = false,
                                Message = $"Lỗi khi tiến hành thêm sản phẩm vào giỏ hàng người dùng: {ex.Message}",
                                Data = false
                            };
                        }
                    }
                }

                return new ServiceResult<bool>
                {
                    IsSuccess = true,
                    Message = $"Thêm sản phẩm vào giỏ hàng người dùng thành công",
                    Data = false
                };
            }
        }

        public async Task<ServiceResult<int>> GetCartDbCount(UserEntity user)
        {
            //Tìm cart dựa vào userID
            Cart cart = new Cart();
            try
            {
                cart = _cartRepository.GetCartByUserId(user.Id).Result;
            }
            catch (Exception ex)
            {
                return new ServiceResult<int>
                {
                    IsSuccess = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = 0
                };
            }

            //Nếu không tìm thấy cart
            if (cart == null)
            {
                return new ServiceResult<int>
                {
                    IsSuccess = true,
                    Message = "Giỏ hàng trống",
                    Data = 0
                };
            }

            //Tìm các cartDetail
            List<CartDetails> lstCartDetailFound = new List<CartDetails>();
            try
            {
                lstCartDetailFound = _cartDetailsRepository.GetByCartId(cart.Id).Result.ToList();
            }
            catch (Exception ex)
            {
                return new ServiceResult<int>
                {
                    IsSuccess = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = 0
                };
            }

            //Nếu không tìm thấy cartDetail nào
            if (lstCartDetailFound == null || !lstCartDetailFound.Any())
            {
                return new ServiceResult<int>
                {
                    IsSuccess = true,
                    Message = "Giỏ hàng trống",
                    Data = 0
                };
            }

            int tong = 0;
            foreach (var cartDetail in lstCartDetailFound)
            {
                if (cartDetail.Quantity != null)
                {
                    tong += cartDetail.Quantity.Value;
                }
            }

            return new ServiceResult<int>
            {
                IsSuccess = true,
                Message = "Lấy số lượng sản phẩm trong giỏ hàng thành công",
                Data = tong
            };
        }

        public async Task<ServiceResult<bool>> UpdateCartDb(UserEntity user, List<CartDetails> lstCartDetails)
        {
            //Tìm cart dựa vào userID
            Cart cart = new Cart();
            try
            {
                cart = _cartRepository.GetCartByUserId(user.Id).Result;
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = false
                };
            }

            //Nếu không tìm thấy cart
            if (cart == null)
            {
                //Tạo contact cho user
                Contacts newContact = new Contacts
                {
                    Id = Guid.NewGuid(),
                    Name = user.Name,
                    FullName = null,
                    Address = user.Address,
                    DateOfBirth = null,
                    ImageUrl = null,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Content = null,
                    CreatedByUserId = user.Id,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = user.Id,
                    LastModifiedOnDate = DateTime.Now,
                    Isdeleted = false
                };

                //Tạo mới cart cho user
                Cart newCart = new Cart
                {
                    Id = Guid.NewGuid(),
                    IdUser = user.Id,
                    IdContact = newContact.Id,
                    Status = 1,
                    Description = "string.Empty",
                    CreatedByUserId = user.Id,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = user.Id,
                    LastModifiedOnDate = DateTime.Now,
                    Isdeleted = false
                };

                try
                {
                    await _contactRepository.SaveAsync(newContact);
                }
                catch (Exception ex)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi khi thêm thông tin contact cho người dùng: {ex.Message}",
                        Data = false
                    };
                }

                try
                {
                    await _cartRepository.SaveAsync(newCart);
                }
                catch (Exception ex)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi khi tạo mới giỏ hàng cho người dùng: {ex.Message}",
                        Data = false
                    };
                }

                cart = newCart;
            }

            //update lần lượt từng cartDetail
            foreach (var cartDetails in lstCartDetails)
            {
                //Tìm cartDetail cần update
                CartDetails cartDetailFound = new CartDetails();
                try
                {
                    cartDetailFound = _cartDetailsRepository.GetByCartAndProduct(cart.Id, cartDetails.IdProduct).Result;
                }
                catch (Exception ex)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi khi thêm sản phẩm vào giỏ hàng của người dùng: {ex.Message}",
                        Data = false
                    };
                }

                //Đã tìm thấy
                if (cartDetailFound != null)
                {
                    cartDetailFound.Quantity = cartDetails.Quantity;

                    //update db
                    try
                    {
                        await _cartDetailsRepository.SaveAsync(cartDetailFound);
                    }
                    catch (Exception ex)
                    {
                        return new ServiceResult<bool>
                        {
                            IsSuccess = false,
                            Message = $"Lỗi khi thêm sản phẩm vào giỏ hàng của người dùng: {ex.Message}",
                            Data = false
                        };
                    }
                }
            }

            return new ServiceResult<bool>
            {
                IsSuccess = true,
                Message = $"Thêm sản phẩm vào giỏ hàng của người dùng thành công",
                Data = true
            };
        }

        public async Task<ServiceResult<bool>> RemoveFromCartDb(UserEntity user, Guid productId)
        {
            //Tìm cart dựa vào userID
            Cart cart = new Cart();
            try
            {
                cart = _cartRepository.GetCartByUserId(user.Id).Result;
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = $"Lỗi: {ex.Message}",
                    Data = false
                };
            }

            //Nếu không tìm thấy cart
            if (cart == null)
            {
                //Tạo contact cho user
                Contacts newContact = new Contacts
                {
                    Id = Guid.NewGuid(),
                    Name = user.Name,
                    FullName = null,
                    Address = user.Address,
                    DateOfBirth = null,
                    ImageUrl = null,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Content = null,
                    CreatedByUserId = user.Id,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = user.Id,
                    LastModifiedOnDate = DateTime.Now,
                    Isdeleted = false
                };

                //Tạo mới cart cho user
                Cart newCart = new Cart
                {
                    Id = Guid.NewGuid(),
                    IdUser = user.Id,
                    IdContact = newContact.Id,
                    Status = 1,
                    Description = "string.Empty",
                    CreatedByUserId = user.Id,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = user.Id,
                    LastModifiedOnDate = DateTime.Now,
                    Isdeleted = false
                };

                try
                {
                    await _contactRepository.SaveAsync(newContact);
                }
                catch (Exception ex)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi khi thêm thông tin contact cho người dùng: {ex.Message}",
                        Data = false
                    };
                }

                try
                {
                    await _cartRepository.SaveAsync(newCart);
                }
                catch (Exception ex)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = $"Lỗi khi tạo mới giỏ hàng cho người dùng: {ex.Message}",
                        Data = false
                    };
                }

                cart = newCart;
            }

            //Tìm cartDetail cần xóa
            CartDetails cartDetailFound = new CartDetails();
            try
            {
                cartDetailFound = _cartDetailsRepository.GetByCartAndProduct(cart.Id, productId).Result;
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi xóa sản phẩm trong giỏ hàng của người dùng: {ex.Message}",
                    Data = false
                };
            }

            //Nếu không tìm thấy cartDetail
            if (cartDetailFound == null)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = $"Không tìm thấy sản phẩm trong giỏ hàng",
                    Data = false
                };
            }

            //Xóa cartDetail
            try
            {
                await _cartDetailsRepository.DeleteAsync(cartDetailFound.Id);
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi xóa sản phẩm trong giỏ hàng của người dùng: {ex.Message}",
                    Data = false
                };
            }

            return new ServiceResult<bool>
            {
                IsSuccess = true,
                Message = $"Xóa sản phẩm khỏi giỏ hàng thành công",
                Data = true
            };
        }

        public async Task<IEnumerable<Cart>> LocCartTheoNhieuDK(CartQueryModel queryModel)
        {
            return await _cartRepository.LocCartTheoNhieuDK(queryModel);
        }
    }
}
