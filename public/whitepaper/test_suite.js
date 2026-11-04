/**
 * Test Suite for Google Mind the Gap Website (TDD)
 * Verifies all 7 requirements before committing and pushing.
 */

const fs = require('fs');
const path = require('path');
const test = require('node:test');
const assert = require('node:assert/strict');

const nonTechDir = path.resolve(__dirname);
const indexPath = path.join(nonTechDir, 'index.html');
const whitepaperPath = path.join(nonTechDir, 'whitepaper.html');
const stylesPath = path.join(nonTechDir, 'styles.css');
const appJsPath = path.join(nonTechDir, 'app.js');
const pdfFileName = 'Google Mind the Gap - AI-Enhanced White Paper.pdf';
const pdfPath = path.join(nonTechDir, pdfFileName);

test('Requirement 1: Theme persistence and dark mode consistency on full paper', () => {
  const whitepaperHtml = fs.readFileSync(whitepaperPath, 'utf8');
  
  // 1a. Must not default to data-theme="light" in HTML tag
  assert.ok(
    !whitepaperHtml.includes('<html lang="en" data-theme="light">'),
    'whitepaper.html should not have hardcoded data-theme="light"'
  );

  // 1b. Must have inline theme initialization script in <head> to prevent flash of light mode
  assert.ok(
    whitepaperHtml.includes('localStorage.getItem') && whitepaperHtml.includes('setAttribute(\'data-theme\''),
    'whitepaper.html must read localStorage theme in head and set data-theme accordingly'
  );
});

test('Requirement 2: Preferred format options without horizontal scroll', () => {
  const indexHtml = fs.readFileSync(indexPath, 'utf8');
  
  // Must contain the exact required options
  assert.ok(indexHtml.includes('>Campus Visit<'), 'Must contain "Campus Visit" option');
  assert.ok(indexHtml.includes('>Virtual<'), 'Must contain "Virtual" option');
  assert.ok(indexHtml.includes('>Transport Request Inperson<'), 'Must contain "Transport Request Inperson" option');

  // Must not contain the old long overflowing descriptions in options
  assert.ok(!indexHtml.includes('(Nearest Google Hub)'), 'Must not contain long subtitle in option');
  assert.ok(!indexHtml.includes('(Google Meet)'), 'Must not contain long subtitle in option');
});

test('Requirement 3: Cohort size options do not contain the word cohort', () => {
  const indexHtml = fs.readFileSync(indexPath, 'utf8');
  
  // Extract options inside select id="cohort-size"
  const selectMatch = indexHtml.match(/<select[^>]*id=["']cohort-size["'][^>]*>([\s\S]*?)<\/select>/i);
  assert.ok(selectMatch, 'cohort-size select element must exist');
  
  const optionsHtml = selectMatch[1];
  const optionTexts = [...optionsHtml.matchAll(/<option[^>]*>([^<]*)<\/option>/gi)].map(m => m[1]);
  
  // None of the option text contents should have the word "cohort" (case insensitive)
  for (const text of optionTexts) {
    assert.ok(
      !/cohort/i.test(text),
      `Option "${text}" should not contain the word "cohort"`
    );
  }
});

test('Requirement 4: Fluid motion sliding blue circle/pill indicator around White Paper / Summary', () => {
  const indexHtml = fs.readFileSync(indexPath, 'utf8');
  const stylesCss = fs.readFileSync(stylesPath, 'utf8');
  const appJs = fs.readFileSync(appJsPath, 'utf8');
  
  // Nav must have a sliding indicator element
  assert.ok(
    indexHtml.includes('nav-slider-pill') || indexHtml.includes('nav-indicator'),
    'index.html must have an active sliding pill element in navbar'
  );
  
  // CSS must have transition properties for fluid movement
  assert.ok(
    stylesCss.includes('nav-slider-pill') || stylesCss.includes('nav-indicator'),
    'styles.css must style the sliding indicator'
  );

  // app.js must handle sliding position to Summary on scroll
  assert.ok(
    appJs.includes('nav-slider') || appJs.includes('navIndicator') || appJs.includes('updateNavIndicator') || appJs.includes('sliderPill'),
    'app.js must dynamically calculate and animate the sliding indicator position'
  );
});

test('Requirement 5: Dark mode / Light mode toggle at bottom right', () => {
  const indexHtml = fs.readFileSync(indexPath, 'utf8');
  const stylesCss = fs.readFileSync(stylesPath, 'utf8');
  
  // Theme toggle must be a floating button outside the top navbar
  assert.ok(
    indexHtml.includes('class="floating-theme-toggle"') || indexHtml.includes('class="floating-theme-btn"'),
    'index.html must have floating theme toggle button'
  );
  
  // Verify it is NOT inside .nav-actions
  const navActionsMatch = indexHtml.match(/<div class=["']nav-actions["']>([\s\S]*?)<\/div>/i);
  if (navActionsMatch) {
    assert.ok(
      !navActionsMatch[1].includes('id="theme-toggle"'),
      'Theme toggle should not be inside .nav-actions'
    );
  }

  // CSS must position it fixed at bottom right
  assert.ok(
    stylesCss.includes('bottom:') && stylesCss.includes('right:') && stylesCss.includes('position: fixed'),
    'styles.css must position theme toggle fixed at bottom right'
  );
});

test('Requirement 6: Download PDF points to the original Google Mind the Gap PDF', () => {
  const indexHtml = fs.readFileSync(indexPath, 'utf8');
  const whitepaperHtml = fs.readFileSync(whitepaperPath, 'utf8');
  
  // PDF file must exist on disk and be > 50KB
  assert.ok(fs.existsSync(pdfPath), 'PDF file must exist in workspace');
  const stats = fs.statSync(pdfPath);
  assert.ok(stats.size > 50000, 'PDF file must be non-empty (at least 50KB)');
  
  // In index.html, hero download button and banner download button must link directly to the PDF
  assert.ok(
    indexHtml.includes(encodeURI(pdfFileName)) || indexHtml.includes(pdfFileName),
    'index.html must link to the original PDF file'
  );
  
  // In whitepaper.html, download button must link to the PDF
  assert.ok(
    whitepaperHtml.includes(encodeURI(pdfFileName)) || whitepaperHtml.includes(pdfFileName),
    'whitepaper.html must link to the original PDF file'
  );
});

test('Requirement 7: Brand badge text changed to "White Paper"', () => {
  const indexHtml = fs.readFileSync(indexPath, 'utf8');
  
  // Must contain "White Paper" as badge text
  assert.ok(
    indexHtml.includes('<span class="brand-badge">White Paper</span>') ||
    indexHtml.includes('class="brand-badge" id="brand-badge">White Paper</span>'),
    'Brand badge must be exactly "White Paper"'
  );
  
  // Must NOT contain "White Paper • Google" or "White paper google"
  assert.ok(!indexHtml.includes('White Paper &bull; Google'), 'Must not contain "White Paper • Google"');
  assert.ok(!indexHtml.includes('White paper google'), 'Must not contain "White paper google"');
});

test('Requirement 8: Mind the Gap portal links to https://sites.research.google/gr/mindthegap/', () => {
  const indexHtml = fs.readFileSync(indexPath, 'utf8');
  const whitepaperHtml = fs.readFileSync(whitepaperPath, 'utf8');
  
  const targetUrl = 'https://sites.research.google/gr/mindthegap/';
  assert.ok(indexHtml.includes(targetUrl), 'index.html must link to https://sites.research.google/gr/mindthegap/');
  assert.ok(whitepaperHtml.includes(targetUrl), 'whitepaper.html must link to https://sites.research.google/gr/mindthegap/');
});

