# Configuration

## Environment Variables

The API supports overriding configuration via environment variables.

### Database

- `ConnectionStrings__DefaultConnection`

### JWT

- `JwtSettings__Key`
- `JwtSettings__Issuer`
- `JwtSettings__Audience`
- `JwtSettings__DurationInMinutes`
- `JwtSettings__RefreshTokenDurationInDays`

### CORS

- `Cors__AllowedOrigins__0`
- `Cors__AllowedOrigins__1`
- `Cors__AllowedOrigins__2`

## Notes

- Do not store production secrets in `appsettings.json`.
- Use environment variables (or user-secrets for local development) for secrets.
