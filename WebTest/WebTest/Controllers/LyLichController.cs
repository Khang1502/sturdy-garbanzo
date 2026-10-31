using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebTest.Models;
namespace WebTest.Controllers;
public class LyLichController : Controller {
    private readonly QLHVContext db;
    public LyLichController(QLHVContext context) { db = context; }
    private void Nap() { ViewBag.Lops = new SelectList(db.Lops.AsNoTracking().ToList(), "Malop", "Tenlop");
        ViewBag.HocViens = new SelectList(db.LyLiches.AsNoTracking().ToList(), "Mshv", "Tenhv");
        ViewBag.MonHocs = new SelectList(db.MonHocs.AsNoTracking().ToList(), "Msmh", "Tenmh"); }
    private void KiemTra(LyLich n) { if (!db.Lops.Any(x => x.Malop == n.Malop)) ModelState.AddModelError("Malop", "Chọn lớp hợp lệ.");
        if (n.Ngaysinh > DateTime.Today) ModelState.AddModelError("Ngaysinh", "Ngày sinh không được ở tương lai."); }
    public IActionResult Index() => View(db.LyLiches.AsNoTracking().ToList());
    [HttpGet] public IActionResult them() { Nap(); return View(new LyLich()); }
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult them(LyLich n) {
        KiemTra(n);
        if (db.LyLiches.Any(x => x.Mshv == n.Mshv)) ModelState.AddModelError("", "Mã đã tồn tại.");
        if (!ModelState.IsValid) { Nap(); return View(n); }
        db.LyLiches.Add(n); db.SaveChanges(); return RedirectToAction(nameof(Index));
    }
    [HttpGet] public IActionResult sua(string id) {
        var entity = db.LyLiches.Find(id); if (entity == null) return NotFound(); Nap(); return View(entity);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult sua(LyLich n) {
        KiemTra(n); if (!ModelState.IsValid) { Nap(); return View(n); }
        var entity = db.LyLiches.Find(n.Mshv); if (entity == null) return NotFound();
        entity.Tenhv = n.Tenhv;
        entity.Ngaysinh = n.Ngaysinh;
        entity.Phai = n.Phai;
        entity.Malop = n.Malop;
        db.SaveChanges(); return RedirectToAction(nameof(Index));
    }
    [HttpGet] public IActionResult xoa(string id) {
        var entity = db.LyLiches.Find(id); if (entity == null) return NotFound();
        ViewBag.CoLienQuan = db.DiemThis.Any(x => x.Mshv == id); return View(entity);
    }
    [HttpPost, ActionName("xoa"), ValidateAntiForgeryToken]
    public IActionResult XoaPost(string id) {
        var entity = db.LyLiches.Find(id); if (entity == null) return NotFound();
        ViewBag.CoLienQuan = db.DiemThis.Any(x => x.Mshv == id);
        if (ViewBag.CoLienQuan) { ModelState.AddModelError("", "Không thể xóa vì có dữ liệu liên quan."); return View("xoa", entity); }
        try { db.LyLiches.Remove(entity); db.SaveChanges(); }
        catch (DbUpdateException) { ModelState.AddModelError("", "Không thể xóa vì có dữ liệu liên quan."); return View("xoa", entity); }
        return RedirectToAction(nameof(Index));
    }
}
