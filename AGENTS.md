# AGENTS.md

This file provides guidance to agents when working with code in this repository.

## Stack
- **Backend**: ASP.NET Core 8 (C#) — `backend/` — no test project exists
- **Frontend**: Next.js 16 + React 19 + TypeScript (strict mode) — `frontend/`
- **DB**: PostgreSQL 16 via EF Core (Npgsql); migrations auto-applied on startup via `db.Database.Migrate()`

## Commands

### Backend (run from `backend/`)
```
dotnet run                        # dev server on http://localhost:8080
dotnet build                      # compile
dotnet ef migrations add <Name>   # add a migration
dotnet ef database update         # apply migrations manually
```

### Frontend (run from `frontend/`)
```
npm run dev       # Next.js dev on http://localhost:3000
npm run build
npm run lint      # eslint (next/core-web-vitals + next/typescript)
```

### Full stack (Docker)
```
cd backend && bash build.sh       # builds Docker image (no-cache)
docker-compose up                 # starts api + postgres; api on :8080, db on :5432
```
> `docker-compose.yml` is at the repo root; `backend/.env` must exist (copy from `backend/.env.example`).

## Environment / Config

- Backend reads config from `appsettings.json` **and** env vars; Docker uses `backend/.env`
- `Npgsql.EnableLegacyTimestampBehavior` is set globally in `Program.cs` — all `DateTime` values must be stored/retrieved as UTC
- JWT expires in 2 hours (hardcoded in `AuthService.GenerateToken`)
- CORS allowed origins configured in `appsettings.json` under `"Cors:AllowedOrigins"` (comma-separated)
- Frontend API base URL: `NEXT_PUBLIC_API_URL` (see `frontend/env.example`)

## Backend Architecture

- Repository pattern: all DB access via `ITaskRepository`, `ITaskCommentRepository`, `IActivityLogRepository` — never query `AppDbContext` directly in services
- Every mutating task operation (create/update/delete/assign) **must** call `ActivityLogService.LogAsync(...)` — see `TaskService.cs`
- `ExceptionMiddleware` maps `KeyNotFoundException → 404`, `ArgumentException/InvalidOperationException → 400`, `UnauthorizedAccessException → 401` — services throw these types intentionally
- `User.GetUserId()` extension (`Extensions/ClaimsPrincipalExtensions.cs`) extracts `ClaimTypes.NameIdentifier` from JWT; use it in all controllers instead of reading claims directly
- Roles: `"USER"` (default) and `"ADMIN"`; only `ADMIN` can hit `PUT /api/tasks/{id}/assign`
- Status values (UPPER_CASE strings): `PENDING`, `IN_PROGRESS`, `COMPLETED`
- Priority values (UPPER_CASE strings): `LOW`, `MEDIUM`, `HIGH`
- `TaskItem` entity is named `TaskItem` in C# (not `Task`) to avoid conflict with `System.Threading.Tasks.Task`

## Frontend Architecture

- Path alias `@/*` maps to `frontend/*` (not `frontend/src/`)
- Route groups: `(auth)` for login/register, `(dashboard)` for protected pages
- Most `lib/` files (`api/`, `context/`, `hooks/`, `utils/`) are currently empty stubs — implement as needed
- UI components in `components/ui/` follow Radix UI + shadcn/ui conventions (CVA + `cn()` utility pattern)
- Forms use `react-hook-form` + `zod` for validation; toasts via `react-hot-toast`

## Code Style

### C#
- File-scoped namespaces (`namespace TaskMaster.Foo;`)
- Nullable reference types enabled (`<Nullable>enable</Nullable>`); use `?` and null-forgiving `!` carefully
- Implicit usings enabled — no need to import `System`, `System.Collections.Generic`, etc.
- DTOs live in `DTOs/<Domain>/` subdirectories; namespace must match folder (`TaskMaster.DTOs.Tasks`, not `TaskMaster.DTOs`)

### TypeScript / React
- Strict TypeScript (`"strict": true`); no `any` without justification
- No testing framework is configured — there are no tests
