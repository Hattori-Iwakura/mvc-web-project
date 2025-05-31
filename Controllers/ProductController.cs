using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc_web_project.Data;
using mvc_web_project.Models;

namespace mvc_web_project.Controllers
{
    public class ProductController : Controller
    {
        private readonly FitnessDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(FitnessDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Hiển thị danh sách sản phẩm với bộ lọc
        public async Task<IActionResult> Index(int? categoryId, decimal? minPrice, decimal? maxPrice, bool? inStock)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            // Lọc theo khoảng giá
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Lọc theo tình trạng còn hàng
            if (inStock.HasValue && inStock.Value)
            {
                query = query.Where(p => p.QuantityInStock > 0);
            }

            // Lấy danh sách categories cho filter
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.CategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.InStock = inStock;

            // Lấy tên category nếu đang lọc theo category
            if (categoryId.HasValue)
            {
                var category = await _context.Categories.FindAsync(categoryId);
                ViewBag.CategoryName = category?.Name;
            }

            var products = await query.ToListAsync();
            return View(products);
        }

        // GET: Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Hiển thị form tạo sản phẩm mới
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        // POST: Xử lý tạo sản phẩm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,CategoryId,QuantityInStock")] Product product, IFormFile? imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "products");
                        Directory.CreateDirectory(uploadsFolder); // Đảm bảo thư mục tồn tại

                        // Tạo tên file duy nhất
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Lưu file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        product.ImageUrl = "/img/products/" + uniqueFileName;
                    }
                    else
                    {
                        // Gán ảnh mặc định nếu không có ảnh được tải lên
                        product.ImageUrl = "/img/products/default-product.jpg";
                    }

                    product.CreatedAt = DateTime.UtcNow;
                    product.UpdatedAt = DateTime.UtcNow;

                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi tạo sản phẩm: " + ex.Message);
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        // GET: Hiển thị form chỉnh sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        // POST: Xử lý chỉnh sửa sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,Price,CategoryId,QuantityInStock,ImageUrl")] Product product, IFormFile? imageFile)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Xóa ảnh cũ nếu có và không phải ảnh mặc định
                        if (!string.IsNullOrEmpty(product.ImageUrl) && 
                            !product.ImageUrl.EndsWith("default-product.jpg"))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Lưu ảnh mới
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "products");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        product.ImageUrl = "/img/products/" + uniqueFileName;
                    }

                    product.UpdatedAt = DateTime.UtcNow;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật sản phẩm: " + ex.Message);
                }
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        // GET: Hiển thị form xóa
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
                
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Xóa file ảnh nếu tồn tại và không phải ảnh mặc định
                if (!string.IsNullOrEmpty(product.ImageUrl) && 
                    !product.ImageUrl.EndsWith("default-product.jpg"))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}