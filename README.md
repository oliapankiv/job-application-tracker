# Job Application Tracker

A full-stack app for tracking job applications through the hiring pipeline — log applications, move them across a Kanban board as they progress, keep recruiter contacts and follow-up reminders attached to each one, and see response-rate stats on a dashboard.

## Features

- **Applications** — company, role, source, salary, location, work type, applied date, notes, tags
- **Kanban board** — drag applications between Applied → Screening → Interview → Offer → Rejected (Angular CDK drag-and-drop), backed by a status-change history/timeline
- **Contacts** — recruiter/interviewer name, role, email, phone, LinkedIn per application
- **Reminders** — follow-up due dates with an "upcoming reminders" view
- **Dashboard** — total/this-month/this-week counts, response rate, active interviews, offers, weekly trend and status-breakdown charts
- **Search & filter** — by company/title text search and status
- **Auth** — email/password registration and login, JWT-protected API, data scoped per user

## Stack

**Backend** — `backend/`
- .NET 10 Web API (ASP.NET Core, controllers)
- Entity Framework Core 10 + **SQL Server**
- ASP.NET Core Identity + JWT bearer authentication
- FluentValidation for request validation
- Scalar for interactive OpenAPI docs (dev only)

**Frontend** — `frontend/`
- Angular 21 (standalone components, signals)
- Angular Material + Angular CDK (drag-and-drop for the Kanban board)
- ngx-charts for dashboard visualizations
- Reactive Forms, JWT auth interceptor/guard

## Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 20+ and npm
- [Angular CLI](https://angular.dev/tools/cli): `npm install -g @angular/cli`
- [Docker](https://www.docker.com/) (to run SQL Server locally) — or your own SQL Server instance

## Running it locally

### 1. Database (SQL Server)

From `backend/`, create a `.env` file with the `sa` password Docker and the app will use:

```bash
cd backend
echo "MSSQL_SA_PASSWORD=<your-strong-password>" > .env
```

Then start SQL Server via Docker Compose:

```bash
docker compose up -d
```

This runs SQL Server 2022 in a container, exposed on **host port 1434** (mapped to avoid clashing with any SQL Server instance you might already have on the default 1433). Wait about 30 seconds for it to report healthy:

```bash
docker compose ps
```

If you'd rather point at your own SQL Server instance, just update the `ConnectionStrings:DefaultConnection` value in `backend/appsettings.json` (or `appsettings.Development.json`) instead of using the container.

### 2. Backend API

```bash
cd backend
dotnet tool install --global dotnet-ef   # first time only
dotnet ef database update                # applies migrations, creates the JobTracker database
dotnet run
```

The API listens on `http://localhost:5299` (see `backend/Properties/launchSettings.json`). In development, interactive API docs are available at `http://localhost:5299/scalar/v1`.

### 3. Frontend

In a separate terminal:

```bash
cd frontend
npm install
npm start
```

The app is served at `http://localhost:4200` and calls the API at `http://localhost:5299/api` (see `frontend/src/environments/environment.ts`). The backend allows this origin via CORS.

### 4. Use it

Open `http://localhost:4200`, register an account, and start adding applications.

## Configuration notes

- **JWT signing key**: `backend/appsettings.json` ships with a placeholder `Jwt:Key`. Replace it with a long random secret before deploying anywhere beyond local dev.
- **SQL Server password**: there's no built-in default — both the Docker container and the API read `MSSQL_SA_PASSWORD` from `backend/.env` (gitignored, not committed). Set it to whatever you like locally; just keep it out of source control.
- **CORS**: allowed origins for the API are configured under `Cors:AllowedOrigins` in `backend/appsettings.json` (defaults to `http://localhost:4200`).
