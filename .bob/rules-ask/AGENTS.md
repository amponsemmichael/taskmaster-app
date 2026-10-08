# AGENTS.md — Ask Mode

This file provides guidance to agents when working with code in this repository.

## Non-Obvious Documentation Context

- **`lib/` files are empty stubs**: `frontend/lib/api/`, `frontend/lib/context/`, `frontend/lib/hooks/`, `frontend/lib/utils/` all exist but contain no code — they are scaffolded placeholders.
- **No tests exist**: There is no test project in `backend/` and no test runner configured in `frontend/`. The test infrastructure is absent, not just empty.
- **`TaskItem` is the task entity** (not `Task`) — searching for "Task model" will mislead; the EF entity and C# class is always `TaskItem`.
- **Docker compose is at repo root**, but the `backend/.env` file (separate from repo root) is what the compose `env_file` points to.
- **Two separate config layers in backend**: `appsettings.json` has defaults; environment variables (via `backend/.env` in Docker) override them. `appsettings.Development.json` only adjusts log levels.
- **JWT is stateless, 2-hour expiry** (hardcoded). There is no refresh token mechanism.
- **Activity logs are append-only** — there is no update or delete on `ActivityLog` entities; they record `CREATED`, `UPDATED`, `DELETED`, `ASSIGNED` actions.
- **`ADMIN` role is the only special role** — assigned manually in the DB; registration always creates `"USER"` role.
