using System;
using System.Collections.Generic;

namespace BookRatingPlatform.Domain.Entities;

public sealed class Book
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string NormalizedTitle { get; set; } = string.Empty;
    public string? Isbn13 { get; set; }
    public int? PublicationYear { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool HasLocalCorrections { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
    public ICollection<SeriesBook> SeriesPlacements { get; set; } = new List<SeriesBook>();
    public AudiobookWork? AudiobookWork { get; set; }
    public ICollection<ExternalBookReference> ExternalReferences { get; set; } = new List<ExternalBookReference>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

public sealed class Series
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string NormalizedTitle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSeedRequired { get; set; }

    public ICollection<SeriesBook> SeriesBooks { get; set; } = new List<SeriesBook>();
}

public sealed class Author
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}

public sealed class Genre
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
}

public sealed class BookAuthor
{
    public string BookId { get; set; } = string.Empty;
    public Book Book { get; set; } = null!;
    public string AuthorId { get; set; } = string.Empty;
    public Author Author { get; set; } = null!;
    public int AuthorOrder { get; set; } = 1;
}

public sealed class BookGenre
{
    public string BookId { get; set; } = string.Empty;
    public Book Book { get; set; } = null!;
    public string GenreId { get; set; } = string.Empty;
    public Genre Genre { get; set; } = null!;
}

public sealed class SeriesBook
{
    public string SeriesId { get; set; } = string.Empty;
    public Series Series { get; set; } = null!;
    public string BookId { get; set; } = string.Empty;
    public Book Book { get; set; } = null!;
    public decimal SeriesNumber { get; set; }
}

public sealed class AudiobookWork
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string BookId { get; set; } = string.Empty;
    public Book Book { get; set; } = null!;
    public string Narrator { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string ReliableSourceId { get; set; } = string.Empty;
}

public sealed class ExternalBookReference
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string BookId { get; set; } = string.Empty;
    public Book Book { get; set; } = null!;
    public string ProviderName { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public string AttributionText { get; set; } = string.Empty;
    public DateTimeOffset LastSyncedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class Rating
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string BookId { get; set; } = string.Empty;
    public Book Book { get; set; } = null!;
    public decimal OverallScore { get; set; }
    public uint RowVersion { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<UserBookCategoryScore> CategoryScores { get; set; } = new List<UserBookCategoryScore>();
}

public sealed class RatingCategory
{
    public string Id { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public sealed class UserBookCategoryScore
{
    public string RatingId { get; set; } = string.Empty;
    public Rating Rating { get; set; } = null!;
    public string RatingCategoryId { get; set; } = string.Empty;
    public RatingCategory RatingCategory { get; set; } = null!;
    public decimal Score { get; set; }
}

public sealed class Review
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string BookId { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsSpoiler { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<ReviewComment> Comments { get; set; } = new List<ReviewComment>();
}

public sealed class ReviewComment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ReviewId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class UserFollow
{
    public string FollowerUserId { get; set; } = string.Empty;
    public string FolloweeUserId { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class Shelf
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ICollection<ShelfBook> ShelfBooks { get; set; } = new List<ShelfBook>();
}

public sealed class ShelfBook
{
    public string ShelfId { get; set; } = string.Empty;
    public string BookId { get; set; } = string.Empty;
    public DateTimeOffset AddedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

public sealed class ImportRun
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProviderName { get; set; } = string.Empty;
    public DateTimeOffset StartedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAtUtc { get; set; }
    public int RecordsProcessed { get; set; }
    public int RecordsDeduplicated { get; set; }
    public int FailuresCount { get; set; }
    public string? FailureLogJson { get; set; }
}
