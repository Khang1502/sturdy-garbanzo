using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebTest.Models;
namespace WebTest.Controllers;
public class LopController : Controller {
    private readonly QLHVContext db;
    public LopController(QLHVContext context) { db = context; }
    private void Nap() { ViewBag.Lops = new SelectList(db.Lops.AsNoTracking().ToList(), "Malop", "Tenlop");
        ViewBag.HocViens = new SelectList(db.LyLiches.AsNoTracking().ToList(), "Mshv", "Tenhv");
        ViewBag.MonHocs = new SelectList(db.MonHocs.AsNoTracking().ToList(), "Msmh", "Tenmh"); }
    private void KiemTra(Lop n) {  }
    public IActionResult Index() => View(db.Lops.AsNoTracking().ToList());
    [HttpGet] public IActionResult them() { Nap(); return View(new Lop()); }
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult them(Lop n) {
        KiemTra(n);
        if (db.Lops.Any(x => x.Malop == n.Malop)) ModelState.AddModelError("", "Mã đã tồn tại.");
        if (!ModelState.IsValid) { Nap(); return View(n); }
        db.Lops.Add(n); db.SaveChanges(); return RedirectToAction(nameof(Index));
    }
    [HttpGet] public IActionResult sua(string id) {
        var entity = db.Lops.Find(id); if (entity == null) return NotFound(); Nap(); return View(entity);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult sua(Lop n) {
        KiemTra(n); if (!ModelState.IsValid) { Nap(); return View(n); }
        var entity = db.Lops.Find(n.Malop); if (entity == null) return NotFound();
        entity.Tenlop = n.Tenlop;
        db.SaveChanges(); return RedirectToAction(nameof(Index));
    }
    [HttpGet] public IActionResult xoa(string id) {
        var entity = db.Lops.Find(id); if (entity == null) return NotFound();
        ViewBag.CoLienQuan = db.LyLiches.Any(x => x.Malop == id); return View(entity);
    }
    [HttpPost, ActionName("xoa"), ValidateAntiForgeryToken]
    public IActionResult XoaPost(string id) {
        var entity = db.Lops.Find(id); if (entity == null) return NotFound();
        ViewBag.CoLienQuan = db.LyLiches.Any(x => x.Malop == id);
        if (ViewBag.CoLienQuan) { ModelState.AddModelError("", "Không thể xóa vì có dữ liệu liên quan."); return View("xoa", entity); }
        try { db.Lops.Remove(entity); db.SaveChanges(); }
        catch (DbUpdateException) { ModelState.AddModelError("", "Không thể xóa vì có dữ liệu liên quan."); return View("xoa", entity); }
        return RedirectToAction(nameof(Index));
    }
}
