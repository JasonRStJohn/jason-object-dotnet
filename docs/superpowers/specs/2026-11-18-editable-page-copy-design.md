# Editable copy for /work and /projects: design

**Date:** 2026-11-18
**Branch:** off `feature/design-identity` (the Field Manual identity pass, unmerged)
**Status:** draft, builder check done, awaiting owner approval

## Why

The words on `/work` and `/projects` change with the market: what problems Jason is pitching, how each is framed, which projects prove it. Today both pages are hard-coded Razor, so every wording change is a code edit and a deploy. PRODUCT.md principle 5: "Copy that changes with the market (offers, project write-ups) belongs in the admin, not in a deploy." The admin today edits only record entries (posts).

The next piece of planned work is a copy clean-up of `/work`. Building this first means that clean-up happens as admin edits, not as code that later has to be migrated.

## Goals

- Every piece of prose on `/work` and `/projects` is editable from the admin at `/admin`, behind the existing single-admin login.
- Projects can be added, edited, reordered, hidden and deleted from the admin.
- On the day it ships, both pages render **identically** to today (same words, same order, same layout), because a migration copies the current text into the database.
- Layout, colour and structure stay in code, governed by DESIGN.md.

## Non-goals

- No page builder: the admin cannot add sections, change layout, or pick colours.
- No new categories from the admin (owner decision, 2026-11-18: "If we need more categories we can revisit the code again."). The three categories are fixed in code.
- No editing of Home, About, the record pages, the app bar or the footer.
- No drafts, previews, scheduled publishing or revision history. Saving publishes.
- No images or file uploads.
- No change to the inquiry helper's mechanics (mailto, clipboard) or its timeline chips.

## Anti-goals (failure even if it works)

- Pages that look different from today after the migration runs.
- An admin edit that can break the page layout or inject script (stored XSS).
- Copy that is duplicated in two places and can drift: each category's wording lives once and feeds both pages.
- The preview or a test run changing the **live** database (see Constraints).
- An em-dash introduced by the seeded copy (VOICE.md). The seed is today's copy verbatim, which already has none.

## Constraints

- Stack is fixed: .NET 9 Blazor Server, MudBlazor 9.6, EF Core 9, SQL Server. Follow the existing patterns: `PostService` (an `IDbContextFactory` service, tested against the EF in-memory provider), `PersistentLoader` for prerender/interactive loading, `MarkdownRenderer` for Markdown, `[Authorize]` admin pages modelled on `Admin/Posts.razor` and `Admin/PostEdit.razor`.
- `Program.cs` runs `Database.MigrateAsync()` on startup. **The design preview container currently points at the live database**, so before any build with the new migration is previewed, the preview must use its own database (for example a copy named `MeDotNet_Preview` on the same SQL Server, restored from a backup of the live one). The live `:5000` container is never rebuilt from this branch until the owner says deploy.
- The repo is public. The seed text is already public on the live site, so committing it is fine. Never commit secrets.
- Client naming rule: the seeded text already follows it; the admin makes the owner responsible from then on.
- **Sanitize on render (owner requirement, 2026-11-18).** Every piece of admin-authored HTML is sanitized at render time, even though only the owner edits it, so a compromised admin account cannot plant script.
  - Plain-text fields render as text (Razor-encoded), never `MarkupString`.
  - Markdown fields go through `MarkdownRenderer`, and **its output passes an allowlist HTML sanitizer before it reaches the page**: known tags only, no `on*` event attributes, no `style`, and URLs limited to http, https, mailto and relative. This is a separate fix that must land first (see "Prerequisite" below).
  - The only route to `MarkupString` is the renderer's sanitized output. A test fails the build if any other `MarkupString` cast appears in `Components/`.

## Prerequisite: close the attribute-injection gap

Found 2026-11-18 while checking this requirement. `MarkdownRenderer` uses `UseAdvancedExtensions()`, which includes Markdig's generic-attributes syntax, so Markdown like `hello {onclick="alert(1)"}` renders as `<p onclick="alert(1)">`. Raw HTML and `javascript:` links are already blocked; event attributes are not. This affects record entries on the live site today.

The fix:
- Drop generic attributes from the pipeline.
- Add the output sanitizer.
- Add tests for the injection cases.

It ships as a hotfix off `main`, ahead of and independent from this feature.

## Approaches considered

1. **Typed tables (chosen).** A `Category` row per fixed category, a `Project` table, and a small set of named `PageText` blocks. The admin gets labelled forms. It matches how posts already work, validates field by field, and keeps each piece of copy in one place.
2. **One JSON document per page,** edited in a large text box. Fast to build, but one malformed edit breaks the page, there is no per-field validation, and it is miserable on a phone.
3. **Generic content blocks** (pages composed of ordered blocks). Flexible, but it is a page builder, far beyond two pages, and it erodes the rule that layout and colour are design-time decisions.

The owner agreed the shape ("That's basically what I had in mind"); the storage choice is the builder's call, recorded here.

## Design

### Data model

**Category** (exactly three rows, keyed by the existing slugs, never created or deleted from the admin):

| Field | Used on | Today's example (`ai-systems`) |
|---|---|---|
| `Key` (PK, string) | both; also the URL anchor and `?category=` value | `ai-systems` |
| `SortOrder` | both | 1 |
| `WorkTitle` | /work panel heading | `1. Curated AI Harnesses & Workflow Architecture` |
| `WorkTag` | /work panel pill | `Architecture / Sprint` |
| `WorkProblem` (Markdown) | /work "The problem" | the current paragraph, without the bold "The problem:" label (the label stays in code) |
| `WorkApproach` (Markdown) | /work "How I approach it" disclosure | current paragraph; `<strong>ClaudePress</strong>` becomes `**ClaudePress**` |
| `WorkLinkText` | /work "See the..." button | `See the AI harness & workflow systems` |
| `InquiryLabel` | inquiry helper chip and email subject | `AI Harness & Workflow Architecture` |
| `ProjectsShortLabel` | /projects filter chip | `AI Harnesses` |
| `ProjectsTitle` | /projects group heading | `AI Harnesses & Agent-Directed Systems` |
| `ProjectsSubtitle` | /projects group intro | current sentence |

The accent colour stays in code, mapped from `Key` (`ai-systems` violet, `platforms` cyan, `claudepress` green), as it is today in `GroupAccent()`.

**Project:**

| Field | Notes |
|---|---|
| `Id` | int PK |
| `CategoryKey` | FK to Category |
| `SortOrder` | order within its category |
| `Title` | plain text, required |
| `Badge` | pill text, e.g. `Open Source`, `Previous Employer` |
| `Meta` | the mono line, e.g. `2026 · Python, WebSockets, Rust, Docker` |
| `Body` | Markdown, required |
| `Tags` | stack list; stored as one newline-separated string, edited as one tag per line |
| `LinkText`, `LinkHref` | optional pair; both or neither; `LinkHref` must be `http(s)` |
| `IsEmployer` | shows the employer caption (that caption is a PageText block) |
| `RecordProject` | optional; links to `/notes?project=...` as today |
| `IsVisible` | hidden projects are kept but not shown; default true |

`BadgeColor` in today's `ProjectEntry` is unused by the Field Manual markup and is dropped.

**PageText** (named blocks; the set of keys is fixed in code, the admin edits values only):

| Key | Page | Format |
|---|---|---|
| `work.intro` | /work opening paragraph | Markdown |
| `work.together.contract` | "How we can work together", contract paragraph (body after the bold label) | Markdown |
| `work.together.fulltime` | same, full-time paragraph | Markdown |
| `work.inquiry.intro` | the "Start a conversation" instructions | Markdown |
| `projects.intro` | /projects opening paragraph | Markdown |
| `projects.employerNote` | the caption under employer projects | plain text |

The headings themselves ("Work with me", "How we can work together", "Start a conversation", "Projects"), the bold field labels, the button labels ("Ask about this track"), the leadership inquiry option ("Full-Time / Fractional Leadership") and the timeline chips stay in code. They are interface, not market copy. The builder may move any of these into PageText only if it is trivial and the owner agrees.

### Seeding

**"Today's copy" means the copy at the head of `feature/design-identity`** (this branch's base), not `main`. That branch's copy is frozen from the moment this branch starts: the planned `/work` copy clean-up waits and happens as admin edits after this ships (owner, 2026-11-18: build the admin before the clean-up). If any copy edit does land on `design-identity` before merge, the seed must be re-synced from it before merging.

**Shipping order** *(builder's note; the deploy itself is the owner's call)*: `design-identity` can go live on its own at any time, since it has no database change. This branch stacks on it; if the owner deploys both at once, it is one release that changes the design and migrates the database, so take a SQL Server backup first (SQL Server is outside the nightly backup).

One EF migration creates the three tables and inserts today's copy verbatim (from `Work.razor` and `Projects.razor` at the head of `feature/design-identity`), including all 3 categories, every project in today's order, and the 6 PageText blocks. A test asserts that the seeded counts and a sample of fields match.

### Public pages

`Work.razor` and `Projects.razor` read through a new `PageCopyService` (all three categories with their visible projects, PageText by key), loaded with `PersistentLoader` so prerender and the interactive pass don't double-query. Layout, wrappers and CSS classes stay as they are; only the source of the strings changes. **"Identical" is judged visually** (the before/after screenshots), not by the DOM: small markup changes are fine where the look holds.

**Two Markdown modes** *(builder's call, settled after the builder check)*:
- **Inline** for text that continues a bold label in code (`WorkProblem`, `work.together.contract`, `work.together.fulltime`): bold, italic, links and inline code only, rendered without a wrapping paragraph so it sits in the same `<p>` after the label. Add `MarkdownRenderer.ToInlineHtml` sharing the same pipeline and link-scheme allowlist. The admin form rejects a blank line in these fields ("one paragraph only").
- **Block** for everything else Markdown (`WorkApproach`, project `Body`, the PageText intros): rendered by `ToHtml` into a `div` carrying the same typography and measure classes the `MudText` used, so paragraphs never nest. Links inside rendered Markdown get the site's link styling (the inquiry intro's email link was a `MudLink`; it must look the same).

**When a category has no visible projects** *(builder's call)*: `/projects` omits that group and its filter chip, and the matching `/work` panel hides its "See the..." button (the panel itself, its text and "Ask about this track" stay). `PageCopyService` exposes, per category, whether it has visible projects, so both pages use one rule. The inquiry helper's track chips become the three `InquiryLabel`s in category order, followed by the fixed leadership option. The "Ask about this track" buttons select their category's `InquiryLabel`.

If a PageText key is missing, the block renders nothing and the page still works.

### Admin

Admin nav gains **Projects** and **Page text** beside **Manage Posts**.

- `/admin/projects`: a table grouped by category: title, badge, visible toggle, up/down reorder within the category, edit, delete (with the same confirm dialog posts use). "New project" picks a category.
- `/admin/projects/{id}` and `/admin/projects/new`: a form with one labelled field per Project column; Markdown body as a multi-line field; tags as one per line.
- `/admin/categories/{key}`: an edit form for a category's fields, grouped under "/work" and "/projects" headings so it is clear where each appears. Reached from the Page text screen.
- `/admin/page-text`: one screen listing the three categories (links to their forms) and the six PageText blocks as labelled multi-line fields, each with a line saying where it appears. One Save button.

Each admin form has a "View page" link to the public page it affects. Saving shows the existing success snackbar. Validation: required fields non-empty; link pair both-or-neither; `LinkHref` http(s) only.

### Testing

- `PageCopyService` tests on the EF in-memory provider, like `PostServiceTests`: category order, hidden projects excluded, project order, PageText lookup, missing key.
- Validation tests for the Project rules.
- A seed test: the migration's seed data matches today's copy (counts plus spot checks, including that no seeded string contains an em-dash).
- After the build, before/after screenshots of `/work` and `/projects` in both themes at desktop and phone against the **preview database**, showing no visible change.

## Left to the builder

- Table and column names, and exact service method names.
- Whether reordering is up/down buttons or drag; up/down is enough.
- Field lengths and indexes.
- Whether to cache page copy in memory (not required at this traffic; if it is added, saves must invalidate it).
- How the preview gets its own database, as long as the live database is never migrated by the preview.
