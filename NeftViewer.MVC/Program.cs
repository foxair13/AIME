using Microsoft.AspNetCore.Identity;
using NeftViewer.Data.DataContext;
using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.Models;
using NeftViewer.Data.Repositories.Contracts;
using NeftViewer.Data.Repositories.EntityRepositories;
using NeftViewer.Data.UnitOfWork.Contracts;
using NeftViewer.Data.UnitOfWork;
using NeftViewer.MVC.Data;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.BL;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("NeftViewerContext");
builder.Services.AddDbContext<NeftViewerContext>(options =>
              options.UseNpgsql(connectionString, b => b.MigrationsAssembly("NeftViewer.MVC")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IGenericRepository<AspNetUsers>, AspNetUsersRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAspNetUsersService, AspNetUsersService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//          .AddCookie(options =>
//          {
//              options.LoginPath = "/Identity/Account/Login"; // ������� URL �������� �����
//          });
builder.Services.AddControllersWithViews();
builder.Services.AddResponseCaching();

builder.Services.AddRazorPages();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var neftViewerDbContext = scope.ServiceProvider.GetRequiredService<NeftViewerContext>();
    neftViewerDbContext.Database.EnsureCreated();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
