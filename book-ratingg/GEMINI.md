# Gemini Agent Package — Book Rating Platform

These files are the Gemini-specific implementation instructions for the book-rating platform. Gemini must read `GEMINI-PROJECT-CONTEXT.md` and `gemini-collaboration-handoff.md` before coding.

Gemini is expected to produce code, migrations, tests, and documentation. It must not add ML, LLM calls, chat plugins, external AI accounts, or paid token usage.

## Package Index

| File | Purpose |
| :--- | :--- |
| `GEMINI.md` | Entrypoint, system coordinator workflow, and vertical-slice execution rules |
| `GEMINI-PROJECT-CONTEXT.md` | Full product requirements, catalog seed list (25 series + 25 standalones), rating rules, deterministic recommendation contract, backend/frontend/ingestion/security contracts |
| `gemini-collaboration-handoff.md` | Domain ownership boundaries, handoff reporting template, decision log, and completed vertical-slice handoff records |

## Gemini System Coordinator

Coordinate Gemini's implementation work for the book-rating platform:

1. **Repository Inspection**: Before coding, inspect the repository and identify existing conventions.
2. **Vertical Slices**: Work in small vertical slices:
   - **Slice 1**: Canonical Domain Entities, PostgreSQL/EF Core Schema, Migrations & 50-Item Seed Catalog (25 Series + 25 Standalone Books)
   - **Slice 2**: Authoritative Rating Service (`1.0`–`10.0` one-decimal validation, optional data-driven Category Scores, Book Arithmetic Mean, Rating-Count-Weighted Series Aggregate)
   - **Slice 3**: Deterministic Recommendation Service (Candidate Generation, Multi-Signal Ranking, Human-Readable Explanations)
   - **Slice 4**: ASP.NET Core API Controllers/Endpoints, Ownership Authorization, Reviews (with Spoiler Flags), Comments, Follows, Shelves & Provider Ingestion
   - **Slice 5**: Desktop-First React/TypeScript Frontend & End-to-End Verification Suite
3. **Source of Truth**: Keep backend calculations authoritative and keep API contracts documented.
4. **Contract Integrity**: Do not silently change rating rules, entity names, or exclusions.
5. **Task Reporting**: For every completed task, report in `gemini-collaboration-handoff.md`:
   - Objective
   - Files changed
   - Database or API changes
   - Tests run and results
   - Known risks
   - Follow-up work
6. **Escalation Rules**: Escalate conflicts involving score formulas, authorization, external data licensing, or scope.