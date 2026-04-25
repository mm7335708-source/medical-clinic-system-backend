using FluentValidation;
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
    public class AuthService : IAuthService
    {
        private readonly IApplicationDbContext _context;
        private readonly IValidator<LoginRequestDto> _loginValidator;
        private readonly IValidator<RefreshTokenRequestDto> _refreshTokenValidator;
        private readonly IValidator<LogoutRequestDto> _logoutValidator;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IApplicationDbContext context,
            IValidator<LoginRequestDto> loginValidator,
            IValidator<RefreshTokenRequestDto> refreshTokenValidator,
            IValidator<LogoutRequestDto> logoutValidator,
            IPasswordHasherService passwordHasherService,
            ITokenService tokenService,
            ILogger<AuthService> logger)
        {
            _context = context;
            _loginValidator = loginValidator;
            _refreshTokenValidator = refreshTokenValidator;
            _logoutValidator = logoutValidator;
            _passwordHasherService = passwordHasherService;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new AppValidationException(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var normalizedInput = request.UserNameOrEmail.Trim().ToLower();

            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x =>
                    !x.IsDeleted &&
                    (
                        x.UserName.ToLower() == normalizedInput ||
                        (x.Email != null && x.Email.ToLower() == normalizedInput)
                    ));

            if (user == null)
            {
                throw new UnauthorizedException("Invalid username/email or password.");
            }

            if (!user.IsActive)
            {
                throw new ForbiddenException("This account is inactive.");
            }

            var isPasswordValid = _passwordHasherService.VerifyPassword(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new UnauthorizedException("Invalid username/email or password.");
            }

            var roleName = user.Role?.Name ?? "N/A";
            var response = await CreateAuthResponseAsync(user, roleName);

            _logger.LogInformation("User logged in successfully. UserId: {UserId}", user.Id);

            return response;
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var validationResult = await _refreshTokenValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new AppValidationException(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var hashedToken = _tokenService.HashRefreshToken(request.RefreshToken);

            var refreshToken = await _context.RefreshTokens
                .Include(x => x.User)
                    .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.TokenHash == hashedToken && !x.IsDeleted);

            if (refreshToken == null || !refreshToken.IsUsable)
            {
                throw new UnauthorizedException("Refresh token is invalid or expired.");
            }

            var user = refreshToken.User;

            if (user.IsDeleted || !user.IsActive)
            {
                throw new ForbiddenException("The user is not allowed to refresh the session.");
            }

            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.UpdatedAt = DateTime.UtcNow;

            var roleName = user.Role?.Name ?? "N/A";
            var response = await CreateAuthResponseAsync(user, roleName);

            _logger.LogInformation("Refresh token used successfully. UserId: {UserId}", user.Id);

            return response;
        }

        public async Task LogoutAsync(LogoutRequestDto request)
        {
            var validationResult = await _logoutValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new AppValidationException(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var hashedToken = _tokenService.HashRefreshToken(request.RefreshToken);

            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == hashedToken && !x.IsDeleted);

            if (refreshToken == null)
            {
                return;
            }

            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("User logged out successfully. UserId: {UserId}", refreshToken.UserId);
        }

        private async Task<LoginResponseDto> CreateAuthResponseAsync(User user, string roleName)
        {
            await RevokeActiveRefreshTokensAsync(user.Id);

            var accessToken = _tokenService.GenerateAccessToken(user, roleName);
            var accessTokenExpiresAt = _tokenService.GetAccessTokenExpiration();
            var refreshTokenValue = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiresAt = _tokenService.GetRefreshTokenExpiration();

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = _tokenService.HashRefreshToken(refreshTokenValue),
                ExpiresAt = refreshTokenExpiresAt
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                ExpiresAt = accessTokenExpiresAt,
                RefreshToken = refreshTokenValue,
                RefreshTokenExpiresAt = refreshTokenExpiresAt,
                UserId = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                RoleName = roleName
            };
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
