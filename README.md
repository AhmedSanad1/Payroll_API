# Payroll API

Backend for a payroll management system: employee records, monthly payroll runs, and
printable payroll reports. Built on ASP.NET Core with a Clean Architecture layering,
EF Core, and SQL Server.

The Angular client that consumes this API lives at
[Payroll_Angular](https://github.com/AhmedSanad1/Payroll_Angular).

## Stack

| Concern | Choice |
|---|---|
| Runtime | .NET 10 (`net10.0`) |
| Data access | EF Core 10 + SQL Server |
| Auth | JWT bearer, rotating refresh tokens, BCrypt password hashing |
| Validation | FluentValidation 12 |
| Mapping | AutoMapper 16 |
| Logging | Serilog (console + rolling file, Elasticsearch-format JSON) |
| Reporting | ReportViewerCore — RDLC templates rendered to PDF |
| API docs | Swashbuckle / OpenAPI |
| Tests | xUnit across 5 test projects, unit + integration |

## Architecture

Four projects, dependencies pointing inward only:

```
Domain          entities, enums, domain interfaces, date/service-year calculations
  ^
Application     DTOs, services, validators, repository interfaces, ApiResponse envelope
  ^
Infrastructure  EF Core contexts, repositories, migrations, seeding, JWT, RDLC reports
  ^
PayRollApi      controllers, DI wiring, middleware, auth, rate limiting, Swagger
```

`Domain` references nothing. `Application` depends only on `Domain` and defines the
interfaces `Infrastructure` implements, so persistence and reporting concerns stay out
of the business layer.

Cross-cutting pieces worth pointing at:

- `Application/Common/ApiResponse.cs` — every endpoint returns the same success/fail
  envelope, so the client has one shape to handle.
- `PayRollApi/Controllers/Common/CrudControllerBase.cs` — the repetitive CRUD surface is
  inherited rather than copy-pasted across ten controllers.
- `Application/Resources/Languages.*.resx` — all user-facing messages are resource-backed
  in Arabic and English, resolved per request.

## The domain model

### Monthly snapshot

A payroll run is a **write-once snapshot**. When a month is generated, every
`PayrollItem` row stores the values used at that moment — employee name, department,
grade, base salary, each incentive percentage and amount, absent days, net salary.

The rows are never updated in place. Regenerating a month deletes and recreates them,
and approving a run freezes it. This means editing an employee's grade today cannot
retroactively change what a payroll run from three months ago says it paid them.

### Salary calculation

Each employee's net salary is assembled from four inputs:

1. **Base salary** — from the employee's job grade (3 fixed, seeded grades).
2. **Department incentive** — a percentage attached to the department.
3. **Service incentive** — a percentage from the tier their completed years of service
   fall into (`ServiceIncentiveTier.MinYearsExceeded`).
4. **Attendance adjustment** — a bonus or deduction percentage resolved from the
   `AttendanceRule` band matching the month's absent-day count.

Two calculation modes are supported, selected in payroll settings:

- **Additive** — every percentage applies to the base salary, then the results are summed.
- **Compound** — each percentage applies to the running total, so incentives stack on
  top of each other.

The mode is recorded on the run itself, not just in settings, so an old run always
reports the mode it was actually calculated with.

## API

All routes are under `/api`, all require a bearer token except the auth endpoints.

| Area | Route | Notes |
|---|---|---|
| Auth | `POST /api/auth/login` | Returns access + refresh token |
| | `POST /api/auth/refresh` | Rotates the refresh token |
| | `POST /api/auth/logout` | Revokes the current refresh token |
| | `GET  /api/auth/me` | Current user |
| Employees | `GET/POST/PUT/DELETE /api/employees` | Paged list, soft delete |
| | `GET /api/employees/{id}/absences` | |
| | `GET /api/employees/{id}/payslip` | |
| Departments | `GET/POST/PUT/DELETE /api/departments` | `GET /lookup` for dropdowns |
| Job grades | `GET /api/job-grades`, `PUT /{id}` | Fixed rows — editable, not creatable |
| Absences | `GET /api/absences/month`, `POST /batch` | Bulk monthly entry |
| Attendance rules | `GET/POST/PUT/DELETE /api/attendance-rules` | Absence-day bands |
| Incentive tiers | `GET/POST/PUT/DELETE /api/service-incentive-tiers` | Service-year bands |
| Settings | `GET/PUT /api/payroll-settings` | Single row, calculation mode |
| Payroll runs | `GET/POST /api/payroll-runs` | Generate a month |
| | `GET /{id}/items`, `POST /{id}/approve` | |
| Reports | `GET /api/reports/{attendance\|incentives-deductions\|employees\|salaries}` | JSON |
| | `GET /api/reports/{name}/pdf` | RDLC-rendered PDF |

Swagger UI is served in development at `/swagger`.

## Security

- Access tokens are short-lived; refresh tokens are **rotated on every use** — the old
  token is revoked as the new one is issued, so a replayed refresh token fails.
- Only the hash of a refresh token is persisted; the raw token exists only in the response.
- Passwords are hashed with BCrypt at work factor 12, and `PasswordNeedsRehash` is
  checked on login — so raising the work factor later silently upgrades existing hashes
  as users sign in, with no migration and no forced reset.
- Rate limiting is two-tier: a 300 req/min sliding window per IP globally, and a fixed
  5 req/min window on `/api/auth/login` to slow password guessing.
- A global exception handler returns RFC 7807 `ProblemDetails` with a correlation id and
  never leaks stack traces.

## Getting started

```bash
git clone https://github.com/AhmedSanad1/Payroll_API.git
cd Payroll_API
```

Configuration is not committed. Create `PayRollApi/appsettings.json` (it is gitignored)
using `PayRollApi/appsettings.Development.json.example` as the starting point:

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=PayRollDB;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "<a 32+ byte random secret>",
    "Issuer": "PayRollApi",
    "Audience": "PayRollApi",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 7
  },
  "AdminSeed": { "Username": "<admin>", "Password": "<password>" }
}
```

Apply migrations and run:

```bash
dotnet ef database update --project Infrastructure --startup-project PayRollApi
dotnet run --project PayRollApi
```

Reference data (job grades, attendance rules, incentive tiers, payroll settings) and the
seed admin user are created on first start.

## Tests

```bash
dotnet test
```

Five test projects: unit tests for the domain calculations, application validators and
infrastructure services, plus `PayRollApi.IntegrationTests` — which drives the full
payroll lifecycle end to end against a test host, and covers auth and rate limiting.

## Further reading

[`docs/SYSTEM-GUIDE-AR.md`](docs/SYSTEM-GUIDE-AR.md) — a detailed Arabic reference
covering every screen, the rules behind it, the database schema, and the full API
surface.
