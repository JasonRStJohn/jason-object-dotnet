# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users

Two audiences, served by one site with no forked front door. Visitors sort themselves by reading the evidence, not by picking a door.

- **Prospective contract clients** (the audience arriving most often right now). Operations leads, small-business owners, library and municipal staff, and technical founders. Each has a specific problem: an AI rollout with no guardrails, software they've outgrown, or a fragile WordPress install. They're deciding whether this person can solve it and is safe to bring in.
- **Hiring managers for lead, staff or principal engineering roles** (the preferred outcome). They're judging seniority, architectural judgment, and whether this person is a stable hire or someone building an exit.

Full-time is preferred. Contract work is the realistic path while the market works out what AI does to hiring. The site has to land contract work now without reading to a hiring manager as a consultancy storefront.

## Product Purpose

jasonobject.work exists to turn qualified visitors into paid work: contract engagements now, and a full-time lead role when the right one appears. Success is inquiries and interviews that lead to income, not traffic or admiration.

It does this as a working record of directing AI agents to build production software. Published specs, plans, post-mortems and notes show the method instead of claiming it. The `/work` page names the three problems Jason is brought in to solve, and `/projects` links each one to proof.

## Positioning

*Proposed during init, not yet explicitly confirmed:* a former teacher turned lead architect who treats directing AI agents as an instruction problem (decomposition, scaffolding, feedback, verification). It lets one person deliver what used to take a team. Neighbors can claim "AI-accelerated development"; they can't show the published spec → plan → review trail, the post-mortems, or the teaching background that explains why the method works.

Voice: a calm tech lead who diagnoses what breaks and how the architecture fixes it, then links to proof. It doesn't pitch the person.

## Operating Context

- Visitors usually arrive from LinkedIn, a job application, a résumé link, or a referral, and often read on a phone.
- Most read Home, then `/work` or `/projects`, then perhaps one record entry, then send an email. The `/work` inquiry helper builds a structured `mailto:` (no form backend, no spam surface).
- Jason publishes and edits through the site's own admin: record entries at `/admin/posts`, and the copy on `/work` and `/projects` at `/admin/projects` and `/admin/page-text`. Every record entry is hand-curated and redacted before it's published. Nothing is published automatically from the repo.

## Capabilities and Constraints

- **Stack is fixed:** .NET 9 Blazor Server, MudBlazor 9.6, EF Core 9, ASP.NET Identity, SQL Server, Docker Compose. Never propose leaving Blazor/MudBlazor.
- **Live and public** on the `jasonobject.work` apex, behind NPM and a Cloudflare Tunnel. WebSockets must stay enabled on the proxy host for the Blazor circuit.
- **The repository is public.** Anything committed is published.
- Routes: `/` (Home), `/notes` and `/notes/{slug}` (The Record, filterable by kind and project; `/posts` redirects), `/projects` (grouped `#ai-systems`, `#platforms`, `#claudepress`), `/about`, `/work`.
- Record entry kinds: Spec, Plan, PostMortem, Note. Each has a colour-coded rail. Entries have no featured images; images may appear inside an entry body.
- **Editable page copy:** all prose on `/work` and `/projects` lives in the database and is edited in the admin (spec `docs/superpowers/specs/2026-11-18-editable-page-copy-design.md`). The three categories (AI systems, platforms, ClaudePress) are fixed in code, and so are layout, colour, headings and button labels. A fourth category is a code change. Admin Markdown is sanitized when it renders.
- Single admin account. Password changes happen in-app at `/account/changepassword`. Never delete and reseed the account.
- Not in scope: pricing, service catalogues or engagement packages, comments, tags, view counts, a résumé download.

## Brand Commitments

- Name **JasonObject**, the `{J}` logo mark, the JSON-object bio card, and the "My first computer didn't play games" BASIC hook on Home.
- Voice follows `~/sites/VOICE.md`, including **no em-dashes** in site copy.
- Small personal touches (the JSON card's `"dogs": 2`, the Chasma ship bullets, the Rust aside) are part of the identity. They show a person, not a vendor.

## Evidence on Hand

- The Record: published entries in the site database, sourced from `docs/superpowers/specs/` and `plans/` and redacted by hand.
- Public repos that can be named and linked: ClaudePress, agentic-job-search, jason-object-dotnet, Chasma, cyoa.
- Portraits in `src/MeDotNet/wwwroot/img/` (`jason-st-john.jpg`, `jason-cabin.jpg`, `foggy-coast.jpg`, `social-card.jpg`).
- Career facts trace to `../medotnet-source/` (kept outside the repo on purpose).
- **Absent, so don't fabricate:** testimonials, named client references, case-study metrics, revenue or outcome numbers, pricing. There is also no imagery of the work itself (screenshots, process diagrams), which is the known highest-value gap.

## Product Principles

1. **Show the work, don't claim it.** Every assertion points to an artifact. A published spec or post-mortem beats an adjective.
2. **Diagnose the problem, not the person.** Lead with what breaks for the visitor and how the architecture fixes it.
3. **Sell decisions, not clients.** Describe freelance and employer work by its architecture. Never name clients, towns or live URLs. Never claim outcomes, and never invent anything (every career claim traces to source material).
4. **One site, both asks, neither subordinated.** Contract availability is visible now. Full-time readiness stays credible and never reads as an afterthought.
5. **The owner can keep it current.** Copy that changes with the market (offers, project write-ups) belongs in the admin, not in a deploy.

## Accessibility & Inclusion

No product-specific standard was set beyond the WCAG AA baseline expected of a public professional site. Respect reduced-motion and keep both light and dark modes legible.
