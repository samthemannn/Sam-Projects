# Gemini Project Context — Book Rating Platform

Build a desktop-first personal book-rating platform using **React/TypeScript**, **ASP.NET Core/C#**, **PostgreSQL**, and **Entity Framework Core**.

## 1. MVP Scope & Exclusions

- **Supported MVP Capabilities**: Accounts, book and series browsing, user-selected rating categories, `1.0`–`10.0` book ratings, reviews, spoiler flags, comments, follows, shelves, and deterministic recommendations.
- **Rating Model**: Users rate individual books. Series scores aggregate the books in the series. Public scores must always show the rating count.
- **Audiobook Rules**: Audiobooks are separate rateable works only when reliable audiobook metadata exists.
- **Strict Exclusions**: Reading progress, reading status, reading groups, ML, LLMs, chat plugins, external AI accounts, and paid token services are strictly excluded.
- **Seed Catalog Requirement**: The seed catalog must include 25 series and 25 standalone books, including *A Court of Thorns and Roses* and *Dungeon Crawler Carl*.

## 2. System Coordinator Rules

- **Inspect Before Coding**: Before coding, inspect the repository and identify existing conventions.
- **Small Vertical Slices**: Work in small, verifiable vertical slices.
- **Authoritative Backend**: Keep backend calculations authoritative and keep API contracts documented.
- **Contract Stability**: Do not silently change rating rules, entity names, or exclusions.
- **Task Reporting**: For every completed task, report files changed, API/schema changes, tests run, known risks, and recommended next task.
- **Escalation Triggers**: Escalate conflicts involving score formulas, authorization, external data licensing, or scope.

## 3. Product Requirements & User Flows

1. **Discovery & Search**: Browse or search for a book or series.
2. **Work & Series Detail Views**: View book score, rating count, authors, genres, series placement, reviews, and audiobook data when available.
3. **Overall Book Rating**: Submit or edit one overall rating from `1.0` to `10.0` (one decimal precision).
4. **Optional Category Scoring**: Optionally select and score user-selected rating categories without replacing the overall rating.
5. **Reviews, Spoilers & Comments**: Write a review, mark it as a spoiler, and comment on reviews. Spoiler reviews must remain hidden behind an explicit reveal action.
6. **Social & Shelves**: Follow users and add books to shelves.
7. **Deterministic Recommendations**: Receive local recommendations based on genres, rating history, popularity, and similar users.
8. **Access Control**: Unauthenticated users may browse public data but cannot create or mutate user content.

## 4. Catalog Handoff Specification

### Canonical Domain Entities
- `Book`
- `Series`
- `Author`
- `Genre`
- `BookAuthor`
- `BookGenre`
- `SeriesBook`
- `AudiobookWork`
- `ExternalBookReference`

### Required 25 Seed Series

| # | Series Name | Primary Author |
| :--- | :--- | :--- |
| 1 | A Court of Thorns and Roses | Sarah J. Maas |
| 2 | Harry Potter | J.K. Rowling |
| 3 | The Lord of the Rings | J.R.R. Tolkien |
| 4 | Mistborn | Brandon Sanderson |
| 5 | Dungeon Crawler Carl | Matt Dinniman |
| 6 | Bridgerton | Julia Quinn |
| 7 | Outlander | Diana Gabaldon |
| 8 | The Twisted Series | Ana Huang |
| 9 | Chestnut Springs | Elsie Silver |
| 10 | Off-Campus | Elle Kennedy |
| 11 | The Millennium Series | Stieg Larsson |
| 12 | The Harry Hole Series | Jo Nesbø |
| 13 | The Dublin Murder Squad | Tana French |
| 14 | The Lincoln Lawyer Series | Michael Connelly |
| 15 | The Thursday Murder Club | Richard Osman |
| 16 | The Hunger Games | Suzanne Collins |
| 17 | Red Rising | Pierce Brown |
| 18 | The Expanse | James S.A. Corey |
| 19 | Dune | Frank Herbert |
| 20 | The Murderbot Diaries | Martha Wells |
| 21 | The Kingsbridge Series | Ken Follett |
| 22 | The Wolf Hall Trilogy | Hilary Mantel |
| 23 | The Poldark Series | Winston Graham |
| 24 | The Shardlake Series | C.J. Sansom |
| 25 | The Maisie Dobbs Series | Jacqueline Winspear |

### Required 25 Seed Standalone Books

| # | Book Title | Author |
| :--- | :--- | :--- |
| 1 | The Night Circus | Erin Morgenstern |
| 2 | Piranesi | Susanna Clarke |
| 3 | Circe | Madeline Miller |
| 4 | The Invisible Life of Addie LaRue | V.E. Schwab |
| 5 | The Priory of the Orange Tree | Samantha Shannon |
| 6 | The Love Hypothesis | Ali Hazelwood |
| 7 | Beach Read | Emily Henry |
| 8 | Book Lovers | Emily Henry |
| 9 | Red, White & Royal Blue | Casey McQuiston |
| 10 | The Notebook | Nicholas Sparks |
| 11 | Gone Girl | Gillian Flynn |
| 12 | The Silent Patient | Alex Michaelides |
| 13 | The Girl on the Train | Paula Hawkins |
| 14 | The Guest List | Lucy Foley |
| 15 | The Woman in Cabin 10 | Ruth Ware |
| 16 | Project Hail Mary | Andy Weir |
| 17 | The Martian | Andy Weir |
| 18 | Klara and the Sun | Kazuo Ishiguro |
| 19 | Station Eleven | Emily St. John Mandel |
| 20 | Recursion | Blake Crouch |
| 21 | All the Light We Cannot See | Anthony Doerr |
| 22 | The Book Thief | Markus Zusak |
| 23 | Pachinko | Min Jin Lee |
| 24 | A Thousand Splendid Suns | Khaled Hosseini |
| 25 | The Nightingale | Kristin Hannah |

### Catalog Integrity Rules
- Preserve external IDs and source metadata in `ExternalBookReference`.
- Deduplicate by normalized title, normalized author, and source identifiers.
- Never store copyrighted book text.

## 5. Rating Contract

- **Single Active Rating per User/Book**: Enforce a unique constraint on `(UserId, BookId)` (or `(UserId, AudiobookWorkId)` when rating an audiobook work). Editing updates the existing record in place.
- **Bounds & Precision**: Accept only decimal values in `[1.0, 10.0]` with at most 1 decimal place (`numeric(3,1)` / `Math.Round(score * 10) == score * 10`).
- **Category Ratings**: Optional and user-selected. Stored in relational rows (`RatingCategory` + `UserBookCategoryScore`), never as hardcoded columns on `Rating`. Category scores never overwrite or auto-compute the user's overall rating.
- **Book Aggregate Formula**:
  $$\text{BookAggregate} = \frac{1}{N} \sum_{i=1}^{N} r_i \quad (\text{where } N = \text{RatingCount})$$
  When $N = 0$, return `score: null`, `ratingCount: 0`, and display state `"Not yet rated"`.
- **Series Aggregate Formula**:
  $$\text{SeriesAggregate} = \frac{\sum_{b \in S_{\text{rated}}} (\text{BookAggregate}_b \times \text{RatingCount}_b)}{\sum_{b \in S_{\text{rated}}} \text{RatingCount}_b}$$
  Always return `aggregateScore`, `totalRatingCount`, and `contributingBookCount` (number of books in the series with at least 1 valid rating). When `contributingBookCount == 0`, return `score: null` and `"Not yet rated"`.
- **Required Rating Test Cases**: Bounds (`< 1.0`, `> 10.0`), precision (`8.55` rejected, `8.5` accepted), duplicate prevention, in-place edits, zero ratings (`null` / `"Not yet rated"`), rating-count-weighted series score calculation, and concurrent submission safety.

## 6. Recommendation Contract

- **Zero External AI/ML**: Deterministic algorithm using only local PostgreSQL application data. No ML models, LLM calls, chat plugins, external AI accounts, or paid token usage.
- **Pipeline Separation**:
  1. `ICandidateGenerator`: Generates unseen candidate books excluding books already rated or shelved by the user.
  2. `IRecommendationRanker`: Computes deterministic composite scores from weighted signals:
     - **Genre Similarity** (overlap with genres of books rated $\ge 7.5$ by the user)
     - **Author & Series Affinity** (same author or next/sibling book in a series rated $\ge 8.0$ by the user)
     - **Similar-User Rating Patterns** (books rated $\ge 8.0$ by users with low mean absolute difference on co-rated books)
     - **Popularity Baseline** (Bayesian/log-damped rating count and aggregate score)
  3. `IRecommendationExplainer`: Builds human-readable deterministic explanations and structured `contributingSignals`.
- **Output Payload Fields**: `bookId`, `rank`, `score`, `reasonType`, `explanation` (e.g., `"Because you rated Dungeon Crawler Carl highly."`), and `contributingSignals`.

## 7. Backend, Frontend, Data Ingestion & Security Contracts

- **Backend (ASP.NET Core + EF Core + PostgreSQL)**:
  - Endpoints for `auth`, `books`, `series`, `search`, `ratings`, `rating-categories`, `reviews`, `comments`, `follows`, `shelves`, `imports`, and `recommendations`.
  - DTO request/response separation, fluent/attribute validation, cursor/page pagination, RFC 7807 `ProblemDetails` structured errors, OpenAPI documentation, and server-side ownership authorization.
  - Idempotent seed and migration support that preserves existing data.
- **Frontend (React + TypeScript)**:
  - Desktop-first layout with screens for Discovery, Search, Book Detail, Series Detail, Rating Editor, Reviews, Profiles, Shelves, Recommendations, and Authentication.
  - Displays the authenticated user's own rating separately from the public aggregate score and rating count.
  - Optional category score controls, explicit spoiler reveal toggle, and complete loading/empty/error/success states.
  - Zero frontend recalculation of book/series scores or recommendation explanations.
- **Data Ingestion**:
  - Provider-backed metadata import with `ImportRun` audit tracking (timestamps, status, failures, attribution, source IDs).
  - Deduplication by normalized title/author/external ID while preserving local manual corrections and guaranteeing all 50 seed entries remain available offline.
- **Security & Testing**:
  - Ownership checks on all mutating endpoints, HTML encoding/sanitization against XSS, parameterized EF Core queries against SQLi, CSRF protection where cookies are used, unique constraints against duplicate writes, and rate limiting against abusive write volume.
