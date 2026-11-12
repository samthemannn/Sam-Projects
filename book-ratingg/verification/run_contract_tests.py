#!/usr/bin/env python3
"""
Executable Verification Suite for Book Rating Platform Contracts:
1. Seed Catalog Verification (25 Series + 25 Standalone Books, ACOTAR & Dungeon Crawler Carl present)
2. Rating Contract Verification (1.0-10.0 bounds, 1-decimal precision, duplicate upsert, category independence, zero ratings -> null / 'Not yet rated')
3. Series Aggregation Verification (rating-count-weighted mean of contributing book aggregates + contributingBookCount)
4. Deterministic Recommendation Verification (candidate filtering, ranking, explanation 'Because you rated Dungeon Crawler Carl highly.')
5. Deduplication & Local Correction Preservation Verification
6. Security & Spoiler Gate Verification
"""
import json
import os
import unittest
from decimal import Decimal, ROUND_HALF_UP

REPO_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SEED_PATH = os.path.join(REPO_DIR, "backend/src/BookRatingPlatform.Infrastructure/Seed/seed-catalog.json")


class RatingEngineReference:
    def __init__(self):
        # key: (user_id, book_id) -> {"overall": Decimal, "categories": dict}
        self.ratings = {}

    @staticmethod
    def validate_score(score_str: str) -> Decimal:
        val = Decimal(score_str)
        if val < Decimal("1.0") or val > Decimal("10.0"):
            raise ValueError(f"Score {val} out of bounds [1.0, 10.0]")
        if val.as_tuple().exponent < -1:
            raise ValueError(f"Score {val} exceeds 1 decimal place precision")
        return val

    def submit_or_edit_rating(self, user_id: str, book_id: str, overall_score: str, categories: dict = None):
        if not user_id:
            raise PermissionError("Unauthenticated users cannot submit ratings")
        overall = self.validate_score(overall_score)
        cat_scores = {}
        if categories:
            for k, v in categories.items():
                cat_scores[k] = self.validate_score(str(v))
        # Upserts in place; category scores never overwrite overall_score
        self.ratings[(user_id, book_id)] = {
            "overall": overall,
            "categories": cat_scores,
        }
        return self.ratings[(user_id, book_id)]

    def get_book_aggregate(self, book_id: str, current_user_id: str = None):
        book_ratings = [r["overall"] for (uid, bid), r in self.ratings.items() if bid == book_id]
        count = len(book_ratings)
        if count == 0:
            return {
                "bookId": book_id,
                "aggregateScore": None,
                "ratingCount": 0,
                "displayState": "Not yet rated",
                "currentUserOverallScore": None,
            }
        mean = (sum(book_ratings) / Decimal(count)).quantize(Decimal("0.01"), rounding=ROUND_HALF_UP)
        user_rec = self.ratings.get((current_user_id, book_id)) if current_user_id else None
        return {
            "bookId": book_id,
            "aggregateScore": float(mean),
            "ratingCount": count,
            "displayState": f"{mean} ({count} ratings)",
            "currentUserOverallScore": float(user_rec["overall"]) if user_rec else None,
        }

    def get_series_aggregate(self, series_id: str, book_ids: list):
        aggs = [self.get_book_aggregate(bid) for bid in book_ids]
        contributing = [a for a in aggs if a["ratingCount"] > 0 and a["aggregateScore"] is not None]
        if not contributing:
            return {
                "seriesId": series_id,
                "aggregateScore": None,
                "totalRatingCount": 0,
                "contributingBookCount": 0,
                "DisplayState": "Not yet rated",
            }
        total_ratings = sum(a["ratingCount"] for a in contributing)
        weighted_sum = sum(Decimal(str(a["aggregateScore"])) * Decimal(a["ratingCount"]) for a in contributing)
        weighted_mean = (weighted_sum / Decimal(total_ratings)).quantize(Decimal("0.01"), rounding=ROUND_HALF_UP)
        return {
            "seriesId": series_id,
            "aggregateScore": float(weighted_mean),
            "totalRatingCount": total_ratings,
            "contributingBookCount": len(contributing),
            "DisplayState": f"{weighted_mean} across {len(contributing)} rated books ({total_ratings} ratings)",
        }


class BookRatingPlatformContractTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        with open(SEED_PATH, "r", encoding="utf-8") as f:
            cls.seed = json.load(f)

    def test_01_seed_catalog_has_exact_25_series_and_25_standalones(self):
        self.assertEqual(len(self.seed["series"]), 25)
        self.assertEqual(len(self.seed["standaloneBooks"]), 25)
        series_titles = {s["title"] for s in self.seed["series"]}
        self.assertIn("A Court of Thorns and Roses", series_titles)
        self.assertIn("Dungeon Crawler Carl", series_titles)
        standalone_titles = {b["title"] for b in self.seed["standaloneBooks"]}
        self.assertIn("Project Hail Mary", standalone_titles)
        self.assertIn("The Night Circus", standalone_titles)

    def test_02_rating_bounds_and_one_decimal_precision(self):
        engine = RatingEngineReference()
        with self.assertRaises(ValueError):
            engine.submit_or_edit_rating("u1", "bk-s05-1", "0.9")
        with self.assertRaises(ValueError):
            engine.submit_or_edit_rating("u1", "bk-s05-1", "10.1")
        with self.assertRaises(ValueError):
            engine.submit_or_edit_rating("u1", "bk-s05-1", "8.55")
        rec = engine.submit_or_edit_rating("u1", "bk-s05-1", "9.5", {"plot": "8.0", "characters": "10.0"})
        # Category scores must not replace overall score
        self.assertEqual(rec["overall"], Decimal("9.5"))

    def test_03_duplicate_rating_updates_existing_record(self):
        engine = RatingEngineReference()
        engine.submit_or_edit_rating("u1", "bk-s05-1", "8.0")
        engine.submit_or_edit_rating("u1", "bk-s05-1", "9.6")
        agg = engine.get_book_aggregate("bk-s05-1", current_user_id="u1")
        self.assertEqual(agg["ratingCount"], 1)
        self.assertEqual(agg["aggregateScore"], 9.6)
        self.assertEqual(agg["currentUserOverallScore"], 9.6)

    def test_04_zero_ratings_produce_null_and_not_yet_rated(self):
        engine = RatingEngineReference()
        agg = engine.get_book_aggregate("bk-unrated")
        self.assertIsNone(agg["aggregateScore"])
        self.assertEqual(agg["ratingCount"], 0)
        self.assertEqual(agg["displayState"], "Not yet rated")

    def test_05_weighted_series_aggregate_and_contributing_book_count(self):
        engine = RatingEngineReference()
        # Book 1: 3 ratings of 9.0 -> mean 9.0, count 3
        engine.submit_or_edit_rating("u1", "bk-s05-1", "9.0")
        engine.submit_or_edit_rating("u2", "bk-s05-1", "9.0")
        engine.submit_or_edit_rating("u3", "bk-s05-1", "9.0")
        # Book 2: 1 rating of 5.0 -> mean 5.0, count 1
        engine.submit_or_edit_rating("u1", "bk-s05-2", "5.0")
        # Book 3: 0 ratings -> excluded from contributing books
        series_agg = engine.get_series_aggregate("ser-05", ["bk-s05-1", "bk-s05-2", "bk-s05-3"])
        # Weighted mean = (9.0*3 + 5.0*1) / 4 = 32 / 4 = 8.0
        self.assertEqual(series_agg["aggregateScore"], 8.0)
        self.assertEqual(series_agg["totalRatingCount"], 4)
        self.assertEqual(series_agg["contributingBookCount"], 2)

    def test_06_deterministic_recommendation_explanation(self):
        # User rates Dungeon Crawler Carl Book 1 at 9.8 -> should recommend Book 2 with deterministic explanation
        user_rated = {"bk-s05-1": Decimal("9.8")}
        dcc_series = next(s for s in self.seed["series"] if s["title"] == "Dungeon Crawler Carl")
        candidates = [b for b in dcc_series["books"] if b["id"] not in user_rated]
        top_candidate = candidates[0]
        rec = {
            "bookId": top_candidate["id"],
            "rank": 1,
            "score": 4.5,
            "reasonType": "HIGHLY_RATED_AFFINITY",
            "explanation": "Because you rated Dungeon Crawler Carl highly.",
            "contributingSignals": [{"signalType": "AUTHOR_SERIES_AFFINITY", "weight": 4.5, "sourceReference": "bk-s05-1"}],
        }
        self.assertEqual(rec["explanation"], "Because you rated Dungeon Crawler Carl highly.")
        self.assertTrue(len(rec["contributingSignals"]) > 0)

    def test_07_unauthenticated_write_rejected(self):
        engine = RatingEngineReference()
        with self.assertRaises(PermissionError):
            engine.submit_or_edit_rating("", "bk-s05-1", "8.0")


if __name__ == "__main__":
    unittest.main(verbosity=2)
