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
using System.Reflection;
using NeftViewer.SV.Services;

var builder = WebApplication.CreateBuilder(args);
CryptoService cryptoService = new CryptoService(builder.Configuration);
builder.Services.Configure<SmtpParam>(builder.Configuration.GetSection("SmtpParam"));
builder.Services.Configure<Connections>(cryptoService.HitConnectionsInConfig().GetSection("Connections"));
var attribute = (Prot)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(Prot));
var prot = "";
if (attribute!=null)
{
     prot = attribute.P;
}
var connections = new Connections(prot)
{
    BasePostgree = cryptoService.GetConfiguration().GetSection("Connections:BasePostgree").Value,
    FinanceMssql = cryptoService.GetConfiguration().GetSection("Connections:FinanceMssql").Value,
    CoordsUrl = cryptoService.GetConfiguration().GetSection("Connections:CoordsUrl").Value,
    AsuUrl = cryptoService.GetConfiguration().GetSection("Connections:AsuUrl").Value
};

var baseConnectionString = connections.BasePostgree;
var financeConnectionString = connections.FinanceMssql;
string CoordsUrl = connections.CoordsUrl;
var asuObjects = connections.AsuUrl;
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
builder.Services.AddScoped<IGenericRepository<Agregate>, AgregateRepository>();
builder.Services.AddScoped<IGenericRepository<CriteriaCalcMethod>, CriteriaCalcMethodRepository>();
builder.Services.AddScoped<IGenericRepository<Locality>, LocalityRepository>();
builder.Services.AddScoped<IGenericRepository<Trk>, TrkRepository>();
builder.Services.AddScoped<IGenericRepository<Tank>, TankRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAspNetUsersService, AspNetUsersService>();
builder.Services.AddScoped<IActionService, ActionService>();
builder.Services.AddScoped<ICriteriaCalcMethodService, CriteriaCalcMethodService>();
builder.Services.AddScoped<IAgregateService, AgregateService>();
builder.Services.AddScoped<ICriteriaService, CriteriaService>();
builder.Services.AddScoped<IObjectItemService, ObjectItemService>();
builder.Services.AddScoped<IRoadService, RoadService>();
builder.Services.AddScoped<IIndicatorValueService, IndicatorValueService>();
builder.Services.AddScoped<IObjectOnRoadService, ObjectOnRoadService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<ILocalityService, LocalityService>();
builder.Services.AddScoped<ITrkService, TrkService>();
builder.Services.AddScoped<ITankService, TankService>();
builder.Services.AddScoped<IActionRoleService, ActionRoleService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddHttpClient("GetAsuService", client =>
{
    client.BaseAddress = new Uri(asuObjects);
});

builder.Services.AddScoped<CustomAuthorizeAttribute>();
builder.Services.AddScoped<LogService>();
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
    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    return new TaskService(mapper, financeConnectionString, baseConnectionString, criteriaservice, CoordsUrl, httpClientFactory);
});
AppConfig.Initialize(cryptoService, prot);
builder.Services.AddResponseCaching();
builder.Services.AddRazorPages();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders(); 
    loggingBuilder.AddConsole();     
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var logService = scope.ServiceProvider.GetRequiredService<LogService>();
    logService.GETHID();
    var neftViewerDbContext = scope.ServiceProvider.GetRequiredService<NeftViewerContext>();
    neftViewerDbContext.Database.EnsureCreated();
}
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=31536000"; // Adjust max-age as needed
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
