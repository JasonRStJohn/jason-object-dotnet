# Phase 6 — Problem-Solver Positioning & 2026 Portfolio Update (`jasonobject.work`)

**Date:** 2026-11-14  
**Branch:** `feature/problem-buckets` (branched off `main` at `3d88d3f`)  
**Updates:** `Work.razor`, `Projects.razor`, `Home.razor` (extends Phase 5 `2026-08-05-jasonobject-record-design.md`)

## Goal

Update `jasonobject.work` so it supports active contract and architectural consulting conversations (enterprise AI harness & ERP integration, civilian VTOL airspace tooling, public library / municipal WordPress fleet migrations, and civic software) without reading like a sales pitch or alienating full-time Lead/Principal engineering opportunities.

The voice and structure follow one core rule: **diagnose the technical problem rather than pitching the person.**

## Decisions

| Decision | Choice |
|---|---|
| Positioning voice | Calm Tech Lead / Problem Solver — explain what breaks in the wild, how the architecture solves it, and link directly to proof |
| `/work` structure | Three Problem-Pattern Buckets (*Curated AI Harnesses & Workflow Architecture*, *Custom Line-of-Business & Civic Software*, *Legacy Web & Fleet Migrations*) followed by a single *How We Can Work Together* card covering both contract and full-time leadership |
| `/projects` structure | Grouped into three anchored sections (`#ai-systems`, `#platforms`, `#claudepress`) matching the three `/work` buckets so proof links jump directly to the relevant systems |
| Client & contract privacy | Public GitHub repos (`ClaudePress`, `agentic-job-search`, `jason-object-dotnet`, `Chasma`, `cyoa`) are named and linked; private contract and client work (VTOL airspace, Oracle Fusion + AI workforce harness, municipal and small-business migrations) is described by engineering problem and architecture without naming clients |
| Future rename readiness | `ClaudePress` is described as a model-agnostic Nuxt 3 + PocketBase layer alongside its vertical layers (`ShelfPress`, `PawPress`), structured so renaming `ClaudePress` to `AnchorPress` later is a single string update |
| `/` (Home) update | Keep existing hero and JSON bio card; update `"builds"` value in the JSON card and add a single quiet signpost sentence linking to `/work` above *Latest from the record* |
| Rollback strategy | All work lives on `feature/problem-buckets`; `main` sits at the exact commit (`3d88d3f`) running on `jasonobject.work` prior to this change |

## Page Changes

### 1. `/work` (`src/MeDotNet/Components/Pages/Work.razor`)

Replaces the three brief "Full-time / Contract / The thing I'm most interested in" cards with:

1. **Page intro (`Typo.body1`):**
   > I take on contract architecture and development work—and I'm open to the right lead or principal engineering role. Below are the three problems people usually bring me in to solve.

2. **Three Problem-Diagnosis Cards (`MudPaper`, `pa-6 mb-4`, `Elevation="1"`):**
   - **Card 1: Curated AI Harnesses & Workflow Architecture**
     - *The problem:* Organizations adopting AI usually get stuck between two bad options: pasting sensitive context into raw chat windows with no guardrails, or buying black-box "agent" platforms that hallucinate the moment they touch messy enterprise data.
     - *How I approach it:* Directing models reliably is an instruction and systems-architecture problem. I build constrained, multi-stage harnesses that sit between existing systems of record (ERPs, databases, internal APIs) and the people making decisions—separating context assembly, generation, and cold verification so outputs are auditable and repeatable.
     - *Action link:* `See the AI harness & workflow systems →` (`/projects#ai-systems`)
   - **Card 2: Custom Line-of-Business & Civic Software**
     - *The problem:* Operations teams, startups, and municipalities regularly outgrow spreadsheets and off-the-shelf SaaS, or get trapped paying steep licensing fees for rigid legacy software that doesn't match how their people actually work.
     - *How I approach it:* I architect and ship custom web platforms and automation engines in .NET, C#, Vue/Nuxt, and SQL—from multi-workflow production systems and airspace tooling to civic utilities and operational dashboards. Clean data models, permissions that match the floor, and codebases another engineer can actually maintain.
     - *Action link:* `See the platform & line-of-business builds →` (`/projects#platforms`)
   - **Card 3: Legacy Web & Fleet Migrations**
     - *The problem:* Small businesses, public library systems, and local governments often run on aging WordPress installs buried under twenty plugins—slow, fragile on update day, and confusing for staff who just need to post an agenda, a catalog update, or an event.
     - *How I approach it:* I built **ClaudePress** (and vertical layers like **ShelfPress** for public libraries and **PawPress** for specialty businesses)—a fast, model-agnostic Nuxt 3 + PocketBase stack with an admin interface tailored to the organization's actual content, paired with an automated migration pipeline that moves sites off WordPress without billing for a hundred hours of hand-copying.
     - *Action link:* `See the web platforms & migrations →` (`/projects#claudepress`)

3. **Engagement Card (`MudPaper`, `pa-6 mb-8`, `Elevation="1"`):**
   - Heading: `How we can work together`
   - **Contract & Architecture:** Available for scoped system builds, ERP/AI harness architecture, fractional technical leadership, and single-site or fleet web migrations.
   - **Full-Time Leadership:** Open to Lead, Staff, or Principal engineering roles (remote or around Upstate New York) with teams that value clear architecture and pragmatic AI adoption over hype.

4. **Contact Block:** Retained unchanged (Email, GitHub, LinkedIn).

---

### 2. `/projects` (`src/MeDotNet/Components/Pages/Projects.razor`)

Replaces the flat `_projects` list with three anchored sections (`id="ai-systems"`, `id="platforms"`, `id="claudepress"`), adding the 2026 systems and updating the former-employer caption.

#### Section 1: `id="ai-systems"` — AI Harnesses & Agent-Directed Systems
- **Enterprise Workforce & ERP AI Harness**
  - *Meta:* `Contract Architecture, 2026 · Enterprise ERP Integration, Curated AI Harness`
  - *Body:* Scoping and architecting a workforce management platform that bridges enterprise ERP data (Oracle Fusion) with a curated AI evaluation harness—giving operational leaders real-time, grounded feedback on personnel moves across multiple projects and regions without exposing raw data to unconstrained prompts.
- **Site-Creator 2.0 — Agentic Site Factory**
  - *Meta:* `Internal Production Pipeline, 2026 · Multi-Agent Workflow, Nuxt 3, PocketBase, Vitest`
  - *Body:* A five-department, model-agnostic agentic pipeline (Intake, Architecture, Design, Implementation, and QA) that ingests client briefs, design mockups, or legacy WordPress exports and produces tested, production-ready web applications. Each stage operates from isolated Markdown context files with automated verification gates between handoffs.
- **Agentic Job Search**
  - *Meta:* `Personal / Open Source, 2026 · Multi-Agent Harness, Python, Markdown`
  - *Body:* A portable multi-department AI workflow that separates generation (resume tailoring and drafting) from cold verification (zero-context screener simulation and source-of-truth auditing). Built entirely in plain Markdown context files with no framework lock-in.
  - *Link:* `Source on GitHub` (`https://github.com/JasonRStJohn/agentic-job-search`)
- **jasonobject.work — this site**
  - *Meta:* `Personal, 2026 · Blazor Web App, .NET 9, EF Core, ASP.NET Identity, MudBlazor, Docker`
  - *Body:* Existing copy retained, including `Source on GitHub` and `Read the record` links.

#### Section 2: `id="platforms"` — Line-of-Business, Airspace & Real-Time Systems
- **Civilian VTOL Airspace & ATC Tooling**
  - *Meta:* `Contract, 2026 · Aviation & Airspace Operations Software`
  - *Body:* Contract engineering for an emerging aviation technology company building mid-range air traffic control and operational software to support the rollout of civilian VTOL aircraft.
- **Six Former-Employer Systems** (*Job Management Platform 2.0*, *Automation scripting engine*, *Ballot production & tracking*, *Client Portal 2.0*, *Multi-workflow job system*, *Reactive invoicing system*)
  - *Meta & Body:* Existing descriptions retained.
  - *Employer caption fix:* Changed from `"Built at my current employer — not publicly available."` to `"Built during an 8-year tenure as Lead Developer — proprietary system, ask me about the architecture."`
- **Chasma & CYOA**
  - *Meta:* `Personal / Open Source, 2026 · Python, WebSockets, Rust, Docker`
  - *Body:* Two explorations in real-time state and systems programming: **Chasma**, an online multiplayer 2D space arena game running over WebSockets, and **CYOA**, an interactive branching narrative engine built in Rust while learning the language alongside an agent.
  - *Link:* `Chasma on GitHub` (`https://github.com/JasonRStJohn/Chasma`)

#### Section 3: `id="claudepress"` — Web Platforms & Legacy Migrations
- **ClaudePress, ShelfPress & PawPress**
  - *Meta:* `Open Source Core + Vertical Layers, 2026 · Nuxt 3, Vue, TypeScript, PocketBase, Tailwind, Docker`
  - *Body:* A modular, self-hosted Nuxt 3 + PocketBase content and application layer designed to replace bloated WordPress stacks. Extended into specialized vertical platforms including **ShelfPress** (for public library systems with catalog, event, and board-governance needs) and **PawPress** (for specialty breeders and kennels).
  - *Link:* `ClaudePress on GitHub` (`https://github.com/JasonRStJohn/ClaudePress`)
- **Municipal & Small-Business Platform Migrations**
  - *Meta:* `Contract, 2026 · Nuxt 3, PocketBase, Automated Content Migration, VPS Infrastructure`
  - *Body:* Migrating municipal village government and commercial sites off fragile legacy WordPress installs onto self-hosted ClaudePress stacks—preserving years of public records, meeting minutes, and search indexing while giving non-technical staff a clean, foolproof publishing workflow.
- **CateredTo** & **AGFTC website**
  - *Meta & Body:* Existing historical cards retained at the bottom of this section.

---

### 3. `/` (`src/MeDotNet/Components/Pages/Home.razor`)

1. **JSON Card (`"builds"` field):**
   Updated from:
   ```json
   "builds": "the internal tools others work in",
   ```
   to:
   ```json
   "builds": "line-of-business platforms, AI harnesses, and the tools others work in",
   ```
2. **Signpost line under the bio paragraph:**
   Added directly below the bio `<MudText>` and above `Latest from the record`:
   > Taking on contract architecture, platform builds, and legacy migrations now—and open to the right lead engineering role. [See the problems I solve →](/work)

## Testing & Verification

- Existing unit test suite (`dotnet test`) must pass with zero regressions.
- Verify `/`, `/work`, and `/projects` render cleanly and that `/work` anchor links (`/projects#ai-systems`, `/projects#platforms`, `/projects#claudepress`) match the section IDs on `/projects`.
