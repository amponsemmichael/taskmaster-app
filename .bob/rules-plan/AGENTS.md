# AGENTS.md — Plan Mode

This file provides guidance to agents when working with code in this repository.

## Non-Obvious Architectural Constraints

- **Repository layer is mandatory**: Services never touch `AppDbContext` directly — all DB operations go through repository interfaces. Adding a new entity requires: Model → DbSet in `AppDbContext` → Repository interface + implementation → Service → Controller.
- **Activity logging is a cross-cutting concern tied to TaskService**: Any new task mutation operation must log an activity entry. There is no AOP/decorator — it's manual calls in `TaskService`.
- **ExceptionMiddleware is the sole error boundary**: It is registered in `Program.cs` (implicitly via `app.UseMiddleware<ExceptionMiddleware>()` — check if registered; the service layer relies on it). Services throw specific exception types expecting this middleware to handle HTTP mapping.
- **Migrations are forward-only and auto-applied**: `db.Database.Migrate()` runs at every app startup. Rolling back requires a new migration. Plan schema changes carefully.
- **`DeadlineNotificationService` is a hosted background service**: It runs on a timer and sends email notifications. Email config (`EmailSettings`) must be populated or it will silently fail. SMTP credentials are in `appsettings.json` / `backend/.env`.
- **No frontend state management library**: The `AuthContext` stub is the intended auth store. Global state is React Context only — no Redux/Zustand.
- **Frontend `lib/` is intentionally empty**: The API client, auth context, and hooks need to be implemented before any frontend feature work. Plan to build these foundations first.
- **CORS is config-driven**: `"Cors:AllowedOrigins"` in `appsettings.json` is a comma-separated string. Changing allowed origins requires config change, not code change.
- **No integration tests, no unit tests**: The project has zero test coverage. Any test strategy must start from scratch with tooling decisions (xUnit for backend, Jest/Vitest for frontend).
