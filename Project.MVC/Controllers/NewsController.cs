using Microsoft.AspNetCore.Mvc;
using Project.Business.Interface.Services;
using SERP.NewsMng.Business.Models;
using System;
using System.Threading.Tasks;

namespace Project.MVC.Controllers
{
    public class NewsController : Controller
    {
        private readonly IContentBaseService _contentBaseService;

        public NewsController(IContentBaseService contentBaseService)
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
        }
    }
}
