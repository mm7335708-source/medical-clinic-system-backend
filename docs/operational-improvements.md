# Operational Improvements (Non-Identity)

This document lists backend improvements applied to the project outside of user management.

## Appointments

- Prevents booking conflicts while allowing re-booking of previously cancelled slots.
  - Conflict checks now ignore `Cancelled` appointments.
- Schedule validation was tightened to require the start time to be strictly inside working hours.
- Status updates now require using the dedicated cancel endpoint for cancellations (to preserve a cancellation reason).

## Soft Delete

- Added global query filters (`IsDeleted == false`) for all main entities in `AppDbContext`.
  - This reduces the risk of accidentally returning deleted records.
- Patient visits deletion was changed from hard delete to soft delete.

## Auditing

- `CreatedAt/UpdatedAt` is already maintained.
- Added automatic `CreatedBy/UpdatedBy/DeletedBy` population when a request is authenticated (based on JWT claims).

## Doctor Scoping

- Users can be linked to doctors via `Users.DoctorId`.
- When a doctor user is linked, appointment and visit endpoints enforce access to the doctor’s own records.

## Health

- Added `GET /health` (anonymous) to validate API liveness and database connectivity.

## Rate Limiting

- Added rate limiting for `POST /api/auth/login` to reduce brute-force attempts.
  - Policy: 5 requests per minute per client window.
