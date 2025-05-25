using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface;
using Project.Business.Interface.Services;
using Project.DbManagement.Entity;
using Project.MVC.ImageExtractor;
using SERP.NewsMng.Business.Models;
using SERP.NewsMng.Business.Models.QueryModels;
using System;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class NewsController : Controller
    {
        private readonly IContentBaseBusiness _contentBaseService;

        public NewsController(IContentBaseBusiness contentBaseService)
        {
            _contentBaseService = contentBaseService;
        }

        // [GET] Danh sách bài viết
        public async Task<IActionResult> News(int pageSize = 6)
        {
            var now = DateTime.Now;
            var queryModel = new ContentBaseQueryModel
            {
                CurrentPage = 1,
                PageSize = pageSize
            };

            var result = await _contentBaseService.GetAllAsync(queryModel);

            var contentBaseModels = result.Content
                .Where(x => x.IsPublish == true && x.PublishStartDate != null && x.PublishStartDate < now)
                .Select(x => new ContentBase
                {
                    Id = x.Id,
                    Title = x.Title,
                    PublishStartDate = x.PublishStartDate,
                    SeoUri = x.SeoUri,
                    SeoTitle = x.SeoTitle,
                    SeoDescription = x.SeoDescription,
                    SeoKeywords = x.SeoKeywords,
                    Content = x.Content,
                    PublishEndDate = x.PublishEndDate,
                    IsPublish = x.IsPublish
                }).ToList();

            ViewBag.TotalPages = result.TotalPages;
            ViewBag.PageSize = pageSize;

            return View(contentBaseModels);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMoreNews(int page = 1, int pageSize = 6)
        {
            var now = DateTime.Now;
            var queryModel = new ContentBaseQueryModel
            {
                CurrentPage = page,
                PageSize = pageSize
            };

            var result = await _contentBaseService.GetAllAsync(queryModel);

            var contentBaseModels = result.Content
                .Where(x => x.IsPublish == true && x.PublishStartDate != null && x.PublishStartDate < now)
                .Select(x => new ContentBase
                {
                    Id = x.Id,
                    Title = x.Title,
                    PublishStartDate = x.PublishStartDate,
                    SeoUri = x.SeoUri,
                    SeoTitle = x.SeoTitle,
                    SeoDescription = x.SeoDescription,
                    SeoKeywords = x.SeoKeywords,
                    Content = x.Content,
                    PublishEndDate = x.PublishEndDate,
                    IsPublish = x.IsPublish
                }).ToList();

            return PartialView("_NewsItemsPartial", contentBaseModels);
        }



        public async Task<IActionResult> Detail(Guid id)
        {
            var defaultImageUrl = Url.Content("~/assets/images/Defaultimage.jpg");
            var article = await _contentBaseService.FindAsync(id);
            if (article == null)
                return NotFound();

            var relatedArticles = await _contentBaseService.SearchContentBaseByKeywordsAsync(article.Title ?? "");
            relatedArticles = relatedArticles.Where(a => a.Id != id).ToList();

            // Thêm ảnh cho bài viết liên quan dựa vào Content
            foreach (var post in relatedArticles)
            {
                var images = HtmlImageExtractor.ExtractImageSources(post.Content ?? "", 1);
                if (images.Any())
                {
                    if (ViewBag.RelatedImages == null) ViewBag.RelatedImages = new Dictionary<Guid, string>();
                    ((Dictionary<Guid, string>)ViewBag.RelatedImages)[post.Id] = images.First();
                }
                else
                {
                    if (ViewBag.RelatedImages == null) ViewBag.RelatedImages = new Dictionary<Guid, string>();
                    ((Dictionary<Guid, string>)ViewBag.RelatedImages)[post.Id] = defaultImageUrl;
                }
            }

            ViewBag.RelatedPosts = relatedArticles;

            return View(article);
        }


        /*
        // [GET] Tạo bài viết mới
        public IActionResult Create()
        {
            _contentBaseService = contentBaseService;
        }

        // [GET] Danh sách bài viết
        public async Task<IActionResult> Index()
        {
            var articles = await _contentBaseService.GetAllAsync();
            return View(articles);
        }

        // [GET] Chi tiết bài viết
        public async Task<IActionResult> Details(Guid id)
        {
            var article = await _contentBaseService.GetByIdAsync(id);
            if (article == null)
                return NotFound();

            return View(article);
        }

        // [GET] Tạo bài viết mới
        public IActionResult Create()
        {
            return View();
        }

        // [POST] Tạo bài viết mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContentBaseModel model)
        {
            if (ModelState.IsValid)
            {
                await _contentBaseService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // [GET] Sửa bài viết
        public async Task<IActionResult> Edit(Guid id)
        {
            var article = await _contentBaseService.GetByIdAsync(id);
            if (article == null)
                return NotFound();

            return View(article);
        }

        // [POST] Lưu bài viết sau sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContentBaseModel model)
        {
            if (ModelState.IsValid)
            {
                await _contentBaseService.UpdateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // [POST] Xóa bài viết
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _contentBaseService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }*/
    }
}
