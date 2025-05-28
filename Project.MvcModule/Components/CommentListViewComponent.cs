using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.Business.Model;
using Project.DbManagement;
using Project.DbManagement.Entity;
using Project.MvcModule.Models;
using SERP.Framework.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.MvcModule
{


    public class CommentListViewComponent : BaseViewComponent
    {
        private readonly ICommentsBusiness _commentsBusiness;

        public CommentListViewComponent(ICommentsBusiness commentsBusiness)
        {
            _commentsBusiness=commentsBusiness;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid productId, int currentPage = 1)
        {
            var data = await _commentsBusiness.GetAllAsync(new CommentsModel()
            {
                PageSize = 4,
                ObjectId = productId,
                Status = 1,
                CurrentPage = currentPage,
            });

            var res = AutoMapperUtils.AutoMap<CommentsEntity, CommentsViewModel>(data.Content.ToList());

            ViewBag.ProductId = productId;
            ViewBag.CurrentPage = currentPage;
            ViewBag.HasMore = data.TotalRecords > currentPage * 4;

            var viewPath = GetViewPath("CommentList", "CommentList.cshtml");
            return View(viewPath, res);
        }
    }
}
