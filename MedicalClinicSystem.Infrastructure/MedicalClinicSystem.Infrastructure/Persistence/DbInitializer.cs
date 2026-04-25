using MedicalClinicSystem.Application.Services.Interfaces;
using MedicalClinicSystem.Domain.Entities;
using MedicalClinicSystem.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MedicalClinicSystem.Infrastructure.Persistence
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(
            AppDbContext context,
            IServiceProvider serviceProvider,
            ILogger<DbInitializer> logger)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            await _context.Database.MigrateAsync();

            await SeedSpecialtiesAsync();
            await SeedClinicsAsync();
            await SeedAdminUserAsync();
        }

        private async Task SeedSpecialtiesAsync()
        {
            if (await _context.Specialties.AnyAsync())
                return;

            var specialties = new List<Specialty>
            {
                new Specialty
                {
                    Name = "باطنية",
                    Description = "طب الباطنية"
                },
                new Specialty
                {
                    Name = "أسنان",
                    Description = "طب الأسنان"
                },
                new Specialty
                {
                    Name = "أطفال",
                    Description = "طب الأطفال"
                },
                new Specialty
                {
                    Name = "نسائية",
                    Description = "طب النسائية والتوليد"
                }
            };

            await _context.Specialties.AddRangeAsync(specialties);
            await _context.SaveChangesAsync();
        }

        private async Task SeedClinicsAsync()
        {
            if (await _context.Clinics.AnyAsync())
                return;

            var clinics = new List<Clinic>
            {
                new Clinic
                {
                    ClinicName = "عيادة الشفاء",
                    Address = "بغداد - الكرادة",
                    City = "بغداد",
                    PhoneNumber = "07700000001",
                    Latitude = 33.3152,
                    Longitude = 44.3661
                },
                new Clinic
                {
                    ClinicName = "عيادة النخبة",
                    Address = "بغداد - المنصور",
                    City = "بغداد",
                    PhoneNumber = "07700000002",
                    Latitude = 33.3029,
                    Longitude = 44.3449
                }
            };

            await _context.Clinics.AddRangeAsync(clinics);
            await _context.SaveChangesAsync();
        }

        private async Task SeedAdminUserAsync()
        {
            var adminRole = await _context.Roles
                .FirstOrDefaultAsync(x => x.Name == "Admin" && !x.IsDeleted);

            if (adminRole == null)
            {
                _logger.LogWarning("Role باسم Admin غير موجود. تأكد من Seed الخاص بـ Roles.");
                return;
            }

            var adminExists = await _context.Users
                .AnyAsync(x => x.UserName == "admin" && !x.IsDeleted);

            if (adminExists)
            {
                _logger.LogInformation("Admin user already exists.");
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();

            var adminUser = new User
            {
                Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                FullName = "System Administrator",
                UserName = "admin",
                Email = "admin@clinic.local",
                PhoneNumber = "0000000000",
                PasswordHash = passwordHasher.HashPassword("Admin@123"),
                RoleId = adminRole.Id,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(adminUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Default admin user has been created successfully.");
        }
    }
}