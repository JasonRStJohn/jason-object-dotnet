# Phase 7 — UX Integration: Top Nav, Hero CTAs, Filterable Projects & Inquiry Helper

**Date:** 2026-11-14  
**Branch:** `feature/ux-integration`  
**Updates:** `MainLayout.razor`, `Home.razor`, `Projects.razor`, `Work.razor`, `About.razor`, `App.razor`, `app.css`

## Goal

Integrate the high-utility UX patterns from the HTML mockup (top-bar desktop navigation without a persistent side drawer, clear hero CTAs and role/availability context, filterable project cards with stack chips, and a spam-free client-side inquiry builder on `/work`) while preserving the site's MudBlazor visual identity, authentic voice, and strict no-em-dash rule (`VOICE.md`).

## Decisions

| Area | Decision |
|---|---|
| Navigation (`MainLayout.razor`) | Close the persistent desktop drawer (`DrawerVariant.Temporary` for mobile only). Desktop shows horizontal nav links (`Home`, `The Record`, `Projects`, `About`) and a `Work with me` button in `MudAppBar`. Account and social links move to a clean page footer (plus the mobile drawer). |
| Hero (`Home.razor`) | Keep the BASIC hook, portrait, JSON card, and bio prose. Add a subtle role/availability caption above the `h1` and two clear CTA buttons (`Work with me` filled, `Browse projects` outlined) below the bio. |
| Projects (`Projects.razor`) | Add `MudChip` category filters (`All`, `AI Harnesses`, `Platforms & .NET`, `Web & Migrations`) matching the pattern on `/notes`, plus status badges and stack tags on each card. |
| Work (`Work.razor`) | Keep the three problem-diagnosis cards; add an interactive client-side Inquiry Helper (`#inquiry`) that formats a `mailto:` link or copies the structured inquiry to the clipboard via `ISnackbar` + JS interop. |
| Punctuation (`VOICE.md`) | Remove all em-dashes (`—`) across `Home.razor`, `Work.razor`, `Projects.razor`, and `About.razor`, replacing them with periods, commas, colons, or parentheses. |
