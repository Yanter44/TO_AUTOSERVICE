using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Globalization;
using System.Text;
using ToMainApi.Common;
using ToMainApi.DbContext;
using ToMainApi.Features.Support;
using ToMainApi.Interfaces;
using ToMainApi.Middlewares;
using ToMainApi.Services;
using ToMainApi.Services.AI;
using ToMainApi.Services.BackgorundServices;
using ToMainApi.Services.Cache;
using ToMainApi.Services.Crypt;
using ToMainApi.Services.Mail;
using ToMainApi.Services.Notifications;
using Shared.ApplicationContracts;
using ToMainApi.Services.RabbitMq.Consumers;

var builder = WebApplication.CreateBuilder(args);
var culture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

builder.Services.AddControllers();
var key = builder.Configuration["Jwt:Key"];
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience, 

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["jwt"];
                if (string.IsNullOrEmpty(token))
                {
                    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                    if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                    {
                        token = authHeader.Substring("Bearer ".Length);
                    }
                }
                context.Token = token;
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToMainApi",
        Version = "v1"
    });

    options.AddSecurityDefinition("Token", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Description = "Введите JWT токен"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Token"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost4200",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors.Select(x =>
                string.IsNullOrWhiteSpace(x.ErrorMessage) ? "Некорректное значение" : x.ErrorMessage))
            .Distinct()
            .ToList();
        var response = new ServiceResponse<object>
        {
            Success = false,
            Message = errors.Count == 0 ? "Некорректные данные запроса" : string.Join("; ", errors),
            Data = null
        };
        return new BadRequestObjectResult(response);
    };
});
//builder.Services.AddMassTransit(x =>
//{
//    x.AddConsumer<DiagnosticCardConsumer>();
//    x.AddConsumer<ApplicationRejectedConsumer>();

//    x.UsingRabbitMq((context, cfg) =>
//    {
//        cfg.Host("localhost", "/", h =>
//        {
//            h.Username("guest");
//            h.Password("guest");
//        });
//        cfg.ConfigureEndpoints(context);
//    });
//});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration["ToServiceDb:ConnectionString"]));
builder.Services.AddAuthorization();

var redisconnectionstring = builder.Configuration["Redis:ConnectionString"];
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(redisconnectionstring);
});
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddSingleton<IInfisicalService, InfisicalService>();

builder.Services.AddHttpClient<IEmailService, MailerSendService>();
builder.Services.AddTransient<IEncryptService, AesEncryptionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAgentService, AgentService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddHttpClient<ICloudinaryService, CloudinaryService>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);
});
builder.Services.AddScoped<IModeratorService, ModeratorService>();
builder.Services.AddScoped<IPromptService, PromptService>();
builder.Services.AddScoped<IPtoService, PtoService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPhotoUploadRequirementService, PhotoUploadRequirementService>();
builder.Services.AddScoped<IDocumentUploadRequirementService, DocumentUploadRequirementService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IApplicationNotificationService, ApplicationNotificationsService>();
builder.Services.AddScoped<IPaymentNotificationService, PaymentsNotificationsService>();
builder.Services.AddScoped<IApplicationChargeService, ApplicationChargeService>();
builder.Services.AddScoped<IPhotoGalleryService, PhotoGalleryService>();
builder.Services.AddScoped<IinvitationService, InvitationService>();
builder.Services.AddScoped<IRefferalService, RefferalService>();
builder.Services.AddScoped<IArchiveCryptoService, ArchiveCryptoService>();

builder.Services.AddScoped<INotificationsSender,NotificationSender>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<INeuronNetworkService, NeuronNetworkService>();
builder.Services.AddHttpClient<INeuronNetwork, RouteAiProvider>(client =>
{
    client.Timeout = TimeSpan.FromMinutes(5);
});

builder.Services.AddSingleton<IUserBlockCache, UserBlockCache>();
builder.Services.AddTransient<IimageMetadataEditor, ImageMetadataEditorService>();
builder.Services.AddTransient<ICoordinateFormatConverterService, CoordinateFormatConverter>();
builder.Services.AddTransient<IimageTextOverlayService, ImageTextOverlayService>();
builder.Services.AddTransient<IApplicationPhotoService, ApplicationPhotoService>();
builder.Services.AddTransient<IImageValidator, ImageValidator>();

builder.Services.AddHostedService<RefreshTokenCleanupService>();
//builder.Services.AddHostedService<PhotoProcessorForApplicationsService>();
//builder.Services.AddHostedService<ApplicationPublisherWorker>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddSignalR();

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
});

var app = builder.Build();
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger("GlobalException");
        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        if (error != null)
            logger.LogError(error, "Необработанная ошибка");

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new ServiceResponse<object>
        {
            Success = false,
            Message = ResponseMessages.Error
        });
    });
});
await DbInitializer.SeedAdminAsync(app);
await DbInitializer.SeedVehicleCategories(app);
app.UseCors("AllowLocalhost4200");


app.UseAuthentication();
app.UseMiddleware<BlockedUserMiddleware>();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    Console.WriteLine($"{DateTime.Now:HH:mm:ss} → {context.Request.Method} {context.Request.Path}");
    await next();
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapHub<NotificationHub>("/notificationHub");
app.MapControllers();

app.Run();
