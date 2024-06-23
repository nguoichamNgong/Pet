using Microsoft.AspNetCore.Authorization;
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
	public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Products.OrderByDescending(p => p.Id).Include(p => p.Category).Include(p => p.Brand).ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            if (ModelState.IsValid)
            {


                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã có trong database");
                    return View(product);
                }

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filepath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filepath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;
                }

                _dataContext.Add(product);
                await _dataContext.SaveChangesAsync();
                TempData["succes"] = "Thêm pet thành công";
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
        }
        public async Task<IActionResult> Edit(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            var exitted_product = _dataContext.Products.Find(product.Id);
            if (ModelState.IsValid)
            {


                product.Slug = product.Name.Replace(" ", "-");


                if (product.ImageUpload != null)
                {

                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filepath = Path.Combine(uploadsDir, imageName);

                    string oldfilePath = Path.Combine(uploadsDir, exitted_product.Image);
                    try
                    {
                        if (System.IO.File.Exists(oldfilePath))
                        {
                            System.IO.File.Delete(oldfilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Đã xảy ra lỗi khi xóa hình ảnh pet. ");
                    }

                    FileStream fs = new FileStream(filepath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    exitted_product.Image = imageName;

                }

                exitted_product.Name = product.Name;
                exitted_product.Description = product.Description;
                exitted_product.Price = product.Price;
                exitted_product.CategoryId = product.CategoryId;
                exitted_product.BrandId = product.BrandId;

                _dataContext.Update(exitted_product);
                await _dataContext.SaveChangesAsync();
                TempData["succes"] = "Cập nhật thú cưng thành công";
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
        }
        public async Task<IActionResult> Delete(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            if (!string.Equals(product.Image, "nonam.jpg"))
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                string oldfilePath = Path.Combine(uploadsDir, product.Image);

                if (System.IO.File.Exists(oldfilePath))
                {
                    System.IO.File.Delete(oldfilePath);
                }
            }

            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            TempData["Error"] = "Sản phẩm đã xóa";
            return RedirectToAction("Index");
        }
    }
}
