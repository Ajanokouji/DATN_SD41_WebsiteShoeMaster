using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using SERP.Framework.Common;

namespace Project.Business.Implement
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUserRepository _userRepository;

        public UserBusiness(IUserRepository userRepository)
        {
            _userRepository=userRepository;
        }

        public async Task<User> DeleteAsync(Guid id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<User>> DeleteAsync(Guid[] deleteIds)
        {
            return await _userRepository.DeleteAsync(deleteIds);
        }

        public async Task<User> FindAsync(Guid id)
        {
            return await _userRepository.FindAsync(id);
        }

        public async Task<Pagination<User>> GetAllAsync(UserQueryModel queryModel)
        {
            return await _userRepository.GetAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(UserQueryModel queryModel)
        {
            return await _userRepository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<User>> ListAllAsync(UserQueryModel queryModel)
        {
            return await _userRepository.ListAllAsync(queryModel);
        }

        public async Task<IEnumerable<User>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _userRepository.ListByIdsAsync(ids);
        }

        public async Task<User> PatchAsync(User model)
        {
            var exist = await _userRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException("User not found");
            }
            var update = new User
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

        public async Task<User> SaveAsync(User user)
        {
            var res = await SaveAsync(new[] { user });
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<User>> SaveAsync(IEnumerable<User> users)
        {
            return await _userRepository.SaveAsync(users);
        }
    }
}