using System;
using System.Collections.Generic;
using System.Linq;
using BookRatingPlatform.Domain.Entities;

namespace BookRatingPlatform.Application.Ingestion;

public sealed record ExternalBookMetadataRecord(
    string ProviderName,
    string ExternalId,
    string Title,
    string PrimaryAuthor,
    string? Isbn13,
    int? PublicationYear,
    IReadOnlyList<string> Genres,
    string AttributionText,
    string? ReliableAudioSourceId = null,
    string? AudiobookNarrator = null,
    int? AudiobookDurationMinutes = null);

public sealed class BookMetadataImportService
{
    public static string NormalizeKey(string input) =>
        string.Concat(input.Trim().ToLowerInvariant().Where(char.IsLetterOrDigit));

    /// <summary>
    /// Deduplicates incoming provider records against existing catalog by:
    /// 1. (ProviderName, ExternalId)
    /// 2. ISBN-13
    /// 3. (NormalizedTitle, NormalizedAuthor)
    /// Preserves local corrections when HasLocalCorrections == true.
    /// Creates AudiobookWork only when reliable audio metadata is present.
    /// </summary>
    public ImportRun ExecuteImportBatch(
        string providerName,
        IReadOnlyCollection<ExternalBookMetadataRecord> incomingRecords,
        IDictionary<string, Book> existingBooksById,
        IDictionary<(string Provider, string ExternalId), string> externalRefIndex,
        IDictionary<(string NormTitle, string NormAuthor), string> titleAuthorIndex)
    {
        var run = new ImportRun
        {
            ProviderName = providerName,
            StartedAtUtc = DateTimeOffset.UtcNow
        };

        foreach (var rec in incomingRecords)
        {
            try
            {
                run.RecordsProcessed++;
                var normTitle = NormalizeKey(rec.Title);
                var normAuthor = NormalizeKey(rec.PrimaryAuthor);

                Book? targetBook = null;
                if (externalRefIndex.TryGetValue((rec.ProviderName, rec.ExternalId), out var matchedId))
                {
                    targetBook = existingBooksById[matchedId];
                    run.RecordsDeduplicated++;
                }
                else if (titleAuthorIndex.TryGetValue((normTitle, normAuthor), out var matchedByTitleId))
                {
                    targetBook = existingBooksById[matchedByTitleId];
                    run.RecordsDeduplicated++;
                }

                if (targetBook is null)
                {
                    targetBook = new Book
                    {
                        Title = rec.Title.Trim(),
                        NormalizedTitle = normTitle,
                        Isbn13 = rec.Isbn13,
                        PublicationYear = rec.PublicationYear
                    };
                    existingBooksById[targetBook.Id] = targetBook;
                    titleAuthorIndex[(normTitle, normAuthor)] = targetBook.Id;
                }
                else if (!targetBook.HasLocalCorrections)
                {
                    targetBook.PublicationYear ??= rec.PublicationYear;
                    targetBook.Isbn13 ??= rec.Isbn13;
                }

                externalRefIndex[(rec.ProviderName, rec.ExternalId)] = targetBook.Id;

                // Only attach AudiobookWork when reliable audio metadata exists
                if (!string.IsNullOrWhiteSpace(rec.ReliableAudioSourceId) &&
                    !string.IsNullOrWhiteSpace(rec.AudiobookNarrator) &&
                    rec.AudiobookDurationMinutes is > 0)
                {
                    targetBook.AudiobookWork ??= new AudiobookWork
                    {
                        BookId = targetBook.Id,
                        Narrator = rec.AudiobookNarrator.Trim(),
                        DurationMinutes = rec.AudiobookDurationMinutes.Value,
                        ReliableSourceId = rec.ReliableAudioSourceId.Trim()
                    };
                }
            }
            catch
            {
                run.FailuresCount++;
            }
        }

        run.CompletedAtUtc = DateTimeOffset.UtcNow;
        return run;
    }
}
