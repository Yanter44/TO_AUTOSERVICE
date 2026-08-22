using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
using ToMainApi.DbContext;
using ToMainApi.Interfaces;
using ToMainApi.Services;
using ToMainApi.Services.AI;
using ToMainApi.Services.BackgorundServices;
using ToMainApi.Services.Crypt;
using ToMainApi.Services.Mail;
using ToMainApi.Services.Notifications;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
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
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowLocalhost8000",
//        policy =>
//        {
//            policy.WithOrigins("http://localhost:8000")
//                  .AllowAnyHeader()
//                  .AllowAnyMethod()
//                  .AllowCredentials();
//        });
//});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        foreach (var kv in context.ModelState)
        {
            Console.WriteLine($"Поле: {kv.Key}");

            foreach (var error in kv.Value.Errors)
            {
                Console.WriteLine($"Ошибка: {error.ErrorMessage}");

                if (error.Exception != null)
                    Console.WriteLine(error.Exception);
            }
        }
        return new BadRequestObjectResult(context.ModelState);
    };
});
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration["ToServiceDb:ConnectionString"]));
builder.Services.AddAuthorization();
var redisconnectionstring = builder.Configuration["Redis:ConnectionString"];
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(redisconnectionstring);
});
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddHttpClient<IEmailService, MailerSendService>();
builder.Services.AddTransient<IEncryptService, AesEncryptionService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IAgentService, AgentService>();
builder.Services.AddTransient<IAdminService, AdminService>();
builder.Services.AddHttpClient<ICloudinaryService, CloudinaryService>();
builder.Services.AddTransient<IModeratorService, ModeratorService>();
builder.Services.AddTransient<IPromptService, PromptService>();
builder.Services.AddTransient<IPtoService, PtoService>();
builder.Services.AddTransient<IApplicationService, ApplicationService>();
builder.Services.AddTransient<IVehicleService, VehicleService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IPhotoUploadRequirementService, PhotoUploadRequirementService>();
builder.Services.AddTransient<IDocumentUploadRequirementService, DocumentUploadRequirementService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<INotificationSender, NotificationSender>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddHttpClient<INeuronNetworkStrategy,NanoBananaAiProvider>();
builder.Services.AddHttpClient<INeuronNetworkStrategy,ReveAiProvider>();
builder.Services.AddScoped<NeuronNetworkDispatcher>();
builder.Services.AddTransient<IimageMetadataEditor, ImageMetadataEditorService>();
builder.Services.AddTransient<ICoordinateFormatConverterService, CoordinateFormatConverter>();
builder.Services.AddTransient<IimageTextOverlayService, ImageTextOverlayService>();
builder.Services.AddTransient<IApplicationPhotoService, ApplicationPhotoService>();
builder.Services.AddTransient<IImageValidator, ImageValidator>();
builder.Services.AddHostedService<RefreshTokenCleanupService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddSignalR();

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command",LogLevel.None);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
});
var app = builder.Build();

await DbInitializer.SeedAdminAsync(app);
await DbInitializer.SeedVehicleCategories(app);
//app.UseCors("AllowLocalhost8000");

app.MapHub<NotificationHub>("/notificationHub");

app.UseAuthentication();
app.UseAuthorization();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.MapControllers();

app.Run();
