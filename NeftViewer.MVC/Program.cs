using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using NeftViewer.Data.DataContext;
using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.Models;
using NeftViewer.Data.Repositories.Contracts;
using NeftViewer.Data.Repositories.EntityRepositories;
using NeftViewer.Data.UnitOfWork.Contracts;
using NeftViewer.Data.UnitOfWork;
using NeftViewer.MVC.Data;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.BL.Services;
using NeftViewer.Core.ActionFilters;
using Microsoft.AspNetCore.Identity.UI.Services;
using NeftViewer.MVC.Service;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.Configuration;
using NeftViewer.MVC.Options;
using NeftViewer.MVC;
using NeftViewer.MVC.FinanceModels;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<SmtpParam>(builder.Configuration.GetSection("SmtpParam"));
builder.Services.Configure<Connections>(builder.Configuration.GetSection("Connections"));
var connections =builder.Configuration.GetSection("Connections").Get<Connections>();
var connectionString = connections.BasePostgree;
var FinanceconnectionString = connections.FinanceMssql;
builder.Services.AddDbContext<NeftViewerContext>(options =>
              options.UseNpgsql(connectionString, b => b.MigrationsAssembly("NeftViewer.MVC")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IGenericRepository<AspNetUser>, AspNetUsersRepository>();
builder.Services.AddScoped<IGenericRepository<NeftViewer.Data.Models.Action>, ActionRepository>();
builder.Services.AddScoped<IGenericRepository<ActionRole>, ActionRoleRepository>();
builder.Services.AddScoped<IGenericRepository<Criteria>, CriteriaRepository>();
builder.Services.AddScoped<IGenericRepository<Road>, RoadRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAspNetUsersService, AspNetUsersService>();
builder.Services.AddScoped<IActionService, ActionService>();
builder.Services.AddScoped<ICriteriaService, CriteriaService>();
builder.Services.AddScoped<IRoadService, RoadService>();
builder.Services.AddScoped<IActionRoleService, ActionRoleService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<CustomAuthorizeAttribute>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
builder.Services.AddDbContext<FinanceViewerContext>(options =>
        options.UseSqlServer(FinanceconnectionString));
builder.Services.AddAutoMapper(typeof(TaskService));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddHostedService(serviceProvider =>
{
    var mapper = serviceProvider.GetRequiredService<IMapper>();
    var criteriaservice = serviceProvider.GetRequiredService<IServiceScopeFactory>();
    return new TaskService(mapper, FinanceconnectionString, connectionString, criteriaservice);
});
AppConfig.Initialize(connections);
builder.Services.AddResponseCaching();
builder.Services.AddRazorPages();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var neftViewerDbContext = new NeftViewerContext(connectionString);
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
