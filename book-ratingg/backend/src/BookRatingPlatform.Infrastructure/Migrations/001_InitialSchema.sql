-- 001_InitialSchema.sql
-- Book Rating Platform PostgreSQL Schema (Authoritative Backend Persistence)

CREATE TABLE IF NOT EXISTS authors (
    id VARCHAR(64) PRIMARY KEY,
    name VARCHAR(256) NOT NULL,
    normalized_name VARCHAR(256) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS genres (
    id VARCHAR(64) PRIMARY KEY,
    name VARCHAR(128) NOT NULL,
    slug VARCHAR(128) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS series (
    id VARCHAR(64) PRIMARY KEY,
    title VARCHAR(256) NOT NULL,
    normalized_title VARCHAR(256) NOT NULL UNIQUE,
    description TEXT NULL,
    is_seed_required BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS books (
    id VARCHAR(64) PRIMARY KEY,
    title VARCHAR(256) NOT NULL,
    normalized_title VARCHAR(256) NOT NULL,
    isbn13 VARCHAR(13) NULL UNIQUE,
    publication_year INT NULL,
    description TEXT NULL,
    cover_image_url TEXT NULL,
    has_local_corrections BOOLEAN NOT NULL DEFAULT FALSE,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS book_authors (
    book_id VARCHAR(64) NOT NULL REFERENCES books(id) ON DELETE CASCADE,
    author_id VARCHAR(64) NOT NULL REFERENCES authors(id) ON DELETE CASCADE,
    author_order INT NOT NULL DEFAULT 1,
    PRIMARY KEY (book_id, author_id)
);

CREATE TABLE IF NOT EXISTS book_genres (
    book_id VARCHAR(64) NOT NULL REFERENCES books(id) ON DELETE CASCADE,
    genre_id VARCHAR(64) NOT NULL REFERENCES genres(id) ON DELETE CASCADE,
    PRIMARY KEY (book_id, genre_id)
);

CREATE TABLE IF NOT EXISTS series_books (
    series_id VARCHAR(64) NOT NULL REFERENCES series(id) ON DELETE CASCADE,
    book_id VARCHAR(64) NOT NULL REFERENCES books(id) ON DELETE CASCADE,
    series_number NUMERIC(5,1) NOT NULL,
    PRIMARY KEY (series_id, book_id)
);

CREATE TABLE IF NOT EXISTS audiobook_works (
    id VARCHAR(64) PRIMARY KEY,
    book_id VARCHAR(64) NOT NULL UNIQUE REFERENCES books(id) ON DELETE CASCADE,
    narrator VARCHAR(256) NOT NULL,
    duration_minutes INT NOT NULL CHECK (duration_minutes > 0),
    reliable_source_id VARCHAR(128) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS external_book_references (
    id VARCHAR(64) PRIMARY KEY,
    book_id VARCHAR(64) NOT NULL REFERENCES books(id) ON DELETE CASCADE,
    provider_name VARCHAR(64) NOT NULL,
    external_id VARCHAR(128) NOT NULL,
    attribution_text TEXT NOT NULL,
    last_synced_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE (provider_name, external_id)
);

CREATE TABLE IF NOT EXISTS rating_categories (
    id VARCHAR(64) PRIMARY KEY,
    key VARCHAR(64) NOT NULL UNIQUE,
    label VARCHAR(128) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS ratings (
    id VARCHAR(64) PRIMARY KEY,
    user_id VARCHAR(64) NOT NULL,
    book_id VARCHAR(64) NOT NULL REFERENCES books(id) ON DELETE CASCADE,
    overall_score NUMERIC(3,1) NOT NULL CHECK (overall_score >= 1.0 AND overall_score <= 10.0),
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE (user_id, book_id)
);

CREATE TABLE IF NOT EXISTS user_book_category_scores (
    rating_id VARCHAR(64) NOT NULL REFERENCES ratings(id) ON DELETE CASCADE,
    rating_category_id VARCHAR(64) NOT NULL REFERENCES rating_categories(id) ON DELETE RESTRICT,
    score NUMERIC(3,1) NOT NULL CHECK (score >= 1.0 AND score <= 10.0),
    PRIMARY KEY (rating_id, rating_category_id)
);

CREATE TABLE IF NOT EXISTS reviews (
    id VARCHAR(64) PRIMARY KEY,
    user_id VARCHAR(64) NOT NULL,
    book_id VARCHAR(64) NOT NULL REFERENCES books(id) ON DELETE CASCADE,
    body TEXT NOT NULL,
    is_spoiler BOOLEAN NOT NULL DEFAULT FALSE,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE (user_id, book_id)
);

CREATE TABLE IF NOT EXISTS review_comments (
    id VARCHAR(64) PRIMARY KEY,
    review_id VARCHAR(64) NOT NULL REFERENCES reviews(id) ON DELETE CASCADE,
    user_id VARCHAR(64) NOT NULL,
    body TEXT NOT NULL,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS user_follows (
    follower_user_id VARCHAR(64) NOT NULL,
    followee_user_id VARCHAR(64) NOT NULL CHECK (follower_user_id <> followee_user_id),
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    PRIMARY KEY (follower_user_id, followee_user_id)
);

CREATE TABLE IF NOT EXISTS shelves (
    id VARCHAR(64) PRIMARY KEY,
    user_id VARCHAR(64) NOT NULL,
    name VARCHAR(128) NOT NULL,
    UNIQUE (user_id, name)
);

CREATE TABLE IF NOT EXISTS shelf_books (
    shelf_id VARCHAR(64) NOT NULL REFERENCES shelves(id) ON DELETE CASCADE,
    book_id VARCHAR(64) NOT NULL REFERENCES books(id) ON DELETE CASCADE,
    added_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    PRIMARY KEY (shelf_id, book_id)
);

CREATE TABLE IF NOT EXISTS import_runs (
    id VARCHAR(64) PRIMARY KEY,
    provider_name VARCHAR(64) NOT NULL,
    started_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    completed_at_utc TIMESTAMPTZ NULL,
    records_processed INT NOT NULL DEFAULT 0,
    records_deduplicated INT NOT NULL DEFAULT 0,
    failures_count INT NOT NULL DEFAULT 0,
    failure_log_json JSONB NULL
);
