using AutoMapper;
using FluentValidation;
using MedicalClinicSystem.Application.DTOs.Common;
using MedicalClinicSystem.Application.DTOs.Identity;
using MedicalClinicSystem.Application.Exceptions;
using MedicalClinicSystem.Application.Interfaces.Persistence;
using MedicalClinicSystem.Application.Services.Interfaces;
using MedicalClinicSystem.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppValidationException = MedicalClinicSystem.Application.Exceptions.ValidationException;

namespace MedicalClinicSystem.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IValidator<CreateUserRequestDto> _createValidator;
        private readonly IValidator<UpdateUserRequestDto> _updateValidator;
        private readonly IValidator<UpdateUserStatusRequestDto> _updateStatusValidator;
        private readonly IValidator<ChangePasswordRequestDto> _changePasswordValidator;
        private readonly IValidator<ResetPasswordRequestDto> _resetPasswordValidator;
        private readonly IPasswordHasherService _passwordHasherService;

        public UserService(
            IApplicationDbContext context,
            IMapper mapper,
            ILogger<UserService> logger,
            IValidator<CreateUserRequestDto> createValidator,
            IValidator<UpdateUserRequestDto> updateValidator,
            IValidator<UpdateUserStatusRequestDto> updateStatusValidator,
            IValidator<ChangePasswordRequestDto> changePasswordValidator,
            IValidator<ResetPasswordRequestDto> resetPasswordValidator,
            IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _updateStatusValidator = updateStatusValidator;
            _changePasswordValidator = changePasswordValidator;
            _resetPasswordValidator = resetPasswordValidator;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<UserResponseDto> CreateAsync(CreateUserRequestDto request)
        {
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new AppValidationException(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            await EnsureRoleExistsAsync(request.RoleId);
            await EnsureDoctorLinkIsValidAsync(request.RoleId, request.DoctorId);
            await EnsureUserNameIsUniqueAsync(request.UserName);
            await EnsureEmailIsUniqueAsync(request.Email);

            var user = _mapper.Map<User>(request);
            user.PasswordHash = _passwordHasherService.HashPassword(request.Password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("تم إنشاء مستخدم جديد بنجاح. UserId: {UserId}", user.Id);

            return await GetByIdAsync(user.Id);
        }

        public async Task<UserResponseDto> UpdateAsync(Guid id, UpdateUserRequestDto request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new AppValidationException(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (user == null)
            {
                throw new NotFoundException("المستخدم غير موجود.");
            }

            await EnsureNotDemotingLastAdminAsync(user.RoleId, request.RoleId, user.IsActive);
            await EnsureRoleExistsAsync(request.RoleId);
            await EnsureDoctorLinkIsValidAsync(request.RoleId, request.DoctorId);
            await EnsureUserNameIsUniqueAsync(request.UserName, id);
            await EnsureEmailIsUniqueAsync(request.Email, id);

            _mapper.Map(request, user);

            // Keep the model consistent: only Doctor users can have a DoctorId.
            var doctorRoleId = await GetDoctorRoleIdAsync();
            if (doctorRoleId == Guid.Empty || user.RoleId != doctorRoleId)
            {
                user.DoctorId = null;
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("تم تحديث المستخدم بنجاح. UserId: {UserId}", user.Id);

            return await GetByIdAsync(user.Id);
        }

        public async Task<UserResponseDto> GetByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (user == null)
            {
                throw new NotFoundException("المستخدم غير موجود.");
            }

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            var users = await _context.Users
                .Where(x => !x.IsDeleted)
                .Include(x => x.Role)
                .OrderBy(x => x.FullName)
                .ToListAsync();

            return _mapper.Map<List<UserResponseDto>>(users);
        }

        public async Task<PagedResultDto<UserResponseDto>> GetPagedAsync(PaginationRequestDto request)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var query = _context.Users
                .Where(x => !x.IsDeleted)
                .Include(x => x.Role)
                .OrderBy(x => x.FullName)
                .AsQueryable();

            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = _mapper.Map<List<UserResponseDto>>(users);

            return new PagedResultDto<UserResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<UserResponseDto> UpdateStatusAsync(Guid id, UpdateUserStatusRequestDto request)
        {
            var validationResult = await _updateStatusValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new AppValidationException(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (user == null)
            {
                throw new NotFoundException("المستخدم غير موجود.");
            }

            if (user.IsActive && request.IsActive == false)
            {
                await EnsureNotDisablingLastAdminAsync(user.RoleId);
            }

            user.IsActive = request.IsActive!.Value;
            user.UpdatedAt = DateTime.UtcNow;

            if (!user.IsActive)
            {
                await RevokeActiveRefreshTokensAsync(user.Id);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "تم تحديث حالة المستخدم. UserId: {UserId}, IsActive: {IsActive}",
                user.Id,
                user.IsActive);

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> GetCurrentAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (user == null)
            {
                throw new NotFoundException("المستخدم غير موجود.");
            }

            await EnsureNotDeletingLastAdminAsync(user.RoleId, user.IsActive);

            user.IsDeleted = true;
            user.IsActive = false;
            user.DeletedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await RevokeActiveRefreshTokensAsync(user.Id);

            await _context.SaveChangesAsync();

            _logger.LogInformation("تم حذف المستخدم منطقيًا. UserId: {UserId}", user.Id);
        }

        public async Task ChangePasswordAsync(Guid id, ChangePasswordRequestDto request)
        {
            var validationResult = await _changePasswordValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new AppValidationException(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (user == null)
            {
                throw new NotFoundException("المستخدم غير موجود.");
            }

            var isCurrentPasswordValid = _passwordHasherService.VerifyPassword(
                request.CurrentPassword,
                user.PasswordHash);

            if (!isCurrentPasswordValid)
            {
                throw new BusinessRuleException("كلمة المرور الحالية غير صحيحة.");
            }

            if (request.CurrentPassword == request.NewPassword)
            {
                throw new BusinessRuleException("كلمة المرور الجديدة يجب أن تكون مختلفة عن الحالية.");
            }

            user.PasswordHash = _passwordHasherService.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await RevokeActiveRefreshTokensAsync(user.Id);

            await _context.SaveChangesAsync();

            _logger.LogInformation("تم تغيير كلمة المرور بنجاح. UserId: {UserId}", user.Id);
        }

        public async Task ResetPasswordAsync(Guid id, ResetPasswordRequestDto request)
        {
            var validationResult = await _resetPasswordValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new AppValidationException(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (user == null)
            {
                throw new NotFoundException("المستخدم غير موجود.");
            }

            user.PasswordHash = _passwordHasherService.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await RevokeActiveRefreshTokensAsync(user.Id);
            await _context.SaveChangesAsync();

            _logger.LogInformation("تمت إعادة تعيين كلمة المرور بواسطة الأدمن. UserId: {UserId}", user.Id);
        }

        public async Task RevokeSessionsAsync(Guid id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (user == null)
            {
                throw new NotFoundException("المستخدم غير موجود.");
            }

            await RevokeActiveRefreshTokensAsync(user.Id);
            await _context.SaveChangesAsync();

            _logger.LogInformation("تم إبطال جلسات المستخدم (refresh tokens). UserId: {UserId}", user.Id);
        }

        private async Task EnsureRoleExistsAsync(Guid roleId)
        {
            var roleExists = await _context.Roles
                .AnyAsync(x => x.Id == roleId && !x.IsDeleted);

            if (!roleExists)
            {
                throw new NotFoundException("الدور المحدد غير موجود.");
            }
        }

        private async Task EnsureUserNameIsUniqueAsync(string userName, Guid? currentUserId = null)
        {
            var normalizedUserName = userName.Trim().ToLower();

            var exists = await _context.Users
                .AnyAsync(x =>
                    !x.IsDeleted &&
                    x.UserName.ToLower() == normalizedUserName &&
                    (!currentUserId.HasValue || x.Id != currentUserId.Value));

            if (exists)
            {
                throw new ConflictException("اسم المستخدم مستخدم مسبقًا.");
            }
        }

        private async Task EnsureEmailIsUniqueAsync(string? email, Guid? currentUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            var normalizedEmail = email.Trim().ToLower();

            var exists = await _context.Users
                .AnyAsync(x =>
                    !x.IsDeleted &&
                    x.Email != null &&
                    x.Email.ToLower() == normalizedEmail &&
                    (!currentUserId.HasValue || x.Id != currentUserId.Value));

            if (exists)
            {
                throw new ConflictException("البريد الإلكتروني مستخدم مسبقًا.");
            }
        }

        private async Task<Guid> GetAdminRoleIdAsync()
        {
            var adminRoleId = await _context.Roles
                .Where(x => x.Name == "Admin" && !x.IsDeleted && x.IsActive)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            return adminRoleId;
        }

        private async Task<Guid> GetDoctorRoleIdAsync()
        {
            var doctorRoleId = await _context.Roles
                .Where(x => x.Name == "Doctor" && !x.IsDeleted && x.IsActive)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            return doctorRoleId;
        }

        private async Task EnsureDoctorLinkIsValidAsync(Guid roleId, Guid? doctorId)
        {
            var doctorRoleId = await GetDoctorRoleIdAsync();
            var isDoctorRole = doctorRoleId != Guid.Empty && roleId == doctorRoleId;

            if (!isDoctorRole)
            {
                if (doctorId.HasValue)
                {
                    throw new BusinessRuleException("لا يمكن ربط DoctorId إلا لمستخدم بدور Doctor.");
                }

                return;
            }

            if (!doctorId.HasValue)
            {
                throw new BusinessRuleException("يجب تحديد DoctorId لمستخدم بدور Doctor.");
            }

            var doctorExists = await _context.Doctors.AnyAsync(x => x.Id == doctorId.Value && !x.IsDeleted);
            if (!doctorExists)
            {
                throw new NotFoundException("الطبيب المحدد غير موجود.");
            }
        }

        private async Task EnsureNotDisablingLastAdminAsync(Guid userRoleId)
        {
            var adminRoleId = await GetAdminRoleIdAsync();
            if (adminRoleId == Guid.Empty || userRoleId != adminRoleId)
            {
                return;
            }

            var activeAdminCount = await _context.Users
                .CountAsync(x => !x.IsDeleted && x.IsActive && x.RoleId == adminRoleId);

            if (activeAdminCount <= 1)
            {
                throw new BusinessRuleException("لا يمكن تعطيل آخر مستخدم Admin في النظام.");
            }
        }

        private async Task EnsureNotDeletingLastAdminAsync(Guid userRoleId, bool userIsActive)
        {
            var adminRoleId = await GetAdminRoleIdAsync();
            if (adminRoleId == Guid.Empty || userRoleId != adminRoleId)
            {
                return;
            }

            if (!userIsActive)
            {
                return;
            }

            var activeAdminCount = await _context.Users
                .CountAsync(x => !x.IsDeleted && x.IsActive && x.RoleId == adminRoleId);

            if (activeAdminCount <= 1)
            {
                throw new BusinessRuleException("لا يمكن حذف آخر مستخدم Admin في النظام.");
            }
        }

        private async Task EnsureNotDemotingLastAdminAsync(Guid currentRoleId, Guid newRoleId, bool userIsActive)
        {
            var adminRoleId = await GetAdminRoleIdAsync();
            if (adminRoleId == Guid.Empty)
            {
                return;
            }

            var isCurrentlyAdmin = currentRoleId == adminRoleId;
            var willStayAdmin = newRoleId == adminRoleId;

            if (!isCurrentlyAdmin || willStayAdmin)
            {
                return;
            }

            if (!userIsActive)
            {
                return;
            }

            var activeAdminCount = await _context.Users
                .CountAsync(x => !x.IsDeleted && x.IsActive && x.RoleId == adminRoleId);

            if (activeAdminCount <= 1)
            {
                throw new BusinessRuleException("لا يمكن تغيير دور آخر مستخدم Admin إلى دور آخر.");
            }
        }

        private async Task RevokeActiveRefreshTokensAsync(Guid userId)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(x =>
                    x.UserId == userId &&
                    x.IsActive &&
                    x.RevokedAt == null &&
                    !x.IsDeleted &&
                    x.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
