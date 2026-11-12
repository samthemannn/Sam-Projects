# Antigravity Vertical Slice Execution Queue

- [x] **Slice 1: Canonical Catalog Entities, PostgreSQL Schema & 50-Item Seed Catalog**
  - Entities: `Book`, `Series`, `Author`, `Genre`, `BookAuthor`, `BookGenre`, `SeriesBook`, `AudiobookWork`, `ExternalBookReference`
  - Seed: 25 required series + 25 required standalone books (`seed-catalog.json` + `CatalogSeeder.cs`)
- [x] **Slice 2: Authoritative Rating Engine & Series Aggregation**
  - `1.0`–`10.0` decimal validation (1 decimal precision)
  - Data-driven `RatingCategory` & `UserBookCategoryScore`
  - Arithmetic mean book aggregate & rating-count-weighted series aggregate
  - `"Not yet rated"` (`null` score) zero-rating state
- [x] **Slice 3: Deterministic Recommendation Engine & Provider Ingestion**
  - `CandidateGenerator`, `RecommendationRanker`, `RecommendationExplainer`
  - Deterministic explanations (e.g., `"Because you rated Dungeon Crawler Carl highly."`)
  - `BookMetadataImportService` with `ImportRun` audit tracking & normalized deduplication
- [~] **Slice 4: ASP.NET Core API Endpoints, Reviews/Spoilers, Social & Ownership Auth**
  - API scaffold currently includes health, book search, series listing, and review reads.
  - Remaining: authentication, rating writes, reviews/comments writes, follows, shelves, imports, recommendations, and ownership policies.
- [~] **Slice 5: Desktop-First React/TypeScript Frontend & Contract Verification Suite**
  - Frontend shell, score panel, spoiler review card, and contract verification are included.
  - Remaining: API client wiring, authentication screens, rating editor, shelves, recommendations, and end-to-end browser tests.
