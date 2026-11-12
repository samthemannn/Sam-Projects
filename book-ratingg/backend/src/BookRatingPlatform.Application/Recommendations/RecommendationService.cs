using System;
using System.Collections.Generic;
using System.Linq;

namespace BookRatingPlatform.Application.Recommendations;

public sealed record ContributingSignal(
    string SignalType,
    decimal Weight,
    string SourceReference);

public sealed record RecommendationItemDto(
    string BookId,
    int Rank,
    decimal Score,
    string ReasonType,
    string Explanation,
    IReadOnlyList<ContributingSignal> ContributingSignals);

public sealed record BookMetadataSnapshot(
    string BookId,
    string Title,
    string PrimaryAuthor,
    string? SeriesId,
    string? SeriesTitle,
    IReadOnlyList<string> Genres,
    decimal? PublicAggregateScore,
    int PublicRatingCount);

public sealed record UserRatingSnapshot(
    string UserId,
    string BookId,
    decimal OverallScore);

public sealed class DeterministicRecommendationService
{
    public IReadOnlyList<RecommendationItemDto> RecommendForUser(
        string targetUserId,
        IReadOnlyCollection<BookMetadataSnapshot> catalog,
        IReadOnlyCollection<UserRatingSnapshot> allRatings,
        IReadOnlySet<string> userShelvedBookIds,
        int maxResults = 10)
    {
        var userRatings = allRatings
            .Where(r => r.UserId == targetUserId)
            .ToDictionary(r => r.BookId, r => r.OverallScore);

        var catalogById = catalog.ToDictionary(b => b.BookId);

        // Stage 1: Candidate Generation (exclude already rated or shelved books)
        var candidates = catalog
            .Where(b => !userRatings.ContainsKey(b.BookId) && !userShelvedBookIds.Contains(b.BookId))
            .ToList();

        // High-rated books by the target user (>= 8.0)
        var highRatedUserBooks = userRatings
            .Where(kv => kv.Value >= 8.0m && catalogById.ContainsKey(kv.Key))
            .OrderByDescending(kv => kv.Value)
            .Select(kv => (Book: catalogById[kv.Key], Score: kv.Value))
            .ToList();

        // Similar users (mean absolute difference <= 1.5 on co-rated books)
        var similarUsers = FindSimilarUsers(targetUserId, userRatings, allRatings);

        // Stage 2 & 3: Deterministic Ranking and Explanation
        var scored = new List<(BookMetadataSnapshot Book, decimal TotalScore, string PrimaryReasonType, string Explanation, List<ContributingSignal> Signals)>();

        foreach (var candidate in candidates)
        {
            var signals = new List<ContributingSignal>();
            string primaryReasonType = "POPULARITY";
            string explanation = $"Popular on the platform ({candidate.PublicRatingCount} ratings).";
            decimal bestSignalScore = 0m;

            // Signal A: Series/Author Affinity from highest-rated book
            var seriesOrAuthorMatch = highRatedUserBooks.FirstOrDefault(hr =>
                (hr.Book.SeriesId is not null && hr.Book.SeriesId == candidate.SeriesId) ||
                string.Equals(hr.Book.PrimaryAuthor, candidate.PrimaryAuthor, StringComparison.OrdinalIgnoreCase));

            if (seriesOrAuthorMatch.Book is not null)
            {
                decimal weight = seriesOrAuthorMatch.Book.SeriesId == candidate.SeriesId ? 4.5m : 3.5m;
                signals.Add(new ContributingSignal("AUTHOR_SERIES_AFFINITY", weight, seriesOrAuthorMatch.Book.BookId));
                if (weight > bestSignalScore)
                {
                    bestSignalScore = weight;
                    primaryReasonType = "HIGHLY_RATED_AFFINITY";
                    explanation = $"Because you rated {seriesOrAuthorMatch.Book.Title} highly.";
                }
            }

            // Signal B: Genre Overlap with highly rated books
            var genreMatch = highRatedUserBooks.FirstOrDefault(hr =>
                hr.Book.Genres.Intersect(candidate.Genres, StringComparer.OrdinalIgnoreCase).Any());

            if (genreMatch.Book is not null)
            {
                int sharedGenres = genreMatch.Book.Genres
                    .Intersect(candidate.Genres, StringComparer.OrdinalIgnoreCase)
                    .Count();
                decimal weight = Math.Min(3.0m, sharedGenres * 1.2m);
                signals.Add(new ContributingSignal("GENRE_SIMILARITY", weight, genreMatch.Book.BookId));
                if (weight > bestSignalScore)
                {
                    bestSignalScore = weight;
                    primaryReasonType = "GENRE_SIMILARITY";
                    explanation = $"Because you rated {genreMatch.Book.Title} highly.";
                }
            }

            // Signal C: Similar-user high ratings
            var similarUserLikes = allRatings
                .Where(r => r.BookId == candidate.BookId && r.OverallScore >= 8.0m && similarUsers.Contains(r.UserId))
                .ToList();

            if (similarUserLikes.Count > 0)
            {
                decimal weight = Math.Min(2.5m, similarUserLikes.Count * 1.25m);
                signals.Add(new ContributingSignal("SIMILAR_USERS", weight, $"similar_users:{similarUserLikes.Count}"));
                if (weight > bestSignalScore)
                {
                    bestSignalScore = weight;
                    primaryReasonType = "SIMILAR_USERS";
                    explanation = $"Recommended by {similarUserLikes.Count} reader(s) with similar rating history.";
                }
            }

            // Signal D: Popularity & Aggregate Quality Baseline
            if (candidate.PublicRatingCount > 0 && candidate.PublicAggregateScore.HasValue)
            {
                decimal popWeight = decimal.Round(
                    (candidate.PublicAggregateScore.Value / 10.0m) * Math.Min(2.0m, candidate.PublicRatingCount / 10.0m),
                    2);
                if (popWeight > 0)
                {
                    signals.Add(new ContributingSignal("POPULARITY", popWeight, $"ratings:{candidate.PublicRatingCount}"));
                }
            }

            decimal totalScore = decimal.Round(signals.Sum(s => s.Weight), 2);
            if (totalScore > 0)
            {
                scored.Add((candidate, totalScore, primaryReasonType, explanation, signals));
            }
        }

        return scored
            .OrderByDescending(x => x.TotalScore)
            .ThenBy(x => x.Book.Title, StringComparer.Ordinal)
            .Take(maxResults)
            .Select((item, idx) => new RecommendationItemDto(
                BookId: item.Book.BookId,
                Rank: idx + 1,
                Score: item.TotalScore,
                ReasonType: item.PrimaryReasonType,
                Explanation: item.Explanation,
                ContributingSignals: item.Signals))
            .ToList();
    }

    private static HashSet<string> FindSimilarUsers(
        string targetUserId,
        IReadOnlyDictionary<string, decimal> targetUserRatings,
        IReadOnlyCollection<UserRatingSnapshot> allRatings)
    {
        var similar = new HashSet<string>();
        if (targetUserRatings.Count == 0) return similar;

        var byUser = allRatings
            .Where(r => r.UserId != targetUserId)
            .GroupBy(r => r.UserId);

        foreach (var group in byUser)
        {
            var shared = group.Where(r => targetUserRatings.ContainsKey(r.BookId)).ToList();
            if (shared.Count == 0) continue;

            decimal mad = shared.Average(r => Math.Abs(r.OverallScore - targetUserRatings[r.BookId]));
            if (mad <= 1.5m)
            {
                similar.Add(group.Key);
            }
        }

        return similar;
    }
}
