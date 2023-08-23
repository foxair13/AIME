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

using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Mvc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<SmtpParam>(builder.Configuration.GetSection("SmtpParam"));
builder.Services.Configure<Connections>(builder.Configuration.GetSection("Connections"));
var connections =builder.Configuration.GetSection("Connections").Get<Connections>();
var baseConnectionString = connections.BasePostgree;
var financeConnectionString = connections.FinanceMssql;
string CoordsUrl = connections.CoordsUrl;
builder.Services.AddDbContext<NeftViewerContext>(options =>
              options.UseNpgsql(baseConnectionString, b => b.MigrationsAssembly("NeftViewer.MVC")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<IGenericRepository<AspNetUser>, AspNetUsersRepository>();
builder.Services.AddScoped<IGenericRepository<NeftViewer.Data.Models.Action>, ActionRepository>();
builder.Services.AddScoped<IGenericRepository<ActionRole>, ActionRoleRepository>();
builder.Services.AddScoped<IGenericRepository<Criteria>, CriteriaRepository>();

builder.Services.AddScoped<IGenericRepository<ObjectItem>, ObjectItemRepository>();
builder.Services.AddScoped<IGenericRepository<Road>, RoadRepository>();
builder.Services.AddScoped<IGenericRepository<IndicatorValue>, IndicatorValueRepository>();
builder.Services.AddScoped<IGenericRepository<ObjectOnRoad>, ObjectOnRoadRepository>();
builder.Services.AddScoped<IGenericRepository<Owner>, OwnerRepository>();
builder.Services.AddScoped<IGenericRepository<Area>, AreaRepository>();
builder.Services.AddScoped<IGenericRepository<Locality>, LocalityRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAspNetUsersService, AspNetUsersService>();
builder.Services.AddScoped<IActionService, ActionService>();
builder.Services.AddScoped<ICriteriaService, CriteriaService>();
builder.Services.AddScoped<IObjectItemService, ObjectItemService>();
builder.Services.AddScoped<IRoadService, RoadService>();
builder.Services.AddScoped<IIndicatorValueService, IndicatorValueService>();
builder.Services.AddScoped<IObjectOnRoadService, ObjectOnRoadService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<ILocalityService, LocalityService>();
builder.Services.AddScoped<IActionRoleService, ActionRoleService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<CustomAuthorizeAttribute>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(baseConnectionString));
builder.Services.AddAutoMapper(typeof(TaskService));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddHostedService(serviceProvider =>
{
    var mapper = serviceProvider.GetRequiredService<IMapper>();
    var criteriaservice = serviceProvider.GetRequiredService<IServiceScopeFactory>();
    return new TaskService(mapper, financeConnectionString, baseConnectionString, criteriaservice, CoordsUrl);
});
AppConfig.Initialize(connections);
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
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        if (ctx.File.Name.EndsWith(".js"))
        {
            ctx.Context.Response.Headers["Content-Type"] = "application/javascript";
        }
    }
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
