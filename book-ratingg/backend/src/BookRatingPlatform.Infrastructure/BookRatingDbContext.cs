using BookRatingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookRatingPlatform.Infrastructure;

public sealed class BookRatingDbContext(DbContextOptions<BookRatingDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Series> Series => Set<Series>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Shelf> Shelves => Set<Shelf>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>().HasKey(x => x.Id);
        modelBuilder.Entity<Book>().HasIndex(x => x.NormalizedTitle);
        modelBuilder.Entity<Series>().HasKey(x => x.Id);
        modelBuilder.Entity<Author>().HasKey(x => x.Id);
        modelBuilder.Entity<Genre>().HasKey(x => x.Id);

        modelBuilder.Entity<BookAuthor>().HasKey(x => new { x.BookId, x.AuthorId });
        modelBuilder.Entity<BookGenre>().HasKey(x => new { x.BookId, x.GenreId });
        modelBuilder.Entity<SeriesBook>().HasKey(x => new { x.SeriesId, x.BookId });
        modelBuilder.Entity<SeriesBook>().Property(x => x.SeriesNumber).HasPrecision(5, 1);

        modelBuilder.Entity<Rating>().HasKey(x => x.Id);
        modelBuilder.Entity<Rating>().HasIndex(x => new { x.UserId, x.BookId }).IsUnique();
        modelBuilder.Entity<Rating>().Property(x => x.OverallScore).HasPrecision(3, 1);
        modelBuilder.Entity<Rating>().Property(x => x.RowVersion).IsRowVersion();

        modelBuilder.Entity<UserBookCategoryScore>().HasKey(x => new { x.RatingId, x.RatingCategoryId });
        modelBuilder.Entity<UserBookCategoryScore>().Property(x => x.Score).HasPrecision(3, 1);
        modelBuilder.Entity<Review>().HasIndex(x => new { x.UserId, x.BookId }).IsUnique();
        modelBuilder.Entity<UserFollow>().HasKey(x => new { x.FollowerUserId, x.FolloweeUserId });
        modelBuilder.Entity<Shelf>().HasIndex(x => new { x.UserId, x.Name }).IsUnique();
        modelBuilder.Entity<ShelfBook>().HasKey(x => new { x.ShelfId, x.BookId });

        modelBuilder.Entity<BookAuthor>().HasOne(x => x.Book).WithMany(x => x.BookAuthors).HasForeignKey(x => x.BookId);
        modelBuilder.Entity<BookAuthor>().HasOne(x => x.Author).WithMany(x => x.BookAuthors).HasForeignKey(x => x.AuthorId);
        modelBuilder.Entity<BookGenre>().HasOne(x => x.Book).WithMany(x => x.BookGenres).HasForeignKey(x => x.BookId);
        modelBuilder.Entity<BookGenre>().HasOne(x => x.Genre).WithMany(x => x.BookGenres).HasForeignKey(x => x.GenreId);
        modelBuilder.Entity<SeriesBook>().HasOne(x => x.Series).WithMany(x => x.SeriesBooks).HasForeignKey(x => x.SeriesId);
        modelBuilder.Entity<SeriesBook>().HasOne(x => x.Book).WithMany(x => x.SeriesPlacements).HasForeignKey(x => x.BookId);
        modelBuilder.Entity<AudiobookWork>().HasOne(x => x.Book).WithOne(x => x.AudiobookWork).HasForeignKey<AudiobookWork>(x => x.BookId);
        modelBuilder.Entity<ExternalBookReference>().HasOne(x => x.Book).WithMany(x => x.ExternalReferences).HasForeignKey(x => x.BookId);
        modelBuilder.Entity<Rating>().HasOne(x => x.Book).WithMany(x => x.Ratings).HasForeignKey(x => x.BookId);
        modelBuilder.Entity<Review>().HasOne(x => x.Book).WithMany(x => x.Reviews).HasForeignKey(x => x.BookId);
        modelBuilder.Entity<ReviewComment>().HasOne<Review>().WithMany(x => x.Comments).HasForeignKey(x => x.ReviewId);
        modelBuilder.Entity<ShelfBook>().HasOne<Shelf>().WithMany(x => x.ShelfBooks).HasForeignKey(x => x.ShelfId);
    }
}

