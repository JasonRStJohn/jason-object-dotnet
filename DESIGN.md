---
name: JasonObject
description: A field manual for one engineer's work, set in the language of the Chasma architectural infographic.
colors:
  navy-ground: "#0B1220"
  navy-ground-raised: "#0D172A"
  navy-panel: "#0F1A2E"
  manual-ink: "#E6EDF7"
  manual-ink-muted: "#9AABC4"
  manual-ink-faint: "#5F708A"
  hairline: "#22304A"
  outline-strong: "#3A4A66"
  signal-cyan: "#22D3EE"
  signal-amber: "#FBBF24"
  signal-violet: "#A78BFA"
  signal-green: "#34D399"
  signal-coral: "#FB7185"
  grid-line: "rgba(96, 165, 250, 0.075)"
  paper-ground: "#F4F7FB"
  paper-ground-raised: "#EAF0F7"
  paper-panel: "#FFFFFF"
  print-ink: "#0B1220"
  print-ink-muted: "#44536B"
  print-hairline: "#CDD6E3"
  print-outline-strong: "#A9B5C7"
  print-cyan: "#0E7490"
  print-amber: "#B45309"
  print-violet: "#6D28D9"
  print-green: "#047857"
  print-coral: "#BE123C"
  graph-paper-line: "rgba(14, 116, 144, 0.07)"
typography:
  display:
    fontFamily: "Archivo, Helvetica Neue, Arial, sans-serif"
    fontSize: "clamp(2.25rem, 1.4rem + 3.4vw, 4.25rem)"
    fontWeight: 850
    lineHeight: 1.04
    letterSpacing: "-0.01em"
    fontVariation: "'wdth' 112"
  headline:
    fontFamily: "Archivo, Helvetica Neue, Arial, sans-serif"
    fontSize: "2.75rem"
    fontWeight: 850
    lineHeight: 1.04
    letterSpacing: "-0.01em"
    fontVariation: "'wdth' 112"
  title:
    fontFamily: "Archivo, Helvetica Neue, Arial, sans-serif"
    fontSize: "1.125rem"
    fontWeight: 800
    lineHeight: 1.3
    letterSpacing: "0.01em"
    fontVariation: "'wdth' 112"
  title-entry:
    fontFamily: "Archivo, Helvetica Neue, Arial, sans-serif"
    fontSize: "1.1875rem"
    fontWeight: 700
    lineHeight: 1.35
    letterSpacing: "0"
  lede:
    fontFamily: "Archivo, Helvetica Neue, Arial, sans-serif"
    fontSize: "1.25rem"
    fontWeight: 400
    lineHeight: 1.5
  body:
    fontFamily: "Archivo, Helvetica Neue, Arial, sans-serif"
    fontSize: "1.0625rem"
    fontWeight: 400
    lineHeight: 1.65
    letterSpacing: "0"
  body-sm:
    fontFamily: "Archivo, Helvetica Neue, Arial, sans-serif"
    fontSize: "0.9375rem"
    fontWeight: 400
    lineHeight: 1.6
  nav-label:
    fontFamily: "Archivo, Helvetica Neue, Arial, sans-serif"
    fontSize: "0.8125rem"
    fontWeight: 700
    lineHeight: 1.5
    letterSpacing: "0.08em"
  button:
    fontFamily: "Archivo, Helvetica Neue, Arial, sans-serif"
    fontSize: "0.875rem"
    fontWeight: 750
    lineHeight: 1.75
    letterSpacing: "0.04em"
  data-figure:
    fontFamily: "Martian Mono, ui-monospace, SFMono-Regular, Consolas, monospace"
    fontSize: "clamp(1.5rem, 1.1rem + 1.2vw, 2rem)"
    fontWeight: 700
    lineHeight: 1.1
    fontFeature: "'tnum' 1, 'zero' 1"
  label:
    fontFamily: "Martian Mono, ui-monospace, SFMono-Regular, Consolas, monospace"
    fontSize: "0.6875rem"
    fontWeight: 600
    lineHeight: 1
    letterSpacing: "0.06em"
  code:
    fontFamily: "Martian Mono, ui-monospace, SFMono-Regular, Consolas, monospace"
    fontSize: "0.82rem"
    fontWeight: 400
    lineHeight: 1.75
    fontFeature: "'tnum' 1, 'zero' 1"
rounded:
  focus: "6px"
  button: "10px"
  panel: "14px"
  pill: "999px"
spacing:
  unit: "4px"
  inline: "16px"
  panel: "24px"
  column-gap: "48px"
  section: "56px"
  grid-cell: "44px"
components:
  button-cta:
    backgroundColor: "{colors.signal-amber}"
    textColor: "{colors.navy-ground}"
    typography: "{typography.button}"
    rounded: "{rounded.button}"
    padding: "6px 16px"
  button-cta-print:
    backgroundColor: "{colors.print-amber}"
    textColor: "{colors.paper-panel}"
    typography: "{typography.button}"
    rounded: "{rounded.button}"
    padding: "6px 16px"
  button-outlined:
    backgroundColor: "transparent"
    textColor: "{colors.signal-cyan}"
    typography: "{typography.button}"
    rounded: "{rounded.button}"
    padding: "5px 15px"
  panel:
    backgroundColor: "{colors.navy-panel}"
    textColor: "{colors.manual-ink}"
    rounded: "{rounded.panel}"
    padding: "{spacing.panel}"
  panel-print:
    backgroundColor: "{colors.paper-panel}"
    textColor: "{colors.print-ink}"
    rounded: "{rounded.panel}"
    padding: "{spacing.panel}"
  tag:
    backgroundColor: "transparent"
    textColor: "{colors.signal-amber}"
    typography: "{typography.label}"
    rounded: "{rounded.pill}"
    padding: "0.42em 0.8em"
  record-entry:
    backgroundColor: "{colors.navy-panel}"
    textColor: "{colors.manual-ink}"
    rounded: "{rounded.panel}"
    padding: "1.1rem 1.35rem 1.2rem"
  figure-tile:
    backgroundColor: "{colors.navy-panel}"
    textColor: "{colors.signal-cyan}"
    typography: "{typography.data-figure}"
    rounded: "{rounded.panel}"
    padding: "1.1rem 0.75rem"
  nav-link:
    backgroundColor: "transparent"
    textColor: "{colors.manual-ink-muted}"
    typography: "{typography.nav-label}"
    padding: "8px 0"
  nav-link-active:
    backgroundColor: "transparent"
    textColor: "{colors.manual-ink}"
    typography: "{typography.nav-label}"
    padding: "8px 0"
  chip-filter:
    backgroundColor: "transparent"
    textColor: "{colors.signal-cyan}"
    typography: "{typography.label}"
    rounded: "{rounded.pill}"
    height: "36px"
  chip-filter-selected:
    backgroundColor: "{colors.signal-cyan}"
    textColor: "{colors.navy-ground}"
    typography: "{typography.label}"
    rounded: "{rounded.pill}"
    height: "36px"
---

# Design System: JasonObject

## Overview

**Creative North Star: "The Field Manual"**

The site is a field manual for one engineer's own work, drawn in the language of his Chasma architectural infographic. Deep navy ground under a faint 44px arena grid, panels drawn with thin coloured outlines instead of raised shadows, heavy expanded uppercase headings, and monospace reserved for data. Every colour is a role: a visitor who learns that violet means AI work on /work reads the same violet on /projects and on a Plan entry in the record without being told.

Dark is the default and the manual's native state. Light mode is not a different design: it is the same manual printed on graph paper, the same outlines on white panels, the same accents deepened until they pass AA on paper, and the drifting Chasma ships redrawn as pencil sketches. Both modes ship, both must stay legible, and both share one set of semantic roles.

Density is deliberately low. Home and /work show one claim per panel and put the detail behind a quiet "How I approach it" disclosure; /projects may run denser because it is the evidence index. Long reading sits at a book measure with a side column of facts beside it on desktop. The system rejects the stock MudBlazor portfolio it replaced: the indigo app bar, raised grey cards, Roboto, and colours that mean nothing.

**Key Characteristics:**
- Navy ground (dark default) or graph paper (light), with the arena grid visible in both.
- Outlined panels at 14px radius; no shadows on anything that sits on the page.
- Five semantic accents, each owning one category, never decoration.
- Archivo heavy, expanded and uppercase for display; Archivo for reading; Martian Mono only for data and labels.
- The animated Chasma ships are brand, present in both modes, absent under reduced motion.
- Built on Blazor and MudBlazor: the theme is overridden, never replaced.

## Colors

A navy-and-ink neutral system carrying five signal accents, each assigned a meaning that holds across every page and both modes.

### Primary
- **Signal Cyan** (dark, light `print-cyan`): platforms and line-of-business work, Spec entries, links, focus rings, the active-nav bar, filter chips and the outlined secondary button. It is also the colour of every figure on the Home strip, because figures are facts and use the link colour rather than a category colour.

### Secondary
- **Signal Amber** (dark, light `print-amber`): the call to action. "Work with me" is the only filled button on most screens, and it is amber. Amber also owns Note entries, the jason.json code panel, the inquiry panel and the About facts panel.

### Tertiary
- **Signal Violet** (dark, light `print-violet`): AI harness and agent-directed work, and Plan entries.
- **Signal Green** (dark, light `print-green`): web platforms, migrations and ClaudePress. Also the availability dot and JSON string values.
- **Signal Coral** (dark, light `print-coral`): post-mortems. Also JSON numerals and the sketched ship flame.

### Neutral
- **Navy Ground** / **Paper Ground**: the page. Painted inline in the document head from the theme cookie so there is no flash before the theme loads.
- **Navy Panel** / **Paper Panel**: every outlined surface (panels, cards, entry rows, figure tiles).
- **Navy Ground Raised** / **Paper Ground Raised**: code blocks and table headers inside long-form entries.
- **Manual Ink** / **Print Ink**: headings and body text.
- **Manual Ink Muted** / **Print Ink Muted**: secondary copy, nav at rest, dates, fact labels, ledes.
- **Hairline** / **Print Hairline**: default panel outlines, header rules, the footer and app-bar borders.
- **Outline Strong** / **Print Outline Strong**: neutral panels and figure tiles, inputs, scrollbar thumb.
- **Grid Line** / **Graph Paper Line**: the 44px arena grid. Blue-tinted on navy, cyan-tinted on paper.

### Named Rules
**The Colour Is A Role Rule.** Violet is AI, cyan is platforms, green is migrations and ClaudePress, amber is the call to action and notes, coral is post-mortems. A panel picks one role and everything inside it reads that role. Never use an accent because a screen needs colour.

**The Neutral Figures Rule.** Facts are not categories. Figure tiles and the record navigation panel take a neutral outline, never a category accent, and every figure numeral shares the same cyan. An entry's facts panel takes that entry's kind, because the kind is the fact it reports.

**The Same Manual, Printed Rule.** Light mode keeps every role and only swaps the value for its print twin. A new colour needs both a dark value and a print value that passes AA on its own ground before it ships.

**The Two Signals Rule.** Kind is never carried by colour alone. Record entries pair each colour with a line style (Spec solid, Plan dashed, Post-mortem double and heavier, Note dotted) so the kind survives without colour.

## Typography

**Display Font:** Archivo, variable, self-hosted (with Helvetica Neue, Arial)
**Body Font:** Archivo (same family)
**Label/Mono Font:** Martian Mono, variable, self-hosted (with ui-monospace, SFMono-Regular, Consolas)

**Character:** One grotesque doing two jobs: run wide, heavy and uppercase it is the manual's engineering heading; at normal width and weight it is a calm reading face. Martian Mono is the spec-sheet voice for numbers, code and pill labels.

### Hierarchy
- **Display** (850, fluid 2.25 to 4.25rem, uppercase, 112% width): the Home hook only, capped at 18ch.
- **Headline** (850, 2.75rem, uppercase, 112% width; 2rem on phones): the page h1 on every other page.
- **Title** (800, 1.125rem, uppercase, 112% width): section and panel titles, coloured by the panel's accent.
- **Title Entry** (700, 1.1875rem, sentence case): record entry and project titles. These are names, so they stay in sentence case.
- **Lede** (400, 1.25rem): the Home sub-hook and an entry's summary, in muted ink, capped at 60ch.
- **Body** (400, 1.0625rem, 1.65): reading copy, `text-wrap: pretty`. Long-form entries run at 1.7.
- **Body Small** (400, 0.9375rem): entry summaries, panel intros, aside copy.
- **Nav Label** (700, 0.8125rem, 0.08em, uppercase, sans): app-bar navigation and figure labels.
- **Button** (750, 0.875rem, 0.04em, uppercase).
- **Label** (Martian Mono 600, 0.6875rem, 0.06em, uppercase): pill tags, chips, disclosure toggles, fact labels.
- **Data Figure** (Martian Mono 700, fluid 1.5 to 2rem, tabular, slashed zero): the Home figure strip.

### Named Rules
**The Mono Is Data Rule.** Martian Mono appears only on data and labels: figures, dates, reading time, code, the JSON card, pill tags, chips, fact labels, stack lists. Never on a sentence, a heading or a button.

**The No Kicker Rule.** No small label sits above a heading to announce it. A heading states its own subject; a category belongs in the pill tag at the panel header's right.

**The Book Measure Rule.** Reading copy stops at 68 to 72ch: 68ch for page prose, 72ch for record entries. Prose inside panels stops at 75ch.

## Layout

A single MudBlazor Large container (max 1280px) with 16/24/40px side padding at phone, tablet and desktop, sitting over a fixed full-viewport arena grid of 44px cells. Spacing follows MudBlazor's 4px scale.

Reading columns hold their measure and **side columns fill the space beside the measure** on desktop rather than letting the container width stretch the prose. From 1024px a record entry runs a 72ch body beside a 15 to 21rem fact column; About runs a 68ch narrative beside a 17 to 22rem portrait and facts column; from 960px each /work track puts its problem at 68ch beside a 14 to 17rem action column divided by a hairline. Side columns pin with `position: sticky` only when the viewport is tall enough to hold the whole column (640px for entry facts, 900px for the About column).

Below those widths everything stacks to one column, and side content that would repeat what the phone already shows is dropped: entry type and date already sit under the title, and About ends on how to reach Jason rather than on repeated facts.

The Home hero is a two-column grid from 960px (bio left, jason.json right, 3.5rem gap), then a figure strip of two tiles on phones and four on desktop. Sections are separated by about 56px.

**The Progressive Disclosure Rule.** Lower density first: each panel states one problem, and the method sits behind a native `details` disclosure with a mono "+" toggle in the panel's accent. Detail is one click away, never on the first read.

## Elevation & Depth

Flat. Outlines carry structure; tonal layering carries depth. Panels, cards, entry rows, figure tiles and filled buttons all have `box-shadow: none` in both modes. Hover and focus answer with a stronger outline and a 5% accent wash, never a lift.

### Shadow Vocabulary
- **Floating layer** (`0 18px 40px -12px rgba(2,6,16,0.55), 0 4px 12px -4px rgba(2,6,16,0.35)`): menus, popovers, dialogs, snackbars and the mobile drawer (MudBlazor elevation 8 and above). Only things that float over the page.

### Named Rules
**The Outlines Not Elevation Rule.** Anything that sits on the page is drawn, not lifted. If a surface needs emphasis, thicken or colour its outline (1.5px default, 2px strong, 3px for post-mortems).

## Shapes

Gently rounded rectangles with drawn outlines. Panels and entry rows are 14px; buttons are 10px, squarer than MudBlazor's default to match the panels; tags and chips are full pills; focus rings and the disclosure toggle box are 6px. Outlines are 1.5px by default, 2px for strong panels and entry rows, 3px for post-mortems. Accent outlines are mixed at 60 to 70% of the accent so the line reads as ink rather than neon, and reach full strength on hover or focus. Header rules are 1px hairlines.

## Components

### Buttons
- **Shape:** gently squared (10px).
- **Primary (CTA):** filled Signal Amber with dark contrast ink in dark mode, Print Amber with white in light. Flat in every state. One per screen region: "Work with me" in the app bar and the hero, "Open in Email Client" in the inquiry panel.
- **Outlined:** 1.5px Signal Cyan outline, cyan label. "Browse projects", "Ask about this track", copy actions. The aside CTA on an entry uses an amber outline.
- **Text:** uppercase label with a trailing arrow, for "Read the whole record" and project links.
- **Focus:** 2px cyan outline offset 3px; never shown for pointer focus.

### Chips
- **Style:** full pills, Martian Mono uppercase, 1px outline in cyan (kind and category filters) or neutral (project filters).
- **State:** the selected filter is a filled pill with the palette's contrast ink. Clickable chips are at least 36px tall.

### Cards / Containers
- **Corner Style:** 14px.
- **Background:** Navy Panel or Paper Panel.
- **Shadow Strategy:** none (see Elevation & Depth).
- **Border:** 1.5px accent at 70% (`fm-panel` with an accent role), neutral Outline Strong for uncategorised panels, 2px for the inquiry panel.
- **Header:** title on the left in the panel's accent, pill tag on the right, 1px hairline beneath.
- **Internal Padding:** 16px on phones, 24px from 600px.
- **Grouped cards:** project cards inside a category take the group's accent at 38% and rise to 75% on hover.

### Inputs / Fields
- **Style:** MudBlazor outlined text fields on the panel surface, Outline Strong stroke.
- **Focus:** cyan stroke and cyan caret.

### Navigation
- **App bar:** translucent ground (92% navy or 94% white), 1px hairline beneath, flat, 68px tall. The `{J}` mark and an uppercase expanded wordmark at left; nav links, the amber CTA and the theme toggle at right.
- **Links:** Nav Label type in muted ink, brightening to full ink on hover over 160ms. The current page wears a 2px cyan bar under its label.
- **Mobile:** below 960px the links move into a temporary drawer behind a menu button; below 600px the CTA becomes a compact "Contact" and the wordmark narrows.

### Pill Tag
Mono uppercase label in the accent, a 1px outline at 65% and a 10% fill. It sits at the right of a panel or entry header and names the category, kind or engagement type. It is the only place a category is named in small type.

### Record Entry Row
The record's signature row. A 2px outline whose colour and line style both come from the entry kind, the title in sentence case with the kind tag at its right, the date in mono beneath, a muted summary at up to 75ch. The whole card is the hit area; the title link stays the accessible name. Hover and focus bring the outline to full strength, wash the surface at 5% and colour the title.

### Figure Tile
Real numbers only. Mono figure in cyan over a sans uppercase label, centred, in a neutral Outline Strong frame. Two across on phones, four on desktop.

### jason.json Code Panel
An amber panel holding the bio as a JSON object in Martian Mono: punctuation muted, keys cyan, strings green, numbers coral. Header carries the filename in mono and a "schema: human" tag.

### Chasma Arena (signature)
A fixed layer behind everything: the 44px grid in both modes, plus Chasma ships (exact vertices from the game: grey hull `#888e99`, a tail chevron in a player colour `#facc15`, `#ef4444`, `#3b82f6` or `#10b981`, and a flickering red flame) that drift across the viewport at 115 to 160px/s, the first after 2.5s and then every 14 to 28s, sometimes chased by a second. Both modes draw ships at the same size, 48px tall (one pixel per drawing unit). Dark ships sit at 32% opacity with a faint glow. Light mode draws the same ships as pencil sketches: graphite hull traced twice, an ink-coloured hatched tail using the print palette, the flame as three strokes, roughened by an SVG turbulence filter, at 72% opacity. The layer is `pointer-events: none` and `aria-hidden`, and spawns nothing under `prefers-reduced-motion`. The same ship appears as the bullet beside Jason's roles on Home and on the social card.

## Do's and Don'ts

### Do:
- **Do** keep dark as the default and ship light as the same manual printed on graph paper, with the sketched ships.
- **Do** keep the animated Chasma ships and the arena grid in both modes; they are brand.
- **Do** build on Blazor and MudBlazor, styling through the theme and the field-manual classes rather than replacing components.
- **Do** give every panel, group and entry exactly one semantic role: violet AI, cyan platforms, green migrations and ClaudePress, amber CTA and notes, coral post-mortems.
- **Do** keep figure tiles and record navigation neutral.
- **Do** hold body copy at 68 to 72ch and fill the space beside it with a side column on desktop.
- **Do** put detail behind a disclosure and keep the first read to one claim per panel.
- **Do** pair every colour-coded kind with a second signal (line style, label).
- **Do** use outlines for emphasis: 1.5px default, 2px strong, 3px post-mortem.

### Don't:
- **Don't** put a kicker or eyebrow label above a heading.
- **Don't** set sentences, headings or buttons in Martian Mono.
- **Don't** use an accent colour decoratively, or give one figure a different colour from the rest; every figure shares one treatment.
- **Don't** add shadows to anything that sits on the page; only floating layers get one.
- **Don't** use em-dashes in site copy.
- **Don't** let prose run the full container width.
- **Don't** fall back to the stock MudBlazor look: indigo app bar, raised grey cards, Roboto.
- **Don't** put anything in the design, its assets or this file that should not be public; the repository is public.
