# AGENTS.md — Agent (Coding) Mode

This file provides guidance to agents when working with code in this repository.

## Non-Obvious Coding Rules

### Backend
- **Always log activity after mutations**: Every create/update/delete/assign in `TaskService` calls `_activityLogService.LogAsync(new CreateActivityLogDto { ... })`. New task operations must follow this pattern.
- **Throw typed exceptions for HTTP semantics**: `ExceptionMiddleware` maps exception types to HTTP codes. Throw `KeyNotFoundException` for 404, `ArgumentException` / `InvalidOperationException` for 400, `UnauthorizedAccessException` for 401 — never return `IActionResult` error responses from services.
- **Get current user via extension method**: Use `User.GetUserId()` (from `TaskMaster.Extensions`) in controllers — not `User.FindFirst(...)`.
- **Status/Priority are UPPER_CASE strings, not enums**: `"PENDING"`, `"IN_PROGRESS"`, `"COMPLETED"` / `"LOW"`, `"MEDIUM"`, `"HIGH"`. Validate with `IsValidStatus` / `IsValidPriority` helpers (see `TaskService`).
- **`TaskItem` not `Task`**: The model is named `TaskItem` to avoid collision with `System.Threading.Tasks.Task`.
- **DateTime must be UTC**: `AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true)` is active — always use `DateTime.UtcNow`, never `DateTime.Now`.
- **DTO namespaces must match folder structure**: `DTOs/Task/` → `namespace TaskMaster.DTOs.Tasks;` (note plural). Mismatch causes import failures in controllers.
- **Migrations auto-run at startup**: `db.Database.Migrate()` runs in `Program.cs`. Adding a migration does not require manual `dotnet ef database update` in dev.

### Frontend
- **`@/*` alias resolves to `frontend/`**, not `frontend/src/`. E.g., `@/lib/api/tasks` → `frontend/lib/api/tasks.ts`.
- **`lib/` stubs are empty**: `lib/api/`, `lib/context/`, `lib/hooks/`, `lib/utils/` files exist but are empty — fill them when implementing features.
- **Backend runs on port 8080**, not the typical 5000/5001. Frontend env: `NEXT_PUBLIC_API_URL=http://localhost:8080/api`.
- **No test runner is configured** — do not attempt to run or write tests without first adding a test framework.
