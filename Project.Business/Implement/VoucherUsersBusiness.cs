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
using SkiaSharp;

namespace Project.Business.Implement
{
    public class VoucherUsersBusiness : IVoucherUsersBusiness
    {
        private readonly IVoucherUsersRepository _voucherUsersRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private const string VoucherUsersListCacheKey = "VoucherUsersList";
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public VoucherUsersBusiness(IVoucherUsersRepository voucherUsersRepository, IMemoryCache cache)
        {
            _voucherUsersRepository = voucherUsersRepository;
            _cache = cache;
            _logger = Log.ForContext<VoucherUsersBusiness>();
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(15))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        }

        public async Task<VoucherUsers> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _voucherUsersRepository.DeleteAsync(id);
                if (result != null)
                {
                    _cache.Remove(VoucherUsersListCacheKey);
                    _logger.Information($"VoucherUser {id} deleted successfully", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error deleting voucherUsers {id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<VoucherUsers>> DeleteAsync(Guid[] deleteIds)
        {
            try
            {
                var result = await _voucherUsersRepository.DeleteAsync(deleteIds);
                if (result != null && result.Any())
                {
                    _cache.Remove(VoucherUsersListCacheKey);
                    _logger.Information("Multiple voucherUsers deleted successfully: {VoucherIds}", string.Join(", ", deleteIds));
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting multiple voucherUsers: {VoucherUserIds}", string.Join(", ", deleteIds));
                throw;
            }
        }

        public async Task<VoucherUsers> FindAsync(Guid id)
        {
            try
            {
                var voucherUser = await _voucherUsersRepository.FindAsync(id);
                if (voucherUser == null)
                {
                    _logger.Warning("VoucherUser {VoucherId} not found", id);
                }
                return voucherUser;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error finding VoucherUser {VoucherId}", id);
                throw;
            }
        }

        public async Task<Pagination<VoucherUsers>> GetAllAsync(VoucherUsersQueryModel queryModel)
        {
            try
            {
                if (queryModel.PageSize == 0 && queryModel.CurrentPage == 0)
                {
                    if (_cache.TryGetValue(VoucherUsersListCacheKey, out Pagination<VoucherUsers> cachedVouchers))
                    {
                        return cachedVouchers;
                    }

                    var voucherUsers = await _voucherUsersRepository.GetAllAsync(queryModel);
                    _cache.Set(VoucherUsersListCacheKey, voucherUsers, _cacheOptions);
                    return voucherUsers;
                }

                return await _voucherUsersRepository.GetAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting all VoucherUser");
                throw;
            }
        }

        public async Task<int> GetCountAsync(VoucherUsersQueryModel queryModel)
        {
            try
            {
                return await _voucherUsersRepository.GetCountAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting VoucherUsers count");
                throw;
            }
        }

        public async Task<IEnumerable<VoucherUsers>> ListAllAsync(VoucherUsersQueryModel queryModel)
        {
            try
            {
                return await _voucherUsersRepository.ListAllAsync(queryModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing all VoucherUsers");
                throw;
            }
        }

        public async Task<IEnumerable<VoucherUsers>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            try
            {
                return await _voucherUsersRepository.ListByIdsAsync(ids);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error listing VoucherUser by ids: {VoucherIds}", string.Join(", ", ids));
                throw;
            }
        }

        public async Task<VoucherUsers> PatchAsync(VoucherUsers model)
        {
            var exist = await _voucherUsersRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException("VoucherUser not found");
            }
            var update = new VoucherUsers
            {
                Id = exist.Id,
                UserId = exist.UserId,
                VoucherId = exist.VoucherId
            };

            update.VoucherId = model.VoucherId;
            update.UserId = model.UserId;

            return await SaveAsync(update);
        }

        public async Task<VoucherUsers> SaveAsync(VoucherUsers voucherUser)
        {
            try
            {
                var result = await _voucherUsersRepository.SaveAsync(voucherUser);
                if (result != null)
                {
                    _cache.Remove(VoucherUsersListCacheKey);
                    _logger.Information("VoucherUser {VoucherId} saved successfully", voucherUser.Id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error saving VoucherUser {VoucherId}", voucherUser.Id);
                throw;
            }
        }

        public async Task<IEnumerable<VoucherUsers>> SaveAsync(IEnumerable<VoucherUsers> voucherUser)
        {
            return await _voucherUsersRepository.SaveAsync(voucherUser);
        }

        public async Task<VoucherUsers> UpdateVoucherUserAsync(VoucherUsers voucherUser)
        {
            try
            {
                var exist = await _voucherUsersRepository.FindAsync(voucherUser.Id);
                if (exist == null)
                {
                    _logger.Warning("VoucherUser {VoucherId} saved successfully", voucherUser.Id);
                    throw new ArgumentException("VoucherUser not found");
                }

                // Update voucher information
                exist.VoucherId = voucherUser.VoucherId;
                exist.UserId = voucherUser.UserId;
                exist.IsUsed = voucherUser.IsUsed;

                var result = await SaveAsync(exist);
                _logger.Information("VoucherUser {VoucherId} saved successfully", voucherUser.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating VoucherUser {VoucherId}", voucherUser.Id);
                throw;
            }
        }

        public async Task<Pagination<UserEntity>> SearchUser(UserQueryModel queryModel)
        {
            try
            {
                var users = await _voucherUsersRepository.SearchUser(queryModel);
                return users;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error when search user");
                throw;
            }
        }

        public async Task<IEnumerable<UserEntity>> GetUsersByIds(Guid[] ids)
        {
            try
            {
                var exists = await _voucherUsersRepository.GetUsersByIds(ids);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error when get users by ids");
                throw;
            }
        }

        public async Task<IEnumerable<VoucherUsers>> FindByVidUid(IEnumerable<VoucherUsers> voucherUsers)
        {
            try
            {
                var voucherUsersFind = await _voucherUsersRepository.FindByVidUid(voucherUsers);
                return voucherUsersFind;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error when Find voucher user by voucherid and userid");
                throw;
            }
        }

        public async Task<IEnumerable<VoucherUsers>> ListAllByUserIdAsync(Guid userId)
        {
            var res = await ListAllAsync(new VoucherUsersQueryModel()
            {
                UserId =userId
            });
            return res;
        }
    }
}
