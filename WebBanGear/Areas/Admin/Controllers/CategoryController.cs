using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pet.Constants;
using Pet.Models;
using Pet.Repository;

namespace Pet.Areas.Admin.Controllers
{

    [Area("Admin")]
	[Authorize]
	public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Categories.OrderByDescending(p => p.Id).ToListAsync());
        }
        public async Task<IActionResult> Edit(int Id)
        {
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);
            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {

            if (ModelState.IsValid)
            {


                category.Slug = category.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Danh mục đã có trong database");
                    return View(category);
                }

                _dataContext.Add(category);
                await _dataContext.SaveChangesAsync();
                TempData["succes"] = "Thêm danh mục thành công";
                return RedirectToAction("Index");
            }
            else
            {
                // Lưu thông báo lỗi vào TempData để hiển thị trên giao diện người dùng
                TempData["Error"] = "Model có một số chỗ đang bị lỗi";

                // Tạo danh sách lỗi để hiển thị
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);

                // Trả về view để hiển thị lại form với các thông báo lỗi
                return BadRequest(errorMessage);
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryModel category)
        {

            if (ModelState.IsValid)
            {


                category.Slug = category.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Danh mục đã có trong database");
                    return View(category);
                }

                _dataContext.Update(category);
                await _dataContext.SaveChangesAsync();
                TempData["succes"] = "cập nhật danh mục thành công";
                return RedirectToAction("Index");
            }
            else
            {
                // Lưu thông báo lỗi vào TempData để hiển thị trên giao diện người dùng
                TempData["Error"] = "Model có một số chỗ đang bị lỗi";

                // Tạo danh sách lỗi để hiển thị
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);

                // Trả về view để hiển thị lại form với các thông báo lỗi
                return BadRequest(errorMessage);
            }

            return View(category);
        }


        public async Task<IActionResult> Delete(int Id)
        {
            var category = await _dataContext.Categories.FindAsync(Id);
            if (category == null)
            {
                return NotFound();
            }

            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Danh mục đã xóa";
            return RedirectToAction("Index");
        }
    }

}


