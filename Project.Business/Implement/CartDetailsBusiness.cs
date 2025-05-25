using Microsoft.Extensions.Caching.Memory;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using Serilog;
using SERP.Framework.Common;

namespace Project.Business.Implement
{
    public class CartDetailsBusiness : ICartDetailsBusiness
    {
        private readonly ICartDetailsRepository _cartDetailsRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private const string CartDetailsListCacheKey = "CartDetailsList";
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CartDetailsBusiness(ICartDetailsRepository cartDetailsRepository, IMemoryCache cache)
        {
            _cartDetailsRepository = cartDetailsRepository;
            _cache = cache;
            _logger = Log.ForContext<CartDetailsBusiness>();
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
        }

        public async Task<CartDetails> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _cartDetailsRepository.DeleteAsync(id);
                if (result != null)
                {
                    _cache.Remove(CartDetailsListCacheKey);
                    _logger.Information("Cart details {CartDetailsId} deleted successfully", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting cart details {CartDetailsId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<CartDetails>> DeleteAsync(Guid[] deleteIds)
        {
            try
            {
                var result = await _cartDetailsRepository.DeleteAsync(deleteIds);
                if (result != null && result.Any())
                {
                    _cache.Remove(CartDetailsListCacheKey);
                    _logger.Information("Multiple cart details deleted successfully: {CartDetailsIds}", string.Join(", ", deleteIds));
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting multiple cart details: {CartDetailsIds}", string.Join(", ", deleteIds));
                throw;
            }
        }

        public async Task<CartDetails> FindAsync(Guid id)
        {
            try
            {
                var cartDetails = await _cartDetailsRepository.FindAsync(id);
                if (cartDetails == null)
                {
                    _logger.Warning("Cart details {CartDetailsId} not found", id);
                }
                return cartDetails;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error finding cart details {CartDetailsId}", id);
                throw;
            }
        }

        public async Task<Pagination<CartDetails>> GetAllAsync(CartDetailsQueryModel queryModel)
        {
            try
            {
                if (queryModel.PageSize == 0 && queryModel.CurrentPage == 0)
                {
                    if (_cache.TryGetValue(CartDetailsListCacheKey, out Pagination<CartDetails> cachedCartDetails))
                    {
                        return cachedCartDetails;
                    }

                    var cartDetails = await _cartDetailsRepository.GetAllAsync(queryModel);
                    _cache.Set(CartDetailsListCacheKey, cartDetails, _cacheOptions);
                    return cartDetails;
                }

                return await _cartDetailsRepository.GetAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting all cart details");
                throw;
            }
        }

        public async Task<int> GetCountAsync(CartDetailsQueryModel queryModel)
        {
            try
            {
                return await _cartDetailsRepository.GetCountAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting cart details count");
                throw;
            }
        }

        public async Task<IEnumerable<CartDetails>> ListAllAsync(CartDetailsQueryModel queryModel)
        {
            try
            {
                return await _cartDetailsRepository.ListAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing all cart details");
                throw;
            }
        }

        public async Task<IEnumerable<CartDetails>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _cartDetailsRepository.ListByIdsAsync(ids);
        }

        public async Task<CartDetails> PatchAsync(CartDetails model)
        {
            var exist = await _cartDetailsRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException(CartDetailsConstant.CartDetailsNotFound);
            }
            var update = new CartDetails
            {
                Id = exist.Id,
                IdCart = exist.IdCart,
                IdProduct = exist.IdProduct,
                Quantity = exist.Quantity,
                IsOnSale = exist.IsOnSale,
                Code = exist.Code
            };

            if (model.Quantity != null)
            {
                update.Quantity = model.Quantity;
            }

            if (model.IsOnSale != null)
            {
                update.IsOnSale = model.IsOnSale;
            }

            if (!string.IsNullOrEmpty(model.Code))
            {
                update.Code = model.Code;
            }

            return await SaveAsync(update);
        }

        public async Task<CartDetails> SaveAsync(CartDetails productEntity)
        {
            var res = await SaveAsync(new[] { productEntity });
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<CartDetails>> SaveAsync(IEnumerable<CartDetails> productEntities)
        {
            return await _cartDetailsRepository.SaveAsync(productEntities);
        }

        public async Task<CartDetails> GetByCartAndProduct(Guid cartId, Guid productId,string sku)
        {
            try
            {
                return await _cartDetailsRepository.GetByCartAndProduct(cartId, productId, sku);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting cart details for cart {CartId} and product {ProductId}", cartId, productId);
                throw;
            }
        }

        public async Task<IEnumerable<CartDetails>> GetByCartId(Guid cartId)
        {
            try
            {
                return await _cartDetailsRepository.GetByCartId(cartId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting cart details for cart {CartId}", cartId);
                throw;
            }
        }
    }
}
