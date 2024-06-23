using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pet.Constants;
using Pet.Models;
using Pet.Repository;


namespace Pet.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Brands.OrderByDescending(p => p.Id).ToListAsync());
        }

        public async Task<IActionResult> Edit(int Id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);
            return View(brand);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {

            if (ModelState.IsValid)
            {


                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "loài này đã có trong database");
                    return View(brand);
                }

                _dataContext.Add(brand);
                await _dataContext.SaveChangesAsync();
                TempData["succes"] = "Thêm loài thành công";
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

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandModel brand)
        {

            if (ModelState.IsValid)
            {


                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", " loài này đã có trong database");
                    return View(brand);
                }

                _dataContext.Update(brand);
                await _dataContext.SaveChangesAsync();
                TempData["succes"] = "cập nhật loài thành công";
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

            return View(brand);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var brand = await _dataContext.Brands.FindAsync(Id);
            if (brand == null)
            {
                return NotFound();
            }

            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Thương hiệu đã được xóa";
            return RedirectToAction("Index");
        }


    }
}
