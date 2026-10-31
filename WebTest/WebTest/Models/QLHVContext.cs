using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace WebTest.Models;
public class Lop {
    [Key, Required, StringLength(50), Display(Name = "Mã lớp")] public string Malop { get; set; } = "";
    [Required, StringLength(50), Display(Name = "Tên lớp")] public string Tenlop { get; set; } = "";
}
public class LyLich {
    [Key, Required, StringLength(50), Display(Name = "Mã số học viên")] public string Mshv { get; set; } = "";
    [Required, StringLength(50), Display(Name = "Tên học viên")] public string Tenhv { get; set; } = "";
    [Required, DataType(DataType.Date), Display(Name = "Ngày sinh")] public DateTime? Ngaysinh { get; set; }
    [Required, Display(Name = "Phái")] public bool? Phai { get; set; }
    [Required, StringLength(50), Display(Name = "Mã lớp")] public string Malop { get; set; } = "";
}
public class MonHoc {
    [Key, Required, StringLength(50), Display(Name = "Mã môn học")] public string Msmh { get; set; } = "";
    [Required, StringLength(50), Display(Name = "Tên môn học")] public string Tenmh { get; set; } = "";
    [Required, Range(1, 1000), Display(Name = "Số tiết")] public int? Sotiet { get; set; }
}
public class DiemThi {
    [Required, StringLength(50), Display(Name = "Mã học viên")] public string Mshv { get; set; } = "";
    [Required, StringLength(50), Display(Name = "Mã môn học")] public string Msmh { get; set; } = "";
    [Required, Range(0, 10), Display(Name = "Điểm")] public double? Diem { get; set; }
}
public class QLHVContext : DbContext {
    public QLHVContext(DbContextOptions<QLHVContext> options) : base(options) { }
    public DbSet<Lop> Lops => Set<Lop>();
    public DbSet<LyLich> LyLiches => Set<LyLich>();
    public DbSet<MonHoc> MonHocs => Set<MonHoc>();
    public DbSet<DiemThi> DiemThis => Set<DiemThi>();
    protected override void OnModelCreating(ModelBuilder b) {
        b.Entity<Lop>().ToTable("lop");
        b.Entity<LyLich>().ToTable("lylich").HasOne<Lop>().WithMany().HasForeignKey(x => x.Malop).OnDelete(DeleteBehavior.Restrict);
        b.Entity<MonHoc>().ToTable("monhoc");
        b.Entity<DiemThi>().ToTable("diemthi").HasKey(x => new { x.Mshv, x.Msmh });
        b.Entity<DiemThi>().HasOne<LyLich>().WithMany().HasForeignKey(x => x.Mshv).OnDelete(DeleteBehavior.Restrict);
        b.Entity<DiemThi>().HasOne<MonHoc>().WithMany().HasForeignKey(x => x.Msmh).OnDelete(DeleteBehavior.Restrict);
        // Keep the table and column names used in the lab database diagram.
        foreach (var e in b.Model.GetEntityTypes()) foreach (var prop in e.GetProperties()) prop.SetColumnName(prop.Name.ToLowerInvariant());
    }
}
