using Lab03.Data;
using Lab03.Models;
using Lab03.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lab03.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IConfigRepository _configRepository;
        private readonly ApplicationDbContext _db; // Đảm bảo rằng bạn đã inject ApplicationDbContext vào contructor
        
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IConfigRepository configRepository, ApplicationDbContext db)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _configRepository = configRepository; 
            _db = db;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index(string SearchString = "", int? Category = null)
        {
            var products = _db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                products = products.Where(x => x.Name.ToUpper().Contains(SearchString.ToUpper()));
            }

            if (Category.HasValue)
            {
                products = products.Where(p => p.CategoryId == Category.Value);
            }

            // Lấy toàn bộ danh sách danh mục để hiển thị trong dropdown
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View(await products.ToListAsync());
        }


        // Xử lý thêm sản phẩm mới
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Viết thêm hàm SaveImage (tham khảo bào 02)
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return Path.Combine("/images", image.FileName);
        }


        // Hiển thị thông tin chi tiết sản phẩm
        [HttpGet]
        public async Task<IActionResult> Display(int productId)
        {
            // Lấy product dựa trên productId
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Tạo đối tượng giohang
            GioHang giohang = new GioHang()
            {
                ProductId = productId,
                Product = product,
                Quantity = 1
            };

            // Kiểm tra ConfigId của product
            if (product.ConfigId != null)
            {
                // Lấy config liên quan
                var config = await _configRepository.GetByIdAsync(product.ConfigId);

                if (config != null)
                {
                    // Truy cập vào thuộc tính ManHinh của Config
                    string manHinh = config.ManHinh;

                   
                }
            }

            // Trả về view Display với giohang
            return View(giohang);
        }


        [HttpPost]
        [Authorize]
        public IActionResult Display(GioHang giohang, int productQuantity)
        {
            // Lấy userId của người dùng hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            giohang.ApplicationUserId = userId;

            // Lưu số lượng vào đối tượng giohang từ form
            giohang.Quantity = productQuantity;

            try
            {
                // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng của người dùng hay chưa
                var existingCartItem = _db.GioHang.FirstOrDefault(item => item.ProductId == giohang.ProductId && item.ApplicationUserId == userId);

                if (existingCartItem == null)
                {
                    // Nếu sản phẩm chưa tồn tại trong giỏ hàng, kiểm tra `ProductId` có tồn tại trong `Products` không
                    var productExists = _db.Products.Any(p => p.Id == giohang.ProductId);
                    if (!productExists)
                    {
                        // Sản phẩm không tồn tại trong `Products`, thông báo lỗi cho người dùng
                        ModelState.AddModelError("", "Sản phẩm không tồn tại.");
                        return View(giohang);
                    }

                    // Thêm mới sản phẩm vào giỏ hàng
                    _db.GioHang.Add(giohang);
                }
                else
                {
                    // Nếu sản phẩm đã tồn tại trong giỏ hàng, cập nhật số lượng
                    existingCartItem.Quantity += giohang.Quantity;
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                _db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Xử lý ngoại lệ DbUpdateException
                // Log lỗi (nếu cần) và thông báo lỗi cho người dùng
                ModelState.AddModelError("", "Có lỗi xảy ra trong quá trình cập nhật giỏ hàng. Vui lòng thử lại.");
                return View(giohang);
            }


            TempData["SuccessMessage"] = "Đã thêm vào giỏ hàng";

            // Chuyển hướng đến hành động Index sau khi hoàn tất
            return RedirectToAction("Index");
        }



        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);

                // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                if (imageUrl == null)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                else
                {
                    // Lưu hình ảnh mới
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                // Cập nhật các thông tin khác của sản phẩm
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;

                await _productRepository.UpdateAsync(existingProduct);

                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
