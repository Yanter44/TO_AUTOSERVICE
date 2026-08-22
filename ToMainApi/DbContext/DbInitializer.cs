using Microsoft.EntityFrameworkCore;
using ToMainApi.Interfaces;
using ToMainApi.Models.Entities;

namespace ToMainApi.DbContext
{
    public static class DbInitializer
    {
        public static async Task SeedAdminAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<AppDbContext>();
                var encryptService = services.GetRequiredService<IEncryptService>();
                var configuration = services.GetRequiredService<IConfiguration>();
                var authservice = services.GetRequiredService<IAuthService>();

                if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
                {
                    await dbContext.Database.MigrateAsync();
                }

                var adminSection = configuration.GetSection("AdminAccount");
                var adminEmail = adminSection["Email"];
                var adminPassword = adminSection["Password"];
                var adminFio = adminSection["FIO"] ?? "Администратор";
                var adminRole = adminSection["RoleType"] ?? "Admin";

                if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
                {
                    return;
                }
                var adminExists = await dbContext.Users.AnyAsync(u => u.Email == adminEmail);
                if (!adminExists)
                {
                    var encryptedPassword = encryptService.Encrypt(adminPassword);
                    var adminUser = new User
                    {
                        Email = adminEmail,
                        FIO = adminFio,
                        Password = encryptedPassword,
                        RoleType = adminRole,
                        RegDate = DateTime.UtcNow
                    };

                    adminUser.AdminProfile = new AdminProfile
                    {
                        User = adminUser
                    };
                    await dbContext.Users.AddAsync(adminUser);
                    await dbContext.SaveChangesAsync();
                    Console.WriteLine($"[SEED] Аккаунт администратора {adminEmail} и его AdminProfile успешно созданы.");
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<AppDbContext>>();
                logger.LogError(ex, "Произошла ошибка при инициализации учетной записи администратора.");
            }
        }
        public static async Task SeedVehicleCategories(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<AppDbContext>();
                var encryptService = services.GetRequiredService<IEncryptService>();
                var configuration = services.GetRequiredService<IConfiguration>();

                if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
                {
                    await dbContext.Database.MigrateAsync();
                }
                if (!await dbContext.VehicleCategories.AnyAsync())
                {
                    var listOfVehicleCategories = new List<VehicleCategory>()
                    {
                        new VehicleCategory { Name = "L", Description = "Мототранспортные средства" },
                        new VehicleCategory { Name = "M1", Description = "Легковые автомобили" },
                        new VehicleCategory { Name = "M1 (Такси)", Description = "Легковые автомобили (такси)" },
                        new VehicleCategory { Name = "M2", Description = "Автобусы до 5 тонн" },
                        new VehicleCategory { Name = "M3", Description = "Автобусы свыше 5 тонн" },
                        new VehicleCategory { Name = "N1", Description = "Грузовые автомобили до 3.5 тонн" },
                        new VehicleCategory { Name = "N2", Description = "Грузовые автомобили от 3.5 до 12 тонн" },
                        new VehicleCategory { Name = "N3", Description = "Грузовые автомобили свыше 12 тонн" },
                        new VehicleCategory { Name = "O1", Description = "Прицепы до 0.75 тонн" },
                        new VehicleCategory { Name = "O2", Description = "Прицепы от 0.75 до 3.5 тонн" },
                        new VehicleCategory { Name = "O3", Description = "Прицепы от 3.5 до 10 тонн" },
                        new VehicleCategory { Name = "O4", Description = "Прицепы свыше 10 тонн" }
                    };
                    await dbContext.VehicleCategories.AddRangeAsync(listOfVehicleCategories);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сидинге категорий ТС: {ex.Message}");
            }
        }
    }
}
