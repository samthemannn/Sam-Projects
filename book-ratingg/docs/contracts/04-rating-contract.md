# Gemini Rating Contract
Implement one active overall Rating per user/book. Accept only decimal values from 1.0 through 10.0 with one decimal precision. Editing a rating updates the existing record.
Category ratings are optional and user-selected. Store categories as data, not database columns. Category scores must not silently replace the user's overall rating.
Book aggregate = arithmetic mean of valid overall ratings. Always return aggregate score and rating count. Series aggregate = rating-count-weighted mean of contributing book aggregates and must return contributing-book count. Zero ratings produce a null score and a clear "Not yet rated" state.
Add unit and integration tests for bounds, precision, duplicate prevention, edits, zero ratings, weighted series scores, and concurrent submissions.
