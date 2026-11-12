import React from 'react';
import { BookAggregateDto } from '../types/contracts';

interface Props {
  aggregate: BookAggregateDto;
}

/**
 * Displays backend-computed public aggregate score & rating count separately
 * from the authenticated user's own rating. Never recalculates scores on the client.
 */
export const BookScorePanel: React.FC<Props> = ({ aggregate }) => {
  return (
    <section className="book-score-panel" aria-label="Book Ratings">
      <div className="public-score" data-testid="public-score">
        <h3>Public Rating</h3>
        {aggregate.aggregateScore === null || aggregate.ratingCount === 0 ? (
          <span className="not-yet-rated">Not yet rated (0 ratings)</span>
        ) : (
          <div>
            <span className="score-value">{aggregate.aggregateScore.toFixed(2)}</span>
            <span className="rating-count">({aggregate.ratingCount} ratings)</span>
          </div>
        )}
      </div>

      <div className="user-score" data-testid="user-score">
        <h3>Your Rating</h3>
        {aggregate.currentUserOverallScore === null ? (
          <span>You have not rated this book</span>
        ) : (
          <span>{aggregate.currentUserOverallScore.toFixed(1)} / 10.0</span>
        )}
      </div>
    </section>
  );
};
