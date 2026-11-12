export interface AudiobookDto {
  id: string;
  narrator: string;
  durationMinutes: number;
  reliableSourceId: string;
}

export interface BookAggregateDto {
  bookId: string;
  aggregateScore: number | null;
  ratingCount: number;
  displayState: string; // "Not yet rated" when ratingCount === 0
  currentUserOverallScore: number | null;
  currentUserCategoryScores: Record<string, number> | null;
}

export interface SeriesAggregateDto {
  seriesId: string;
  aggregateScore: number | null;
  totalRatingCount: number;
  contributingBookCount: number;
  totalSeriesBookCount: number;
  displayState: string;
}

export interface ReviewDto {
  id: string;
  userId: string;
  userDisplayName: string;
  bookId: string;
  body: string;
  isSpoiler: boolean;
  createdAtUtc: string;
}

export interface ContributingSignalDto {
  signalType: string;
  weight: number;
  sourceReference: string;
}

export interface RecommendationItemDto {
  bookId: string;
  rank: number;
  score: number;
  reasonType: string;
  explanation: string;
  contributingSignals: ContributingSignalDto[];
}
