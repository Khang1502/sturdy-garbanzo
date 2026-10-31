using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebTest.Models;
using Xunit;

namespace WebTest.Tests;

public sealed class LabApp : WebApplicationFactory<Program>
{
    private readonly SqliteConnection sales = new("Data Source=:memory:");
    private readonly SqliteConnection students = new("Data Source=:memory:");
    public LabApp() {
        sales.Open(); students.Open();
        sales.CreateCollation("Vietnamese_CI_AS", (a,b) => string.Compare(a,b, StringComparison.OrdinalIgnoreCase));
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services => {
            services.RemoveAll<DbContextOptions<QLBHContext>>();
            services.RemoveAll<DbContextOptions<QLHVContext>>();
            services.AddDbContext<QLBHContext>(o => o.UseSqlite(sales));
            services.AddDbContext<QLHVContext>(o => o.UseSqlite(students));
        });
    }
    public HttpClient Start() {
        var client = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<QLBHContext>();
        db.Database.EnsureCreated();
        db.Nhasanxuats.Add(new() { Mansx = "NSX01", Tennsx = "Hãng mẫu", Diachi = "TP.HCM" });
        db.Loaihanghoas.Add(new() { Maloai = "L01", Tenloai = "Điện tử" });
        db.Hanghoas.Add(new() { Mahang = "H01", Tenhang = "Sản phẩm mẫu", Donvitinh = "Cái", Dongia = 25000, Maloai = "L01", Mansx = "NSX01" });
        db.Khachhangs.Add(new() { Makh = "KH01", Tenkh = "Khách mẫu", Namsinh = 2000, Phai = true, Dienthoai = "0901234567", Diachi = "TP.HCM", Password = "test123" });
        db.Nhanviens.Add(new() { Manv = "NV01", Tennv = "Nhân viên mẫu", Ngaysinh = new DateTime(2000,1,1), Phai = true, Diachi = "TP.HCM", Password = "test123" });
        db.SaveChanges();
        var hv = scope.ServiceProvider.GetRequiredService<QLHVContext>();
        hv.Database.EnsureCreated();
        hv.Lops.Add(new() { Malop = "C01", Tenlop = "Lớp mẫu" }); hv.SaveChanges();
        return client;
    }
    protected override void Dispose(bool disposing) { base.Dispose(disposing); if (disposing) { sales.Dispose(); students.Dispose(); } }
}

public class LabTests
{
    private static string Token(string html) {
        var match = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        Assert.True(match.Success, "Form must contain an antiforgery token.");
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }
    private static async Task<HttpResponseMessage> Post(HttpClient client, string form, string action, Dictionary<string,string> data) {
        var page = await client.GetAsync(form); Assert.Equal(HttpStatusCode.OK, page.StatusCode);
        data["__RequestVerificationToken"] = Token(await page.Content.ReadAsStringAsync());
        return await client.PostAsync(action, new FormUrlEncodedContent(data));
    }
    private static Task<HttpResponseMessage> Login(HttpClient c, string password = "test123") => Post(c, "/KhachHang/indexDangNhap", "/KhachHang/dangnhap", new() { ["Makh"] = "KH01", ["Password"] = password });

    [Theory]
    [InlineData("NhaSanXuat", "NSX01")]
    [InlineData("LoaiHangHoa", "L01")]
    [InlineData("NhanVien", "NV01")]
    [InlineData("KhachHang", "KH01")]
    [InlineData("HangHoa", "H01")]
    public async Task CrudPagesRenderWithLayout(string controller, string id) {
        using var app = new LabApp(); using var c = app.Start();
        foreach (var path in new[] { $"/{controller}", $"/{controller}/them", $"/{controller}/sua/{id}", $"/{controller}/xoa/{id}" }) {
            var r = await c.GetAsync(path); Assert.Equal(HttpStatusCode.OK, r.StatusCode);
            var html = await r.Content.ReadAsStringAsync(); Assert.Contains("/lib/bootstrap/dist/css/bootstrap.min.css", html);
            Assert.DoesNotContain("value=\"test123\"", html);
        }
        Assert.Equal(HttpStatusCode.NotFound, (await c.GetAsync($"/{controller}/sua/unknown")).StatusCode);
    }
    [Fact]
    public async Task ManufacturerCanBeCreatedEditedDeletedAndDuplicateIsRejected() {
        using var app = new LabApp(); using var c = app.Start();
        var data = new Dictionary<string,string> { ["Mansx"]="TEST", ["Tennsx"]="Mới", ["Diachi"]="TP.HCM" };
        Assert.Equal(HttpStatusCode.Redirect, (await Post(c,"/NhaSanXuat/them","/NhaSanXuat/them",new(data))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await Post(c,"/NhaSanXuat/them","/NhaSanXuat/them",new(data))).StatusCode);
        data["Tennsx"]="Đã sửa";
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/NhaSanXuat/sua/TEST","/NhaSanXuat/sua",new(data))).StatusCode);
        using (var s=app.Services.CreateScope()) Assert.Equal("Đã sửa", s.ServiceProvider.GetRequiredService<QLBHContext>().Nhasanxuats.Find("TEST")!.Tennsx);
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/NhaSanXuat/xoa/TEST","/NhaSanXuat/xoa",new(){["id"]="TEST"})).StatusCode);
        using (var s=app.Services.CreateScope()) Assert.Null(s.ServiceProvider.GetRequiredService<QLBHContext>().Nhasanxuats.Find("TEST"));
    }
    [Fact]
    public async Task RelatedManufacturerCannotBeDeletedAndCsrfIsRequired() {
        using var app = new LabApp(); using var c = app.Start();
        var r=await Post(c,"/NhaSanXuat/them","/NhaSanXuat/xoa",new(){["id"]="NSX01"});
        Assert.Equal(HttpStatusCode.OK,r.StatusCode);
        using var s=app.Services.CreateScope(); Assert.NotNull(s.ServiceProvider.GetRequiredService<QLBHContext>().Nhasanxuats.Find("NSX01"));
        Assert.Equal(HttpStatusCode.BadRequest,(await c.PostAsync("/NhaSanXuat/xoa",new FormUrlEncodedContent(new Dictionary<string,string>{["id"]="NSX01"}))).StatusCode);
    }
    [Fact]
    public async Task ValidationRejectsMissingNamesAndFutureBirthdays() {
        using var app=new LabApp();using var c=app.Start();
        Assert.Equal(HttpStatusCode.OK,(await Post(c,"/NhaSanXuat/them","/NhaSanXuat/them",new(){["Mansx"]="BAD",["Tennsx"]="",["Diachi"]="TP.HCM"})).StatusCode);
        Assert.Equal(HttpStatusCode.OK,(await Post(c,"/NhanVien/them","/NhanVien/them",new(){["Manv"]="BAD",["Tennv"]="Test",["Ngaysinh"]="2999-01-01",["Phai"]="true",["Diachi"]="TP.HCM",["Password"]="test"})).StatusCode);
        using var s=app.Services.CreateScope();var db=s.ServiceProvider.GetRequiredService<QLBHContext>();Assert.Null(db.Nhasanxuats.Find("BAD"));Assert.Null(db.Nhanviens.Find("BAD"));
    }
    [Fact]
    public async Task LoginRegistrationProfileAndLogoutWork() {
        using var app=new LabApp(); using var c=app.Start();
        Assert.Equal(HttpStatusCode.OK,(await Login(c,"wrong")).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect,(await c.GetAsync("/KhachHang/ThongTin")).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect,(await Login(c)).StatusCode);
        var data=new Dictionary<string,string>{["Makh"]="OTHER",["Tenkh"]="Đổi tên",["Namsinh"]="2001",["Phai"]="false",["Dienthoai"]="0901234567",["Diachi"]="Hà Nội",["Password"]=""};
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/KhachHang/ThongTin","/KhachHang/ThongTin",data)).StatusCode);
        using(var s=app.Services.CreateScope()) { var db=s.ServiceProvider.GetRequiredService<QLBHContext>();Assert.Equal("Đổi tên",db.Khachhangs.Find("KH01")!.Tenkh);Assert.Null(db.Khachhangs.Find("OTHER")); }
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/KhachHang/ThongTin","/KhachHang/dangxuat",new())).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect,(await c.GetAsync("/KhachHang/ThongTin")).StatusCode);
        data["Makh"]="NEW";data["Password"]="newpass";
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/KhachHang/dangky","/KhachHang/dangky",data)).StatusCode);
    }
    [Fact]
    public async Task ProductAjaxAndImageUploadWork() {
        using var app=new LabApp();using var c=app.Start();
        Assert.Contains("ten",await c.GetStringAsync("/HangHoa/ThongTinLoai?id=L01"));
        Assert.Contains("diachi",await c.GetStringAsync("/HangHoa/ThongTinNsx?id=NSX01"));
        Assert.Equal(HttpStatusCode.NotFound,(await c.GetAsync("/HangHoa/ThongTinLoai?id=unknown")).StatusCode);
        Assert.Equal(HttpStatusCode.OK,(await c.GetAsync("/HangHoa/ChiTiet/H01")).StatusCode);
        var html=await c.GetStringAsync("/HangHoa/them");
        using var content=new MultipartFormDataContent();
        foreach(var pair in new Dictionary<string,string>{["Mahang"]="IMG",["Tenhang"]="Ảnh mẫu",["Donvitinh"]="Cái",["Dongia"]="200",["Maloai"]="L01",["Mansx"]="NSX01",["__RequestVerificationToken"]=Token(html)}) content.Add(new StringContent(pair.Value),pair.Key);
        content.Add(new ByteArrayContent(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAusB9Wl6mVQAAAAASUVORK5CYII=")),"anh","tiny.png");
        Assert.Equal(HttpStatusCode.Redirect,(await c.PostAsync("/HangHoa/them",content)).StatusCode);
        using var scope=app.Services.CreateScope();var file=scope.ServiceProvider.GetRequiredService<QLBHContext>().Hanghoas.Find("IMG")!.Hinh;
        Assert.NotNull(file); Assert.Equal(HttpStatusCode.OK,(await c.GetAsync("/Hinh/"+file)).StatusCode);
        var env=scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();File.Delete(Path.Combine(env.WebRootPath,"Hinh",file!));
    }
    [Theory]
    [InlineData("-1", true)]
    [InlineData("200", false)]
    public async Task InvalidImageAndNegativePriceAreNotSaved(string price, bool validImage) {
        using var app=new LabApp();using var c=app.Start();
        var html=await c.GetStringAsync("/HangHoa/them");using var content=new MultipartFormDataContent();
        foreach(var pair in new Dictionary<string,string>{["Mahang"]="BAD",["Tenhang"]="Sai",["Donvitinh"]="Cái",["Dongia"]=price,["Maloai"]="L01",["Mansx"]="NSX01",["__RequestVerificationToken"]=Token(html)}) content.Add(new StringContent(pair.Value),pair.Key);
        content.Add(new ByteArrayContent(validImage ? Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAusB9Wl6mVQAAAAASUVORK5CYII=") : System.Text.Encoding.UTF8.GetBytes("not an image")),"anh","bad.png");
        Assert.Equal(HttpStatusCode.OK,(await c.PostAsync("/HangHoa/them",content)).StatusCode);
        using var s=app.Services.CreateScope(); Assert.Null(s.ServiceProvider.GetRequiredService<QLBHContext>().Hanghoas.Find("BAD"));
    }
    [Fact]
    public async Task CartCombinesQuantitiesAndCheckoutSavesOrderAndLines() {
        using var app=new LabApp(); using var c=app.Start();
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/","/Home/chonHanghoa",new(){["id"]="H01",["txtSoluong"]="2"})).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/","/Home/chonHanghoa",new(){["id"]="H01",["txtSoluong"]="1"})).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest,(await Post(c,"/","/Home/chonHanghoa",new(){["id"]="H01",["txtSoluong"]="-1"})).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/","/Home/LapDonDatHang",new())).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect,(await Login(c)).StatusCode);
        Assert.Equal(HttpStatusCode.OK,(await Post(c,"/Home/GioHang","/Home/LapDonDatHang",new(){["Diachigh"]="TP.HCM",["Ngaygh"]="2000-01-01"})).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/Home/GioHang","/Home/LapDonDatHang",new(){["Diachigh"]="TP.HCM",["Ngaygh"]=DateTime.Today.AddDays(1).ToString("yyyy-MM-dd")})).StatusCode);
        using var s=app.Services.CreateScope(); var db=s.ServiceProvider.GetRequiredService<QLBHContext>();
        var order=Assert.Single(db.Phieudathangs.Include(x=>x.Chitietphieudathangs));
        Assert.Equal("KH01",order.Makh);var line=Assert.Single(order.Chitietphieudathangs);Assert.Equal(3d,line.Soluong);Assert.Equal(25000d,line.Dongia);
        Assert.Equal(HttpStatusCode.OK,(await Post(c,"/Home/GioHang","/Home/LapDonDatHang",new(){["Diachigh"]="TP.HCM",["Ngaygh"]=DateTime.Today.AddDays(1).ToString("yyyy-MM-dd")})).StatusCode);
        Assert.Equal(1,db.Phieudathangs.Count());
    }
    [Fact]
    public async Task StudentCrudAndForeignKeyValidationWork() {
        using var app=new LabApp();using var c=app.Start();
        foreach(var controller in new[]{"LyLich","Lop","MonHoc","DiemThi"}) {
            Assert.Equal(HttpStatusCode.OK,(await c.GetAsync("/"+controller)).StatusCode);
            Assert.Equal(HttpStatusCode.OK,(await c.GetAsync("/"+controller+"/them")).StatusCode);
        }
        var data=new Dictionary<string,string>{["Mshv"]="HV01",["Tenhv"]="Học viên",["Ngaysinh"]="2000-01-01",["Phai"]="true",["Malop"]="C01"};
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/LyLich/them","/LyLich/them",new(data))).StatusCode);
        Assert.Equal(HttpStatusCode.OK,(await Post(c,"/LyLich/them","/LyLich/them",new(data))).StatusCode);
        data["Tenhv"]="Học viên mới";
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/LyLich/sua/HV01","/LyLich/sua",new(data))).StatusCode);
        Assert.Equal(HttpStatusCode.OK,(await Post(c,"/Lop/them","/Lop/xoa",new(){["id"]="C01"})).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/MonHoc/them","/MonHoc/them",new(){["Msmh"]="M01",["Tenmh"]="MVC",["Sotiet"]="45"})).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/DiemThi/them","/DiemThi/them",new(){["Mshv"]="HV01",["Msmh"]="M01",["Diem"]="8.5"})).StatusCode);
        Assert.Equal(HttpStatusCode.OK,(await Post(c,"/LyLich/them","/LyLich/xoa",new(){["id"]="HV01"})).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/DiemThi/xoa/HV01?mon=M01","/DiemThi/xoa",new(){["id"]="HV01",["mon"]="M01"})).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/LyLich/them","/LyLich/xoa",new(){["id"]="HV01"})).StatusCode);
    }

    [Theory]
    [InlineData("LoaiHangHoa", "Maloai", "Tenloai")]
    [InlineData("NhanVien", "Manv", "Tennv")]
    [InlineData("KhachHang", "Makh", "Tenkh")]
    [InlineData("HangHoa", "Mahang", "Tenhang")]
    public async Task OtherModulesCanCreateEditAndDelete(string controller, string key, string name) {
        using var app = new LabApp(); using var c = app.Start();
        var data = new Dictionary<string,string> { [key]="TEST", [name]="Mẫu mới" };
        if (controller is "NhanVien" or "KhachHang") {
            data["Phai"]="true";data["Diachi"]="TP.HCM";data["Password"]="test";
            if(controller=="NhanVien") data["Ngaysinh"]="2000-01-01";
            else { data["Namsinh"]="2000";data["Dienthoai"]="0901234567"; }
        }
        if (controller=="HangHoa") { data["Donvitinh"]="Cái";data["Dongia"]="10000";data["Maloai"]="L01";data["Mansx"]="NSX01"; }
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,$"/{controller}/them",$"/{controller}/them",new(data))).StatusCode);
        Assert.Equal(HttpStatusCode.OK,(await Post(c,$"/{controller}/them",$"/{controller}/them",new(data))).StatusCode);
        data[name]="Đã sửa";
        if(data.ContainsKey("Password")) data["Password"]="";
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,$"/{controller}/sua/TEST",$"/{controller}/sua",new(data))).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,$"/{controller}/xoa/TEST",$"/{controller}/xoa",new(){["id"]="TEST"})).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,(await c.GetAsync($"/{controller}/sua/TEST")).StatusCode);
    }
    [Fact]
    public async Task AnonymousCartItemCanBeRemoved() {
        using var app = new LabApp(); using var c = app.Start();
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/","/Home/chonHanghoa",new(){["id"]="H01",["txtSoluong"]="1"})).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect,(await Post(c,"/Home/GioHang","/Home/XoaKhoiGio",new(){["id"]="H01"})).StatusCode);
        Assert.DoesNotContain("<td>H01</td>",await c.GetStringAsync("/Home/GioHang"));
    }

    [Fact]
    public void SqlServerSchemasCanBeGenerated() {
        using var sales = new QLBHContext(new DbContextOptionsBuilder<QLBHContext>().UseSqlServer("Server=localhost;Database=QLBH;Integrated Security=True").Options);
        using var students = new QLHVContext(new DbContextOptionsBuilder<QLHVContext>().UseSqlServer("Server=localhost;Database=QLHV;Integrated Security=True").Options);
        var sql=sales.Database.GenerateCreateScript();var sql2=students.Database.GenerateCreateScript();
        Assert.DoesNotContain("ON DELETE CASCADE",sql);Assert.Contains("[tenhang] nvarchar(50) NULL",sql);Assert.Contains("CREATE TABLE [hanghoa]",sql);Assert.Contains("CREATE TABLE [lylich]",sql2);
        var output=Environment.GetEnvironmentVariable("LAB_SCHEMA_OUTPUT");
        if(output!=null) { Directory.CreateDirectory(output);File.WriteAllText(Path.Combine(output,"QLBH.schema.sql"),sql);File.WriteAllText(Path.Combine(output,"QLHV.schema.sql"),sql2); }
    }
}
