using Lab03.Data;
using Lab03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab03.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangController : Controller
    {
        private readonly ApplicationDbContext _db;

        public KhachHangController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<ApplicationUser> applications = _db.ApplicationUser.ToList();
            return View(applications);
        }
    }
}
