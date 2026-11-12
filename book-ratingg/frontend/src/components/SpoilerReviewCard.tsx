import React, { useState } from 'react';
import { ReviewDto } from '../types/contracts';

interface Props {
  review: ReviewDto;
}

export const SpoilerReviewCard: React.FC<Props> = ({ review }) => {
  const [isRevealed, setIsRevealed] = useState(false);

  const shouldHideBody = review.isSpoiler && !isRevealed;

  return (
    <article className="review-card" aria-label={`Review by ${review.userDisplayName}`}>
      <header className="review-card__header">
        <strong>{review.userDisplayName}</strong>
        <time dateTime={review.createdAtUtc}>{new Date(review.createdAtUtc).toLocaleDateString()}</time>
      </header>
      {shouldHideBody ? (
        <div className="review-card__spoiler-gate" data-testid="spoiler-gate">
          <p>This review contains spoilers.</p>
          <button
            type="button"
            onClick={() => setIsRevealed(true)}
            data-testid="reveal-spoiler-btn"
          >
            Reveal Spoiler Review
          </button>
        </div>
      ) : (
        <p className="review-card__body" data-testid="review-body">
          {review.body}
        </p>
      )}
    </article>
  );
};
