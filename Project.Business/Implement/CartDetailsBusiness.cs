using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using SERP.Framework.Common;

namespace Project.Business.Implement
{
    public class CartDetailsBusiness : ICartDetailsBusiness
    {
        private readonly ICartDetailsRepository _cartDetailsRepository;

        public CartDetailsBusiness(ICartDetailsRepository cartDetailsRepository)
        {
            _cartDetailsRepository = cartDetailsRepository;
        }

        public async Task<CartDetails> DeleteAsync(Guid contentId)
        {
            return await _cartDetailsRepository.DeleteAsync(contentId);
        }

        public async Task<IEnumerable<CartDetails>> DeleteAsync(Guid[] deleteIds)
        {
            return await _cartDetailsRepository.DeleteAsync(deleteIds);
        }

        public async Task<CartDetails> FindAsync(Guid contentId)
        {
            return await _cartDetailsRepository.FindAsync(contentId);
        }

        public async Task<Pagination<CartDetails>> GetAllAsync(CartDetailsQueryModel queryModel)
        {
            return await _cartDetailsRepository.GetAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(CartDetailsQueryModel queryModel)
        {
            return await _cartDetailsRepository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<CartDetails>> ListAllAsync(CartDetailsQueryModel queryModel)
        {
            return await _cartDetailsRepository.ListAllAsync(queryModel);
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
    }
}
