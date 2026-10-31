using Microsoft.AspNetCore.Mvc;
using WebTest.Models;
using Microsoft.EntityFrameworkCore;

namespace WebTest.Controllers
{
    public class NhaSanXuatController : Controller
    {
        private readonly QLBHContext db;

        public NhaSanXuatController(QLBHContext context)
        {
            db = context;
        }

        // Hiển thị danh sách
        public IActionResult Index()
        {
            return View(db.Nhasanxuats.AsNoTracking().ToList());
        }

        // Mở trang thêm
        [HttpGet]
        public IActionResult them()
        {
            return View(new Nhasanxuat());
        }

        // Mở trang sửa
        [HttpGet]
        public IActionResult sua(string id)
        {
            var entity = db.Nhasanxuats.Find(id);

            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        // Lưu thông tin đã sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult sua(
            [Bind("Mansx,Tennsx,Diachi")] Nhasanxuat n)
        {
            if (!ModelState.IsValid)
            {
                return View(n);
            }

            var entity = db.Nhasanxuats.Find(n.Mansx);

            if (entity == null)
            {
                return NotFound();
            }

            entity.Tennsx = n.Tennsx;
            entity.Diachi = n.Diachi;

            db.SaveChanges();

            return RedirectToAction("Index");
        }
        // Mở trang xác nhận xóa
        [HttpGet]
        public IActionResult xoa(string id)
        {
            var entity = db.Nhasanxuats.Find(id);

            if (entity == null)
            {
                return NotFound();
            }

            ViewBag.CoLienQuan = db.Hanghoas.Any(x => x.Mansx == id);
            return View(entity);
        }

        // Xóa khi người dùng xác nhận
        [HttpPost, ActionName("xoa")]
        [ValidateAntiForgeryToken]
        public IActionResult xoa_Post(string id)
        {
            var entity = db.Nhasanxuats.Find(id);

            if (entity == null)
            {
                return NotFound();
            }

            ViewBag.CoLienQuan = db.Hanghoas.Any(x => x.Mansx == id);
            if (ViewBag.CoLienQuan)
            {
                ModelState.AddModelError("", "Không thể xóa vì dữ liệu đang được sử dụng ở bảng liên quan.");
                return View("xoa", entity);
            }

            try
            {
                db.Nhasanxuats.Remove(entity);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    "",
                    "Không xóa được. Hãy kiểm tra dữ liệu liên quan "
                    + "và các ràng buộc trong database.");

                return View("xoa", entity);
            }
        }

        // Nhận dữ liệu khi bấm Lưu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult them(
            [Bind("Mansx,Tennsx,Diachi")] Nhasanxuat n)
        {
            if (string.IsNullOrWhiteSpace(n.Mansx))
            {
                ModelState.AddModelError(
                    "Mansx", "Bạn chưa nhập mã nhà sản xuất.");
            }
            else if (db.Nhasanxuats.Any(x => x.Mansx == n.Mansx))
            {
                ModelState.AddModelError(
                    "Mansx", "Mã nhà sản xuất đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                return View(n);
            }

            db.Nhasanxuats.Add(n);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}