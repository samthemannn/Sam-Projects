# Gemini Recommendation Contract
Implement a deterministic recommendation service with no ML, LLM, chat plugin, external AI account, or paid token usage.
Use only application data: genre similarity, author/series affinity, the user's rating history, popularity, and similar-user rating patterns. Separate candidate generation, ranking, and explanation for maintainability.
Every result must include book ID, rank, score, reason type, explanation, and contributing signals. Example explanation: "Because you rated Dungeon Crawler Carl highly." Do not send user ratings, reviews, or profile data to external inference services.
