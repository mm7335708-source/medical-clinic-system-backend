using MedicalClinicSystem.Application.Services.Interfaces;
using MedicalClinicSystem.Domain.Entities.Identity;
using MedicalClinicSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MedicalClinicSystem.Tests
{
    public class TestAppFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = $"mcs-tests-{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(AppDbContext));
                services.RemoveAll(typeof(IDbInitializer));

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });

                services.AddScoped<IDbInitializer, NoopDbInitializer>();

                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                Seed(db, scope.ServiceProvider.GetRequiredService<IPasswordHasherService>());
            });
        }

        private static void Seed(AppDbContext db, IPasswordHasherService passwordHasher)
        {
            var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            var adminExists = db.Users.Any(x => x.UserName == "admin" && !x.IsDeleted);
            if (adminExists)
            {
                return;
            }

            var adminUser = new User
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                FullName = "System Administrator",
                UserName = "admin",
                Email = "admin@clinic.local",
                PhoneNumber = "0000000000",
                PasswordHash = passwordHasher.HashPassword("Admin@123"),
                RoleId = adminRoleId,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(adminUser);
            db.SaveChanges();
        }
    }

    internal class NoopDbInitializer : IDbInitializer
    {
        public Task InitializeAsync() => Task.CompletedTask;
    }
}
