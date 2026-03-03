# Frontend Development Plan — "Search My Life"

## Phase 0: Project Scaffolding
1. **Create the Vue 3 project** using `create-vue` (Vite-based) inside a `frontend/` directory at the repo root.
2. **Add Vuetify 3** as the UI framework.
3. **Add Vue Router** for page navigation.
4. **Add Pinia** for state management (auth state, entries, search results).
5. **Configure project structure:**
```
frontend/
├── public/
├── src/
│   ├── assets/            # Static assets, global styles
│   ├── components/        # Reusable UI components
│   ├── views/             # Page-level components (routed)
│   ├── router/            # Vue Router config
│   ├── stores/            # Pinia stores
│   ├── services/          # API client, crypto, AI helpers
│   ├── utils/             # Shared helpers (date formatting, etc.)
│   ├── App.vue
│   └── main.js
├── tests/                 # Unit tests (Vitest)
├── index.html
├── vite.config.js
└── package.json
```

---

## Phase 1: Authentication UI
**Goal:** Registration, login, and JWT token management.

| Task | Details |
|---|---|
| `views/LoginView.vue` | Email + password form, calls auth API, stores JWT |
| `views/RegisterView.vue` | Registration form with password confirmation |
| `stores/authStore.js` | Pinia store: `user`, `token`, `isAuthenticated`, `login()`, `logout()`, `register()` |
| `services/apiClient.js` | Axios instance with JWT interceptor (attaches `Authorization` header) |
| `router/index.js` | Navigation guards — redirect unauthenticated users to `/login` |
| **Unit tests** | Test auth store logic (token storage, logout clearing state) |

---

## Phase 2: Encrypted Journaling (Core CRUD + Crypto)
**Goal:** Create, read, update, delete journal entries — encrypted client-side.

| Task | Details |
|---|---|
| `services/cryptoService.js` | AES-GCM encryption/decryption via **Web Crypto API**. Derive key from user password using PBKDF2. Exports `encrypt(plaintext, password)` → `{ciphertext, iv}` and `decrypt(ciphertext, iv, password)` |
| `views/JournalEntryView.vue` | Rich text editor for writing entries. On save: encrypt → send ciphertext to API |
| `views/JournalListView.vue` | List of entries (decrypted client-side on load) |
| `components/EntryCard.vue` | Displays a single entry: date, summary, emotion tag, expandable body |
| `stores/journalStore.js` | Pinia store: `entries[]`, `createEntry()`, `fetchEntries()`, `deleteEntry()` — handles encrypt/decrypt flow |
| **Unit tests** | Test `cryptoService.js` round-trip (encrypt → decrypt), test store actions |

> **Key design decision:** The encryption key is derived from the user's password and **never sent to the server**. Store the derived key in memory only (Pinia store) — cleared on logout.

---

## Phase 3: Semantic Search UI
**Goal:** Natural language search bar that queries the vector search backend.

| Task | Details |
|---|---|
| `components/SearchBar.vue` | Vuetify text field with debounced input, emits search query |
| `views/SearchResultsView.vue` | Displays ranked results from the API, each decrypted client-side |
| `services/searchService.js` | `searchEntries(query)` → calls backend semantic search endpoint, returns matched entries |
| `stores/searchStore.js` | Pinia store: `query`, `results[]`, `isSearching`, `search()` |
| **Unit tests** | Test search store state transitions |

---

## Phase 4: Timeline View + Emotional Metadata
**Goal:** Interactive timeline grouped by month, color-coded by emotion.

| Task | Details |
|---|---|
| `views/TimelineView.vue` | Main timeline page — groups entries by month, scrollable |
| `components/TimelineMonth.vue` | Section header + list of entry cards for one month |
| `components/EmotionBadge.vue` | Color-coded chip/badge (e.g., 🟢 happy, 🔴 stressed, 🔵 calm) |
| `components/EntryExpanded.vue` | Expanded entry view with full body, sentiment score, tags, AI summary |
| `utils/emotionColors.js` | Map of emotion → color/icon for consistent theming |
| **Unit tests** | Test emotion mapping, date grouping utility |

---

## Phase 5: AI Reflections & Trends
**Goal:** Generate and display AI-powered reflections and emotional trend charts.

| Task | Details |
|---|---|
| `components/ReflectionCard.vue` | Displays an AI-generated weekly/monthly reflection |
| `views/InsightsView.vue` | Dashboard with emotion trend chart + generated reflections |
| `services/insightService.js` | `generateReflection(timeRange)`, `getEmotionTrends(timeRange)` |
| Chart integration | Use a lightweight chart lib (e.g., Chart.js via `vue-chartjs`) for emotion trends |
| **Unit tests** | Test data transformation for chart input |

---

## Phase 6: Polish & Testing

| Task | Details |
|---|---|
| Responsive layout | Ensure mobile-friendly design with Vuetify breakpoints |
| Dark/light theme | Vuetify theme toggle |
| Error handling | Global error snackbar, API error interceptor |
| Loading states | Skeleton loaders for entries and search results |
| Comprehensive tests | Target core services (`cryptoService`, stores, utils) |

---

## Route Map

| Route | View | Auth Required |
|---|---|---|
| `/login` | `LoginView` | No |
| `/register` | `RegisterView` | No |
| `/timeline` | `TimelineView` | Yes |
| `/entry/new` | `JournalEntryView` | Yes |
| `/entry/:id` | `JournalEntryView` (edit) | Yes |
| `/search` | `SearchResultsView` | Yes |
| `/insights` | `InsightsView` | Yes |

---

## Recommended Starting Order
1. **Phase 0** — Scaffold the project
2. **Phase 1** — Auth UI + routing guards (can stub the API)
3. **Phase 2** — Crypto service + journal CRUD (hardest piece — start early)
4. **Phase 3** — Search bar + results
5. **Phase 4** — Timeline + emotion display
6. **Phase 5** — AI reflections + charts
7. **Phase 6** — Polish
