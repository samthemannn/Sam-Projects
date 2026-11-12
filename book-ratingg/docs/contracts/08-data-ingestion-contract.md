# Gemini Data Ingestion Contract
Implement a provider-backed metadata import process using a permitted books API. Map titles, authors, genres, covers, descriptions, publication metadata, series information, external IDs, and audiobook data when reliably available.
Record import runs, timestamps, failures, attribution, and source IDs. Deduplicate records and preserve local corrections. Ensure all 50 required seed entries remain available even when an external API omits a title. Do not scrape protected sites or store copyrighted book text.
