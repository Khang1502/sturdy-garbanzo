using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTest.Models;

namespace WebTest.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly QLBHContext db;

        public KhachHangController(QLBHContext context)
        {
            db = context;
        }

        // HIỂN THỊ
        public IActionResult Index()
        {
            return View(db.Khachhangs.AsNoTracking().ToList());
        }

        // THÊM: mở form
        [HttpGet]
        public IActionResult them()
        {
            return View(new Khachhang());
        }

        // THÊM: lưu dữ liệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult them(
            [Bind("Makh,Tenkh,Namsinh,Phai,Dienthoai,Diachi,Password")]
            Khachhang n)
        {
            if (string.IsNullOrWhiteSpace(n.Makh))
            {
                ModelState.AddModelError(
                    "Makh", "Bạn chưa nhập mã khách hàng.");
            }
            else if (db.Khachhangs.Any(x => x.Makh == n.Makh))
            {
                ModelState.AddModelError(
                    "Makh", "Mã khách hàng đã tồn tại.");
            }

            KiemTraNamSinh(n.Namsinh);

            if (string.IsNullOrWhiteSpace(n.Password))
                ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu.");

            if (!ModelState.IsValid)
                return View(n);

            db.Khachhangs.Add(n);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // SỬA: mở form
        [HttpGet]
        public IActionResult sua(string id)
        {
            var entity = db.Khachhangs.Find(id);

            if (entity == null)
                return NotFound();

            return View(entity);
        }

        // SỬA: lưu dữ liệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult sua(
            [Bind("Makh,Tenkh,Namsinh,Phai,Dienthoai,Diachi,Password")]
            Khachhang n)
        {
            KiemTraNamSinh(n.Namsinh);

            if (!ModelState.IsValid)
                return View(n);

            var entity = db.Khachhangs.Find(n.Makh);

            if (entity == null)
                return NotFound();

            entity.Tenkh = n.Tenkh;
            entity.Namsinh = n.Namsinh;
            entity.Phai = n.Phai;
            entity.Dienthoai = n.Dienthoai;
            entity.Diachi = n.Diachi;

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
            var entity = db.Khachhangs.Find(id);

            if (entity == null)
                return NotFound();

            ViewBag.CoLienQuan = db.Phieudathangs.Any(x => x.Makh == id);
            return View(entity);
        }

        // XÓA: thực hiện xóa
        [HttpPost, ActionName("xoa")]
        [ValidateAntiForgeryToken]
        public IActionResult xoa_Post(string id)
        {
            var entity = db.Khachhangs.Find(id);

            if (entity == null)
                return NotFound();

            ViewBag.CoLienQuan = db.Phieudathangs.Any(x => x.Makh == id);
            if (ViewBag.CoLienQuan)
            {
                ModelState.AddModelError("", "Không thể xóa vì dữ liệu đang được sử dụng ở bảng liên quan.");
                return View("xoa", entity);
            }

            try
            {
                db.Khachhangs.Remove(entity);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    "",
                    "Không xóa được. Hãy kiểm tra phiếu đặt hàng "
                    + "liên quan và các ràng buộc trong database.");

                return View("xoa", entity);
            }
        }

        // Dùng chung khi thêm và sửa
        private void KiemTraNamSinh(int? namSinh)
        {
            if (namSinh.HasValue &&
                (namSinh.Value < 1 ||
                 namSinh.Value > DateTime.Now.Year))
            {
                ModelState.AddModelError(
                    "Namsinh", "Năm sinh không hợp lệ.");
            }
        }

        [HttpGet]
        public IActionResult indexDangNhap() => View(new LoginModel());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult dangnhap(LoginModel model)
        {
            if (!ModelState.IsValid) return View("indexDangNhap", model);
            var kh = db.Khachhangs.AsNoTracking().FirstOrDefault(x => x.Makh == model.Makh);
            if (kh == null || string.IsNullOrEmpty(kh.Password) || kh.Password != model.Password)
            {
                ModelState.AddModelError("", "Mã khách hàng hoặc mật khẩu không đúng.");
                return View("indexDangNhap", model);
            }
            // Store only the identity and display name, never password/navigation properties.
            HttpContext.Session.SetString("Khachhang", kh.Makh);
            HttpContext.Session.SetString("TenKhachhang", kh.Tenkh ?? kh.Makh);
            return RedirectToAction("Index", "Home");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult dangxuat()
        {
            HttpContext.Session.Remove("Khachhang");
            HttpContext.Session.Remove("TenKhachhang");
            return RedirectToAction("Index", "NhanVien");
        }
        [HttpGet]
        public IActionResult dangky() => View(new Khachhang());

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult dangky([Bind("Makh,Tenkh,Namsinh,Phai,Dienthoai,Diachi,Password")] Khachhang n)
        {
            KiemTraNamSinh(n.Namsinh);
            if (db.Khachhangs.Any(x => x.Makh == n.Makh)) ModelState.AddModelError("Makh", "Mã khách hàng đã tồn tại.");
            if (string.IsNullOrWhiteSpace(n.Password)) ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu.");
            if (!ModelState.IsValid) return View(n);
            db.Khachhangs.Add(n); db.SaveChanges();
            TempData["ThongBao"] = "Đăng ký thành công. Bạn có thể đăng nhập.";
            return RedirectToAction(nameof(indexDangNhap));
        }
        [HttpGet]
        public IActionResult ThongTin()
        {
            var id = HttpContext.Session.GetString("Khachhang");
            if (id == null) return RedirectToAction(nameof(indexDangNhap));
            var kh = db.Khachhangs.AsNoTracking().FirstOrDefault(x => x.Makh == id);
            if (kh == null) { HttpContext.Session.Remove("Khachhang"); return RedirectToAction(nameof(indexDangNhap)); }
            kh.Password = null;
            return View(kh);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ThongTin([Bind("Tenkh,Namsinh,Phai,Dienthoai,Diachi,Password")] Khachhang n)
        {
            var id = HttpContext.Session.GetString("Khachhang");
            if (id == null) return RedirectToAction(nameof(indexDangNhap));
            // Account ID always comes from the session, never from the form.
            ModelState.Remove(nameof(Khachhang.Makh)); n.Makh = id;
            KiemTraNamSinh(n.Namsinh);
            if (!ModelState.IsValid) return View(n);
            var kh = db.Khachhangs.Find(id);
            if (kh == null) return NotFound();
            kh.Tenkh = n.Tenkh; kh.Namsinh = n.Namsinh; kh.Phai = n.Phai;
            kh.Dienthoai = n.Dienthoai; kh.Diachi = n.Diachi;
            if (!string.IsNullOrWhiteSpace(n.Password)) kh.Password = n.Password;
            db.SaveChanges(); HttpContext.Session.SetString("TenKhachhang", kh.Tenkh ?? kh.Makh);
            TempData["ThongBao"] = "Đã cập nhật thông tin khách hàng.";
            return RedirectToAction(nameof(ThongTin));
        }
    }
}