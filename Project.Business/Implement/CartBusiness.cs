using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Business.Implement
{
    public class CartBusiness : ICartBusiness
    {
        private readonly ICartRepository _cartRepository;

        public CartBusiness(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Cart> DeleteAsync(Guid cartId)
        {
            return await _cartRepository.DeleteAsync(cartId);
        }

        public async Task<IEnumerable<Cart>> DeleteAsync(Guid[] deleteIds)
        {
            return await _cartRepository.DeleteAsync(deleteIds);
        }

        public async Task<Cart> FindAsync(Guid cartId)
        {
            return await _cartRepository.FindAsync(cartId);
        }

        public async Task<Pagination<Cart>> GetAllAsync(CartQueryModel queryModel)
        {
            return await _cartRepository.GetAllAsync(queryModel);
        }

        public async Task<int> GetCountAsync(CartQueryModel queryModel)
        {
            return await _cartRepository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<Cart>> ListAllAsync(CartQueryModel queryModel)
        {
            return await _cartRepository.ListAllAsync(queryModel);
        }

        public async Task<IEnumerable<Cart>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _cartRepository.ListByIdsAsync(ids);
        }

        public async Task<Cart> PatchAsync(Cart model)
        {
            var exist = await _cartRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException("Cart not found");
            }

            var update = new Cart
            {
                Id = exist.Id,
                IdTaiKhoan = exist.IdTaiKhoan,
                IdThongTinLienHe = exist.IdThongTinLienHe,
                Status = exist.Status,
                Description = exist.Description,
                CreatedByUserId = exist.CreatedByUserId,
                CreatedOnDate = exist.CreatedOnDate,
                LastModifiedByUserId = exist.LastModifiedByUserId,
                LastModifiedOnDate = exist.LastModifiedOnDate,
                Isdeleted = exist.Isdeleted
            };

            if (model.IdTaiKhoan != Guid.Empty)
            {
                update.IdTaiKhoan = model.IdTaiKhoan;
            }
            if (model.IdThongTinLienHe != Guid.Empty)
            {
                update.IdThongTinLienHe = model.IdThongTinLienHe;
            }
            if (model.Status != 0)
            {
                update.Status = model.Status;
            }
            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                update.Description = model.Description;
            }

            return await SaveAsync(update);
        }

        public async Task<Cart> SaveAsync(Cart cart)
        {
            var res = await SaveAsync(new[] { cart });
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<Cart>> SaveAsync(IEnumerable<Cart> carts)
        {
            return await _cartRepository.SaveAsync(carts);
        }
    }
}
