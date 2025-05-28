using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using Serilog;
using Project.DbManagement;

namespace Project.Business.Implement
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public UserBusiness(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _logger = Log.ForContext<UserBusiness>();
        }

        public async Task<UserEntity> DeleteAsync(Guid id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<UserEntity>> DeleteAsync(Guid[] deleteIds)
        {
            return await _userRepository.DeleteAsync(deleteIds);
        }

        public async Task<UserEntity> FindAsync(Guid id)
        {
            return await _userRepository.FindAsync(id);
        }

        public async Task<Pagination<UserEntity>> GetAllAsync(UserQueryModel queryModel)
        {
            return await _userRepository.GetAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(UserQueryModel queryModel)
        {
            return await _userRepository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<UserEntity>> ListAllAsync(UserQueryModel queryModel)
        {
            return await _userRepository.ListAllAsync(queryModel);
        }

        public async Task<IEnumerable<UserEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _userRepository.ListByIdsAsync(ids);
        }

        public async Task<UserEntity> PatchAsync(UserEntity model)
        {
            var exist = await _userRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException("User not found");
            }
            var update = new UserEntity
            {
                Id = exist.Id,
                Type = exist.Type,
                Username = exist.Username,
                Name = exist.Name,
                PhoneNumber = exist.PhoneNumber,
                Email = exist.Email,
                Address = exist.Address,
                AvartarUrl = exist.AvartarUrl,
                Password = exist.Password,
                UserDetailJson = exist.UserDetailJson,
                CreatedByUserId = exist.CreatedByUserId,
                CreatedOnDate = exist.CreatedOnDate,
                LastModifiedByUserId = exist.LastModifiedByUserId,
                LastModifiedOnDate = exist.LastModifiedOnDate,
                IsDeleted = exist.IsDeleted,
                IsActive = exist.IsActive,
            };

            if (!string.IsNullOrWhiteSpace(model.Username))
            {
                update.Username = model.Username;
            }
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                update.Name = model.Name;
            }
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                update.Email = model.Email;
            }
            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                update.PhoneNumber = model.PhoneNumber;
            }
            if (model.IsActive.HasValue)
            {
                update.IsActive = model.IsActive;
            }

            //Để AvartarUrl có thể để trống
            update.AvartarUrl = model.AvartarUrl;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                update.Password = model.Password;
            }
            if (!string.IsNullOrWhiteSpace(model.UserDetailJson))
            {
                update.UserDetailJson = model.UserDetailJson;
            }
            return await SaveAsync(update);
        }

        public async Task<UserEntity> SaveAsync(UserEntity user)
        {
            var res = await SaveAsync(new[] { user });
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<UserEntity>> SaveAsync(IEnumerable<UserEntity> users)
        {
            return await _userRepository.SaveAsync(users);
        }

        public async Task<UserEntity> GetUserByEmail(string email)
        {
            try
            {

                var users = await _userRepository.ListAllAsync(new UserQueryModel { Email = email });
                var user = users.FirstOrDefault();

                return user;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Lỗi khi tìm người dùng theo email {Email}", email);
                throw new Exception($"Lỗi khi tìm người dùng theo email: {ex.Message}");
            }
        }

        public async Task<UserEntity> CreateUserFromCustomerInfo(CustomerInfoModel customerInfo)
        {
            try
            {
                // Tạo người dùng mới
                var newUser = new UserEntity
                {
                    Id = customerInfo.UserId??Guid.NewGuid(),
                    Type = UserTypeEnum.Customer,
                    Username = customerInfo.Email,
                    Name = customerInfo.FullName,
                    PhoneNumber = customerInfo.PhoneNumber,
                    Email = customerInfo.Email,
                    Address = customerInfo.Address,
                    IsDeleted = false
                };

                var savedUser = await SaveAsync(newUser);

                return savedUser;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Lỗi khi tạo người dùng mới từ thông tin khách hàng");
               throw new Exception($"Lỗi khi tạo người dùng mới từ thông tin khách hàng: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateUserInfo(Guid userId, CustomerInfoModel customerInfo)
        {
            try
            {
                var user = await FindAsync(userId);
                if (user == null)
                {
                    return new ServiceResult<bool>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng",
                        Data = false
                    };
                }

                // Cập nhật thông tin
                user.Name = customerInfo.FullName;
                user.PhoneNumber = customerInfo.PhoneNumber;
                user.Email = customerInfo.Email;
                user.LastModifiedOnDate = DateTime.Now;

                await SaveAsync(user);

                return new ServiceResult<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Cập nhật thông tin người dùng thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Lỗi khi cập nhật thông tin người dùng {UserId}", userId);
                return new ServiceResult<bool>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi cập nhật thông tin người dùng: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<IEnumerable<UserEntity>> LocUserTheoNhieuDK(UserQueryModel queryModel)
        {
            return await _userRepository.LocUserTheoNhieuDK(queryModel);
        }

        public UserEntity UserLogin(string username, string password)
        {
            return _userRepository.UserLogin(username, password);
        }
    }
}