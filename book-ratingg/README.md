# Book Rating Platform

A desktop-first book and series rating platform featuring *A Court of Thorns and Roses*, *Dungeon Crawler Carl*, and 48 additional seed titles.

## Current status

The contract layer, seed catalog, rating engine, deterministic recommendation engine, metadata-ingestion service, backend scaffold, frontend shell, and verification suite are included.

The application deliberately uses no ML, LLM calls, chat plugins, external AI accounts, or paid token services. Recommendations are generated from local catalog and rating data.

## Repository layout

```text
backend/                 ASP.NET Core API and domain/application code
frontend/                React + TypeScript client
docs/contracts/          Agent and implementation contracts
verification/            Dependency-free contract tests
.antigravity/            Execution rules and task queue
```

## Prerequisites

- .NET 8 SDK
- Node.js 20+
- Docker Desktop (recommended for PostgreSQL)

## Start PostgreSQL

```bash
docker compose up -d db
```

The default connection string is available in `backend/.env.example`.

## Run the API

```bash
cd backend
dotnet restore BookRatingPlatform.Api/BookRatingPlatform.Api.csproj
dotnet run --project BookRatingPlatform.Api/BookRatingPlatform.Api.csproj
```

The API exposes `/health`, `/api/books`, `/api/series`, and `/api/recommendations`. The OpenAPI document is available in development mode.

## Run the frontend

```bash
cd frontend
npm install
npm run dev
```

## Verify the contract layer

```bash
python3 verification/run_contract_tests.py
```

## Important product rules

- Users rate individual books from 1.0 to 10.0 in 0.1 increments.
- Category ratings are optional and user-selected.
- Series scores aggregate contributing book ratings by rating count.
- Public scores always include rating counts.
- Reading progress, reading status, and reading groups are not part of the product.

