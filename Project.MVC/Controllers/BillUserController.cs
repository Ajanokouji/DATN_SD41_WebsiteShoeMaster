using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Project.Business.Interface;
using Project.Business.Interface.Repositories;
using Project.Business.Model;
using Project.Common;
using Project.DbManagement;
using Project.DbManagement.Entity;
using SERP.Framework.Common;
using System;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class BillUserController : Controller
    {
        private readonly IUserBusiness _userBusiness;
        private readonly IBillRepository _billBusiness;
        private readonly IBillDetailsBusiness _billDetailsBusiness;

        public BillUserController(IUserBusiness userBusiness,
                                  IBillRepository billBusiness,
                                  IBillDetailsBusiness billDetailsBusiness)
        {
            _userBusiness = userBusiness;
            _billBusiness = billBusiness;
            _billDetailsBusiness = billDetailsBusiness;
        }

        public async Task<IActionResult> BillUser(int page = 1, int pageSize = 5)
        {
            var errAndUserEntity = await CheckUserIsLoginning();
            if (!String.IsNullOrWhiteSpace(errAndUserEntity.Err))
            {
                TempData["ErrBillUserMs"] = errAndUserEntity.Err;
                return View(new Pagination<BillEntity>() { Content = new System.Collections.Generic.List<BillEntity>(), CurrentPage = 1, TotalPages = 1, PageSize = pageSize });
            }

            var user = errAndUserEntity.User;

            var queryModel = new BillQueryModel
            {
                CustomerId = user.Id,
                PageSize = pageSize,
                CurrentPage = page
            };

            var billsPaged = await _billBusiness.GetAllAsync(queryModel);

            return View(billsPaged);
        }

        // Ajax: Lấy danh sách bill phân trang
        [HttpGet]
        public async Task<IActionResult> GetBillsByUserPaged(int page = 1, int pageSize = 5, string? status = null)
        {
            var errAndUserEntity = await CheckUserIsLoginning();
            if (!String.IsNullOrWhiteSpace(errAndUserEntity.Err))
            {
                return Json(new { success = false, message = errAndUserEntity.Err });
            }

            var user = errAndUserEntity.User;

            var queryModel = new BillQueryModel
            {
                CustomerId = user.Id,
                CurrentPage = page,
                PageSize = pageSize,
                Status = string.IsNullOrWhiteSpace(status) ? null : status
            };

            var billsPaged = await _billBusiness.GetAllAsync(queryModel);
            var partialHtml = await RenderPartialViewToString("_BillListPartial", billsPaged);

            return Json(new
            {
                success = true,
                html = partialHtml,
                currentPage = billsPaged.CurrentPage,
                totalPages = billsPaged.TotalPages
            });
        }

        // Ajax: Lấy chi tiết các BillDetails theo BillId
        [HttpGet]
        public async Task<IActionResult> GetBillDetailsByBillId(Guid billId)
        {
            var errAndUserEntity = await CheckUserIsLoginning();
            if (!string.IsNullOrWhiteSpace(errAndUserEntity.Err))
            {
                TempData["ErrBillUserMs"] = errAndUserEntity.Err;
                return View("BillUser",new Pagination<BillEntity>() { Content = new System.Collections.Generic.List<BillEntity>(), CurrentPage = 1, TotalPages = 1, PageSize = 5 });
            }

            try
            {
                var bill = await _billBusiness.FindAsync(billId);
                var billDetails = await _billDetailsBusiness.ListAllByIdBill(billId);
                return Json(new
                {
                    billInfo = bill,
                    details = billDetails
                }); // trả về JSON cho ajax
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi lấy chi tiết hóa đơn."});
            }
        }

        protected async Task<string> RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewEngine = HttpContext.RequestServices.GetService(typeof(Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine))
                                 as Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine;

                var viewResult = viewEngine.FindView(ControllerContext, viewName, false);

                if (!viewResult.Success)
                {
                    throw new InvalidOperationException($"Không tìm thấy view '{viewName}'");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions()
                );

                var renderTask = viewResult.View.RenderAsync(viewContext);
                await renderTask;

                return sw.GetStringBuilder().ToString();
            }
        }


        // Check user trong session và hợp lệ
        private class ErrAndUserEntity
        {
            public string? Err { get; set; }
            public UserEntity? User { get; set; }
        }

        private async Task<ErrAndUserEntity> CheckUserIsLoginning()
        {
            ErrAndUserEntity errAndUserEntity = new ErrAndUserEntity()
            {
                Err = "",
                User = null
            };

            string userJson = HttpContext.Session.GetString(UserConstants.UserSessionKey);

            if (!string.IsNullOrEmpty(userJson))
            {
                var user = JsonConvert.DeserializeObject<UserEntity>(userJson);
                if (user != null)
                {
                    var userFound = await _userBusiness.FindAsync(user.Id);
                    if (userFound != null)
                    {
                        if (userFound.IsDeleted == false && userFound.Type == UserTypeEnum.Customer)
                        {
                            if (userFound.IsActive == false)
                            {
                                errAndUserEntity.Err = "Tài khoản người dùng hiện đã bị tạm dừng hoạt động! Vui lòng kiểm tra lại!";
                            }
                            else
                            {
                                errAndUserEntity.Err = "";
                                errAndUserEntity.User = userFound;
                            }
                        }
                        else
                        {
                            errAndUserEntity.Err = "Không tìm thấy người dùng này! Vui lòng kiểm tra lại!";
                        }
                    }
                    else
                    {
                        errAndUserEntity.Err = "Không tìm thấy người dùng này! Vui lòng kiểm tra lại!";
                    }
                }
                else
                {
                    errAndUserEntity.Err = "Phiên đăng nhập đã hết hạn hoặc người dùng chưa đăng nhập.";
                }
            }
            else
            {
                errAndUserEntity.Err = "Phiên đăng nhập đã hết hạn hoặc người dùng chưa đăng nhập.";
            }

            return errAndUserEntity;
        }
    }
}