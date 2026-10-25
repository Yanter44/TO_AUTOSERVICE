using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToMainApi.Interfaces;
using ToMainApi.Models.Entities;
using ToMainApi.Models.Enums;
using ToMainApi.Services.Crypt;

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
                if (!Enum.TryParse<Role>(adminRole, ignoreCase: true, out var adminRolee))
                {
                    Console.WriteLine($"[SEED] Неизвестная роль в конфиге: '{adminRole}'. Ожидается: Admin, Moderator или Agent.");
                    return;
                }
                var adminExists = await dbContext.Users.AnyAsync(u => u.Email == adminEmail);
                if (!adminExists)
                {
                    var encryptedPassword = PasswordHasher.Hash(adminPassword);
                    var adminUser = new User
                    {
                        Email = adminEmail,
                        FIO = adminFio,
                        Password = encryptedPassword,
                        RoleType = adminRolee,
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
                var configuration = services.GetRequiredService<IConfiguration>();

                if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
                {
                    await dbContext.Database.MigrateAsync();
                }
                if (!await dbContext.VehicleCategoriesBySrts.AnyAsync())
                {
                    var listOfVehicleCategoriesBySrts = new List<VehicleCategoryBySrts>()
                    {
                        new VehicleCategoryBySrts { Name = "A", Description = "Мототранспортные средства" },
                        new VehicleCategoryBySrts { Name = "B", Description = "Легковые автомобили" },
                        new VehicleCategoryBySrts { Name = "C", Description = "Грузовые автомобили"},
                        new VehicleCategoryBySrts { Name = "D", Description = "Автобусы"},
                        new VehicleCategoryBySrts { Name = "E", Description = "Прицепы и полуприцепы"}
                    };
                    await dbContext.VehicleCategoriesBySrts.AddRangeAsync(listOfVehicleCategoriesBySrts);
                    await dbContext.SaveChangesAsync();
                }
                if(!await dbContext.VehicleCategoriesByOkp.AnyAsync())
                {
                    var listOfVehicleCategoriesByOkp = new List<VehicleCategoryByOkp>()
                    {
                        new VehicleCategoryByOkp {Name = "L", Description = "Мототранспортные средства"},
                        new VehicleCategoryByOkp { Name = "M1", Description = "Транспортные средства, используемые для перевозки пассажирова и имеющие помимо места водителя, не более восьми мест для сидения"},
                        new VehicleCategoryByOkp { Name = "M2", Description = "Траспортные средства, используемые помимо места водителя, более восьми мест для сидения, технически допустимая масса которых не превышает 5 тонн" },
                        new VehicleCategoryByOkp { Name = "M3", Description = "Транспортные средства, используемые для перевозки пассажиров, имеющие, помимо места водителя, более восьми мест для сидения, технически допустимая масса, которых превышает 5 тонн"},
                        new VehicleCategoryByOkp { Name = "N1", Description = "Транспортные средства, предназначенные для перевозки грузов, имеющие технически допустимую максимальную массу не более 3,5 тонн"},
                        new VehicleCategoryByOkp { Name = "N2", Description = "Транспортные средства, предназначенные для перевозки грузов, имеющие технически допустимую максимальную массу свыше 3,5 тонн, но не более 12 тонн"},
                        new VehicleCategoryByOkp { Name = "N3", Description = "Транспортные средства, предназначенные для перевозки грузов, имебщие технически допустимую максимальную массу более 12 тонн" },
                        new VehicleCategoryByOkp { Name = "O1", Description = "Прицепы, технически допустимая максимальная масса, которых не более 0,75 тонн." },
                        new VehicleCategoryByOkp { Name = "O2", Description = "Прицепы, технически допустимая максимальная масса, которых свыше 0,75 тонн, но не более 3,5 тонн"},
                        new VehicleCategoryByOkp { Name = "O3", Description = "Прицепы, технически допустимая максимальная масса, которых, свыше 3,5 тонн, но не более 10,0 тонн"},
                        new VehicleCategoryByOkp { Name = "O4", Description = "Прицепы технически допустимая максимальная масса, свыше 10,0 тонн"},

                    };
                    await dbContext.VehicleCategoriesByOkp.AddRangeAsync(listOfVehicleCategoriesByOkp);
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
