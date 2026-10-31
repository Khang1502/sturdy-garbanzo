using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTest.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebTest.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly QLBHContext db;
        private readonly IWebHostEnvironment environment;

        public HangHoaController(QLBHContext context, IWebHostEnvironment environment)
        {
            db = context;
            this.environment = environment;
        }

        // Lấy dữ liệu cho hai danh sách chọn
        private void NapDanhMuc()
        {
            ViewBag.LoaiItems = new SelectList(db.Loaihanghoas.AsNoTracking().ToList(), "Maloai", "Tenloai");
            ViewBag.NsxItems = new SelectList(db.Nhasanxuats.AsNoTracking().ToList(), "Mansx", "Tennsx");
        }

        // Kiểm tra dữ liệu dùng chung cho thêm và sửa
        private void KiemTraDuLieu(Hanghoa n)
        {
            if (n.Dongia.HasValue && n.Dongia.Value < 0)
            {
                ModelState.AddModelError(
                    "Dongia", "Đơn giá không được âm.");
            }

            if (string.IsNullOrWhiteSpace(n.Maloai) ||
                !db.Loaihanghoas.Any(x => x.Maloai == n.Maloai))
            {
                ModelState.AddModelError(
                    "Maloai", "Bạn phải chọn loại hàng hợp lệ.");
            }

            if (string.IsNullOrWhiteSpace(n.Mansx) ||
                !db.Nhasanxuats.Any(x => x.Mansx == n.Mansx))
            {
                ModelState.AddModelError(
                    "Mansx", "Bạn phải chọn nhà sản xuất hợp lệ.");
            }
        }

        // HIỂN THỊ
        public IActionResult Index()
        {
            return View(db.Hanghoas.AsNoTracking().Include(x => x.MaloaiNavigation).Include(x => x.MansxNavigation).ToList());
        }

        // THÊM: mở form
        [HttpGet]
        public IActionResult them()
        {
            NapDanhMuc();
            return View(new Hanghoa());
        }

        // THÊM: lưu dữ liệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult them(
            [Bind("Mahang,Tenhang,Donvitinh,Dongia,Maloai,Mansx")]
            Hanghoa n, IFormFile? anh)
        {
            if (string.IsNullOrWhiteSpace(n.Mahang))
            {
                ModelState.AddModelError(
                    "Mahang", "Bạn chưa nhập mã hàng.");
            }
            else if (db.Hanghoas.Any(x => x.Mahang == n.Mahang))
            {
                ModelState.AddModelError(
                    "Mahang", "Mã hàng đã tồn tại.");
            }

            KiemTraDuLieu(n);
            KiemTraAnh(anh);

            if (!ModelState.IsValid)
            {
                NapDanhMuc();
                return View(n);
            }

            n.Hinh = LuuAnh(anh);
            db.Hanghoas.Add(n);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // SỬA: mở form
        [HttpGet]
        public IActionResult sua(string id)
        {
            var entity = db.Hanghoas.Find(id);

            if (entity == null)
                return NotFound();

            NapDanhMuc();
            return View(entity);
        }

        // SỬA: lưu dữ liệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult sua(
            [Bind("Mahang,Tenhang,Donvitinh,Dongia,Maloai,Mansx")]
            Hanghoa n, IFormFile? anh)
        {
            var current = db.Hanghoas.AsNoTracking().FirstOrDefault(x => x.Mahang == n.Mahang);
            if (current == null) return NotFound();
            n.Hinh = current.Hinh;
            KiemTraDuLieu(n);
            KiemTraAnh(anh);

            if (!ModelState.IsValid)
            {
                NapDanhMuc();
                return View(n);
            }

            var entity = db.Hanghoas.Find(n.Mahang);

            if (entity == null)
                return NotFound();

            entity.Tenhang = n.Tenhang;
            entity.Donvitinh = n.Donvitinh;
            entity.Dongia = n.Dongia;
            if (anh != null && anh.Length > 0) entity.Hinh = LuuAnh(anh);
            entity.Maloai = n.Maloai;
            entity.Mansx = n.Mansx;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // XÓA: mở trang xác nhận
        [HttpGet]
        public IActionResult xoa(string id)
        {
            var entity = db.Hanghoas.Find(id);

            if (entity == null)
                return NotFound();

            ViewBag.CoLienQuan = db.Chitietphieudathangs.Any(x => x.Mahang == id) || db.Chitietphieugiaohangs.Any(x => x.Mahang == id);
            return View(entity);
        }

        // XÓA: thực hiện xóa
        [HttpPost, ActionName("xoa")]
        [ValidateAntiForgeryToken]
        public IActionResult xoa_Post(string id)
        {
            var entity = db.Hanghoas.Find(id);

            if (entity == null)
                return NotFound();

            ViewBag.CoLienQuan = db.Chitietphieudathangs.Any(x => x.Mahang == id) || db.Chitietphieugiaohangs.Any(x => x.Mahang == id);
            if (ViewBag.CoLienQuan)
            {
                ModelState.AddModelError("", "Không thể xóa vì dữ liệu đang được sử dụng ở bảng liên quan.");
                return View("xoa", entity);
            }

            try
            {
                db.Hanghoas.Remove(entity);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    "",
                    "Không xóa được. Hãy kiểm tra chi tiết phiếu "
                    + "liên quan và các ràng buộc trong database.");

                return View("xoa", entity);
            }
        }

        [HttpGet]
        public IActionResult ThongTinLoai(string id)
        {
            var x = db.Loaihanghoas.AsNoTracking().FirstOrDefault(x => x.Maloai == id);
            return x == null ? NotFound() : Json(new { ten = x.Tenloai });
        }
        [HttpGet]
        public IActionResult ThongTinNsx(string id)
        {
            var x = db.Nhasanxuats.AsNoTracking().FirstOrDefault(x => x.Mansx == id);
            return x == null ? NotFound() : Json(new { ten = x.Tennsx, diachi = x.Diachi });
        }
        [HttpGet]
        public IActionResult ChiTiet(string id)
        {
            var x = db.Hanghoas.AsNoTracking().Include(x => x.MaloaiNavigation)
                .Include(x => x.MansxNavigation).FirstOrDefault(x => x.Mahang == id);
            return x == null ? NotFound() : View(x);
        }
        private void KiemTraAnh(IFormFile? file)
        {
            if (file == null || file.Length == 0) return;
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (file.Length > 2 * 1024 * 1024 || !new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(ext))
            {
                ModelState.AddModelError("anh", "Chọn ảnh JPG, PNG, GIF, WebP không quá 2 MB."); return;
            }
            using var stream = file.OpenReadStream();
            var header = new byte[12];
            int count = 0, read;
            while (count < header.Length && (read = stream.Read(header, count, header.Length - count)) > 0) count += read;
            bool valid = count >= 12 && (ext switch {
                ".jpg" or ".jpeg" => header[0] == 0xff && header[1] == 0xd8 && header[2] == 0xff,
                ".png" => header.Take(8).SequenceEqual(new byte[] {137,80,78,71,13,10,26,10}),
                ".gif" => System.Text.Encoding.ASCII.GetString(header, 0, 6) is "GIF87a" or "GIF89a",
                ".webp" => System.Text.Encoding.ASCII.GetString(header, 0, 4) == "RIFF" && System.Text.Encoding.ASCII.GetString(header, 8, 4) == "WEBP",
                _ => false
            });
            if (!valid) ModelState.AddModelError("anh", "Nội dung tệp không đúng định dạng ảnh đã chọn.");
        }
        private string? LuuAnh(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;
            string name = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName).ToLowerInvariant();
            string folder = Path.Combine(environment.WebRootPath, "Hinh");
            Directory.CreateDirectory(folder);
            using var stream = System.IO.File.Create(Path.Combine(folder, name));
            file.CopyTo(stream);
            return name;
        }
    }
}