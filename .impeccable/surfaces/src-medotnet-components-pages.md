---
version: 1
slug: "src-medotnet-components-pages"
primary_target: "src/MeDotNet/Components/Pages"
related_targets: []
---

# Surface brief: jasonobject.work (site-wide identity pass)

Scope: every public page (Home, The Record + entries, Projects, About, Work) plus the app bar, footer and social card. Mode: Persuade (Home, Work, Projects, About); Read (Record entries). Layout, copy and structure stay; this pass replaces the visual system.

Audience and job: contract clients and hiring managers deciding, within seconds, that this is a real person they would enjoy working with who gets results (PRODUCT.md). Proof: the record, the projects, real numbers. Tone: very slightly irreverent.

Constraints (user-confirmed 2026-11-17): MudBlazor stays (theme overridden, not replaced). Dark is the default; light mode must still be good for people who prefer it. Keep the animated Chasma grid and ships, the JSON card, the BASIC hook, the {J} mark. Low density on Home and /work, with detail behind progressive disclosure; /projects may run dense. Social card gets a Chasma ship.

## Direction contract

THESIS: The site is a field manual for Jason's own work, in the language of his Chasma architectural infographic: deep navy, outlined panels colour-coded by meaning, uppercase engineering headings, monospace for data. It refuses the stock-MudBlazor portfolio (indigo bar, raised grey cards, Roboto, colours that mean nothing).

OWN-WORLD: Ground #0B1220 navy over the faint 44px arena grid with drifting ships; panels #0F1A2E with 1.5px accent outlines and 14px radius, no shadows; pill tags in mono at the panel header's right. Accents carry meaning: cyan, amber, green, violet, coral, each owning one category or record kind. Display: Archivo (expanded, uppercase, heavy); body: Archivo; data and tags: Martian Mono. Light mode = the printed manual: white panels on a faint graph-paper grid, the same accents deepened to pass AA.

STORY: The visitor meets a person (the BASIC hook, the JSON card), sees real numbers, scans colour-coded evidence, and reaches Work with me without hunting.

FIRST VIEWPORT: App bar on navy with the {J} mark visible, nav with an active marker, amber Work with me. Hero: name and role line, the h1 hook large in Archivo expanded, bio left, the JSON card as an outlined amber code panel right, a four-tile strip of real figures beneath, one availability sentence beside the CTAs.

FORM: Field Manual, pinned by the user (not on the rolled list). seed: none (pinned): "Take a look at the infographic PDF in ~/sites/Chasma/ That mixed with what I already have is more what I'm thinking." Signature interaction: the drifting Chasma ships over the arena grid (kept). Motion grammar: one entrance, ease-out, nothing else animates on scroll.

FINISH: unreviewed and undocumented is unfinished; this build ends with the finish review, the verdict, DESIGN.md, and every shipping raster carrying its provenance
