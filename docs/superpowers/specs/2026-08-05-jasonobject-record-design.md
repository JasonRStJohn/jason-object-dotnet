# Phase 5 — The Record: Purpose Rework Design Spec

**Date:** 2026-08-05
**Branch:** `feature/the-record`, off `main` **after** `feature/site-content` merges (see Prerequisites)
**Supersedes:** the positioning decisions in `2026-07-07-site-content-design.md` (structure and copy only; that spec's nav, page-title and contact conventions still stand)

## Goal

Change what jasonobject.work is *for*. It currently reads as a portfolio-plus-blog aimed at a full-time job search. It has to serve two audiences at once — hiring managers **and** prospective contract clients — without forking into two sites or two landing pages.

The reframe: **jasonobject.work is a working record of directing AI agents to build production software.** Not a résumé, not a blog — a body of evidence. Both asks live on it; neither is what the site *is*. That is what lets one site serve two audiences: visitors self-select against evidence rather than being asked to classify themselves.

The stack does not change. .NET 9 Blazor Server, MudBlazor 9.6, EF Core 9, Identity, SQL Server, Docker Compose — all retained.

## Why this shape

Four possible offers were ranked by interest: (1) AI workflow consulting/training, (2) small-business sites and CMS, tied with (3) .NET platform contract work, and last (4) full AI-accelerated app builds. The current Home page sells option 4 — the least wanted — and spends the teaching credential as a credibility prop for app-building rather than as the product.

The top-ranked offer has no defined buyer, no named engagement shape, and no paid track record. Building a services page around it now would produce a thin page that also damages the full-time read: hiring managers see someone building an exit. Publishing the method instead lets the offer define itself out of what people actually ask about, and costs nothing if nobody asks.

Rejected alternatives:

- **Two front doors** (a forked homepage: "Hiring?" / "Need help?"). Makes visitors self-classify before they know who you are, and reads as uncertainty about what you do.
- **Productised consulting now** (services page with named offerings). Premature — no buyer, no case studies, and it actively harms the FTE read.

## Decisions

| Decision | Choice |
|---|---|
| Purpose | A working record of directing AI agents to build production software |
| Audiences | Hiring managers and contract clients, served by one unforked site |
| Blog | **Absorbed into The Record.** No separate Blog section or nav item |
| Record entry kinds | `Spec`, `Plan`, `PostMortem`, `Note` — four, not a flat artifact/note split |
| Record layout | One chronological stream, colour-coded left rail per kind, filter chips by kind **and** project |
| Home hero | Direction A — "I sit between the people paying for software and the machines writing it" |
| Home layout | Layout C — typography-first hero, record entries below, sidebar carrying both asks |
| Entry images | **None.** No featured-image column, no upload path. Visual interest comes from rails, chips and framing lines |
| Consulting offer | Stated as an invitation on `/work`, not a service catalogue. No pricing |
| Publishing model | Hand-curated via existing CMS. No auto-publishing from the repo |
| Stack | Unchanged |

## Prerequisites

1. **Merge `feature/site-content` into `main` first.** It is five commits ahead and unmerged (`b548adc`…`b4d3b5e`), carrying the JasonObject rebrand plus the About and Projects pages. This spec builds on that content; branching Phase 5 off an unmerged branch compounds the problem.
2. **Image tooling.** The box has no ImageMagick, PIL or ffmpeg. Resolve by `sudo apt install imagemagick` (preferred — one-time job) or by adding ImageSharp and a small dotnet utility.
3. **Commit or discard** the stray one-line comment in `src/MeDotNet/Program.cs` (note: it reads "MubBlazor", a typo) and decide on the untracked files in `docs/`.

## Structure

### Navigation

App bar title stays **JasonObject**. Drawer items become: **Home · The Record · Projects · About**, then the existing `AuthorizeView` block (Manage Posts, Logout / Login) unchanged. The app bar gains a **Work with me** button, visually distinct from the nav links. The dark-mode toggle stays in the app bar; the GitHub / LinkedIn / Email icon links were moved out of the app bar into a pinned block at the bottom of the drawer (a deviation made in response to app-bar crowding on small screens).

`Blog` disappears as a nav item. `/posts` and `/posts/{slug}` routes are **retained and redirected** to `/notes` and `/notes/{slug}` so nothing already linked breaks.

### Site map

| Route | Status | Role |
|---|---|---|
| `/` | Rewrite | Hero, latest record entries, sidebar with both asks |
| `/notes` | Rewrite of `/posts` | The Record — filterable stream |
| `/notes/{slug}` | Rewrite of `/posts/{slug}` | Single entry |
| `/projects` | Reframe | Nine projects, reordered, linked into the record |
| `/about` | Extend | Narrative arc plus the C-suite beat |
| `/work` | New | Both asks, plainly stated |
| `/admin/posts`, `/admin/posts/{id}` | Extend | Kind and Project selectors added |

## Data model

`Post` currently carries `Id, Title, Slug, Body, PublishedAt, CreatedAt, AuthorId, Author`. Three additions:

```csharp
public enum PostKind { Note = 0, Spec = 1, Plan = 2, PostMortem = 3 }

public PostKind Kind { get; set; } = PostKind.Note;
public string? Project { get; set; }   // free text, null for kind-only entries
public string? Summary { get; set; }   // one-line framing shown under the title
```

`Kind` defaults to `Note` so existing rows migrate without ambiguity. `Project` is deliberately free text rather than a lookup table — there are three values today and a join table earns nothing yet. `Summary` is the framing line that makes an artifact legible to someone who won't open it; it is what turns a raw spec into evidence.

One EF Core migration. `PostService` gains filtered queries by kind and project. Admin editor gains a kind selector, a project text field, and a summary field.

`Summary` is nullable but expected on every published entry; where it is null the record and Home lists render the title alone with no empty line reserved.

**Explicitly not added:** featured image, tags, categories, comments, view counts. Note that this constrains only *entry-level* images — an entry's markdown body can still reference images committed to `wwwroot/img/`, which is how screenshots inside a write-up will work.

### Rail colours

| Kind | Colour | Rationale |
|---|---|---|
| Spec | Purple | Primary — the site's dominant accent |
| Plan | Purple | Same family as Spec; both are forward-looking documents |
| PostMortem | Orange | Deliberately distinct — the most persuasive category deserves to be findable |
| Note | Green | Clearly separate from all artifacts |

## Page designs

### Home (`/`) — layout C

Typography-first. Nothing shares the fold with the opening sentence.

> Jason St. John — lead full-stack developer, Upstate New York
>
> # I sit between the people paying for software and the machines writing it.
>
> Eight years architecting production .NET and Vue/Nuxt platforms, and a decade of teaching before that. Directing AI agents is an instruction problem: decomposition, scaffolding, feedback, and telling the difference between a model that misunderstood and one that simply wasn't given enough. Below is the record of doing it.

Then **LATEST FROM THE RECORD** — the three most recent entries in a two-column split: entries at left (rail, kind label, title, summary), and at right a sidebar card containing `Headshot1crop`, this text, and a link to `/work`:

> Open to lead and staff engineering roles — and taking on contract work now.

The sidebar is where the two-audience problem gets solved: both asks, one quiet block, neither subordinated to the other.

The existing `PersistentLoader` wiring is kept exactly as-is; only the query changes (three most recent published entries of any kind).

**The current "This site is the demo" callout is removed.** Its argument is now made by the record itself, at length and with evidence. Restating it in a box is weaker than showing it.

### The Record (`/notes`)

Heading, one-line description, then two tiers of filter chips — kind (All / Specs / Plans / Post-mortems / Notes) above project (JasonObject / ClaudePress / Whindancer, derived from distinct non-null `Project` values). Below, one chronological stream, newest first.

Each entry: 3px left rail in the kind colour, a kind chip, project and date, title, and the one-line summary. Filtering is server-side via `PostService`; state lives in the query string (`/notes?kind=postmortem&project=jasonobject`) so filtered views are linkable and survive a refresh.

Empty-filter state must say something useful rather than rendering a blank column.

### Projects (`/projects`)

Keep all nine write-ups and the existing static-component approach. Two changes:

1. **Reorder.** AI-directed builds lead — JasonObject, then the ClaudePress/Whindancer work — with employer systems supporting. Current newest-first ordering buries the site's own argument.
2. **Link into the record.** Where a project has published artifacts, the card gets a "Read the record →" link to `/notes?project=<name>`.

### About (`/about`)

The existing four-section narrative holds up and is largely retained. Changes:

- **Add the C-suite beat** to "The thesis": you were the person who could talk to executives about AI without overselling it or burying them in context; the specialist hired afterward had more raw skill and less ability to translate, and it went less well. Framed as a description of the role played, **not** as a claim about outcomes — the counterfactual ("nobody was laid off") is unprovable and reads as credit-taking.
- `Headshot2crop` (cabin, guitars) placed here, where being a person is the point.
- The hiking photo as a wide banner — **requires a real rotation**, not a CSS transform, or it renders sideways in social cards and search previews.

### Work with me (`/work`)

Three blocks, no pricing, no engagement-shape catalogue:

> **Full-time.** Open to lead and staff engineering roles — .NET, Vue/Nuxt, platform architecture. Remote, or on-site around Upstate New York.
>
> **Contract.** Available now. Small-business sites and CMS work, .NET platform and integration work, and full builds where directing agents is how the work gets done.
>
> **The thing I'm most interested in.** If your team is working out how to use AI agents well — not the tooling, the practice — I'd like to talk. It's the work I care most about, and the record on this site is me building the case in public rather than claiming a track record I haven't earned yet.

Then the contact block: email (primary), GitHub, LinkedIn. Static component, no services.

## Assets

| Asset | Destination | Work needed |
|---|---|---|
| `Headshot1crop.jpg` | Home sidebar, `og:image` | Resize to ~800px wide, quality pass. Currently 2469×3392, 1.1 MB |
| `Headshot2crop.jpg` | About | Resize to ~900px wide. Currently 2030×2266, 1.4 MB |
| `IMG_20260707_162952 (2).jpg` | About banner | **Rotate 90° destructively**, crop to banner ratio, resize. Currently 4032×3024, 4.4 MB |
| `default-featured*.jpg` (Disney) | **Not shipped** | Off-message for a site pitching contract work, and an odd note with a live Imagineering application |

All shipped images land in `wwwroot/img/`. `ImagePool/` is a working directory and must be added to `.gitignore` — it holds 15 MB of originals and duplicates (`default-featured.jpg` and `IMG_20260707_162952.jpg` are byte-identical).

**Gap to close separately:** every image in the pool is a photo of Jason. There is no imagery of the *work* — no screenshots of the ClaudePress client sites, no diagram of the spec→plan→review loop. For the contract audience that imagery does more than any portrait. Both can be produced from the repos. Scoped out of this phase, but it is the highest-value visual work remaining.

## Content curation

Publishing is hand-curated. The repo is **not** auto-published — the source documents contain some previous-employer material, and an automatic pipeline would remove the redaction gate.

**Process per entry:** read the source document in full → check for employer names, internal systems, and internal decisions → genericise or cut → write the one-line summary → paste into the CMS with kind and project set → Jason approves before publish.

**First batch (five entries)**, enough to make the page look alive on day one without a redaction marathon:

| Source | Kind | Project |
|---|---|---|
| `2026-07-07-mudblazor-design.md` | Spec | JasonObject |
| `2026-07-07-site-content-design.md` | Spec | JasonObject |
| `2026-07-05-phase2-cms.md` | Plan | JasonObject |
| `/mnt/external/medotnet-review-post-mortem.md` | PostMortem | JasonObject |
| *Written fresh* — "Reviewing every task before it lands" | Note | — |

Long documents are published with their framing summary and full body; no truncation. The `.superpowers/sdd/` task briefs and reports are a candidate second batch but are numerous and repetitive — better mined for one synthesised post-mortem than published individually.

## Testing

Existing `PostServiceTests` and `IdentityAuthServiceTests` must keep passing. New coverage:

- `PostService` filtering by kind, by project, and by both together
- Default `Kind` of `Note` applied to rows created before the migration
- `/posts` → `/notes` redirect preserves the slug
- Empty-filter state renders its message rather than a blank list

## Out of scope

Featured images and any upload path; tags, categories or comments; pricing or engagement-shape pages; a custom MudBlazor theme or palette beyond the rail colours; renaming the `MeDotNet` solution or namespaces; resume download; screenshots and process diagrams (tracked separately as the imagery gap); auto-publishing from the repo.
