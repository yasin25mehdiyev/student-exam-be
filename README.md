# Student Exam Management System — Backend API

A backend API for registering courses, students and their exam results at a school, built as a hands-on learning project to practice **.NET, Entity Framework Core, and Microsoft SQL Server** using **Clean Architecture**.

## Live Demo

**https://student-exam-api.azurewebsites.net/swagger**

Hosted on Azure App Service (Free tier) with an Azure SQL Database (Free tier). The free plan sleeps after ~20 minutes of inactivity, so the first request after a while can take 10-30 seconds to wake up — subsequent ones are fast.

## Tech Stack

| Concern | Choice |
|---|---|
| Language / Runtime | C# / .NET 10 |
| Web framework | ASP.NET Core Web API (controller-based) |
| ORM | Entity Framework Core 10 (Code-First, migrations) |
| Database | Microsoft SQL Server 2022 (Docker container locally, Azure SQL Database in production) |
| Hosting | Azure App Service (Linux, Free tier) |
| API documentation | Swagger / Swashbuckle |
| Architecture | Clean Architecture (Domain / Application / Infrastructure / API) |

## Architecture

The solution is split into four projects, following the Clean Architecture dependency rule — **dependencies only point inward**, so business logic never depends on infrastructure details:

```
StudentExam.Domain          →  no dependencies (pure entities)
        ↑
StudentExam.Application     →  depends on Domain only (DTOs, interfaces, business logic)
        ↑
StudentExam.Infrastructure  →  depends on Application (EF Core, repositories, migrations)
        ↑
StudentExam.Api             →  depends on Application + Infrastructure (controllers, DI wiring)
```

```
src/
├── StudentExam.Domain/            Entities only — Course, Student, Exam. Zero external dependencies.
│
├── StudentExam.Application/       Business logic, independent of EF Core / ASP.NET Core.
│   ├── Common/                    PagedResult<T>, QueryParameters, ServiceResult, custom exceptions
│   ├── Dtos/                      Request/response contracts (CourseDto, CreateCourseDto, ...)
│   ├── Interfaces/
│   │   ├── Repositories/          ICourseRepository, IStudentRepository, IExamRepository, IUnitOfWork
│   │   └── Services/               ICourseService, IStudentService, IExamService, IReportService
│   ├── Services/                  Business logic implementation (validation, orchestration, mapping)
│   └── DependencyInjection.cs
│
├── StudentExam.Infrastructure/    EF Core, SQL Server, data access.
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   └── Configurations/        One IEntityTypeConfiguration<T> per entity (Fluent API)
│   ├── Repositories/              EF Core implementations of the Application-layer interfaces
│   ├── Migrations/
│   └── DependencyInjection.cs
│
└── StudentExam.Api/               Thin controllers, DI composition root.
    ├── Controllers/
    └── Program.cs
```

### Key design patterns

`IUnitOfWork` exposes one repository per aggregate (`Courses`, `Students`, `Exams`) and a single `SaveChangesAsync`, so a business operation commits atomically. Controllers never talk to EF Core directly — business rules (uniqueness checks, cross-entity validation, code normalization) live in `Application/Services`, which return a `ServiceResult` / `ServiceResult<T>` instead of throwing for expected outcomes like "not found" or "duplicate code"; `ApiControllerBase.FromResult(...)` turns that into the right HTTP status, so the Application layer stays unaware of HTTP.

EF Core's `DbUpdateException` (e.g. a foreign key violation) is caught in `Infrastructure` and re-thrown as `ForeignKeyConstraintException`, a type owned by `Application`, so Application never needs to reference `Microsoft.EntityFrameworkCore`. Each entity's SQL Server mapping lives in its own `IEntityTypeConfiguration<T>` under `Persistence/Configurations` rather than one big `OnModelCreating`.

## Database Schema

The schema follows the original spec, with two deliberate deviations explained below.

| Table | Column | Type | Notes |
|---|---|---|---|
| **Courses** | `Code` | `char(3)` | Primary key, user-supplied (e.g. `MTH`) |
| | `Name` | `nvarchar(30)` | |
| | `ClassLevel` | `tinyint` | 1–11 |
| | `TeacherFirstName` | `nvarchar(20)` | |
| | `TeacherLastName` | `nvarchar(20)` | |
| **Students** | `Number` | `int` | Primary key, user-supplied |
| | `FirstName` | `nvarchar(30)` | |
| | `LastName` | `nvarchar(30)` | |
| | `ClassLevel` | `tinyint` | 1–11 |
| **Exams** | `Id` | `int identity` | Primary key (see note below) |
| | `CourseCode` | `char(3)` | Foreign key → `Courses.Code` |
| | `StudentNumber` | `int` | Foreign key → `Students.Number` |
| | `ExamDate` | `date` | |
| | `Score` | `tinyint` | 0–9, enforced by a `CHECK` constraint |

Deviations from the original spec: text columns use `nvarchar` instead of `varchar`, since `varchar` can't reliably store Azerbaijani characters (ə, ş, ç, ğ, ö, ü, ı). `Exams` also gets a surrogate `Id` primary key — the spec has no natural single-column key for an exam, and a surrogate key lets `GET/PUT/DELETE /api/exams/{id}` address it directly instead of needing a 3-column composite key.

Both foreign keys use `ON DELETE NO ACTION` (`DeleteBehavior.Restrict`): a course or student with existing exam records cannot be deleted until those exam records are removed first, preventing silent data loss.

## Getting Started

### Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
  - **On Apple Silicon (M1/M2/M3/M4):** enable *Settings → General → "Use Rosetta for x86/amd64 emulation on Apple Silicon"*. SQL Server only ships an `amd64` image; without Rosetta, the container fails to start on ARM Macs.

### 1. Start SQL Server

```bash
cp .env.example .env   # adjust SA_PASSWORD if you want a different one
docker compose up -d
```

Wait for the container to report healthy:

```bash
docker compose ps   # STATUS should show "(healthy)" after ~20–30s
```

### 2. Configure the connection string

The connection string is kept out of source control via [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

```bash
cd src/StudentExam.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=localhost,1433;Database=StudentExamDb;User Id=sa;Password=<same as SA_PASSWORD in .env>;TrustServerCertificate=True;"
```

### 3. Apply migrations

```bash
cd ../..                                 # back to repo root
dotnet tool install --global dotnet-ef   # once, if not already installed
dotnet ef database update \
  --project src/StudentExam.Infrastructure \
  --startup-project src/StudentExam.Api
```

### 4. Run the API

```bash
dotnet run --project src/StudentExam.Api
```

Open the printed HTTPS URL with `/swagger` appended (e.g. `https://localhost:7274/swagger`) to explore and test every endpoint interactively.

## API Endpoints

### Courses — `/api/courses`

| Method | Route | Description |
|---|---|---|
| GET | `/api/courses` | Paged list — supports `pageNumber`, `pageSize`, `search`, `sortBy`, `sortDirection` |
| GET | `/api/courses/{code}` | Get a single course |
| POST | `/api/courses` | Create a course |
| PUT | `/api/courses/{code}` | Update a course |
| DELETE | `/api/courses/{code}` | Delete a course (blocked while exams reference it) |

### Students — `/api/students`

| Method | Route | Description |
|---|---|---|
| GET | `/api/students` | Paged list — same query parameters as above |
| GET | `/api/students/{number}` | Get a single student |
| POST | `/api/students` | Create a student |
| PUT | `/api/students/{number}` | Update a student |
| DELETE | `/api/students/{number}` | Delete a student (blocked while exams reference it) |

### Exams — `/api/exams`

| Method | Route | Description |
|---|---|---|
| GET | `/api/exams` | Paged list — same query parameters as above |
| GET | `/api/exams/{id}` | Get a single exam |
| POST | `/api/exams` | Create an exam (validates the course and student both exist) |
| PUT | `/api/exams/{id}` | Update an exam's date/score |
| DELETE | `/api/exams/{id}` | Delete an exam |

### Reports — `/api/reports`

| Method | Route | Description |
|---|---|---|
| GET | `/api/reports/students/{number}` | All exam results for one student, plus their average score |
| GET | `/api/reports/class-averages?courseCode=` | Average score grouped by class level, optionally filtered to one course |
| GET | `/api/reports/summary` | Total counts of courses, students and exams across the system |

### Pagination, search and sort

All three list endpoints (`GET /api/courses`, `/api/students`, `/api/exams`) accept the same query parameters:

| Parameter | Default | Notes |
|---|---|---|
| `pageNumber` | `1` | Clamped to a minimum of 1 |
| `pageSize` | `10` | Clamped between 1 and 100 |
| `search` | — | Case-insensitive partial match across the entity's name-like fields (for exams, this also matches the linked course/student names) |
| `sortBy` | entity-specific | Unknown values fall back to the default sort — never an error |
| `sortDirection` | `asc` | `asc` or `desc` |

Response shape:

```json
{
  "items": [ ... ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 42,
  "totalPages": 5
}
```

### HTTP status codes

- `200 OK` — successful read
- `201 Created` — successful create, with a `Location` header pointing to the new resource
- `204 No Content` — successful update/delete (no response body, by REST convention)
- `400 Bad Request` — validation failure (e.g. `Score` outside 0–9)
- `404 Not Found` — referenced resource does not exist
- `409 Conflict` — duplicate key on create, or delete blocked by a foreign key reference

## Deployment

Every push to `main` runs [.github/workflows/azure-deploy.yml](.github/workflows/azure-deploy.yml), which builds the API and deploys it to Azure App Service. EF Core migrations are applied automatically on startup (see `Program.cs`), so there's no separate migration step in production.
