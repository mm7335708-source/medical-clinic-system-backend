# Auth, Users, and Roles Endpoints

## Authentication

### POST `/api/auth/login`

Request body:

```json
{
  "userNameOrEmail": "admin",
  "password": "Admin@123"
}
```

Success response:

```json
{
  "success": true,
  "message": "Login completed successfully.",
  "data": {
    "accessToken": "jwt-token",
    "expiresAt": "2026-04-19T12:00:00Z",
    "refreshToken": "refresh-token",
    "refreshTokenExpiresAt": "2026-04-26T12:00:00Z",
    "userId": "99999999-9999-9999-9999-999999999999",
    "fullName": "System Administrator",
    "userName": "admin",
    "roleName": "Admin"
  }
}
```

### POST `/api/auth/refresh`

Request body:

```json
{
  "refreshToken": "refresh-token"
}
```

### POST `/api/auth/logout`

Request body:

```json
{
  "refreshToken": "refresh-token"
}
```

## Users

All user-management endpoints require `Admin` unless otherwise noted.

### POST `/api/users`

```json
{
  "fullName": "Reception User",
  "userName": "reception1",
  "email": "reception1@clinic.local",
  "phoneNumber": "07700000010",
  "password": "Reception@123",
  "roleId": "22222222-2222-2222-2222-222222222222",
  "doctorId": null,
  "isActive": true
}
```

### PUT `/api/users/{id}`

```json
{
  "fullName": "Reception User Updated",
  "userName": "reception1",
  "email": "reception1@clinic.local",
  "phoneNumber": "07700000010",
  "roleId": "22222222-2222-2222-2222-222222222222",
  "doctorId": null,
  "isActive": true
}
```

### PUT `/api/users/{id}/status`

```json
{
  "isActive": false
}
```

### PUT `/api/users/{id}/reset-password`

```json
{
  "newPassword": "NewPass@123",
  "confirmNewPassword": "NewPass@123"
}
```

### POST `/api/users/{id}/revoke-sessions`

Revokes active refresh tokens for the user.

### GET `/api/users`

Returns all non-deleted users.

### GET `/api/users/paged?pageNumber=1&pageSize=10`

Returns paged users.

### GET `/api/users/{id}`

Returns a single user by id.

### DELETE `/api/users/{id}`

Soft deletes a user.

### GET `/api/users/me`

Allowed roles:
- `Admin`
- `Receptionist`
- `Doctor`

Returns the current authenticated user from JWT claims.

### PUT `/api/users/me/change-password`

Allowed roles:
- `Admin`
- `Receptionist`
- `Doctor`

```json
{
  "currentPassword": "Admin@123",
  "newPassword": "Admin@12345",
  "confirmNewPassword": "Admin@12345"
}
```

## Roles

### GET `/api/roles`

Allowed role:
- `Admin`

Returns all active roles.

### GET `/api/roles/{id}`

Allowed role:
- `Admin`

Returns a single role by id.

## Current Role Access Map

- `Admin`: auth, users, roles, and all protected clinic modules.
- `Receptionist`: clinics, doctors, specialties, schedules, patients, appointments, dashboard read access, own profile/password.
- `Doctor`: appointments read access, patient visits create/update/delete, dashboard read access, own profile/password.

## Protected Operational Endpoints

- `ClinicsController`: `Admin`, `Receptionist`
- `DoctorsController`: `Admin`, `Receptionist`
- `DoctorSchedulesController`: `Admin`, `Receptionist`
- `SpecialtiesController`: `Admin`, `Receptionist`
- `PatientsController`: read for all staff, write for `Admin` and `Receptionist`
- `AppointmentsController`: read for all staff, write for `Admin` and `Receptionist`
- `PatientVisitsController`: read for all staff, write for `Admin` and `Doctor`
- `DashboardController`: all staff

## Notes

- Use `Authorization: Bearer {accessToken}` for protected endpoints.
- After `login`, keep both `accessToken` and `refreshToken`.
- After `refresh`, replace the old refresh token with the new one returned by the API.
- If a user is disabled, deleted, or changes the password, their active refresh tokens are revoked.
- The system prevents disabling/deleting/demoting the last active `Admin` user.
