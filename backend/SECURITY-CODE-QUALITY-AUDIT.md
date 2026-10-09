## Security & Code Quality Audit — TaskMaster Backend

Audit scope: current backend source, configuration, DTOs, repositories, migrations, and ignore rules. The working tree contained pre-existing uncommitted changes, including refresh-token support and `CreateTaskDto`; those changes were reviewed and preserved.

### Category: Authentication & Authorization
| Check | Status | File | Notes |
|---|---|---|---|
| JWT signing key strength and hardcoding | PASS | `backend/Program.cs:19-31`; `backend/appsettings.json:5-6`; `backend/TaskMaster.Api.csproj:8` | No key is committed. Startup rejects absent, placeholder, or under-32-byte keys; a random 48-byte development key is stored locally with .NET user secrets. Production must provide `Jwt__Key` through its secret provider. |
| Access-token expiry and refresh tokens | PASS | `backend/Services/AuthService.cs:17-18,53-70,99-112,130-137`; `backend/Controllers/AuthController.cs:32-36`; `backend/Migrations/20260102000000_HashRefreshTokens.cs:13-36` | Access tokens expire after two hours; refresh tokens are cryptographically random, rotated, and persisted as SHA-256 hashes. The migration invalidates existing refresh sessions. |
| Controller endpoint authentication | PASS | `backend/Controllers/AuthController.cs:20-45`; `backend/Controllers/TasksController.cs:9-11`; `backend/Controllers/TaskActivityController.cs:9-12`; `backend/Controllers/TaskCommentsController.cs:9-12`; `backend/Controllers/UsersController.cs:8-10,20-32`; `backend/Program.cs:162` | Task, activity, comment, and user endpoints require authentication. User listing is ADMIN-only and individual user lookup is self-or-ADMIN. Register, login, refresh, and `/health` are intentionally public. |
| Role-based authorization | PASS | `backend/Controllers/TasksController.cs:62-67` | Assignment requires `ADMIN`; no other endpoint claims a role-specific policy. |
| User-ID claim parsing | PASS | `backend/Extensions/ClaimsPrincipalExtensions.cs:9-14` | Missing and malformed IDs now fail as `UnauthorizedAccessException`, not an unhandled `FormatException`. |
| Per-resource authorization | PASS | `backend/Controllers/TasksController.cs:29-63`; `backend/Services/TaskService.cs:56-74,130-145,200-204`; `backend/Services/TaskCommentService.cs:24-70`; `backend/Repositories/TaskRepository.cs:24-50,98-103` | Reads and comments are restricted to creators/assignees and ADMIN; updates/deletes are restricted to creators and ADMIN; assignment is ADMIN-only. Unauthorized task access returns 404. |

### Category: Input Validation
| Check | Status | File | Notes |
|---|---|---|---|
| DTO validation before service calls | PASS | `backend/DTOs/Auth/RegisterRequestDto.cs:7-15`; `backend/DTOs/Auth/LoginRequestDto.cs:7-14`; `backend/DTOs/Task/CreateTaskDto.cs:7-24`; `backend/DTOs/Task/UpdateTaskDto.cs:1-20`; `backend/DTOs/CreateCommentDto.cs:1-9`; `backend/DTOs/Task/AssignedTaskDto.cs:1-9`; `backend/DTOs/Auth/RefreshRequestDto.cs:1-13`; `backend/Controllers/TasksController.cs:68-75` | DTOs validate lengths and allowed enum values; assignment rejects an empty GUID and refresh tokens are length-limited. |
| Status and priority validation | PASS | `backend/Services/TaskService.cs:22-28,88-104,174-182` | Allowed values are checked server-side before create/update persistence; invalid values become 400 responses. |
| Search-term size limit | PASS | `backend/Controllers/TasksController.cs:83-96` | Empty terms are rejected and `q` is capped at 200 characters. |
| Guid route and claim validation | PASS | `backend/Controllers/TasksController.cs:36-58`; `backend/Controllers/TaskCommentsController.cs:21-35`; `backend/Extensions/ClaimsPrincipalExtensions.cs:9-14` | `[ApiController]` route binding rejects malformed GUIDs with 400; claim GUID parsing is guarded. |

### Category: Data Access & Injection
| Check | Status | File | Notes |
|---|---|---|---|
| Parameterized database queries | PASS | `backend/Repositories/TaskRepository.cs:50-96`; `backend/Services/AuthService.cs:29,46,57-59,73-74` | Queries use EF Core LINQ; no concatenated raw SQL was found. |
| `AppDbContext` lifetime | PASS | `backend/Program.cs:18-20` | `AddDbContext` registers the context as scoped by default. |
| Intentional navigation loading | PASS | `backend/Repositories/TaskRepository.cs:24-35`; `backend/Services/AuthService.cs:57-59` | Task comments are included for task detail retrieval, not list retrieval; refresh-token user data is included where needed. No accidental N+1 pattern was found in inspected queries. |

### Category: Passwords & Secrets
| Check | Status | File | Notes |
|---|---|---|---|
| Password hashing | PASS | `backend/Services/AuthService.cs:32-36,44-50`; `backend/TaskMaster.Api.csproj:12` | BCrypt.Net-Next hashes and verifies passwords using its default work factor (11), not a low explicit cost. |
| SMTP and database credentials | WARN | `backend/appsettings.json:2-3,15-22`; `backend/.env.example:1-17`; `backend/Services/EmailService.cs:117-130` | No usable SMTP credentials are committed. SMTP username/password and production DB credentials must be supplied by the operator; sending fails explicitly if SMTP settings are missing. |
| Secret-file ignore rules | PASS | `.gitignore:32-41`; `backend/appsettings.json:1-26` | Local `.env.*` and secret-bearing local appsettings files are ignored; `.env.example` remains trackable. Committed settings contain no JWT or SMTP credentials. |
| Refresh-token storage at rest | PASS | `backend/Services/AuthService.cs:99-112`; `backend/Repositories/AuthRepository.cs:33-64`; `backend/Migrations/20260102000000_HashRefreshTokens.cs:13-36` | Only SHA-256 token hashes are stored. The migration deletes existing refresh-token rows before replacing plaintext storage, requiring users with existing sessions to authenticate again. |

### Category: Error Handling & Information Leakage
| Check | Status | File | Notes |
|---|---|---|---|
| Exception details returned to clients | PASS | `backend/Middlewares/ExceptionMiddleware.cs:25-67` | Stack traces and unexpected exception messages are not returned; unexpected failures get a generic 500 body. Known 400/404 exception messages are returned, and unauthorized responses are generic. |
| Swagger/OpenAPI exposure | PASS | `backend/Program.cs:74-82` | Swagger middleware is enabled only in Development. |
| Email notification failure reporting | PASS | `backend/Services/EmailService.cs:117-166`; `backend/Services/DeadlineNotificationService.cs:20-33` | SMTP configuration and delivery failures now propagate to the background service, which logs the failure rather than reporting a false success. Durable queue/retry is not implemented. |

### Category: Transport Security
| Check | Status | File | Notes |
|---|---|---|---|
| HTTPS redirection and HSTS | PASS | `backend/Program.cs:143-155` | HTTPS redirection is enabled and HSTS is enabled outside Development. TLS certificate provisioning/termination remains host infrastructure configuration. |
| CORS origin restrictions | PASS | `backend/Program.cs:34-50,99-107`; `backend/appsettings.json:24-26` | Origins are explicit. Production startup requires exact HTTPS origins and rejects wildcard/path/query values; provide production origins with `Cors__AllowedOrigins`. |

### Category: Mass Assignment
| Check | Status | File | Notes |
|---|---|---|---|
| Task creation request model | PASS | `backend/Controllers/TasksController.cs:21-26`; `backend/DTOs/Task/CreateTaskDto.cs:5-21`; `backend/Services/TaskService.cs:30-38` | `POST /api/tasks` accepts `CreateTaskDto`, not `TaskItem`; `CreatedByUserId` is derived from the authenticated claim and assignment fields are not accepted in the DTO. |

### Category: Architecture Compliance
| Check | Status | File | Notes |
|---|---|---|---|
| Database access through repository interfaces | PASS | `backend/Services/AuthService.cs:12-24`; `backend/Repositories/AuthRepository.cs:8-64`; `backend/Services/DeadlineNotificationService.cs:41-52`; `backend/Repositories/TaskRepository.cs:98-123` | Services use repositories; EF Core access for authentication and deadline task queries is encapsulated in repository implementations. |
| Activity logs for mutations | PASS | `backend/Services/TaskService.cs:40-48,106-116,122-155`; `backend/Services/TaskCommentService.cs:22-39` | Create, changed update, delete, assignment, and comment creation log activity. Assignment records the acting admin after this audit's fix. |
| Service dependency registration | PASS | `backend/Program.cs:25-34` | Task-comment and activity-log services are both registered as scoped interface/implementation pairs. |

### Category: Exception Handling Patterns
| Check | Status | File | Notes |
|---|---|---|---|
| Typed exception-to-HTTP mapping | PASS | `backend/Middlewares/ExceptionMiddleware.cs:38-55`; `backend/Services/TaskService.cs:24-28,65-66,90-100,124-125,143-144` | Argument/invalid-operation errors map to 400, missing resources to 404, and unauthorized access to 401. |
| Generic `throw new Exception(...)` | PASS | `backend/` C# source | No generic `throw new Exception(...)` statements were found. |

### Category: Async Correctness
| Check | Status | File | Notes |
|---|---|---|---|
| Blocking `.Result` / `.Wait()` calls | PASS | `backend/` C# source | No `.Result` or `.Wait()` calls were found. |
| Async work awaited | PASS | `backend/Services/TaskService.cs`; `backend/Services/AuthService.cs`; `backend/Services/EmailService.cs`; `backend/Repositories/` | Inspected asynchronous operations are awaited or return task-valued operations directly. |

### Category: Nullable Correctness
| Check | Status | File | Notes |
|---|---|---|---|
| Null-forgiving operators and nullable handling | PASS | `backend/Program.cs:19-31`; `backend/Services/AuthService.cs:124-137`; `backend/Services/DeadlineNotificationService.cs:56-97`; `backend/Models/TaskComment.cs:7`; `backend/Models/RefreshToken.cs:9` | JWT configuration is validated at startup and no service uses null-forgiving configuration access; nullable worker values are guarded. EF-required navigation properties retain the conventional `null!` initialization. |

### Category: DTOs vs Models
| Check | Status | File | Notes |
|---|---|---|---|
| Entity exposure and mapping consistency | PASS | `backend/Controllers/TasksController.cs:22-26,29-55,78-110`; `backend/Models/TaskItem.cs:5-27`; `backend/Repositories/UserRepository.cs:18-43` | Task endpoints currently return `TaskItem`; it contains no password data. User endpoints project to `UserDto` and exclude `PasswordHash`. The task response still exposes persistence fields and is a lower-priority DTO consistency improvement. |

### Category: Naming & Style
| Check | Status | File | Notes |
|---|---|---|---|
| File-scoped namespaces | WARN | `backend/Migrations/20251228091525_AddActivityLogs.cs:6-7`; `backend/Migrations/20260101000000_AddRefreshTokens.cs:8-10` | Application code and the new migration use file-scoped namespaces; older generated migrations retain block-scoped namespaces. |
| Public and private naming conventions | PASS | `backend/Services/TaskService.cs:9-19`; `backend/Controllers/TasksController.cs:12-18` | Public members use PascalCase and private fields use `_camelCase`. |

### Category: Additional Security Checks
| Check | Status | File | Notes |
|---|---|---|---|
| User-controlled email HTML content | PASS | `backend/Services/EmailService.cs:22-40,45-66,68-89,91-115` | Task titles and displayed status values are HTML-encoded before insertion into email bodies. |

### Summary
- 0 FAIL (must fix)
- 3 WARN (remaining limitations)
- 33 PASS

## Fixes Applied
- Removed the committed JWT and SMTP placeholder credentials from `appsettings.json`; required a random JWT key of at least 32 bytes at startup and generated a 48-byte development key into the machine-local .NET user-secrets store.
- Enforced creator/assignee read access, creator/ADMIN update/delete access, ADMIN-only assignment and user listing, and self-or-ADMIN user lookup.
- Hashed refresh tokens at rest and added a migration that invalidates existing refresh tokens; users with existing refresh sessions must log in again after migration.
- Required exact HTTPS-only production CORS origins, enabled HSTS outside Development, and added global and stricter authentication endpoint rate limits.
- Moved authentication and background deadline queries behind repository interfaces.
- Added limits and allowed-value validation to update, comment, assignment, and refresh DTOs; also check for nonempty assignment IDs.
- Restricted bulk user listing to admins and individual user lookup to self/admin.
- Changed email delivery errors from swallowed failures to explicit configuration/delivery failures logged by the background worker.
- Hardened malformed user-ID claim handling, corrected assignment audit actors, and HTML-encoded user-controlled email content.
- Added environment/secret-file ignore rules and cleaned the malformed wrapper text from the root `.gitignore`.
- Preserved the pre-existing uncommitted worktree changes.

## Action Required (manual)
- **Production credentials:** no production JWT key, SMTP credentials, or database connection details were provided. Supply them through the deployment secret manager/environment (`Jwt__Key`, `EmailSettings__Username`, `EmailSettings__Password`, `ConnectionStrings__DefaultConnection`) and rotate any real credentials that may have appeared in Git history. The development JWT key is local-only and is not committed.
- **Production hosting:** provision/verify TLS certificates and trusted reverse-proxy/forwarded-header configuration at the deployment host. Production startup now rejects absent/non-HTTPS CORS origins; set `Cors__AllowedOrigins` to the actual frontend origin when the production domain is known. Rate limits are per process/IP; multi-instance/distributed enforcement requires an infrastructure-level limiter.
- **Notification delivery guarantee:** failures now surface and are logged, but durable retry/queueing and duplicate suppression require a delivery guarantee/operational design beyond the current in-process worker.
