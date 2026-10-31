using Microsoft.EntityFrameworkCore;
using WebTest.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.Cookie.Name = "QLBH";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(60);
});
builder.Services.AddDbContext<QLBHContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("QLBH")));
builder.Services.AddDbContext<QLHVContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("QLHV")));
var app = builder.Build();
if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Home/Error");
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
public partial class Program { }
