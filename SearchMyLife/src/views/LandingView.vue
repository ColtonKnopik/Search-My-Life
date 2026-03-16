<template>
  <div class="landing-page">
    <!-- Nav -->
    <nav class="landing-nav">
      <a class="nav-logo" href="/">Search <span>My Life</span></a>
      <ul class="nav-links">
        <li><a href="#features">Features</a></li>
        <li><a href="#privacy">Privacy</a></li>
        <li><a href="#uses">Use Cases</a></li>
        <li><router-link to="/login" class="btn-nav">Get Started</router-link></li>
      </ul>
    </nav>

    <!-- Hero -->
    <section class="hero">
      <div class="hero-orb-2"></div>
      <div class="hero-orb-3"></div>
      <div class="hero-inner">
        <div class="hero-badge">
          <div class="hero-badge-dot"></div>
          AI-Powered Semantic Search for Your Journal
        </div>

        <h1>Your memories,<br><em>finally searchable</em></h1>

        <p class="hero-sub">
          Ask your journal anything. Search My Life uses AI to find the exact entries that match what you're looking for — by meaning, not just keywords.
        </p>

        <div class="hero-cta">
          <router-link to="/login" class="btn-primary">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 5v14M5 12l7 7 7-7"/></svg>
            Start Journaling Free
          </router-link>
          <a href="#features" class="btn-secondary">See How It Works</a>
        </div>

        <!-- Search Demo -->
        <div class="hero-demo">
          <div class="demo-card">
            <div class="demo-search-bar">
              <span class="demo-search-icon">
                <svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/></svg>
              </span>
              <span class="demo-search-text">"When did I last feel confident about my work?"</span>
            </div>
            <div class="demo-results">
              <div class="demo-result-item">
                <div class="demo-result-dot dot-proud"></div>
                <div class="demo-result-content">
                  <div class="demo-result-date">March 4, 2024</div>
                  <div class="demo-result-text">Finished the presentation and the whole team clapped. I actually felt like I knew what I was doing for the first time in months...</div>
                  <div class="demo-result-tags">
                    <span class="tag">proud</span>
                    <span class="tag">work</span>
                    <span class="tag">confident</span>
                  </div>
                </div>
              </div>
              <div class="demo-result-item">
                <div class="demo-result-dot dot-calm"></div>
                <div class="demo-result-content">
                  <div class="demo-result-date">November 12, 2023</div>
                  <div class="demo-result-text">My manager said she was impressed. Strange feeling — I keep waiting for the other shoe to drop, but today I'm just going to sit with the win.</div>
                  <div class="demo-result-tags">
                    <span class="tag">calm</span>
                    <span class="tag">growth</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- How It Works -->
    <section class="how-it-works" id="how">
      <div class="container">
        <div class="reveal">
          <p class="section-label">How It Works</p>
          <h2 class="section-title">Three steps to<br>know yourself better</h2>
        </div>
        <div class="how-grid">
          <div class="how-step reveal">
            <div class="how-step-num">1</div>
            <h3>Write Freely</h3>
            <p>Journal however you like. Short check-ins, long reflections, bullet points — it all gets captured securely, encrypted before it ever leaves your device.</p>
          </div>
          <div class="how-step reveal">
            <div class="how-step-num">2</div>
            <h3>Ask Anything</h3>
            <p>Type a natural language question like <em>"When was I most stressed about money?"</em> and the AI finds entries that match by meaning — not just matching words.</p>
          </div>
          <div class="how-step reveal">
            <div class="how-step-num">3</div>
            <h3>Reflect & Grow</h3>
            <p>Browse a timeline of your experiences, discover emotional patterns, and let AI surface insights from entries you may have forgotten you'd written.</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Features -->
    <section id="features">
      <div class="container">
        <div class="features-header">
          <div class="reveal">
            <p class="section-label">Features</p>
            <h2 class="section-title">Everything your journal always needed</h2>
          </div>
          <div class="reveal">
            <p class="section-sub">Semantic search, emotional insights, and a beautiful timeline — built for people who actually journal regularly.</p>
          </div>
        </div>
        <div class="features-grid">
          <div class="feature-card reveal">
            <div class="feature-icon">🔍</div>
            <h3>Semantic Search</h3>
            <p>Find entries by meaning, not memory. Ask real questions and get relevant results from months or years ago — even if you never used the exact same words.</p>
          </div>
          <div class="feature-card reveal">
            <div class="feature-icon">🔐</div>
            <h3>End-to-End Encryption</h3>
            <p>Your entries are encrypted in your browser using AES-256 before being stored. Not even the developer can read your journal. Your secrets are truly yours.</p>
          </div>
          <div class="feature-card reveal">
            <div class="feature-icon">📅</div>
            <h3>Timeline View</h3>
            <p>Browse your history month by month. Entries are color-coded by emotion, giving you a visual map of your inner life over time.</p>
          </div>
          <div class="feature-card reveal">
            <div class="feature-icon">💡</div>
            <h3>AI Insights</h3>
            <p>Each entry is tagged with sentiment, emotion, and a short AI summary. Search by emotion — "stressed," "hopeful," "grateful" — to find specific moments.</p>
          </div>
          <div class="feature-card reveal">
            <div class="feature-icon">📈</div>
            <h3>Emotional Trends</h3>
            <p>Generate weekly or monthly reflection reports. Spot recurring patterns and see how your emotional landscape has shifted over time.</p>
          </div>
          <div class="feature-card reveal">
            <div class="feature-icon">⚡</div>
            <h3>Fast & Lightweight</h3>
            <p>No bloated app to install. A clean, focused web interface built for the single purpose of helping you understand yourself.</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Privacy -->
    <section class="privacy" id="privacy">
      <div class="container">
        <div class="privacy-inner">
          <div class="reveal">
            <p class="section-label">Privacy First</p>
            <h2 class="section-title">Your journal is no one else's business</h2>
            <p class="section-sub">Privacy wasn't an afterthought — it's the foundation of the entire architecture.</p>
            <ul class="privacy-points">
              <li>
                <div class="check">✓</div>
                <span>Entries encrypted locally with <strong>AES via Web Crypto API</strong> before upload</span>
              </li>
              <li>
                <div class="check">✓</div>
                <span>The server stores only <strong>ciphertext</strong> — no plaintext, ever</span>
              </li>
              <li>
                <div class="check">✓</div>
                <span>AI embeddings operate on <strong>encrypted data</strong>, not your words</span>
              </li>
              <li>
                <div class="check">✓</div>
                <span>Developer access to your entries is <strong>technically impossible</strong></span>
              </li>
            </ul>
          </div>
          <div class="reveal">
            <div class="privacy-visual">
              <div class="enc-row">
                <div class="enc-icon green">🔒</div>
                <div>
                  <div class="enc-label">Your Entry</div>
                  <div class="enc-sub">Encrypted in your browser</div>
                </div>
                <span class="enc-status secure">Secure</span>
              </div>
              <div class="enc-row">
                <div class="enc-icon green">📡</div>
                <div>
                  <div class="enc-label">Transmission</div>
                  <div class="enc-sub">Only ciphertext sent over HTTPS</div>
                </div>
                <span class="enc-status secure">Encrypted</span>
              </div>
              <div class="enc-row">
                <div class="enc-icon muted">🗄️</div>
                <div>
                  <div class="enc-label">Database</div>
                  <div class="enc-sub">Stores only encrypted blobs</div>
                </div>
                <span class="enc-status never">No plaintext</span>
              </div>
              <div class="enc-row">
                <div class="enc-icon muted">👤</div>
                <div>
                  <div class="enc-label">Developer Access</div>
                  <div class="enc-sub">Cannot decrypt your entries</div>
                </div>
                <span class="enc-status never">None</span>
              </div>
              <div class="enc-row">
                <div class="enc-icon green">🔓</div>
                <div>
                  <div class="enc-label">Decryption</div>
                  <div class="enc-sub">Happens only in your browser</div>
                </div>
                <span class="enc-status secure">Client-side</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Use Cases -->
    <section id="uses">
      <div class="container">
        <div class="reveal">
          <p class="section-label">Use Cases</p>
          <h2 class="section-title">Questions your journal<br>can finally answer</h2>
        </div>
        <div class="use-cases-grid">
          <div class="use-case-card reveal">
            <span class="use-case-emoji">🧠</span>
            <h3>Find Emotional Patterns</h3>
            <p>Search "When do I feel most anxious?" and see a map of every time you've written about that feeling.</p>
          </div>
          <div class="use-case-card reveal">
            <span class="use-case-emoji">🌱</span>
            <h3>Track Personal Growth</h3>
            <p>Compare who you were a year ago to who you are now. Let your own words show you how far you've come.</p>
          </div>
          <div class="use-case-card reveal">
            <span class="use-case-emoji">🎯</span>
            <h3>Revisit Decisions</h3>
            <p>Remember what you were thinking the last time you faced a similar crossroads. Past-you has advice.</p>
          </div>
          <div class="use-case-card reveal">
            <span class="use-case-emoji">📖</span>
            <h3>Rediscover Moments</h3>
            <p>That feeling from a summer three years ago — you wrote about it. Now you can find it in seconds.</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Audience -->
    <section class="audience">
      <div class="container">
        <div class="reveal" style="text-align:center;">
          <p class="section-label">Who It's For</p>
          <h2 class="section-title">Built for people who write<br>about their lives</h2>
        </div>
        <div class="audience-grid">
          <div class="audience-card reveal">
            <span class="audience-emoji">🎓</span>
            <h3>College Students</h3>
            <p>Track the chaos of university life and search your own stress and growth patterns across semesters.</p>
          </div>
          <div class="audience-card reveal">
            <span class="audience-emoji">💼</span>
            <h3>Self-Improvement Focused</h3>
            <p>Use your journal as a data source for your personal development — measurable, searchable, yours.</p>
          </div>
          <div class="audience-card reveal">
            <span class="audience-emoji">📓</span>
            <h3>Dedicated Journalers</h3>
            <p>You've been writing for years. Now actually use what you've written — without reading everything twice.</p>
          </div>
          <div class="audience-card reveal">
            <span class="audience-emoji">🛡️</span>
            <h3>Privacy-Conscious Users</h3>
            <p>You want AI convenience without handing your most personal thoughts to a corporation. This is that.</p>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA Banner -->
    <section class="cta-banner">
      <div class="container">
        <h2 class="reveal">Start searching<br>your story today</h2>
        <p class="reveal">Free to use. Private by design. Built by an indie developer who journals too.</p>
        <div class="reveal">
          <router-link to="/login" class="btn-primary">
            Create Your Free Account
            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14M12 5l7 7-7 7"/></svg>
          </router-link>
        </div>
        <p class="cta-note reveal">No credit card required &nbsp;·&nbsp; Your data is always encrypted &nbsp;·&nbsp; Cancel anytime</p>
      </div>
    </section>

    <!-- Footer -->
    <footer>
      <div class="footer-logo">Search <span>My Life</span></div>
      <p class="footer-tagline">Your memories, semantically searchable.</p>
      <ul class="footer-links">
        <li><a href="#" @click.prevent="$router.push('/')">Home</a></li>
        <li><a href="#features">Features</a></li>
        <li><a href="#privacy">Privacy</a></li>
        <li><router-link to="/login">Sign Up</router-link></li>
        <li><a href="#">Contact</a></li>
      </ul>
      <p class="footer-copy">
        &copy; 2025 Search My Life &nbsp;·&nbsp; Built with care by an independent developer &nbsp;·&nbsp; <a href="#">Privacy Policy</a>
      </p>
    </footer>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'

onMounted(() => {
  // Scroll reveal observer
  const reveals = document.querySelectorAll('.reveal')
  const observer = new IntersectionObserver(
    (entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          const siblings = [...entry.target.parentElement.querySelectorAll('.reveal:not(.visible)')]
          const idx = siblings.indexOf(entry.target)
          setTimeout(() => {
            entry.target.classList.add('visible')
          }, Math.min(idx * 80, 240))
          observer.unobserve(entry.target)
        }
      })
    },
    { threshold: 0.12, rootMargin: '0px 0px -40px 0px' }
  )

  reveals.forEach((el) => observer.observe(el))
})
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Playfair+Display:ital,wght@0,400;0,600;1,400&family=DM+Sans:wght@300;400;500&display=swap');

.landing-page {
  --sml-jungle-teal:   #6b9080;
  --sml-muted-teal:    #a4c3b2;
  --sml-frozen-water:  #cce3de;
  --sml-azure-mist:    #eaf4f4;
  --sml-mint-cream:    #f6fff8;
  --sml-color-primary:   var(--sml-jungle-teal);
  --sml-color-secondary: var(--sml-muted-teal);
  --sml-color-accent:    var(--sml-frozen-water);
  --sml-color-bg:        var(--sml-azure-mist);
  --sml-color-surface:   var(--sml-mint-cream);
  --sml-text-primary:    #263d35;
  --sml-text-secondary:  #4a6b5e;
  --sml-text-muted:      #7a9e90;
  --sml-text-on-primary: #ffffff;
  --sml-border:          var(--sml-frozen-water);
  --sml-border-subtle:   var(--sml-azure-mist);
  --sml-shadow-sm:  0 1px 3px rgba(107,144,128,0.12);
  --sml-shadow-md:  0 4px 12px rgba(107,144,128,0.16);
  --sml-shadow-lg:  0 8px 24px rgba(107,144,128,0.20);
  --sml-radius-sm:  6px;
  --sml-radius-md:  12px;
  --sml-radius-lg:  20px;
  --font-display: 'Playfair Display', Georgia, serif;
  --font-body:    'DM Sans', system-ui, sans-serif;

  background-color: var(--sml-color-bg);
  min-height: 100vh;
  font-family: var(--font-body);
  color: var(--sml-text-primary);
  position: relative;
}

.landing-page::before {
  content: '';
  position: fixed;
  inset: 0;
  background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 256 256' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noise'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.9' numOctaves='4' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noise)' opacity='0.03'/%3E%3C/svg%3E");
  pointer-events: none;
  z-index: 0;
  opacity: 0.4;
}

/* Nav */
.landing-nav {
  position: fixed;
  top: 0; left: 0; right: 0;
  z-index: 100;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 2.5rem;
  height: 64px;
  background: rgba(234, 244, 244, 0.82);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border-bottom: 1px solid var(--sml-border);
}

.nav-logo {
  font-family: var(--font-display);
  font-size: 1.2rem;
  font-weight: 600;
  color: var(--sml-text-primary);
  text-decoration: none;
  letter-spacing: -0.01em;
}

.nav-logo span { color: var(--sml-color-primary); }

.nav-links {
  display: flex;
  align-items: center;
  gap: 2rem;
  list-style: none;
}

.nav-links a {
  text-decoration: none;
  color: var(--sml-text-secondary);
  font-size: 0.9rem;
  font-weight: 500;
  transition: color 0.2s;
}

.nav-links a:hover { color: var(--sml-color-primary); }

.btn-nav {
  background: var(--sml-color-primary);
  color: var(--sml-text-on-primary) !important;
  padding: 0.5rem 1.25rem;
  border-radius: var(--sml-radius-sm);
  font-weight: 500 !important;
  transition: background 0.2s, transform 0.15s !important;
}

.btn-nav:hover {
  background: #5a7a6b !important;
  transform: translateY(-1px);
}

/* Hero */
.hero {
  position: relative;
  min-height: 100vh;
  display: grid;
  place-items: center;
  padding: 7rem 2rem 5rem;
  text-align: center;
  overflow: hidden;
}

.hero::after {
  content: '';
  position: absolute;
  width: 600px; height: 600px;
  background: radial-gradient(circle, rgba(164,195,178,0.22) 0%, transparent 70%);
  top: 10%; left: 50%;
  transform: translateX(-50%);
  pointer-events: none;
  border-radius: 50%;
}

.hero-orb-2, .hero-orb-3 {
  position: absolute;
  pointer-events: none;
  border-radius: 50%;
}

.hero-orb-2 {
  width: 400px; height: 400px;
  background: radial-gradient(circle, rgba(107,144,128,0.12) 0%, transparent 70%);
  bottom: 5%; right: 5%;
}

.hero-orb-3 {
  width: 300px; height: 300px;
  background: radial-gradient(circle, rgba(204,227,222,0.3) 0%, transparent 70%);
  top: 20%; left: 5%;
}

.hero-inner {
  position: relative;
  z-index: 1;
  max-width: 760px;
}

.hero-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: var(--sml-color-surface);
  border: 1px solid var(--sml-border);
  color: var(--sml-color-primary);
  font-size: 0.8rem;
  font-weight: 500;
  padding: 0.35rem 0.9rem;
  border-radius: 999px;
  margin-bottom: 2rem;
  box-shadow: var(--sml-shadow-sm);
  animation: fadeSlideDown 0.6s ease both;
}

.hero-badge-dot {
  width: 6px; height: 6px;
  border-radius: 50%;
  background: var(--sml-color-primary);
  animation: pulse 2s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.5; transform: scale(0.8); }
}

h1 {
  font-family: var(--font-display);
  font-size: clamp(2.8rem, 6vw, 4.8rem);
  font-weight: 600;
  line-height: 1.1;
  letter-spacing: -0.02em;
  color: var(--sml-text-primary);
  margin-bottom: 1.5rem;
  animation: fadeSlideDown 0.7s ease 0.1s both;
}

h1 em {
  font-style: italic;
  color: var(--sml-color-primary);
}

.hero-sub {
  font-size: 1.15rem;
  color: var(--sml-text-secondary);
  max-width: 520px;
  margin: 0 auto 2.5rem;
  font-weight: 300;
  line-height: 1.7;
  animation: fadeSlideDown 0.7s ease 0.2s both;
}

.hero-cta {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  flex-wrap: wrap;
  animation: fadeSlideDown 0.7s ease 0.3s both;
}

.btn-primary {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: var(--sml-color-primary);
  color: var(--sml-text-on-primary);
  padding: 0.85rem 2rem;
  border-radius: var(--sml-radius-md);
  font-family: var(--font-body);
  font-size: 1rem;
  font-weight: 500;
  text-decoration: none;
  border: none;
  cursor: pointer;
  transition: background 0.2s, transform 0.15s, box-shadow 0.2s;
  box-shadow: var(--sml-shadow-md);
}

.btn-primary:hover {
  background: #5a7a6b;
  transform: translateY(-2px);
  box-shadow: var(--sml-shadow-lg);
}

.btn-secondary {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: transparent;
  color: var(--sml-text-secondary);
  padding: 0.85rem 1.75rem;
  border-radius: var(--sml-radius-md);
  font-family: var(--font-body);
  font-size: 1rem;
  font-weight: 500;
  text-decoration: none;
  border: 1.5px solid var(--sml-border);
  cursor: pointer;
  transition: border-color 0.2s, color 0.2s, transform 0.15s;
}

.btn-secondary:hover {
  border-color: var(--sml-color-primary);
  color: var(--sml-color-primary);
  transform: translateY(-2px);
}

/* Demo Card */
.hero-demo {
  margin-top: 4rem;
  animation: fadeSlideUp 0.9s ease 0.5s both;
}

.demo-card {
  background: var(--sml-color-surface);
  border: 1px solid var(--sml-border);
  border-radius: var(--sml-radius-lg);
  padding: 1.5rem;
  box-shadow: var(--sml-shadow-lg);
  max-width: 620px;
  margin: 0 auto;
  text-align: left;
}

.demo-search-bar {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background: var(--sml-color-bg);
  border: 1.5px solid var(--sml-border);
  border-radius: var(--sml-radius-md);
  padding: 0.85rem 1rem;
  margin-bottom: 1.25rem;
}

.demo-search-icon {
  color: var(--sml-color-primary);
  flex-shrink: 0;
  font-size: 1.1rem;
}

.demo-search-text {
  font-size: 0.95rem;
  color: var(--sml-text-secondary);
  font-style: italic;
}

.demo-results {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.demo-result-item {
  background: var(--sml-color-bg);
  border: 1px solid var(--sml-border-subtle);
  border-radius: var(--sml-radius-md);
  padding: 1rem 1.1rem;
  display: flex;
  align-items: flex-start;
  gap: 0.85rem;
  transition: border-color 0.2s, box-shadow 0.2s;
}

.demo-result-item:hover {
  border-color: var(--sml-border);
  box-shadow: var(--sml-shadow-sm);
}

.demo-result-dot {
  width: 10px; height: 10px;
  border-radius: 50%;
  flex-shrink: 0;
  margin-top: 5px;
}

.dot-calm  { background: #a4c3b2; }
.dot-proud { background: #6b9080; }

.demo-result-date {
  font-size: 0.75rem;
  color: var(--sml-text-muted);
  margin-bottom: 0.25rem;
  font-weight: 500;
}

.demo-result-text {
  font-size: 0.88rem;
  color: var(--sml-text-secondary);
  line-height: 1.5;
}

.demo-result-tags {
  display: flex;
  gap: 0.4rem;
  margin-top: 0.5rem;
  flex-wrap: wrap;
}

.tag {
  font-size: 0.72rem;
  background: var(--sml-color-surface);
  border: 1px solid var(--sml-border);
  color: var(--sml-text-muted);
  padding: 0.15rem 0.55rem;
  border-radius: 999px;
}

/* Sections */
section {
  padding: 6rem 2rem;
  position: relative;
}

.container {
  max-width: 1080px;
  margin: 0 auto;
}

.section-label {
  font-size: 0.75rem;
  font-weight: 500;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  color: var(--sml-color-primary);
  margin-bottom: 0.75rem;
}

.section-title {
  font-family: var(--font-display);
  font-size: clamp(2rem, 4vw, 3rem);
  font-weight: 600;
  letter-spacing: -0.02em;
  line-height: 1.15;
  color: var(--sml-text-primary);
  margin-bottom: 1rem;
}

.section-sub {
  font-size: 1.05rem;
  color: var(--sml-text-secondary);
  max-width: 480px;
  line-height: 1.7;
}

/* How It Works */
.how-it-works {
  background: var(--sml-color-surface);
  border-top: 1px solid var(--sml-border);
  border-bottom: 1px solid var(--sml-border);
}

.how-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 2rem;
  margin-top: 3.5rem;
}

.how-step {
  text-align: center;
  padding: 2rem 1.5rem;
  background: var(--sml-color-bg);
  border: 1px solid var(--sml-border-subtle);
  border-radius: var(--sml-radius-lg);
  transition: box-shadow 0.25s, transform 0.25s;
}

.how-step:hover {
  box-shadow: var(--sml-shadow-lg);
  transform: translateY(-4px);
}

.how-step-num {
  width: 48px; height: 48px;
  border-radius: 50%;
  background: var(--sml-color-primary);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-family: var(--font-display);
  font-size: 1.2rem;
  font-weight: 600;
  margin: 0 auto 1.25rem;
}

.how-step h3 {
  font-family: var(--font-display);
  font-size: 1.15rem;
  font-weight: 600;
  color: var(--sml-text-primary);
  margin-bottom: 0.6rem;
}

.how-step p {
  font-size: 0.9rem;
  color: var(--sml-text-secondary);
  line-height: 1.65;
}

/* Features */
.features-header {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4rem;
  align-items: center;
  margin-bottom: 4rem;
}

@media (max-width: 700px) {
  .features-header { grid-template-columns: 1fr; gap: 2rem; }
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
}

.feature-card {
  background: var(--sml-color-surface);
  border: 1px solid var(--sml-border-subtle);
  border-radius: var(--sml-radius-lg);
  padding: 1.75rem;
  transition: box-shadow 0.25s, transform 0.25s, border-color 0.25s;
  position: relative;
  overflow: hidden;
}

.feature-card::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(107,144,128,0.04) 0%, transparent 60%);
  pointer-events: none;
}

.feature-card:hover {
  box-shadow: var(--sml-shadow-lg);
  transform: translateY(-3px);
  border-color: var(--sml-border);
}

.feature-icon {
  width: 44px; height: 44px;
  border-radius: var(--sml-radius-sm);
  background: var(--sml-color-accent);
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 1.1rem;
  font-size: 1.3rem;
}

.feature-card h3 {
  font-family: var(--font-display);
  font-size: 1.05rem;
  font-weight: 600;
  color: var(--sml-text-primary);
  margin-bottom: 0.5rem;
}

.feature-card p {
  font-size: 0.9rem;
  color: var(--sml-text-secondary);
  line-height: 1.65;
}

/* Privacy */
.privacy {
  background: var(--sml-color-surface);
  border-top: 1px solid var(--sml-border);
  border-bottom: 1px solid var(--sml-border);
}

.privacy-inner {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 5rem;
  align-items: center;
}

@media (max-width: 700px) {
  .privacy-inner { grid-template-columns: 1fr; gap: 3rem; }
}

.privacy-visual {
  background: var(--sml-color-bg);
  border: 1px solid var(--sml-border);
  border-radius: var(--sml-radius-lg);
  padding: 2rem;
  box-shadow: var(--sml-shadow-md);
}

.enc-row {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 0;
  border-bottom: 1px solid var(--sml-border-subtle);
}

.enc-row:last-child { border-bottom: none; }

.enc-icon {
  width: 32px; height: 32px;
  border-radius: var(--sml-radius-sm);
  display: flex; align-items: center; justify-content: center;
  font-size: 1rem;
  flex-shrink: 0;
}

.enc-icon.green { background: rgba(107,144,128,0.15); }
.enc-icon.muted { background: rgba(204,227,222,0.4); }

.enc-label {
  font-size: 0.85rem;
  font-weight: 500;
  color: var(--sml-text-primary);
}

.enc-sub {
  font-size: 0.78rem;
  color: var(--sml-text-muted);
}

.enc-status {
  margin-left: auto;
  font-size: 0.72rem;
  font-weight: 500;
  padding: 0.2rem 0.6rem;
  border-radius: 999px;
}

.enc-status.secure { background: rgba(107,144,128,0.15); color: var(--sml-color-primary); }
.enc-status.never  { background: rgba(204,227,222,0.5); color: var(--sml-text-secondary); }

.privacy-points {
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-top: 2rem;
}

.privacy-points li {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  font-size: 0.95rem;
  color: var(--sml-text-secondary);
}

.check {
  width: 20px; height: 20px;
  border-radius: 50%;
  background: var(--sml-color-primary);
  color: white;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.7rem;
  flex-shrink: 0;
  margin-top: 2px;
}

/* Use Cases */
.use-cases-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 1.25rem;
  margin-top: 3.5rem;
}

.use-case-card {
  background: var(--sml-color-surface);
  border: 1px solid var(--sml-border-subtle);
  border-radius: var(--sml-radius-lg);
  padding: 1.75rem 1.5rem;
  transition: box-shadow 0.2s, transform 0.2s;
}

.use-case-card:hover {
  box-shadow: var(--sml-shadow-md);
  transform: translateY(-3px);
}

.use-case-emoji {
  font-size: 1.8rem;
  margin-bottom: 1rem;
  display: block;
}

.use-case-card h3 {
  font-family: var(--font-display);
  font-size: 1rem;
  font-weight: 600;
  color: var(--sml-text-primary);
  margin-bottom: 0.5rem;
}

.use-case-card p {
  font-size: 0.875rem;
  color: var(--sml-text-secondary);
  line-height: 1.6;
}

/* Audience */
.audience {
  background: var(--sml-color-surface);
  border-top: 1px solid var(--sml-border);
  border-bottom: 1px solid var(--sml-border);
}

.audience-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.25rem;
  margin-top: 3rem;
}

.audience-card {
  text-align: center;
  padding: 2rem 1.25rem;
  background: var(--sml-color-bg);
  border-radius: var(--sml-radius-lg);
  border: 1px solid var(--sml-border-subtle);
  transition: box-shadow 0.2s;
}

.audience-card:hover { box-shadow: var(--sml-shadow-md); }

.audience-emoji {
  font-size: 2.2rem;
  display: block;
  margin-bottom: 0.75rem;
}

.audience-card h3 {
  font-size: 0.95rem;
  font-weight: 500;
  color: var(--sml-text-primary);
  margin-bottom: 0.4rem;
}

.audience-card p {
  font-size: 0.82rem;
  color: var(--sml-text-muted);
  line-height: 1.55;
}

/* CTA Banner */
.cta-banner {
  padding: 7rem 2rem;
  text-align: center;
  background: linear-gradient(160deg, var(--sml-azure-mist) 0%, var(--sml-mint-cream) 50%, var(--sml-azure-mist) 100%);
  position: relative;
  overflow: hidden;
}

.cta-banner::before {
  content: '';
  position: absolute;
  width: 700px; height: 700px;
  background: radial-gradient(circle, rgba(107,144,128,0.1) 0%, transparent 65%);
  top: 50%; left: 50%;
  transform: translate(-50%,-50%);
  border-radius: 50%;
  pointer-events: none;
}

.cta-banner h2 {
  font-family: var(--font-display);
  font-size: clamp(2rem, 5vw, 3.5rem);
  font-weight: 600;
  letter-spacing: -0.02em;
  color: var(--sml-text-primary);
  margin-bottom: 1rem;
  position: relative;
}

.cta-banner p {
  font-size: 1.05rem;
  color: var(--sml-text-secondary);
  max-width: 460px;
  margin: 0 auto 2.5rem;
  line-height: 1.7;
  position: relative;
}

.cta-banner .btn-primary { position: relative; }

.cta-note {
  margin-top: 1.25rem;
  font-size: 0.8rem;
  color: var(--sml-text-muted);
  position: relative;
}

/* Footer */
footer {
  background: var(--sml-color-surface);
  border-top: 1px solid var(--sml-border);
  padding: 3rem 2rem;
  text-align: center;
}

.footer-logo {
  font-family: var(--font-display);
  font-size: 1.1rem;
  font-weight: 600;
  color: var(--sml-text-primary);
  margin-bottom: 0.75rem;
}

.footer-logo span { color: var(--sml-color-primary); }

.footer-tagline {
  font-size: 0.85rem;
  color: var(--sml-text-muted);
  margin-bottom: 1.75rem;
}

.footer-links {
  display: flex;
  justify-content: center;
  gap: 2rem;
  list-style: none;
  flex-wrap: wrap;
  margin-bottom: 2rem;
}

.footer-links li {
  display: inline;
}

.footer-links a {
  font-size: 0.85rem;
  color: var(--sml-text-muted);
  text-decoration: none;
  transition: color 0.2s;
}

.footer-links a:hover { color: var(--sml-color-primary); }

.footer-copy {
  font-size: 0.8rem;
  color: var(--sml-text-muted);
}

.footer-copy a {
  color: var(--sml-color-primary);
  text-decoration: none;
}

/* Animations */
@keyframes fadeSlideDown {
  from { opacity: 0; transform: translateY(-16px); }
  to   { opacity: 1; transform: translateY(0); }
}

@keyframes fadeSlideUp {
  from { opacity: 0; transform: translateY(20px); }
  to   { opacity: 1; transform: translateY(0); }
}

.reveal {
  opacity: 0;
  transform: translateY(24px);
  transition: opacity 0.65s ease, transform 0.65s ease;
}

.reveal.visible {
  opacity: 1;
  transform: translateY(0);
}

/* Mobile */
@media (max-width: 640px) {
  .landing-nav { padding: 0 1.25rem; }
  .nav-links { display: none; }
  .hero { padding: 6rem 1.25rem 4rem; }
  section { padding: 4rem 1.25rem; }
}
</style>
