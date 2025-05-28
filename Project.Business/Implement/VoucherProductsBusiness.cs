using LinqKit;
using Microsoft.Extensions.Caching.Memory;
using Nest;
using Org.BouncyCastle.Crypto;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.Entity;
using Serilog;
using SERP.Framework.Business;
using SERP.Framework.Common;
using SERP.Framework.DB.Extensions;

namespace Project.Business.Implement
{
    public class VoucherProductsBusiness : IVoucherProductsBusiness
    {
        private readonly IVoucherProductsRepository _voucherProductsRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private const string VoucherProductsListCacheKey = "VoucherProductsList";
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public VoucherProductsBusiness(IVoucherProductsRepository voucherProductsRepository, IMemoryCache cache)
        {
            _voucherProductsRepository = voucherProductsRepository;
            _cache = cache;
            _logger = Log.ForContext<VoucherProductsBusiness>();
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(15))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        }

        public async Task<VoucherProducts> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _voucherProductsRepository.DeleteAsync(id);
                if (result != null)
                {
                    _cache.Remove(VoucherProductsListCacheKey);
                    _logger.Information($"VoucherProduct {id} deleted successfully", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error deleting voucherProducts {id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<VoucherProducts>> DeleteAsync(Guid[] deleteIds)
        {
            try
            {
                var result = await _voucherProductsRepository.DeleteAsync(deleteIds);
                if (result != null && result.Any())
                {
                    _cache.Remove(VoucherProductsListCacheKey);
                    _logger.Information("Multiple voucherProducts deleted successfully: {VoucherIds}", string.Join(", ", deleteIds));
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting multiple voucherProducts: {VoucherIds}", string.Join(", ", deleteIds));
                throw;
            }
        }

        public async Task<VoucherProducts> FindAsync(Guid id)
        {
            try
            {
                var voucherProduct = await _voucherProductsRepository.FindAsync(id);
                if (voucherProduct == null)
                {
                    _logger.Warning("VoucherProduct {VoucherId} not found", id);
                }
                return voucherProduct;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error finding VoucherProduct {VoucherId}", id);
                throw;
            }
        }

        public async Task<Pagination<VoucherProducts>> GetAllAsync(VoucherProductsQueryModel queryModel)
        {
            try
            {
                if (queryModel.PageSize == 0 && queryModel.CurrentPage == 0)
                {
                    if (_cache.TryGetValue(VoucherProductsListCacheKey, out Pagination<VoucherProducts> cachedVouchers))
                    {
                        return cachedVouchers;
                    }

                    var voucherProducts = await _voucherProductsRepository.GetAllAsync(queryModel);
                    _cache.Set(VoucherProductsListCacheKey, voucherProducts, _cacheOptions);
                    return voucherProducts;
                }

                return await _voucherProductsRepository.GetAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting all voucherProducts");
                throw;
            }
        }

        public async Task<int> GetCountAsync(VoucherProductsQueryModel queryModel)
        {
            try
            {
                return await _voucherProductsRepository.GetCountAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting voucherProducts count");
                throw;
            }
        }

        public async Task<IEnumerable<VoucherProducts>> ListAllAsync(VoucherProductsQueryModel queryModel)
        {
            try
            {
                return await _voucherProductsRepository.ListAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing all voucherProducts");
                throw;
            }
        }

        public async Task<IEnumerable<VoucherProducts>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            try
            {
                return await _voucherProductsRepository.ListByIdsAsync(ids);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing voucherProducts by ids: {VoucherIds}", string.Join(", ", ids));
                throw;
            }
        }

        public async Task<VoucherProducts> PatchAsync(VoucherProducts model)
        {
            var exist = await _voucherProductsRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException("VoucherProducts not found");
            }
            var update = new VoucherProducts
            {
                Id = exist.Id,
                ProductId = exist.ProductId,
                VoucherId = exist.VoucherId,
                VarientProductId = exist.VarientProductId
            };

            update.ProductId = model.ProductId;
            update.VoucherId = model.VoucherId;

            return await SaveAsync(update);
        }

        public async Task<VoucherProducts> SaveAsync(VoucherProducts voucherProduct)
        {
            try
            {
                var result = await _voucherProductsRepository.SaveAsync(voucherProduct);
                if (result != null)
                {
                    _cache.Remove(VoucherProductsListCacheKey);
                    _logger.Information("VoucherProduct {VoucherId} saved successfully", voucherProduct.Id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error saving voucherProduct {VoucherId}", voucherProduct.Id);
                throw;
            }
        }

        public async Task<IEnumerable<VoucherProducts>> SaveAsync(IEnumerable<VoucherProducts> voucherProduct)
        {
            return await _voucherProductsRepository.SaveAsync(voucherProduct);
        }

        public async Task<VoucherProducts> UpdateVoucherProductAsync(VoucherProducts voucherProduct)
        {
            try
            {
                var exist = await _voucherProductsRepository.FindAsync(voucherProduct.Id);
                if (exist == null)
                {
                    _logger.Warning("VoucherProduct {VoucherId} saved successfully", voucherProduct.Id);
                    throw new ArgumentException("VoucherProduct not found");
                }

                // Update voucher information
                exist.VoucherId = voucherProduct.VoucherId;
                exist.ProductId = voucherProduct.ProductId;
                exist.VarientProductId = voucherProduct.VarientProductId;

                var result = await SaveAsync(exist);
                _logger.Information("VoucherProduct {VoucherId} saved successfully", voucherProduct.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating voucherProduct {VoucherId}", voucherProduct.Id);
                throw;
            }
        }

        public async Task<IEnumerable<ProductEntity>> GetProductsByIds(Guid[] ids)
        {
            try
            {
                var exists = await _voucherProductsRepository.GetProductsByIds(ids);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error when get products by ids");
                throw;
            }
        }

        public async Task<Pagination<ProductEntity>> SearchProduct(ProductQueryModel queryModel)
        {
            try
            {
                var products = await _voucherProductsRepository.SearchProduct(queryModel);
                return products;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error when search product");
                throw;
            }
        }

        public async Task<IEnumerable<VoucherProducts>> FindByVcidPidVaid(IEnumerable<VoucherProducts> voucherProducts)
        {
            try
            {
                var voucherProductsFind = await _voucherProductsRepository.FindByVcidPidVaid(voucherProducts);
                return voucherProductsFind;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error when Find voucher product by voucherid, productid and variantid");
                throw;
            }
        }

        public async Task<IEnumerable<VoucherProducts>> ListByVoucherIdAsync(Guid id)
        {
            var res = await _voucherProductsRepository.ListAllAsync(new VoucherProductsQueryModel()
            {
                VoucherId =id
            });
            return res;
        }
    }
}
