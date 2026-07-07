# Phase 4 — Site Content & Identity Design Spec

**Date:** 2026-07-07
**Branch:** feature/site-content (off main @ 818f8b1, post-MudBlazor merge)

## Goal

Make the site say what it should: rebrand from MeDotNet to **JasonObject** (domain: jasonobject.work), and give it real content — a landing page with Jason's identity and thesis, an About page with the career narrative, and a Projects page with nine project write-ups. Portfolio-plus-blog positioning, led by the AI-assisted-engineering angle, with the site itself as the demo.

## Decisions (settled during brainstorming)

| Decision | Choice |
|---|---|
| Purpose | Portfolio + blog, with show-off/playground energy; the site itself is exhibit A |
| Identity angle | AI-assisted engineering lead: teaching background as the edge in directing AI agents; standard lead-dev credentials in support |
| Display name | Jason St. John |
| Brand | JasonObject everywhere user-visible (app bar, page titles, hero, login card). Solution/namespaces stay MeDotNet internally |
| Structure | Three pages: Home (rewritten), About (new), Projects (new); blog unchanged |
| Contact | Email (jasonrstjohn@gmail.com), GitHub (github.com/JasonRStJohn), LinkedIn (linkedin.com/in/jasonrstjohn); location shown as "Upstate NY". No phone, no street address |
| Portfolio contents | Nine projects from docs/Resume_Raw_Materials.txt; employer projects described but unlinked; public ones linked |
| Out of scope | Blog posts (authored via CMS, not code), footer component, code/namespace rename, custom theme/palette, resume download |

Source material: `docs/Jason_StJohn_Resume_AI_Angle.txt`, `docs/Resume_Raw_Materials.txt` (facts must trace to these; no invented claims).

## Structure

### Navigation (MainLayout drawer)
Home, About, Projects, Blog — then the existing `AuthorizeView` section (Manage Posts, Logout / Login), unchanged. App bar title: **JasonObject**. App bar right side gains three icon links before the dark-mode toggle: GitHub (`https://github.com/JasonRStJohn`), LinkedIn (`https://www.linkedin.com/in/jasonrstjohn`), Email (`mailto:jasonrstjohn@gmail.com`), each with an aria-label.

### Pages
- **Home** (`/`, rewrite): hero + demo callout + About/Projects buttons + existing recent-posts section (PersistentLoader wiring kept exactly as is).
- **About** (`/about`, new): four sections of narrative + contact block. Pure static component — no services, no PersistentLoader.
- **Projects** (`/projects`, new): intro line + nine `MudCard`s rendered from a local record list in the component. Pure static component.
- **Page titles:** Home `JasonObject — Jason St. John`; About `About — JasonObject`; Projects `Projects — JasonObject`; Blog `Blog — JasonObject`; post detail `@_post.Title — JasonObject` / `Not Found — JasonObject`; admin page titles stay exactly as they are; Login page `<title>` and card heading become JasonObject.

## Copy (final text — implement verbatim)

### Home

Hero (h1 + tagline + paragraph):

> # Jason St. John
> **Lead full-stack developer. I teach machines to build software.**
>
> Eight years architecting production .NET and Vue/Nuxt platforms — and a teaching background that turns out to be the sharpest tool in AI-assisted engineering. Directing coding agents is an instruction problem: decomposition, scaffolding, feedback, and knowing whether the system misunderstood or just lacked context. I've spent my whole career getting good at exactly that.

Demo callout (MudPaper, below hero):

> **This site is the demo.** A Blazor Web App — EF Core, Identity, MudBlazor, Docker — designed, specced, and built by directing AI coding agents through plan-driven development, with every task reviewed before it landed. [Read the code →](https://github.com/JasonRStJohn/jason-object-dotnet)

Buttons under callout: `About me` → /about, `See the projects` → /projects.

Then the existing **Recent posts** section, unchanged.

### About

**The short version**

> I'm Jason St. John — lead full-stack developer and software architect in Upstate New York. For the last eight-plus years I've built and led development on production .NET and Vue/Nuxt platforms: job management systems, automation engines, election ballot tracking. Before that I ran a web development business, worked IT, and taught social studies. The teaching part matters more than it looks.

**The arc**

> My first computer was an early Mac that wouldn't play any games, so I taught myself BASIC to have some fun with it. That instinct — if the thing doesn't do what you want, learn to make it — never left. College was a tour through engineering, psychology, creative writing, and music before landing in education, because the constant was that I loved learning itself. I taught social studies for a few years, then came back to tech: IT and networking at a small shop in Glens Falls, then four years building custom WordPress themes and plugins under Infinity Graphics. When a C# developer job came along, I took it — and went developer → senior → lead by shipping the systems on the [Projects](/projects) page.

**The thesis**

> Directing AI coding agents well is an instruction problem. You decompose the work, scaffold the context, give formative feedback, and diagnose whether the model misunderstood or simply didn't have what it needed. That is teaching. A graduate degree in education plus a decade of shipping production software is a strange combination that happens to be exactly right for this moment: I can architect the system *and* design the instructions that get it built. This site is the working demo — specced, planned, and implemented by directing AI agents task by task, with review gating every merge.

**Get in touch**

> The fastest way to reach me is email. I'm also on GitHub and LinkedIn.
> - jasonrstjohn@gmail.com
> - github.com/JasonRStJohn
> - linkedin.com/in/jasonrstjohn

### Projects

Intro line:

> Nine projects, newest first. The employer-built systems can't be linked, but they're the bulk of the story — ask me about any of them.

Each card: title, meta line (context · stack), 2–4 sentence body, optional link button. `Built at my current employer — not publicly available.` renders as a muted caption on employer cards.

1. **jasonobject.work — this site** · Personal, 2026 · Blazor Web App, .NET 9, EF Core, ASP.NET Identity, MudBlazor, Docker · Link: [Source on GitHub](https://github.com/JasonRStJohn/jason-object-dotnet)
   > The site you're reading, built as a working demonstration of AI-assisted engineering: every phase ran brainstorm → spec → implementation plan → agent-executed tasks, with code review gating each one. Blazor Web App with a hand-rolled CMS, EF Core, Identity auth, and MudBlazor, shipping via Docker.

2. **Job Management Platform 2.0** · Employer · .NET Core, Vue, Nuxt
   > Lead developer and architect of the company's flagship rebuild — job creation, dynamic workflow management, and progress tracking on a modern stack, with inventory and production reporting on the roadmap.

3. **Automation scripting engine** · Employer · C#, JSON instruction sets
   > A modular automation language that consumes JSON instruction sets to process mailing lists and artwork into personalized, USPS-sorted mail files for the production floor. My favorite part: a workflow that generates its own instructions from defined criteria.

4. **Ballot production & tracking** · Employer · Vue, .NET Framework
   > Primary developer on a high-velocity system carrying election ballot orders from intake through production, then tracking mail pieces to voters and back to boards of elections. It leaned hard on the automation engine above — and the pace taught me a lot about technical debt, by way of the flexibility it cost that engine.

5. **Client Portal 2.0** · Employer · Vue
   > One of three developers who salvaged an outsourced client portal — inherited stack choices and all — reorganizing the codebase into something maintainable enough to hand off for continued development.

6. **Multi-workflow job system** · Employer · C#
   > My first major C# project: re-architected a single-workflow job system so one job could run multiple template-generated workflows, with steps and ordering editable by management on the fly.

7. **Reactive invoicing system** · Employer · Vue, .NET Framework
   > Asked to make invoicing "look like PayPal" in a jQuery shop, I introduced Vue instead — CDN-loaded, no build step, but reactive. It seeded the modern front-end stack the company now uses everywhere.

8. **CateredTo** · Infinity Graphics · WordPress, PHP · Link: [Source on GitHub](https://github.com/JasonRStJohn/catered-to)
   > A menu-editing plugin for a catering business that ballooned into a full quoting system with invoice generation. In the long run it shouldn't have been a WordPress plugin — and it should have taught me more about scope creep than it did.

9. **AGFTC website** · Infinity Graphics · WordPress (underscores base) · Link: [agftc.org](https://agftc.org)
   > My first theme written mostly from scratch, with hand-built support for multiple post types, for the Adirondack/Glens Falls Transportation Council. The organization has since been reorganized, so the live site's days may be numbered.

## Implementation shape

- **Files:** rewrite `Components/Pages/Home.razor` (markup above the recent-posts section; keep the `@code` block and recent-posts markup); create `Components/Pages/About.razor`; create `Components/Pages/Projects.razor` (local `record ProjectEntry(string Title, string Meta, string Body, string? LinkText, string? LinkHref, bool Employer)` list + foreach of MudCards); modify `Components/Layout/MainLayout.razor` (title, nav links, app-bar icon links); PageTitle edits in `Posts.razor`, `PostDetail.razor`; `Pages/Account/Login.cshtml` title + heading.
- Each page keeps the semantic `<h1 class="mud-typography mud-typography-h3 ...">` pattern established in Phase 3 (FocusOnNavigate targets h1).
- About and Projects are static: no `@inject`, no loader, prerender-friendly by construction.
- Route name collision check: `/projects` and `/about` are new routes; no conflicts exist.

## Verification

- Build 0 warnings / 0 errors; tests 14/14 (nothing server-side changes).
- Smoke: all four nav destinations render with correct titles and exactly one h1 each; app-bar icon links resolve (GitHub/LinkedIn/mailto); no "MeDotNet" string remains user-visible (grep the rendered HTML of /, /about, /projects, /posts, /account/login); employer cards show no external links; public cards' links work.
