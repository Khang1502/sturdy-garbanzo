using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebTest.Models;
namespace WebTest.Controllers;
public class DiemThiController : Controller {
    private readonly QLHVContext db;
    public DiemThiController(QLHVContext context) { db = context; }
    private void Nap() { ViewBag.Lops = new SelectList(db.Lops.AsNoTracking().ToList(), "Malop", "Tenlop");
        ViewBag.HocViens = new SelectList(db.LyLiches.AsNoTracking().ToList(), "Mshv", "Tenhv");
        ViewBag.MonHocs = new SelectList(db.MonHocs.AsNoTracking().ToList(), "Msmh", "Tenmh"); }
    private void KiemTra(DiemThi n) { if (!db.LyLiches.Any(x => x.Mshv == n.Mshv)) ModelState.AddModelError("Mshv", "Chọn học viên hợp lệ.");
        if (!db.MonHocs.Any(x => x.Msmh == n.Msmh)) ModelState.AddModelError("Msmh", "Chọn môn học hợp lệ."); }
    public IActionResult Index() => View(db.DiemThis.AsNoTracking().ToList());
    [HttpGet] public IActionResult them() { Nap(); return View(new DiemThi()); }
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult them(DiemThi n) {
        KiemTra(n);
        if (db.DiemThis.Any(x => x.Mshv == n.Mshv && x.Msmh == n.Msmh)) ModelState.AddModelError("", "Mã đã tồn tại.");
        if (!ModelState.IsValid) { Nap(); return View(n); }
        db.DiemThis.Add(n); db.SaveChanges(); return RedirectToAction(nameof(Index));
    }
    [HttpGet] public IActionResult sua(string id, string mon) {
        var entity = db.DiemThis.Find(id, mon); if (entity == null) return NotFound(); Nap(); return View(entity);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult sua(DiemThi n) {
        KiemTra(n); if (!ModelState.IsValid) { Nap(); return View(n); }
        var entity = db.DiemThis.Find(n.Mshv, n.Msmh); if (entity == null) return NotFound();
        entity.Diem = n.Diem;
        db.SaveChanges(); return RedirectToAction(nameof(Index));
    }
    [HttpGet] public IActionResult xoa(string id, string mon) {
        var entity = db.DiemThis.Find(id, mon); if (entity == null) return NotFound();
        ViewBag.CoLienQuan = false; return View(entity);
    }
    [HttpPost, ActionName("xoa"), ValidateAntiForgeryToken]
    public IActionResult XoaPost(string id, string mon) {
        var entity = db.DiemThis.Find(id, mon); if (entity == null) return NotFound();
        ViewBag.CoLienQuan = false;
        if (ViewBag.CoLienQuan) { ModelState.AddModelError("", "Không thể xóa vì có dữ liệu liên quan."); return View("xoa", entity); }
        try { db.DiemThis.Remove(entity); db.SaveChanges(); }
        catch (DbUpdateException) { ModelState.AddModelError("", "Không thể xóa vì có dữ liệu liên quan."); return View("xoa", entity); }
        return RedirectToAction(nameof(Index));
    }
}
