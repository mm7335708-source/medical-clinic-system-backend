# User Management And Authorization Changes

## Summary

This document lists the code changes added to strengthen user management, authentication, and role-based authorization in the project.

## Changes Implemented

### 1. Identity Application Layer

- Added `UpdateUserStatusRequestDto` to support explicit user activation/deactivation.
- Added `RoleResponseDto` to expose system roles through the API.
- Extended `IUserService` with `UpdateStatusAsync`.
- Added `IRoleService` and `RoleService` to retrieve active roles.
- Added `RoleMappingProfile` for mapping `Role` entities to API response DTOs.
- Added `UpdateUserStatusRequestDtoValidator`.
- Updated `UserService` to:
  - validate and update user active status
  - remove duplicated `IsActive` assignment in delete logic

### 2. API Endpoints

- Added `AuthController` with:
  - `POST /api/auth/login`
  - `POST /api/auth/refresh`
  - `POST /api/auth/logout`
- Added `UsersController` with:
  - `POST /api/users`
  - `PUT /api/users/{id}`
  - `PUT /api/users/{id}/status`
  - `PUT /api/users/{id}/reset-password`
  - `POST /api/users/{id}/revoke-sessions`
  - `GET /api/users/me`
  - `PUT /api/users/me/change-password`
  - `GET /api/users`
  - `GET /api/users/paged`
  - `GET /api/users/{id}`
  - `DELETE /api/users/{id}`
- Added `RolesController` with:
  - `GET /api/roles`
  - `GET /api/roles/{id}`

### 3. Authorization

- Added `AppRoles` constants to centralize role names.
- Applied `[Authorize]` rules on existing business controllers.
- Added current-user retrieval through JWT claims.
- Current role rules:
  - `Admin`: full user and role management
  - `Receptionist`: clinic operational modules
  - `Doctor`: read access to operational data and write access to visits

### 4. Startup And Swagger

- Updated `Program.cs` to:
  - configure Swagger with JWT Bearer authentication
  - initialize the database through `IDbInitializer`
  - keep JWT authentication and authorization active
- Updated `TokenService` to include `NameIdentifier` and email claims when available.

### 5. Project Structure Fix

- Corrected `MedicalClinicSystem.API.csproj` project references to point to the actual local project paths.

### 6. Refresh Token Support

- Added `RefreshToken` entity and persistence configuration.
- Added automatic refresh-token revocation when a user is disabled, deleted, or changes the password.
- Added refresh token generation, hashing, rotation, and revocation.
- Extended login response to include refresh token data.
- Added auth refresh and logout endpoints.

## Notes

- Permissions-based authorization was not implemented in this step; the system currently uses role-based authorization.
- Build verification was partially blocked by restricted NuGet network access in the current environment.

## Suggested Next Step

1. Apply the existing `RefreshTokens` migration to the database if you have not done so yet.
2. Add tests for login, refresh, logout, and authorization rules.
3. Move secrets out of `appsettings.json`.
4. Add permissions-based authorization only if you need finer control than roles.
