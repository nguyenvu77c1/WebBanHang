using Lab03.Data;
using Lab03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Lab03.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ApplicationDbContext _db;

        public GioHangController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<GioHang> dsGioHang = _db.GioHang
                .Include("Product")
                .Where(gh => gh.ApplicationUserId == claim.Value)
                .ToList();


            GioHangViewModel giohang = new GioHangViewModel()
            {
                DsGioHang = _db.GioHang
                .Include("Product")
                .Where(gh => gh.ApplicationUserId == claim.Value)
                .ToList(),
                HoaDon = new HoaDon()
            };
            foreach (var item in giohang.DsGioHang)
            {
                item.ProductPrice = item.Quantity * item.Product.Price;

                giohang.HoaDon.Total += item.ProductPrice;
            }

            return View(giohang);
        }

        public IActionResult Giam(int giohangId)
        {
            //Lấy thông tin giỏ hàng tương ứng với giohangId
            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);
            //Giảm số lượng sản phẩm đi 1
            giohang.Quantity -= 1;
            //Nếu số lượng = 0 thì xóa giỏ hàng
            if (giohang.Quantity == 0)
            {
                _db.GioHang.Remove(giohang);
            }    
            // Lưu lại CSDL
            _db.SaveChanges();
            // Quay về trang giỏ hàng
            return RedirectToAction("Index");
        }

        public IActionResult Tang(int giohangId)
        {
            //Lấy thông tin giỏ hàng tương ứng với giohangId
            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);
            //Tăng số lượng sản phẩm đi 1
            giohang.Quantity += 1;
            // Lưu lại CSDL
            _db.SaveChanges();
            // Quay về trang giỏ hàng
            return RedirectToAction("Index");
        }
        public IActionResult Xoa(int giohangId)
        {
            //Lấy thông tin giỏ hàng tương ứng với giohangId
            var giohang = _db.GioHang.FirstOrDefault(gh => gh.Id == giohangId);
            // Xóa giỏ hàng
            _db.GioHang.Remove(giohang);
            // Lưu lại CSDL
            _db.SaveChanges();
            // Quay về trang giỏ hàng
            return RedirectToAction("Index");
        }

        public IActionResult ThanhToan()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            GioHangViewModel giohang = new GioHangViewModel()
            {
                DsGioHang = _db.GioHang
                .Include("Product")
                .Where(gh => gh.ApplicationUserId == claim.Value)
                .ToList(),
                HoaDon = new HoaDon()
            };

            giohang.HoaDon.ApplicationUser = _db.ApplicationUser.FirstOrDefault(user => user.Id == claim.Value);
            giohang.HoaDon.Name = giohang.HoaDon.ApplicationUser.Name;
            giohang.HoaDon.Address = giohang.HoaDon.ApplicationUser.Address;
            giohang.HoaDon.PhoneNumber = giohang.HoaDon.ApplicationUser.PhoneNumber;


            foreach (var item in giohang.DsGioHang)
            {
                item.ProductPrice = item.Quantity * item.Product.Price;

                giohang.HoaDon.Total += item.ProductPrice;
            }
            return View(giohang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThanhToan(GioHangViewModel giohang)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);

            giohang.DsGioHang = _db.GioHang.Include("Product")
             .Where(gh => gh.ApplicationUserId == claim.Value).ToList();

            giohang.HoaDon.ApplicationUserId = claim.Value;
            giohang.HoaDon.OrderDate = DateTime.Now;
            giohang.HoaDon.OrderStatus = "Đang xác nhận";
            giohang.HoaDon.ShippingAddress = giohang.HoaDon.ShippingAddress; // Gán dữ liệu từ form vào đối tượng HoaDon
            giohang.HoaDon.Note = giohang.HoaDon.Note; // Gán dữ liệu từ form vào đối tượng HoaDon

            foreach (var item in giohang.DsGioHang)
            {
                item.ProductPrice = item.Quantity * item.Product.Price;

                giohang.HoaDon.Total += item.ProductPrice;
            }
            _db.HoaDon.Add(giohang.HoaDon);
            _db.SaveChanges();

            foreach (var item in giohang.DsGioHang)
            {
                ChiTietHoaDon chitiethoadon = new ChiTietHoaDon()
                {
                    SanPhamId = item.ProductId,
                    HoaDonId = giohang.HoaDon.Id,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity
                };
                _db.ChiTietHoaDon.Add(chitiethoadon);
                _db.SaveChanges();
            }


            _db.GioHang.RemoveRange(giohang.DsGioHang);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Bạn đã đặt hàng thành công";

            return RedirectToAction("Index", "Home");
        }
    }
}
