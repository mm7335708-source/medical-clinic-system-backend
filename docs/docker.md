# Docker Run

This repo includes a `docker-compose.yml` that runs:

- PostgreSQL (`db`)
- API (`api`)

## Run

```bash
docker compose up --build
```

## API

- Base URL: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`

## Database

- Host: `localhost`
- Port: `5432`
- Database: `MedicalClinicSystemDb`
- User: `postgres`
- Password: `123456`

The API runs migrations on startup via `DbInitializer`.

