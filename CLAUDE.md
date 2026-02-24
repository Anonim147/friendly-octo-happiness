# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Travel Buddy** (TravelGuideApi) — a .NET 8 Web API with a React TypeScript frontend that provides country information and currency comparison features.

## Build & Test Commands

All commands run from the repo root unless noted.

```bash
# Build
dotnet build TravelGuideApi/TravelGuideApi.sln

# Run all tests
dotnet test TravelGuideApi/TravelGuideApi.sln

# Run a specific test project
dotnet test TravelGuideApi/Tests/TravelGuideApi.Application.UnitTests/TravelGuideApi.Application.UnitTests.csproj

# Run a single test class
dotnet test TravelGuideApi/Tests/TravelGuideApi.Application.UnitTests/TravelGuideApi.Application.UnitTests.csproj --filter "ClassName=CountryGetAllQueryHandlerTests"

# Format check
dotnet format --verify-no-changes TravelGuideApi/TravelGuideApi.sln
```

Frontend (React):
```bash
cd TravelGuideApi/TravelGuideUI
npm install
npm start        # Dev server on port 3000, proxies to http://localhost:5000
npm run build
```

The main `TravelGuideApi.csproj` auto-runs `npm install --verbose` during build if `node_modules` is missing.

## Architecture

Clean Architecture with four backend layers under `TravelGuideApi/Src/`:

- **TravelGuideApi** — ASP.NET Core host, controllers, middleware, SPA proxy
- **TravelGuideApi.Application** — CQRS handlers (MediatR), FluentValidation validators, AutoMapper profiles
- **TravelGuideApi.Domain** — Entities (`Country`, `Currency`, `CurrencyComparison`) and domain interfaces
- **TravelGuideApi.Infrastructure** — HTTP clients for external APIs (RestCountries.com, ER-API.com), caching
- **TravelGuideApi.Persistence** — Data access layer
- **TravelGuideUI** — React 18 + TypeScript frontend (`src/components/`, `src/services/`, `src/types/`)

Test projects live under `TravelGuideApi/Tests/`:
- `TravelGuideApi.UnitTests` — basic unit tests (MSTest)
- `TravelGuideApi.Application.UnitTests` — application layer tests (NUnit, Moq, AwesomeAssertions)
- `TravelGuideApi.IntegrationTests` — integration tests

Key libraries: MediatR (CQRS), AutoMapper, FluentValidation, Serilog, Swashbuckle/NSwag (OpenAPI), AspNetCore.SpaProxy.

# Claude Code Workflow Instructions

## MANDATORY: Follow this workflow for EVERY feature request — no exceptions.

### Phase 1: Before Starting Any Work
1. `git checkout master`
2. `git reset --hard HEAD && git clean -fd`
3. `git pull origin master`
4. `dotnet build TravelGuideApi/TravelGuideApi.sln`
5. `dotnet test TravelGuideApi/TravelGuideApi.sln`
6. Confirm build and tests pass before proceeding. If they fail, fix the failure and proceed.

### Phase 2: Create a Feature Branch
6. `git checkout -b feature/<short-descriptive-name>`

### Phase 3: Implement Changes
7. Make all necessary code changes on the feature branch. And add the tests to cover new feature.

### Phase 4: Verify After Changes
8. `dotnet build TravelGuideApi/TravelGuideApi.sln`
9. `dotnet test TravelGuideApi/TravelGuideApi.sln`
10. If build or tests fail, fix the issues before considering the task complete.
11. Build and start an app. Use Playwright MCP to confirm visual changes.
12. Report the final build and test status in your summary.
13. Update CLAUDE.md only if the architecture or build & test commands have changed.
14. Commit all changes.
15. Push branch to origin and create a PR in Github.
16. Pick one of the repository contributors and tag them as mandatory reviewer in PR.

