using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Application.Services;
using AI.SocialNetwork.Domain.Entities;
using AI.SocialNetwork.Infrastructure.DataContext;
using AI.SocialNetwork.Infrastructure.Repositories;
using AI.SocialNetwork.Infrastructure.Repositories.Contracts;
using AI.SocialNetwork.Infrastructure.UnitOfWork;
using AI.SocialNetwork.Infrastructure.UnitOfWork.Contracts;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AI.SocialNetwork.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AISocialNetwork") ??
                       builder.Configuration.GetSection("Connections:BasePostgree").Value;

builder.Services.AddDbContext<AISocialNetworkContext>(options =>
    options.UseNpgsql(connectionString, b => b.MigrationsAssembly("AI.SocialNetwork.Web")),
    ServiceLifetime.Scoped);

// Репозитории (паттерн NeftViewer)
builder.Services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
builder.Services.AddScoped<IGenericRepository<Agent>, GenericRepository<Agent>>();
builder.Services.AddScoped<IGenericRepository<AgentMessage>, GenericRepository<AgentMessage>>();
builder.Services.AddScoped<IGenericRepository<Connection>, GenericRepository<Connection>>();
builder.Services.AddScoped<IGenericRepository<Portfolio>, GenericRepository<Portfolio>>();
builder.Services.AddScoped<IGenericRepository<ReputationEntry>, GenericRepository<ReputationEntry>>();
builder.Services.AddScoped<IGenericRepository<Skill>, GenericRepository<Skill>>();
builder.Services.AddScoped<IGenericRepository<SkillClosure>, GenericRepository<SkillClosure>>();
builder.Services.AddScoped<IGenericRepository<SkillCategory>, GenericRepository<SkillCategory>>();
builder.Services.AddScoped<IGenericRepository<UserSkill>, GenericRepository<UserSkill>>();
builder.Services.AddScoped<IGenericRepository<ProfessionalRole>, GenericRepository<ProfessionalRole>>();
builder.Services.AddScoped<IGenericRepository<RoleSkillWeight>, GenericRepository<RoleSkillWeight>>();
builder.Services.AddScoped<IGenericRepository<Attestation>, GenericRepository<Attestation>>();
builder.Services.AddScoped<IGenericRepository<DevelopmentPlan>, GenericRepository<DevelopmentPlan>>();
builder.Services.AddScoped<IGenericRepository<PlanStep>, GenericRepository<PlanStep>>();
builder.Services.AddScoped<IGenericRepository<Organization>, GenericRepository<Organization>>();
builder.Services.AddScoped<IGenericRepository<Post>, GenericRepository<Post>>();
builder.Services.AddScoped<IGenericRepository<Group>, GenericRepository<Group>>();
builder.Services.AddScoped<IGenericRepository<Membership>, GenericRepository<Membership>>();
builder.Services.AddScoped<IGenericRepository<GroupPermission>, GenericRepository<GroupPermission>>();
builder.Services.AddScoped<IGenericRepository<Forum>, GenericRepository<Forum>>();
builder.Services.AddScoped<IGenericRepository<ForumCategory>, GenericRepository<ForumCategory>>();
builder.Services.AddScoped<IGenericRepository<ForumPermission>, GenericRepository<ForumPermission>>();
builder.Services.AddScoped<IGenericRepository<ForumTopic>, GenericRepository<ForumTopic>>();
builder.Services.AddScoped<IGenericRepository<Subscription>, GenericRepository<Subscription>>();
builder.Services.AddScoped<IGenericRepository<Meeting>, GenericRepository<Meeting>>();
builder.Services.AddScoped<IGenericRepository<Deal>, GenericRepository<Deal>>();
builder.Services.AddScoped<IGenericRepository<DealMilestone>, GenericRepository<DealMilestone>>();
builder.Services.AddScoped<IGenericRepository<Board>, GenericRepository<Board>>();
builder.Services.AddScoped<IGenericRepository<BoardColumn>, GenericRepository<BoardColumn>>();
builder.Services.AddScoped<IGenericRepository<BoardCard>, GenericRepository<BoardCard>>();
builder.Services.AddScoped<IGenericRepository<UserFile>, GenericRepository<UserFile>>();
builder.Services.AddScoped<IGenericRepository<Conversation>, GenericRepository<Conversation>>();
builder.Services.AddScoped<IGenericRepository<ConversationMember>, GenericRepository<ConversationMember>>();
builder.Services.AddScoped<IGenericRepository<Message>, GenericRepository<Message>>();
builder.Services.AddScoped<IGenericRepository<Notification>, GenericRepository<Notification>>();
builder.Services.AddScoped<IGenericRepository<Report>, GenericRepository<Report>>();
builder.Services.AddScoped<IGenericRepository<Review>, GenericRepository<Review>>();
builder.Services.AddScoped<IGenericRepository<KYCRequest>, GenericRepository<KYCRequest>>();
builder.Services.AddScoped<IGenericRepository<ConsentRecord>, GenericRepository<ConsentRecord>>();
builder.Services.AddScoped<IGenericRepository<AuditLogEntry>, GenericRepository<AuditLogEntry>>();
builder.Services.AddScoped<IGenericRepository<SubscriptionPlan>, GenericRepository<SubscriptionPlan>>();
builder.Services.AddScoped<IGenericRepository<PlanSubscriber>, GenericRepository<PlanSubscriber>>();
builder.Services.AddScoped<IGenericRepository<Donation>, GenericRepository<Donation>>();
builder.Services.AddScoped<IGenericRepository<ReferralLink>, GenericRepository<ReferralLink>>();
builder.Services.AddScoped<IGenericRepository<Invoice>, GenericRepository<Invoice>>();
builder.Services.AddScoped<IGenericRepository<RefundRequest>, GenericRepository<RefundRequest>>();
builder.Services.AddScoped<IGenericRepository<WebhookSubscription>, GenericRepository<WebhookSubscription>>();
builder.Services.AddScoped<IGenericRepository<AgentPermission>, GenericRepository<AgentPermission>>();
builder.Services.AddScoped<IGenericRepository<AgentActionLog>, GenericRepository<AgentActionLog>>();
builder.Services.AddScoped<IGenericRepository<AnalyticsEvent>, GenericRepository<AnalyticsEvent>>();
builder.Services.AddScoped<IGenericRepository<Language>, GenericRepository<Language>>();
builder.Services.AddScoped<IGenericRepository<UserLanguage>, GenericRepository<UserLanguage>>();
builder.Services.AddScoped<IGenericRepository<Translation>, GenericRepository<Translation>>();
builder.Services.AddScoped<IGenericRepository<ContentLanguage>, GenericRepository<ContentLanguage>>();
builder.Services.AddScoped<IGenericRepository<OutboxMessage>, GenericRepository<OutboxMessage>>();
builder.Services.AddScoped<IGenericRepository<Hashtag>, GenericRepository<Hashtag>>();
builder.Services.AddScoped<IGenericRepository<PostHashtag>, GenericRepository<PostHashtag>>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISearchRepository, SearchRepository>();

// Сервисы
builder.Services.AddHttpClient("ollama");
builder.Services.AddSingleton(sp =>
{
    var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("ollama");
    http.Timeout = TimeSpan.FromSeconds(120);
    return new OllamaClient(http, "http://localhost:11434");
});
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ICompetenceService, CompetenceService>();
builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddScoped<ISocialService, SocialService>();
builder.Services.AddScoped<IDealService, DealService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IReputationService, ReputationService>();
builder.Services.AddScoped<IMathService, MathService>();
builder.Services.AddScoped<IDynamicFitService, DynamicFitService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IModerationService, OllamaModerationService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<IReferralService, ReferralService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IWebhookService, WebhookService>();
builder.Services.AddSingleton(builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Избегаем циклических ссылок (Agent -> Messages -> Agent), корректно сериализуем граф
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddProblemDetails();
builder.Services.AddSignalR();
builder.Services.AddHttpClient("outbox");
builder.Services.AddHostedService<OutboxProcessor>();
// Kafka: публикатор событий (graceful fallback при недоступном брокере) + потребитель
builder.Services.AddSingleton<IEventPublisher, KafkaEventPublisher>();
builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler(errorApp =>
{
    // B7: ProblemDetails для всех ошибок + маппинг бизнес-исключений (403)
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var status = exception switch
        {
            UnauthorizedAccessException => StatusCodes.Status403Forbidden,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new
        {
            type = $"https://tools.ietf.org/html/rfc9110#section-15.{status switch
            {
                403 => "5.4",
                404 => "5.4",
                _ => "6.1"
            }}",
            title = status switch
            {
                403 => "Forbidden",
                404 => "Not Found",
                _ => "An error occurred while processing your request."
            },
            status,
            detail = exception?.Message
        });
    });
});
app.UseStatusCodePages();
app.UseCors("AllowReactClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<AI.SocialNetwork.Web.Hubs.NotificationsHub>("/hubs/notifications");

app.Run();