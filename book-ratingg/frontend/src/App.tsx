import { BookScorePanel } from './components/BookScorePanel';
import { SpoilerReviewCard } from './components/SpoilerReviewCard';
import { BookAggregateDto, ReviewDto } from './types/contracts';

const featuredAggregate: BookAggregateDto = {
  bookId: 'dcc-1',
  aggregateScore: null,
  ratingCount: 0,
  displayState: 'Not yet rated',
  currentUserOverallScore: null,
  currentUserCategoryScores: null,
};

const previewReview: ReviewDto = {
  id: 'preview-review',
  userId: 'demo-user',
  userDisplayName: 'Demo reader',
  bookId: 'dcc-1',
  body: 'A chaotic, funny, surprisingly heartfelt adventure.',
  isSpoiler: false,
  createdAtUtc: new Date().toISOString(),
};

export function App() {
  return (
    <main className="app-shell">
      <nav className="topbar">
        <div className="brand-mark">BOOKMARKED</div>
        <div className="nav-links"><a href="#discover">Discover</a><a href="#series">Series</a><a href="#recommendations">For you</a></div>
        <button className="profile-button">Sign in</button>
      </nav>

      <section className="hero" id="discover">
        <div>
          <p className="eyebrow">YOUR NEXT FAVORITE STORY</p>
          <h1>Rate what you love.<br /><em>Find what is next.</em></h1>
          <p className="hero-copy">A thoughtful home for book ratings, honest reviews, and series worth getting lost in.</p>
          <div className="search-bar"><span>⌕</span><input aria-label="Search books" placeholder="Search books, authors, or series" /><kbd>⌘ K</kbd></div>
        </div>
        <div className="hero-card"><span className="hero-card__label">FEATURED SERIES</span><strong>Dungeon<br />Crawler Carl</strong><span className="hero-card__author">Matt Dinniman</span><span className="hero-card__badge">Not yet rated</span></div>
      </section>

      <section className="content-grid" id="series">
        <div><div className="section-heading"><div><p className="eyebrow">START EXPLORING</p><h2>Popular series</h2></div><a href="#all">View all →</a></div><div className="series-grid">{['A Court of Thorns and Roses','Mistborn','The Hunger Games'].map((title, index) => <article className="series-card" key={title}><div className={`series-art art-${index + 1}`}><span>{index === 0 ? 'ACOTAR' : index === 1 ? 'MISTBORN' : 'THE\nHUNGER\nGAMES'}</span></div><div><h3>{title}</h3><p>{['Sarah J. Maas','Brandon Sanderson','Suzanne Collins'][index]}</p></div></article>)}</div></div>
        <aside className="rating-preview"><p className="eyebrow">RATING PREVIEW</p><h2>Dungeon Crawler Carl</h2><BookScorePanel aggregate={featuredAggregate} /><div className="review-preview"><SpoilerReviewCard review={previewReview} /></div></aside>
      </section>
    </main>
  );
}

