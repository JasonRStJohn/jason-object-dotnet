# Phase 7 — UX Integration Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Combine the navigation, CTA, filtering, and inquiry-builder UX mechanics from the HTML mockup with `jasonobject.work`'s existing MudBlazor components and authentic voice.

**Architecture:** Update `MainLayout.razor` and `app.css` for responsive top-bar navigation and footer; add role/availability caption and CTA buttons to `Home.razor`; add category filter chips and stack badges to `Projects.razor`; add a client-side Inquiry Helper with clipboard support to `Work.razor` and `App.razor`; strip em-dashes across all pages per `VOICE.md`.

**Tech Stack:** .NET 9, Blazor Server, MudBlazor 9.6, xUnit.

**Spec:** `docs/superpowers/specs/2026-11-14-ux-integration-design.md`

## Global Constraints

- Follow `VOICE.md`: zero em-dashes (`—`) in page prose or titles.
- Never name freelance or contract clients.
- Use pure CSS media queries (`.nav-desktop-only`, `.nav-mobile-only`) for app-bar responsiveness so SSR first paint has no layout shift.

---

### Task 1: Top-Bar Desktop Navigation, Footer & Clipboard Helper (`MainLayout.razor`, `App.razor`, `app.css`, `.gitignore`)

- [ ] Update `.gitignore` to ignore `jasonrstjohn-gmail.com-*/`
- [ ] Add `window.jasonObjectClipboard` helper to `src/MeDotNet/Components/App.razor`
- [ ] Add `.nav-desktop-only` and `.nav-mobile-only` media queries to `src/MeDotNet/wwwroot/app.css`
- [ ] Update `src/MeDotNet/Components/Layout/MainLayout.razor` to use horizontal desktop navigation, mobile temporary drawer, and a page footer

### Task 2: Hero Availability Context & Action Buttons (`Home.razor`, `About.razor`)

- [ ] Add availability/role caption and primary/secondary CTA buttons to `src/MeDotNet/Components/Pages/Home.razor`
- [ ] Remove em-dashes from `Home.razor` and `About.razor`

### Task 3: Filterable Projects with Status & Stack Chips (`Projects.razor`)

- [ ] Add category filter chips (`All`, `AI Harnesses`, `Platforms & .NET`, `Web & Migrations`), status badges, and stack tags to `src/MeDotNet/Components/Pages/Projects.razor`
- [ ] Remove em-dashes from all project titles and descriptions

### Task 4: Interactive Inquiry Helper on `/work` (`Work.razor`)

- [ ] Add track selection buttons on each problem card and the `#inquiry` helper card (`mailto:` + copy-to-clipboard with `ISnackbar`) to `src/MeDotNet/Components/Pages/Work.razor`
- [ ] Remove em-dashes from `Work.razor`
- [ ] Run `dotnet test MeDotNet.sln` and rebuild live container (`docker compose up -d --build`)
