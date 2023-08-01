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
using NeftViewer.BL.Services;
using NeftViewer.Core.ActionFilters;
using Microsoft.AspNetCore.Identity.UI.Services;
using NeftViewer.MVC.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("NeftViewerContext");
builder.Services.AddDbContext<NeftViewerContext>(options =>
              options.UseNpgsql(connectionString, b => b.MigrationsAssembly("NeftViewer.MVC")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IGenericRepository<AspNetUsers>, AspNetUsersRepository>();
builder.Services.AddScoped<IGenericRepository<NeftViewer.Data.Models.Action>, ActionRepository>();
builder.Services.AddScoped<IGenericRepository<ActionRole>, ActionRoleRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAspNetUsersService, AspNetUsersService>();
builder.Services.AddScoped<IActionService, ActionService>();
builder.Services.AddScoped<IActionRoleService, ActionRoleService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<CustomAuthorizeAttribute>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//          .AddCookie(options =>
//          {
//              options.LoginPath = "/Identity/Account/Login"; // ������� URL �������� �����
//          });

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
