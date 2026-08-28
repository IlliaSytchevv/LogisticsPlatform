# LogisticsPlatform — Backend

REST API for a logistics platform: orders (Cross-Dock / Consolidation), warehouse operations, supplies, Stripe payments, PDF/QR documents, and notifications.

The frontend lives in a separate project: `LogisticPlatformFrontend/` (Next.js). This README covers the backend and gateway only.

---

## Solution structure

| Project | Purpose |
|---------|---------|
| `LogisticsPlatform.Domain` | Entities, enums, options — no external dependencies |
| `LogisticsPlatform.Application` | CQS (commands/queries), validators, DTOs, interfaces |
| `LogisticsPlatform.Infrastructure` | EF Core, PostgreSQL, Redis, Stripe, Azure Blob, Identity/JWT |
| `LogisticsPlatform.Web` | ASP.NET Core API — controllers, middleware, Swagger |
| `ReversProxy/OcelotGateway` | Reverse proxy (Ocelot) — single entry point for `/api/*` |
| `LogisticPlatform.UnitTests` | Handler unit tests |
| `LogisticPlatform.IntegrationTests` | Integration tests (Testcontainers) |

Target framework: **.NET 10**.

---

## Architecture

**Clean Architecture** with four layers:

```
Controllers (Web)
      ↓
Dispatcher → Command/Query Handlers (Application)
      ↓
Repositories / Services (Infrastructure)
      ↓
Entities (Domain)
```

- **CQRS** — custom `IDispatcher`; handlers registered via DI.
- **FluentValidation** — decorator pipeline over handlers.
- **Ardalis.Result** — handler results mapped to HTTP status codes in `ApiController`.
- **ASP.NET Core Identity** + **JWT Bearer** (access token) + refresh token in an HttpOnly cookie.

---

## Tech stack

| Area | Technology |
|------|------------|
| API | ASP.NET Core 10, Swagger |
| Database | PostgreSQL 16, EF Core 10 (Npgsql) |
| Cache / locks | Redis 7, RedLock.net |
| Files (photos) | Azure Blob Storage (local: Azurite) |
| Payments | Stripe Checkout + webhooks |
| Documents | QuestPDF (BOL), QRCoder (QR) |
| Export | CsvHelper |
| Gateway | Ocelot + Polly, in-memory rate limit (prod) |

---

## Main features

- **Auth** — register, login, refresh, logout; roles `Admin`, `Dispatcher`, `Driver`.
- **Dashboard** — KPIs, active orders, activity.
- **Orders** — list with tabs/filters, create, CSV export.
- **Order details** — field updates, operations (+ comments/photos), supplies, warehouse photos, comments, timeline.
- **Edit lock** — one active edit session per order (Redis, TTL + heartbeat).
- **Payments** — Stripe checkout, webhook; mutex + checkout lock in Redis.
- **Notifications** — feed with Redis cache.
- **Seed** — demo data for local development.

API base prefix: **`/api/v1`**.

---

## Controllers and authorization

| Route | Access |
|-------|--------|
| `POST /api/v1/auth/*` | Anonymous |
| `GET /api/v1/dashboard/*` | Authorize |
| `GET/POST /api/v1/orders/*` | Authorize; create — `Admin,Dispatcher` |
| `PATCH /api/v1/orders/{id}` and mutations | mostly `Admin,Dispatcher` |
| `POST/DELETE .../edit-lock` | `Admin,Dispatcher` |
| `POST /api/v1/payments/orders/{id}/checkout` | `Admin,Dispatcher` |
| `POST /api/v1/payments/webhook` | AllowAnonymous (Stripe) |
| `POST /api/v1/seed` | no auth (dev only) |

Access token — `Authorization: Bearer {jwt}` header.  
Refresh — `refreshToken` cookie, endpoint `POST /api/v1/auth/refresh-token`.

---

## Redis

| Purpose | Key |
|---------|-----|
| Edit lock | `order-edit:{orderId}` |
| Payment checkout lock | `payment-checkout:{orderId}` |
| Payment mutex (RedLock) | `payment:mutex:{orderId}` |
| Notifications cache | `Logistics_notifications_feed:*` |

Refresh tokens are stored in **PostgreSQL**, not Redis.

---

## Configuration

Sections in `LogisticsPlatform.Web/appsettings.json` (and `appsettings.Development.json`):

```json
{
  "ConnectionStrings": { "DefaultConnection": "..." },
  "Jwt": { "SecretKey", "Issuer", "Audience", "AccessExpirationInMinutes", "RefreshExpirationInDays" },
  "Redis": { "ConnectionString": "localhost:6379,abortConnect=true" },
  "PhotoStorage": { "ConnectionString", "ContainerName": "photos" },
  "Stripe": { "SecretKey", "WebhookSecret", "SuccessUrlTemplate", "CancelUrlTemplate" }
}
```

In production, the same keys are supplied via **environment variables** (`ConnectionStrings__DefaultConnection`, `Jwt__SecretKey`, …).

Do not commit secrets to git — use User Secrets or Development overrides locally.

---

## Gateway (Ocelot)

Separate process: `ReversProxy/OcelotGateway`:

- **Dev:** listens on `http://localhost:5124`, proxies `/api/*` to API on `5217` and `5218` (RoundRobin).
- **Prod:** single downstream `ca-logistics-api:80`; replica load balancing is handled by Azure Container Apps.

If the second API instance on `5218` is not running, every other request through the gateway returns **502**. For normal local work, one API on `5217` is enough (call it directly, or start the second instance).

---

## Migrations and seed

On API startup, `Database.MigrateAsync()` runs — pending migrations are applied automatically.

Migrations live in `LogisticsPlatform.Infrastructure/Migrations/`. **New migrations are added manually by the repo owner** (`dotnet ef migrations add`).

Demo data: `POST http://localhost:5217/api/v1/seed` (if the database is empty after the first run).

Test users (from seed):

| Login | Password | Role |
|-------|----------|------|
| `AdminUser` | `Test123!` | Admin |
| `DispatcherUser` | `Test123!` | Dispatcher |
| `DriverUser` | `Test123!` | Driver |

---

## Docker (infrastructure)

`docker-compose.yml` starts:

| Service | Port | Purpose |
|---------|------|---------|
| `postgres` | 5432 | PostgreSQL |
| `redis` | 6379 | Redis |
| `azurite` | 10000–10002 | Blob emulator (photos) |

API and gateway are **not** in compose — run them with `dotnet run`.

---

## Local setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (PostgreSQL, Redis, Azurite)

### 1. Infrastructure

```powershell
cd LogisticsPlatform
docker compose up -d
```

### 2. API

```powershell
dotnet run --project LogisticsPlatform.Web --launch-profile http
```

- API: **http://localhost:5217**
- Swagger: **http://localhost:5217/swagger**

Migrations apply on first startup. If needed:

```powershell
curl -X POST http://localhost:5217/api/v1/seed
```

### 3. Gateway (optional, prod-like)

```powershell
dotnet run --project ReversProxy/OcelotGateway --launch-profile http
```

- Gateway: **http://localhost:5124**
- Point the frontend at the gateway: `NEXT_PUBLIC_API_URL=http://localhost:5124`

### 4. Second API instance (optional)

Only if you need Ocelot RoundRobin without 502s:

```powershell
dotnet run --project LogisticsPlatform.Web --launch-profile http-5218
```

### 5. Verify login

```powershell
curl -X POST http://localhost:5217/api/v1/auth/login `
  -H "Content-Type: application/json" `
  -d "{\"username\":\"AdminUser\",\"password\":\"Test123!\"}"
```

### 6. Tests

```powershell
dotnet test LogisticPlatform.UnitTests
dotnet test LogisticPlatform.IntegrationTests
```

Integration tests spin up their own PostgreSQL/Redis via Testcontainers (Docker required).

### 7. Quick checks

**Blob photos (Azurite)** — after uploading a warehouse photo locally:

```powershell
docker exec logistics-azurite ls -la /data/__blobstorage__
```

You should see blob files appear under that directory.

**Stripe payments** — checkout and webhooks do not work out of the box. Configure Stripe locally:

- `Stripe:SecretKey` and `Stripe:WebhookSecret` in appsettings / User Secrets / env vars
- A webhook endpoint in the [Stripe Dashboard](https://dashboard.stripe.com/) pointing at your gateway or API (e.g. `POST /api/v1/payments/webhook`) with events such as `checkout.session.completed` and `checkout.session.expired`
- `SuccessUrlTemplate` / `CancelUrlTemplate` pointing at your frontend order page

Without Stripe keys and webhook setup, payment actions will fail or stay in pending state.

---

## CI/CD

GitHub Actions (`.github/workflows/ci-cd.yml`):

1. `dotnet build` + unit/integration tests
2. Docker build → Azure Container Registry (`logistics-api`, `logistics-gateway`)
3. Deploy to Azure Container Apps (API min 2 replicas)

---

## Key paths

| What | File |
|------|------|
| Entry point | `LogisticsPlatform.Web/Program.cs` |
| DI | `LogisticsPlatform.Infrastructure/DependencyInjection.cs` |
| CQRS | `LogisticsPlatform.Application/DependencyInjection.cs` |
| Routes | `LogisticsPlatform.Web/ApiRoutes/ApiRoutes.cs` |
| DbContext | `LogisticsPlatform.Infrastructure/Database/AppDbContext.cs` |
| Seed | `LogisticsPlatform.Infrastructure/Database/Seed/SeedData.cs` |
| Ocelot dev | `ReversProxy/OcelotGateway/ocelot.json` |
| Ocelot prod | `ReversProxy/OcelotGateway/ocelot.Production.json` |
