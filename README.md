# Medical Clinic System (Backend)

ASP.NET Core 8 Web API for managing clinics, doctors, schedules, patients, appointments, and visits.

## Quick Start (Local)

1. Configure database + JWT:
   - `MedicalClinicSystem.API/appsettings.json` (development only), or
   - environment variables (recommended): see `docs/configuration.md`
2. Run the API:
   - `dotnet run --project MedicalClinicSystem.API/MedicalClinicSystem.API.csproj`
3. Open Swagger:
   - `https://localhost:<port>/swagger`

The API runs EF migrations on startup via `DbInitializer`.

## Quick Start (Docker)

See `docs/docker.md`.

## Roles (Current Phase)

- `Admin`: full access, user/role management.
- `Receptionist`: operational modules (patients/appointments/etc.).
- `Doctor`: can manage visits, and is restricted to their own data when `DoctorId` is linked.

## Doctor Linking (`DoctorId`)

For real doctor restrictions, link the user to a doctor record:

- Create/update user with `roleId` = Doctor role id and set `doctorId`.
- The access token will contain claim `DoctorId`.
- Appointment/visit endpoints will enforce that doctors can only access their own `DoctorId`.

## Operational Improvements

See `docs/operational-improvements.md` for non-identity improvements:

- appointment conflict fixes
- soft delete query filters
- audit fields population
- `/health` endpoint
- login rate limiting

## Tests / CI

- Tests project: `MedicalClinicSystem.Tests`
- GitHub Actions workflow: `.github/workflows/ci.yml`

Run tests:

```bash
dotnet test MedicalClinicSystem.Tests/MedicalClinicSystem.Tests.csproj
```

If MSBuild fails intermittently with no output, retry with single-proc build:

```bash
dotnet test MedicalClinicSystem.Tests/MedicalClinicSystem.Tests.csproj -m:1
```

## Security Notes

- Do not store production secrets in `appsettings.json`.
- Use environment variables for `ConnectionStrings__DefaultConnection` and `JwtSettings__Key`.
