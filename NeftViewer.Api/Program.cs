using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.EntityFrameworkCore;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.BL.Services;
using NeftViewer.Core.ActionFilters;
using NeftViewer.Data.DataContext;
using NeftViewer.MVC;
using NeftViewer.MVC.Options;
using NeftViewer.Data.UnitOfWork.Contracts;
using NeftViewer.Data.UnitOfWork;
using NeftViewer.Data.Models;
using NeftViewer.Data.Repositories.Contracts;
using NeftViewer.Data.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using NeftViewer.Data.Repositories.EntityRepositories;
using NeftViewer.MVC.Service;
using NeftViewer.SV.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
CryptoService cryptoService = new CryptoService(builder.Configuration);
//builder.Configuration.AddJsonFile("api_appsettings.json", optional: false, reloadOnChange: true);
builder.Services.Configure<Connections>(cryptoService.HitConnectionsInConfig().GetSection("Connections"));
var attribute = (Prot)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(Prot));
var prot = "";
if (attribute != null)
{
    prot = attribute.P;
}
var connections = new Connections(prot)
{
    BasePostgree = cryptoService.GetConfiguration().GetSection("Connections:BasePostgree").Value,
};

var baseConnectionString = connections.BasePostgree;
builder.Services.AddDbContext<NeftViewerContext>(options =>
    options.UseNpgsql(baseConnectionString, b => b.MigrationsAssembly("NeftViewer.Api")),
    ServiceLifetime.Scoped);
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

builder.Services.AddScoped<CustomAuthorizeAttribute>();

AppConfig.Initialize(cryptoService, prot);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
