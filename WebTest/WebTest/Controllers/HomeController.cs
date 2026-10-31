using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTest.Models;
namespace WebTest.Controllers;
public class HomeController : Controller {
    private readonly QLBHContext db;
    public HomeController(QLBHContext context) { db = context; }
    public IActionResult Index() => View(db.Hanghoas.AsNoTracking().Include(x => x.MaloaiNavigation).Include(x => x.MansxNavigation).ToList());
    public IActionResult Privacy() => View();
    private List<CartItem> ReadCart() => JsonSerializer.Deserialize<List<CartItem>>(HttpContext.Session.GetString("GioHang") ?? "[]") ?? new();
    private void SaveCart(List<CartItem> cart) => HttpContext.Session.SetString("GioHang", JsonSerializer.Serialize(cart));
    private List<CartLine> LoadLines(List<CartItem> cart) {
        var ids = cart.Select(x => x.Mahang).ToList();
        var goods = db.Hanghoas.AsNoTracking().Where(x => ids.Contains(x.Mahang)).ToList();
        return cart.Where(x => goods.Any(g => g.Mahang == x.Mahang)).Select(x => new CartLine { Hang = goods.First(g => g.Mahang == x.Mahang), Soluong = x.Soluong }).ToList();
    }
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult chonHanghoa(string id, int txtSoluong) {
        if (txtSoluong < 1 || txtSoluong > 1000) return BadRequest("Số lượng phải từ 1 đến 1000.");
        if (!db.Hanghoas.Any(x => x.Mahang == id)) return NotFound();
        var cart = ReadCart(); var item = cart.FirstOrDefault(x => x.Mahang == id);
        if (item != null) {
            if (item.Soluong + txtSoluong > 1000) return BadRequest("Mỗi mặt hàng tối đa 1000 sản phẩm.");
            item.Soluong += txtSoluong;
        } else cart.Add(new CartItem { Mahang = id, Soluong = txtSoluong });
        SaveCart(cart); return RedirectToAction(nameof(GioHang));
    }
    [HttpGet]
    public IActionResult GioHang() {
        var kh = db.Khachhangs.AsNoTracking().FirstOrDefault(x => x.Makh == HttpContext.Session.GetString("Khachhang"));
        var cart = ReadCart(); var lines = LoadLines(cart);
        if (lines.Count != cart.Count) {
            SaveCart(lines.Select(x => new CartItem { Mahang = x.Hang.Mahang, Soluong = x.Soluong }).ToList());
            TempData["ThongBao"] = "Một mặt hàng không còn tồn tại đã được bỏ khỏi giỏ.";
        }
        return View(new CartModel { Lines = lines, Diachigh = kh?.Diachi });
    }
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult XoaKhoiGio(string id) {
        var cart = ReadCart(); cart.RemoveAll(x => x.Mahang == id); SaveCart(cart); return RedirectToAction(nameof(GioHang));
    }
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult LapDonDatHang(CartModel model) {
        var kh = db.Khachhangs.Find(HttpContext.Session.GetString("Khachhang"));
        if (kh == null) return RedirectToAction("indexDangNhap", "KhachHang");
        var cart = ReadCart(); model.Lines = LoadLines(cart);
        if (cart.Count == 0) ModelState.AddModelError("", "Giỏ hàng đang trống.");
        if (model.Lines.Count != cart.Count) ModelState.AddModelError("", "Một mặt hàng đã bị xóa. Hãy xóa mặt hàng đó khỏi giỏ trước khi đặt.");
        if (model.Ngaygh?.Date < DateTime.Today) ModelState.AddModelError("Ngaygh", "Ngày giao hàng không được trước hôm nay.");
        if (model.Lines.Any(x => x.Hang.Dongia == null || x.Hang.Dongia < 0 || x.Soluong < 1 || x.Soluong > 1000)) ModelState.AddModelError("", "Mặt hàng chưa có đơn giá hợp lệ hoặc số lượng không hợp lệ.");
        if (!ModelState.IsValid) return View("GioHang", model);
        var order = new Phieudathang {
            Mapdh = "DH" + Guid.NewGuid().ToString("N"), Ngaydh = DateTime.Now,
            Ngaygh = model.Ngaygh, Diachigh = model.Diachigh, Makh = kh.Makh,
            Chitietphieudathangs = model.Lines.Select(x => new Chitietphieudathang { Mahang = x.Hang.Mahang, Soluong = x.Soluong, Dongia = x.Hang.Dongia }).ToList()
        };
        try {
            db.Phieudathangs.Add(order);
            // EF saves the order and all lines together in one transaction.
            db.SaveChanges();
        } catch (DbUpdateException) {
            ModelState.AddModelError("", "Không lưu được đơn hàng. Giỏ hàng được giữ nguyên, vui lòng thử lại.");
            return View("GioHang", model);
        }
        HttpContext.Session.Remove("GioHang");
        TempData["ThongBao"] = "Đã lập đơn đặt hàng " + order.Mapdh;
        return RedirectToAction(nameof(GioHang));
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
