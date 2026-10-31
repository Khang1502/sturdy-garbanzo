using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTest.Models;

namespace WebTest.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly QLBHContext db;

        public NhanVienController(QLBHContext context)
        {
            db = context;
        }

        // HIỂN THỊ
        public IActionResult Index()
        {
            return View(db.Nhanviens.AsNoTracking().ToList());
        }

        // THÊM: mở form
        [HttpGet]
        public IActionResult them()
        {
            return View(new Nhanvien());
        }

        // THÊM: lưu dữ liệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult them(
            [Bind("Manv,Tennv,Ngaysinh,Phai,Diachi,Password")]
            Nhanvien n)
        {
            if (string.IsNullOrWhiteSpace(n.Manv))
            {
                ModelState.AddModelError(
                    "Manv", "Bạn chưa nhập mã nhân viên.");
            }
            else if (db.Nhanviens.Any(x => x.Manv == n.Manv))
            {
                ModelState.AddModelError(
                    "Manv", "Mã nhân viên đã tồn tại.");
            }

            if (string.IsNullOrWhiteSpace(n.Password))
                ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu.");

            if (n.Ngaysinh < new DateTime(1753, 1, 1) || n.Ngaysinh > DateTime.Today)
                ModelState.AddModelError("Ngaysinh", "Ngày sinh phải từ 01/01/1753 đến hôm nay.");

            if (!ModelState.IsValid)
                return View(n);

            db.Nhanviens.Add(n);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // SỬA: mở form
        [HttpGet]
        public IActionResult sua(string id)
        {
            var entity = db.Nhanviens.Find(id);

            if (entity == null)
                return NotFound();

            return View(entity);
        }

        // SỬA: lưu dữ liệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult sua(
            [Bind("Manv,Tennv,Ngaysinh,Phai,Diachi,Password")]
            Nhanvien n)
        {
            if (n.Ngaysinh < new DateTime(1753, 1, 1) || n.Ngaysinh > DateTime.Today)
                ModelState.AddModelError("Ngaysinh", "Ngày sinh phải từ 01/01/1753 đến hôm nay.");

            if (!ModelState.IsValid)
                return View(n);

            var entity = db.Nhanviens.Find(n.Manv);

            if (entity == null)
                return NotFound();

            entity.Tennv = n.Tennv;
            entity.Ngaysinh = n.Ngaysinh;
            entity.Phai = n.Phai;
            entity.Diachi = n.Diachi;

            // Chỉ đổi mật khẩu khi có nhập mật khẩu mới
            if (!string.IsNullOrWhiteSpace(n.Password))
            {
                entity.Password = n.Password;
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // XÓA: mở trang xác nhận
        [HttpGet]
        public IActionResult xoa(string id)
        {
            var entity = db.Nhanviens.Find(id);

            if (entity == null)
                return NotFound();

            ViewBag.CoLienQuan = db.Phieugiaohangs.Any(x => x.Manv == id);
            return View(entity);
        }

        // XÓA: thực hiện xóa
        [HttpPost, ActionName("xoa")]
        [ValidateAntiForgeryToken]
        public IActionResult xoa_Post(string id)
        {
            var entity = db.Nhanviens.Find(id);

            if (entity == null)
                return NotFound();

            ViewBag.CoLienQuan = db.Phieugiaohangs.Any(x => x.Manv == id);
            if (ViewBag.CoLienQuan)
            {
                ModelState.AddModelError("", "Không thể xóa vì dữ liệu đang được sử dụng ở bảng liên quan.");
                return View("xoa", entity);
            }

            try
            {
                db.Nhanviens.Remove(entity);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    "",
                    "Không xóa được. Hãy kiểm tra phiếu giao hàng "
                    + "liên quan và các ràng buộc trong database.");

                return View("xoa", entity);
            }
        }
    }
}