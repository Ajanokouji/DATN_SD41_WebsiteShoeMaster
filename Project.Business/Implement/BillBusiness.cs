using Microsoft.Extensions.Caching.Memory;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using SERP.Framework.Common;
using Serilog;
using Project.DbManagement;
using Project.Common.Constants;
using Project.Business.ModelFactory;
using Project.Business.Model.PatchModel;
using Project.DbManagement.ViewModels;
using Project.Business.Intercepter;

namespace Project.Business.Implement
{
    public class BillBusiness : IBillBusiness
    {
        private readonly IBillRepository _billRepository;
        private readonly IBillDetailsBusiness _billDetailsBusiness;
        private readonly IEnumerable<IBillIntercepterAfterSave> _billIntercepterAfterSaves;
        private readonly IBillModelFactory _billModelFactory;
        private readonly IBillDetailModelFactory _billDetailModelFactory;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private const string BillListCacheKey = "BillList";
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public BillBusiness(
            IBillDetailModelFactory billDetailModelFactory,
            IBillRepository billRepository,
             IEnumerable<IBillIntercepterAfterSave> billIntercepterAfterSaves,
            IBillModelFactory billModelFactory,
            IBillDetailsBusiness billDetailsBusiness, 
            IMemoryCache cache)
        {
            _billModelFactory = billModelFactory;
            _billDetailModelFactory = billDetailModelFactory;
            _billRepository = billRepository;
            _billDetailsBusiness = billDetailsBusiness;
            _billIntercepterAfterSaves = billIntercepterAfterSaves;
            _cache = cache;
            _logger = Log.ForContext<BillBusiness>();
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
        }

        public async Task<BillEntity> DeleteAsync(Guid contentId)
        {
            return await _billRepository.DeleteAsync(contentId);
        }

        public async Task<IEnumerable<BillEntity>> DeleteAsync(Guid[] deleteIds)
        {
            return await _billRepository.DeleteAsync(deleteIds);
        }

        public async Task<BillEntity> FindAsync(Guid contentId)
        {
            return await _billRepository.FindAsync(contentId);
        }

        public async Task<Pagination<BillModel>> GetAllAsync(BillQueryModel queryModel)
        {
            var res = new Pagination<BillModel>();
            var bill = await _billRepository.GetAllAsync(queryModel);
            if (bill != null)
            {
                res.Content = await _billModelFactory.CreateModels(bill.Content, true);
            }
            res.TotalPages= bill.TotalPages;
            res.CurrentPage=bill.CurrentPage;
            res.NumberOfRecords=bill.NumberOfRecords;
            res.PageSize=bill.PageSize;
            return res;
        }

        public async Task<int> GetCountAsync(BillQueryModel queryModel)
        {
            return await _billRepository.GetCountAsync(queryModel);
        }

        public async Task<IEnumerable<BillEntity>> ListAllAsync(BillQueryModel queryModel)
        {
            return await _billRepository.ListAllAsync(queryModel);
        }

        public async Task<IEnumerable<BillEntity>> ListByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _billRepository.ListByIdsAsync(ids);
        }

        public async Task<BillEntity> PatchAsync(BillPatchModel model)
        {
            var exist = await _billRepository.FindAsync(model.Id);

            if (exist == null)
            {
                throw new ArgumentException(BillConstants.BillNotFound);
            }

            var update = new BillEntity()
            {
                Id = exist.Id,
                EmployeeId = exist.EmployeeId,
                CustomerId = exist.CustomerId,
                OrderId = exist.OrderId,
                PaymentMethodId = exist.PaymentMethodId,
                BillCode = exist.BillCode,
                RecipientName = exist.RecipientName,
                RecipientPhone = exist.RecipientPhone,
                RecipientAddress = exist.RecipientAddress,
                TotalAmount = exist.TotalAmount,
                DiscountAmount = exist.DiscountAmount,
                AmountAfterDiscount = exist.AmountAfterDiscount,
                AmountToPay = exist.AmountToPay,
                Status = exist.Status,
                PaymentStatus = exist.PaymentStatus,
                CreatedOnDate = exist.CreatedOnDate,
                RecipientEmail = exist.RecipientEmail,
                LastModifiedOnDate = exist.LastModifiedOnDate,
                PaymentMethod = exist.PaymentMethod,
                FinalAmount= exist.FinalAmount,
                Note = exist.Note,
                Source = exist.Source,
                LastModifiedByUserId = exist.LastModifiedByUserId
            };
            if (!string.IsNullOrWhiteSpace(model.PaymentMethod))
            {
                update.PaymentMethod = model.PaymentMethod;
            }
            if (model.FinalAmount !=null)
            {
                update.FinalAmount = model.FinalAmount;
            }

            if (!string.IsNullOrWhiteSpace(model.BillCode))
            {
                update.BillCode = model.BillCode;
            }

            if (model.EmployeeId != null)
            {
                update.EmployeeId = model.EmployeeId;
            }

            if (model.CustomerId != null)
            {
                update.CustomerId = model.CustomerId;
            }

            if (model.OrderId != null)
            {
                update.OrderId = model.OrderId;
            }

            if (model.PaymentMethodId != null)
            {
                update.PaymentMethodId = model.PaymentMethodId;
            }

            if (!string.IsNullOrWhiteSpace(model.RecipientName))
            {
                update.RecipientName = model.RecipientName;
            }

            if (!string.IsNullOrWhiteSpace(model.RecipientPhone))
            {
                update.RecipientPhone = model.RecipientPhone;
            }

            if (!string.IsNullOrWhiteSpace(model.RecipientEmail))
            {
                update.RecipientEmail = model.RecipientEmail;
            }

            if (!string.IsNullOrWhiteSpace(model.RecipientAddress))
            {
                update.RecipientAddress = model.RecipientAddress;
            }

            if (model.TotalAmount > 0)
            {
                update.TotalAmount = model.TotalAmount;
            }

            if (model.DiscountAmount > 0)
            {
                update.DiscountAmount = model.DiscountAmount;
            }

            if (model.AmountAfterDiscount > 0)
            {
                update.AmountAfterDiscount = model.AmountAfterDiscount;
            }

            if (model.AmountAfterDiscount > 0)
            {
                update.AmountAfterDiscount = model.AmountAfterDiscount;
            }

            if (model.AmountToPay > 0)
            {
                update.AmountToPay = model.AmountToPay;
            }

            if (!string.IsNullOrEmpty(model.Status))
            {
                update.Status = model.Status;
            }

            if (!string.IsNullOrEmpty(model.PaymentStatus))
            {
                update.PaymentStatus = model.PaymentStatus;
            }

            if (model.CreatedOnDate != null)
            {
                update.CreatedOnDate = model.CreatedOnDate;
            }

            if (model.LastModifiedOnDate != null)
            {
                update.LastModifiedOnDate = model.LastModifiedOnDate;
            }


            return await SaveAsync(update);
        }

        public async Task<BillEntity> SaveAsync(BillEntity billEntity)
        {
            var res = await SaveAsync(new[] { billEntity });
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<BillEntity>> SaveAsync(IEnumerable<BillEntity> billEntities)
        {
            var oldEntities = new List<BillEntity>();

             foreach(var bill in billEntities)
            {
                if (bill.Id != Guid.Empty)
                {
                    var oldEntity = await FindAsync( bill.Id);
                    oldEntities.Add(oldEntity);
                }
            }
           var result = await _billRepository.SaveAsync(billEntities);
            foreach (var item in result)
            {
                foreach (var interceptors in _billIntercepterAfterSaves.OrderBy(x => x.Order))
                {
                    var old = oldEntities?.FirstOrDefault(x => x?.Id == item?.Id);
                    await interceptors.Intercept( old, item);
                }
            }
            return result;
        }
        public async Task<BillModel> CreateBill(BillModel model)
        {
            try
            {
                if (model == null)
                {
                    return new BillModel();

                }

                var billEntity = new BillEntity
                {
                    Id = model.Id,
                    BillCode = model.BillCode ?? await GenerateBillCode(),
                    CustomerId = model.CustomerId != null ? Guid.Parse(model.CustomerId.ToString()) : null,
                    RecipientName = model.CustomerName,
                    RecipientEmail = model.CustomerEmail,
                    RecipientPhone = model.CustomerPhone,
                    RecipientAddress = model.CustomerAddress,
                    TotalAmount = model.TotalAmount,
                    DiscountAmount = model.DiscountAmount,
                    FinalAmount = model.FinalAmount,
                    VoucherCode = model.VoucherCode,
                    Status = model.Status,
                    Source= model.Source,
                    AmountAfterDiscount = model.AmountAfterDiscount,
                    AmountToPay = model.AmountToPay,
                    CreatedByUserId = model.CreatedByUserId,
                    EmployeeId = model.EmployeeId,
                    OrderId = model.OrderId,
                    Note = model.Note,
                    VoucherId =  model.VoucherId,
                    PaymentMethodId =  model.PaymentMethodId,
                    PaymentMethod = model.PaymentMethod,
                    PaymentStatus = model.PaymentStatus,
                    CreatedOnDate = DateTime.Now,
                };

                var savedBill = await SaveAsync(billEntity);

                if (savedBill != null && model.BillDetails != null && model.BillDetails.Any())
                {
                    await _billDetailsBusiness.CreateBillDetails( await _billDetailModelFactory.ConvertEntities( model.BillDetails), savedBill.Id);
                }



               var result = await _billModelFactory.CreateModel(savedBill,true);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Lỗi khi tạo hóa đơn");
                return null;
            }
        }

        public async Task<BillModel> GetBillById(Guid billId)
        {
            try
            {

                var bill = await _billRepository.FindAsync(billId);

                if (bill == null)
                {
                    return null;
                }


                var result = await _billModelFactory.CreateModel(bill, true);


                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Lỗi khi lấy thông tin hóa đơn {BillId}", billId);
                return null;
            }
        }

        public async Task<BillModel> GetBillByCode(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return null;
                }

                var bills = await _billRepository.ListAllAsync(new BillQueryModel { BillCode = code });
                var bill = bills.FirstOrDefault();

                if (bill == null)
                {
                    return null;
                }
                var result = await _billModelFactory.CreateModel(bill, true);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Lỗi khi lấy thông tin hóa đơn theo mã {BillCode}", code);
                return null;
            }
        }

        public async Task<List<BillModel>> GetBillsByUserId(Guid userId)
        {
            try
            {
                var userGuid = Guid.Parse(userId.ToString());
                var bills = await _billRepository.ListAllAsync(new BillQueryModel { CustomerId= userGuid });

                if (bills == null || !bills.Any())
                {
                    return new List<BillModel>();

                }

                var result = await _billModelFactory.CreateModels(bills);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Lỗi khi lấy danh sách hóa đơn của người dùng {UserId}", userId);
                return null;
            }
        }

        public async Task<bool> UpdateBillStatus(Guid billId, string status)
        {
            try
            {
                var billGuid = Guid.Parse(billId.ToString());
                var bill = await _billRepository.FindAsync(billGuid);

                if (bill == null)
                {
                    return false;
                }

                bill.Status = status;
                bill.LastModifiedOnDate = DateTime.Now;
                await _billRepository.SaveAsync(bill);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Lỗi khi cập nhật trạng thái hóa đơn {BillId}", billId);
                return false;
            }
        }

        public async Task<bool> UpdatePaymentStatus(Guid billId, string paymentStatus)
        {
            try
            {
                var billGuid = Guid.Parse(billId.ToString());
                var bill = await _billRepository.FindAsync(billGuid);

                if (bill == null)
                {
                    return false;
                }

                bill.PaymentStatus = paymentStatus;
                bill.LastModifiedOnDate = DateTime.Now;
                await _billRepository.SaveAsync(bill);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Lỗi khi cập nhật trạng thái thanh toán hóa đơn {BillId}", billId);
                return false;
            }
        }

        public async Task<bool> UpdatePaymentMethod(Guid billId, string paymentMethod)
        {
            try
            {
                var billGuid = Guid.Parse(billId.ToString());
                var bill = await _billRepository.FindAsync(billGuid);

                if (bill == null)
                {
                    return false;
                }

                bill.PaymentMethod = paymentMethod;
                bill.LastModifiedOnDate = DateTime.Now;
                await _billRepository.SaveAsync(bill);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Lỗi khi cập nhật phương thức thanh toán hóa đơn {BillId}", billId);
                return false;
            }
        }

        public async Task<decimal> ApplyVoucher(string voucherCode, decimal totalAmount)
        {
            try
            {
                if (string.IsNullOrEmpty(voucherCode))
                {
                    return 0;
                }

                // Giả lập logic áp dụng voucher
                // Trong thực tế, bạn sẽ kiểm tra mã voucher trong cơ sở dữ liệu
                decimal discountAmount = 0;

                if (voucherCode.Equals("WELCOME10", StringComparison.OrdinalIgnoreCase))
                {
                    discountAmount = totalAmount * 0.1m; // Giảm 10%
                }
                else if (voucherCode.Equals("SUMMER20", StringComparison.OrdinalIgnoreCase))
                {
                    discountAmount = totalAmount * 0.2m; // Giảm 20%
                }
                else if (voucherCode.Equals("FIXED50K", StringComparison.OrdinalIgnoreCase))
                {
                    discountAmount = 50000; // Giảm 50,000 VND
                    if (discountAmount > totalAmount)
                    {
                        discountAmount = totalAmount;
                    }
                }
                else
                {
                    return 0;
                }

                return discountAmount;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Lỗi khi áp dụng mã giảm giá {VoucherCode}", voucherCode);
                return 0;
            }
        }

        public async Task<string> GenerateBillCode()
        {
            // Tạo mã hóa đơn theo định dạng: HD + năm + tháng + ngày + giờ + phút + giây
            string billCode = "HD" + DateTime.Now.ToString("yyyyMMddHHmmss");
            return billCode;
        }

        public async Task<BillModel> CheckoutFromCart(List<CartItemModel> cartItems, CustomerInfoModel customerInfo, string voucherCode)
        {
            try
            {
                if (cartItems == null || !cartItems.Any())
                {
                    return null;
                }

                if (customerInfo == null)
                {
                    return null;
                }

                // Tính tổng tiền
                decimal totalAmount = cartItems.Sum(item => item.Price * item.Quantity);

                // Áp dụng mã giảm giá nếu có
                decimal discountAmount = 0;
                if (!string.IsNullOrEmpty(voucherCode))
                {
                    var voucherResult = await ApplyVoucher(voucherCode, totalAmount);
                    if (voucherResult!=0)
                    {
                        discountAmount = voucherResult;
                    }
                }

                decimal finalAmount = totalAmount - discountAmount;

                // Tạo chi tiết hóa đơn
                var billDetails = cartItems.Select(item => new BillDetailModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductImage = item.ProductImage,
                    Size = item.Size,
                    Color = item.Color,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    TotalPrice = item.Price * item.Quantity
                }).ToList();

                // Tạo hóa đơn
                var billModel = new BillModel
                {
                    BillCode = await GenerateBillCode(),
                    CustomerName = customerInfo.FullName,
                    CustomerPhone = customerInfo.PhoneNumber,
                    CustomerEmail = customerInfo.Email,
                    CustomerAddress = customerInfo.Address,
                    TotalAmount = totalAmount,
                    DiscountAmount = discountAmount,
                    FinalAmount = finalAmount,
                    VoucherCode = voucherCode,
                    Note = customerInfo.Notes,
                    Status = BillConstants.PendingConfirmation,
                    PaymentMethod = "COD", // Mặc định là COD, có thể thay đổi sau
                    PaymentStatus = BillConstants.PaymentStatusUnpaid,
                    BillDetails = billDetails
                };

                var result = await CreateBill(billModel);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Lỗi khi thanh toán giỏ hàng");
                return null;
            }
        }
        
        public List<BillEntity> GetAllPendingBill()
        {
            return _billRepository.GetAllPendingBill();
        }
        public bool CreatePendingBill(Guid idEmployee)
        {
            try
            {
                _billRepository.CreatePendingBill(idEmployee);
                return true;
            }
            catch
            {
                return false;
            }   
        }
        public bool DeletePendingBill(Guid idBill)
        {
            try
            {
                _billRepository.DeletePendingBill(idBill);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public BillViewModel GetPDBillById(Guid idBill)
        {
            return _billRepository.GetPDBillById(idBill);
        }

        public bool PaymentBill(PaymentBillRequest bill)
        {
            try
            {
                _billRepository.PaymentBill(bill);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<BillEntity> FindByCodeAsync(string billCode)
        {
            var res = await _billRepository.FindByCodeAsync(billCode);
            return res;
        }
    }
}