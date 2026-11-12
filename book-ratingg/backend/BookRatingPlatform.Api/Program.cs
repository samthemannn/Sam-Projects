using BookRatingPlatform.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookRatingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseExceptionHandler();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "book-rating-platform-api" }));

app.MapGet("/api/books", async (string? search, int? limit, BookRatingDbContext db, CancellationToken cancellationToken) =>
{
    var take = Math.Clamp(limit ?? 24, 1, 100);
    var query = db.Books.AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(search))
    {
        var normalized = search.Trim().ToLower();
        query = query.Where(book => book.Title.ToLower().Contains(normalized));
    }

    var books = await query.OrderBy(book => book.Title).Take(take)
        .Select(book => new { book.Id, book.Title, book.CoverImageUrl, book.PublicationYear })
        .ToListAsync(cancellationToken);
    return Results.Ok(books);
});

app.MapGet("/api/series", async (int? limit, BookRatingDbContext db, CancellationToken cancellationToken) =>
{
    var take = Math.Clamp(limit ?? 24, 1, 100);
    var series = await db.Series.AsNoTracking().OrderBy(item => item.Title).Take(take)
        .Select(item => new { item.Id, item.Title, item.Description, bookCount = item.SeriesBooks.Count })
        .ToListAsync(cancellationToken);
    return Results.Ok(series);
});

app.MapGet("/api/books/{bookId}/reviews", async (string bookId, BookRatingDbContext db, CancellationToken cancellationToken) =>
{
    var reviews = await db.Reviews.AsNoTracking().Where(review => review.BookId == bookId)
        .OrderByDescending(review => review.CreatedAtUtc)
        .Select(review => new { review.Id, review.UserId, review.BookId, review.Body, review.IsSpoiler, review.CreatedAtUtc })
        .ToListAsync(cancellationToken);
    return Results.Ok(reviews);
});

app.Run();

public partial class Program;

