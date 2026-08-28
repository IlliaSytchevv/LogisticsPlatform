# LogisticPlatformFrontend

Client cabinet for the logistics platform (Freitty design). Built with **Next.js 15** App Router. Talks to the backend through the **Ocelot gateway** and a same-origin BFF proxy.

Backend README: [`../README.md`](../README.md)

---

## Tech stack

| Layer | Technology |
|-------|------------|
| Framework | Next.js 15.5, React 19, TypeScript 5.7 |
| Data fetching | TanStack Query v5 |
| Auth | Server Actions + httpOnly JWT cookies + Next middleware |
| Client state | Zustand (auth store) |
| JWT parsing | jose |
| Styling | Custom CSS (`globals.css`, `freitty-detail.css`) — Freitty wireframe 1:1 |
| Mobile | CSS `@media (max-width: 1100px)` + `useMediaQuery` (drawer nav, stacked layouts) |
| Build | `output: "standalone"` for Docker |

Tailwind CSS is installed but **not used** for layout — UI relies on semantic CSS classes (`fc-shell`, `order-card`, etc.).

---

## Project structure

```
src/
├── app/
│   ├── layout.tsx              # Root layout + QueryProvider
│   ├── page.tsx                # / → redirect /login
│   ├── globals.css             # Main cabinet styles
│   ├── freitty-detail.css      # Order detail page
│   ├── api/backend/[...path]/  # BFF proxy → gateway
│   ├── (auth)/login/           # Login page
│   └── (cabinet)/              # Authenticated shell
│       ├── dashboard/
│       ├── orders/
│       ├── orders/[id]/
│       └── settings/
├── actions/auth/               # login, logout, refresh, get-session
├── api/
│   ├── fetcher.ts              # Browser → /api/backend/*
│   └── services/               # orders, dashboard, payments, …
├── components/freitty/           # Shell, search, notifications, help
├── hooks/                      # use-session, use-logout, use-media-query
└── middleware.ts               # Route protection
```

---

## Routes

| URL | Page |
|-----|------|
| `/login` | Sign in |
| `/dashboard` | KPIs, active orders, activity (default after login) |
| `/orders` | Order list — tabs, filters, cards/table, export, new order |
| `/orders/[id]` | Order detail — edit, operations, supplies, photos, timeline, BOL/QR, payment |
| `/settings` | Placeholder |

Same URL for **desktop and mobile** — one bundle; layout adapts via CSS and conditional render.

---

## Architecture

```
Browser (client components)
    ↓  fetch("/api/backend/api/v1/...")
Next.js BFF (reads httpOnly cookie, adds Bearer, refreshes on 401)
    ↓
Ocelot Gateway :5124
    ↓
LogisticsPlatform.Web :5217

Login / logout / refresh (Server Actions)
    ↓  direct fetch
Ocelot Gateway :5124
```

- **Browser never calls the API on `:5217` directly** — always same-origin BFF or server actions via gateway.
- Access token stays in an **httpOnly** cookie; client JS does not store JWT in `localStorage`.

---

## Auth flow

1. User submits login → `loginAction` POSTs to `{API_BASE_URL}/api/v1/auth/login`.
2. Server sets cookies: `access_token` (~35 min), `refresh_token` (~7 days).
3. Middleware checks JWT on every page request; missing/expired → `/login?next=…`.
4. Logged-in user on `/` or `/login` → redirect to `/dashboard`.
5. BFF proxy refreshes access token on **401** via `POST /api/v1/auth/refresh-token`.

**Roles:** `Admin`, `Dispatcher`, `Driver` — exposed via `useSession()` (`canWrite`, `isDriver`, etc.).

**Seed users** (from backend seed): `AdminUser` / `DispatcherUser` / `DriverUser`, password `Test123!`.

---

## API services

All data services use `src/api/fetcher.ts` → `/api/backend{path}`:

| Service | Domain |
|---------|--------|
| `dashboard.service.ts` | Metrics, active orders, activity |
| `orders.service.ts` | List, detail, update, operations, supplies, photos, comments, edit lock |
| `payments.service.ts` | Stripe checkout |
| `supplies.service.ts` | Catalog |
| `notifications.service.ts` | Feed |

API prefix: `/api/v1` (`src/lib/api/routes.ts`).

---

## Environment variables

No `.env.example` in repo. Create `.env.local` if needed:

```env
# Gateway URL (required for local full stack)
NEXT_PUBLIC_API_URL=http://localhost:5124

# Optional server-only override (same value)
API_BASE_URL=http://localhost:5124

# Optional cookie names (defaults: access_token, refresh_token)
# ACCESS_TOKEN_COOKIE=access_token
# REFRESH_TOKEN_COOKIE=refresh_token
```

| Variable | Scope | Default |
|----------|-------|---------|
| `NEXT_PUBLIC_API_URL` | Client + build (Docker ARG) | `http://localhost:5124` |
| `API_BASE_URL` | Server only | same fallback chain |

`NEXT_PUBLIC_*` is baked into the client bundle at **build time**. Change it → rebuild for production/Docker.

---

## Local setup

### Prerequisites

- Node.js 22+ (Dockerfile uses `node:22-alpine`)
- Running backend: **gateway on :5124** (and API on :5217) — see [backend README](../README.md)
- Docker for Postgres / Redis / Azurite if you use the full stack

### 1. Install dependencies

```powershell
cd LogisticPlatformFrontend
npm ci
```

### 2. Start backend (separate terminals)

```powershell
# From repo root
docker compose up -d

dotnet run --project LogisticsPlatform.Web --launch-profile http
dotnet run --project ReversProxy/OcelotGateway --launch-profile http
```

Optional seed:

```powershell
curl -X POST http://localhost:5217/api/v1/seed
```

### 3. Start frontend

```powershell
cd LogisticPlatformFrontend
$env:NEXT_PUBLIC_API_URL="http://localhost:5124"
npm run dev
```

- App: **http://localhost:3000**
- Login: **http://localhost:3000/login**
- After login → **http://localhost:3000/dashboard**

Dev server uses **Turbopack** (`next dev --turbopack`).

### 4. Production-like local run

```powershell
$env:NEXT_PUBLIC_API_URL="http://localhost:5124"
npm run build
npm run start
```

### 5. Docker

```powershell
docker build -t logistics-web `
  --build-arg NEXT_PUBLIC_API_URL=http://localhost:5124 `
  .

docker run -p 3000:3000 logistics-web
```

---

## Notes for local development

**Gateway RoundRobin:** Dev Ocelot routes to API ports `5217` and `5218`. If only one API instance is running, every other request through the gateway may return **502**. Either start a second API (`--launch-profile http-5218`) or call the API directly only for debugging — the frontend is designed to use the gateway.

**Stripe payments:** Checkout redirects to Stripe. Configure keys on the **backend** and a Stripe webhook; the frontend only opens the returned `checkoutUrl`.

**Mobile testing:** Chrome DevTools → device toolbar (`Ctrl+Shift+M`) or resize below **1100px** to see the mobile drawer (☰) and stacked layouts.

---

## Scripts

| Command | Description |
|---------|-------------|
| `npm run dev` | Dev server with Turbopack (port 3000) |
| `npm run build` | Production build (webpack) |
| `npm run start` | Serve production build |
| `npm run lint` | ESLint |

---

## CI/CD

On push to `master`, GitHub Actions builds a Docker image `logistics-web` with:

```yaml
build-args:
  NEXT_PUBLIC_API_URL: ${{ secrets.NEXT_PUBLIC_API_URL }}
```

Deploys to Azure Container Apps (`ca-logistics-web`). See `.github/workflows/ci-cd.yml`.

---

## Key files

| Topic | Path |
|-------|------|
| Middleware | `src/middleware.ts` |
| BFF proxy | `src/app/api/backend/[...path]/route.ts` |
| Gateway base URL | `src/lib/api/base-url.ts` |
| Login action | `src/actions/auth/login.action.ts` |
| Cabinet shell | `src/components/freitty/freitty-shell.tsx` |
| Order detail | `src/app/(cabinet)/orders/[id]/page.tsx` |
| Dockerfile | `Dockerfile` |
| Next config | `next.config.ts` |
