using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebTest.Models;
namespace WebTest.Controllers;
public class MonHocController : Controller {
    private readonly QLHVContext db;
    public MonHocController(QLHVContext context) { db = context; }
    private void Nap() { ViewBag.Lops = new SelectList(db.Lops.AsNoTracking().ToList(), "Malop", "Tenlop");
        ViewBag.HocViens = new SelectList(db.LyLiches.AsNoTracking().ToList(), "Mshv", "Tenhv");
        ViewBag.MonHocs = new SelectList(db.MonHocs.AsNoTracking().ToList(), "Msmh", "Tenmh"); }
    private void KiemTra(MonHoc n) {  }
    public IActionResult Index() => View(db.MonHocs.AsNoTracking().ToList());
    [HttpGet] public IActionResult them() { Nap(); return View(new MonHoc()); }
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult them(MonHoc n) {
        KiemTra(n);
        if (db.MonHocs.Any(x => x.Msmh == n.Msmh)) ModelState.AddModelError("", "Mã đã tồn tại.");
        if (!ModelState.IsValid) { Nap(); return View(n); }
        db.MonHocs.Add(n); db.SaveChanges(); return RedirectToAction(nameof(Index));
    }
    [HttpGet] public IActionResult sua(string id) {
        var entity = db.MonHocs.Find(id); if (entity == null) return NotFound(); Nap(); return View(entity);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult sua(MonHoc n) {
        KiemTra(n); if (!ModelState.IsValid) { Nap(); return View(n); }
        var entity = db.MonHocs.Find(n.Msmh); if (entity == null) return NotFound();
        entity.Tenmh = n.Tenmh;
        entity.Sotiet = n.Sotiet;
        db.SaveChanges(); return RedirectToAction(nameof(Index));
    }
    [HttpGet] public IActionResult xoa(string id) {
        var entity = db.MonHocs.Find(id); if (entity == null) return NotFound();
        ViewBag.CoLienQuan = db.DiemThis.Any(x => x.Msmh == id); return View(entity);
    }
    [HttpPost, ActionName("xoa"), ValidateAntiForgeryToken]
    public IActionResult XoaPost(string id) {
        var entity = db.MonHocs.Find(id); if (entity == null) return NotFound();
        ViewBag.CoLienQuan = db.DiemThis.Any(x => x.Msmh == id);
        if (ViewBag.CoLienQuan) { ModelState.AddModelError("", "Không thể xóa vì có dữ liệu liên quan."); return View("xoa", entity); }
        try { db.MonHocs.Remove(entity); db.SaveChanges(); }
        catch (DbUpdateException) { ModelState.AddModelError("", "Không thể xóa vì có dữ liệu liên quan."); return View("xoa", entity); }
        return RedirectToAction(nameof(Index));
    }
}
