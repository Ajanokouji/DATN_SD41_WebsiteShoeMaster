using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using Serilog;

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
                AvartarUrl = exist.AvartarUrl,
                Password = exist.Password,
                UserDetailJson = exist.UserDetailJson,
                CreatedByUserId = exist.CreatedByUserId,
                CreatedOnDate = exist.CreatedOnDate,
                LastModifiedByUserId = exist.LastModifiedByUserId,
                LastModifiedOnDate = exist.LastModifiedOnDate,
                Isdeleted = exist.Isdeleted
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
            if (!string.IsNullOrWhiteSpace(model.AvartarUrl))
            {
                update.AvartarUrl = model.AvartarUrl;
            }
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

        public async Task<ServiceResult<UserEntity>> GetUserByEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return new ServiceResult<UserEntity>
                    {
                        IsSuccess = false,
                        Message = "Email không được để trống"
                    };
                }

                var users = await _userRepository.ListAllAsync(new UserQueryModel { Email = email });
                var user = users.FirstOrDefault();

                if (user == null)
                {
                    return new ServiceResult<UserEntity>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng với email này"
                    };
                }

                return new ServiceResult<UserEntity>
                {
                    IsSuccess = true,
                    Data = user,
                    Message = "Tìm thấy người dùng"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Lỗi khi tìm người dùng theo email {Email}", email);
                return new ServiceResult<UserEntity>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tìm người dùng: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult<UserEntity>> CreateUserFromCustomerInfo(CustomerInfoModel customerInfo)
        {
            try
            {
                if (customerInfo == null)
                {
                    return new ServiceResult<UserEntity>
                    {
                        IsSuccess = false,
                        Message = "Thông tin khách hàng không được để trống"
                    };
                }

                // Kiểm tra xem người dùng đã tồn tại chưa
                var existingUserResult = await GetUserByEmail(customerInfo.Email);
                if (existingUserResult.IsSuccess)
                {
                    return existingUserResult;
                }

                // Tạo người dùng mới
                var newUser = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Type = "Customer",
                    Username = customerInfo.Email,
                    Name = customerInfo.FullName,
                    PhoneNumber = customerInfo.PhoneNumber,
                    Email = customerInfo.Email,
                    Password = "", // Mật khẩu trống, có thể gửi email để thiết lập sau
                    CreatedOnDate = DateTime.Now,
                    Isdeleted = false
                };

                var savedUser = await SaveAsync(newUser);

                return new ServiceResult<UserEntity>
                {
                    IsSuccess = true,
                    Data = savedUser,
                    Message = "Tạo người dùng mới thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Lỗi khi tạo người dùng mới từ thông tin khách hàng");
                return new ServiceResult<UserEntity>
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi tạo người dùng: {ex.Message}"
                };
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
    }
}