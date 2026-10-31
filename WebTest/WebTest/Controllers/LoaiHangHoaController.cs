using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTest.Models;

namespace WebTest.Controllers
{
    public class LoaiHangHoaController : Controller
    {
        private readonly QLBHContext db;

        public LoaiHangHoaController(QLBHContext context)
        {
            db = context;
        }

        // HIỂN THỊ
        public IActionResult Index()
        {
            return View(db.Loaihanghoas.AsNoTracking().ToList());
        }

        // THÊM: mở form
        [HttpGet]
        public IActionResult them()
        {
            return View(new Loaihanghoa());
        }

        // THÊM: lưu dữ liệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult them(
            [Bind("Maloai,Tenloai")] Loaihanghoa n)
        {
            if (string.IsNullOrWhiteSpace(n.Maloai))
            {
                ModelState.AddModelError(
                    "Maloai", "Bạn chưa nhập mã loại.");
            }
            else if (db.Loaihanghoas.Any(x => x.Maloai == n.Maloai))
            {
                ModelState.AddModelError(
                    "Maloai", "Mã loại đã tồn tại.");
            }

            if (!ModelState.IsValid)
                return View(n);

            db.Loaihanghoas.Add(n);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // SỬA: mở form
        [HttpGet]
        public IActionResult sua(string id)
        {
            var entity = db.Loaihanghoas.Find(id);

            if (entity == null)
                return NotFound();

            return View(entity);
        }

        // SỬA: lưu dữ liệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult sua(
            [Bind("Maloai,Tenloai")] Loaihanghoa n)
        {
            if (!ModelState.IsValid)
                return View(n);

            var entity = db.Loaihanghoas.Find(n.Maloai);

            if (entity == null)
                return NotFound();

            entity.Tenloai = n.Tenloai;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // XÓA: mở trang xác nhận
        [HttpGet]
        public IActionResult xoa(string id)
        {
            var entity = db.Loaihanghoas.Find(id);

            if (entity == null)
                return NotFound();

            ViewBag.CoLienQuan = db.Hanghoas.Any(x => x.Maloai == id);
            return View(entity);
        }

        // XÓA: thực hiện xóa
        [HttpPost, ActionName("xoa")]
        [ValidateAntiForgeryToken]
        public IActionResult xoa_Post(string id)
        {
            var entity = db.Loaihanghoas.Find(id);

            if (entity == null)
                return NotFound();

            ViewBag.CoLienQuan = db.Hanghoas.Any(x => x.Maloai == id);
            if (ViewBag.CoLienQuan)
            {
                ModelState.AddModelError("", "Không thể xóa vì dữ liệu đang được sử dụng ở bảng liên quan.");
                return View("xoa", entity);
            }

            try
            {
                db.Loaihanghoas.Remove(entity);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    "",
                    "Không xóa được. Hãy kiểm tra hàng hóa liên quan "
                    + "và các ràng buộc trong database.");

                return View("xoa", entity);
            }
        }
    }
}