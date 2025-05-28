using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.Business.Model;
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

        public async Task<IViewComponentResult> InvokeAsync(Guid productId)
        {


            var data = await _commentsBusiness.GetAllAsync(new CommentsModel()
            {
                PageSize = 4,
                ObjectId = productId
            });


            var res = AutoMapperUtils.AutoMap<CommentsEntity, CommentsViewModel>(data.Content.ToList());

            return View(res); // Truyền vào View danh sách comment
        }
    }
}
