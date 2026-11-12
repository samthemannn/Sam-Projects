using System;
using System.Collections.Generic;
using System.Linq;

namespace BookRatingPlatform.Application.Ratings;

public sealed record CategoryScoreInput(string CategoryKey, decimal Score);

public sealed record SubmitRatingCommand(
    string UserId,
    string BookId,
    decimal OverallScore,
    IReadOnlyList<CategoryScoreInput>? CategoryScores = null);

public sealed record BookAggregateResult(
    string BookId,
    decimal? AggregateScore,
    int RatingCount,
    string DisplayState,
    decimal? CurrentUserOverallScore,
    IReadOnlyDictionary<string, decimal>? CurrentUserCategoryScores);

public sealed record SeriesAggregateResult(
    string SeriesId,
    decimal? AggregateScore,
    int TotalRatingCount,
    int ContributingBookCount,
    int TotalSeriesBookCount,
    string DisplayState);

public static class RatingValidator
{
    public const decimal MinScore = 1.0m;
    public const decimal MaxScore = 10.0m;

    public static void ValidateScore(decimal score, string fieldName = "overallScore")
    {
        if (score < MinScore || score > MaxScore)
        {
            throw new ArgumentOutOfRangeException(
                fieldName,
                score,
                $"Score must be between {MinScore:0.0} and {MaxScore:0.0}.");
        }

        if (decimal.Round(score, 1) != score)
        {
            throw new ArgumentException(
                $"Score '{score}' must have at most one decimal place.",
                fieldName);
        }
    }
}

public sealed class RatingDomainCalculator
{
    public BookAggregateResult CalculateBookAggregate(
        string bookId,
        IReadOnlyCollection<(string UserId, decimal OverallScore, IReadOnlyDictionary<string, decimal> CategoryScores)> ratings,
        string? currentUserId = null)
    {
        if (ratings.Count == 0)
        {
            return new BookAggregateResult(
                BookId: bookId,
                AggregateScore: null,
                RatingCount: 0,
                DisplayState: "Not yet rated",
                CurrentUserOverallScore: null,
                CurrentUserCategoryScores: null);
        }

        decimal mean = decimal.Round(ratings.Average(r => r.OverallScore), 2, MidpointRounding.AwayFromZero);
        var userRecord = currentUserId is null
            ? default
            : ratings.FirstOrDefault(r => r.UserId == currentUserId);

        return new BookAggregateResult(
            BookId: bookId,
            AggregateScore: mean,
            RatingCount: ratings.Count,
            DisplayState: $"{mean:0.00} ({ratings.Count} ratings)",
            CurrentUserOverallScore: userRecord.UserId is null ? null : userRecord.OverallScore,
            CurrentUserCategoryScores: userRecord.UserId is null ? null : userRecord.CategoryScores);
    }

    public SeriesAggregateResult CalculateSeriesAggregate(
        string seriesId,
        IReadOnlyCollection<BookAggregateResult> seriesBookAggregates)
    {
        var contributing = seriesBookAggregates
            .Where(b => b.RatingCount > 0 && b.AggregateScore.HasValue)
            .ToList();

        if (contributing.Count == 0)
        {
            return new SeriesAggregateResult(
                SeriesId: seriesId,
                AggregateScore: null,
                TotalRatingCount: 0,
                ContributingBookCount: 0,
                TotalSeriesBookCount: seriesBookAggregates.Count,
                DisplayState: "Not yet rated");
        }

        int totalRatings = contributing.Sum(b => b.RatingCount);
        decimal weightedSum = contributing.Sum(b => b.AggregateScore!.Value * b.RatingCount);
        decimal weightedMean = decimal.Round(weightedSum / totalRatings, 2, MidpointRounding.AwayFromZero);

        return new SeriesAggregateResult(
            SeriesId: seriesId,
            AggregateScore: weightedMean,
            TotalRatingCount: totalRatings,
            ContributingBookCount: contributing.Count,
            TotalSeriesBookCount: seriesBookAggregates.Count,
            DisplayState: $"{weightedMean:0.00} across {contributing.Count} rated books ({totalRatings} ratings)");
    }
}
