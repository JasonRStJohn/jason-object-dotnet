# Brainstorm & Design Spec: Interactive CYOA Story Engine for MeDotNet

> **Date:** 2026-08-29  
> **Status:** Blue-Sky Brainstorm & Architectural Spec  
> **Target:** Live interactive adventure player on `jasonobject.work` (`medotnet`)  
> **Source Engine Inspiration:** [JasonRStJohn/cyoa](https://github.com/JasonRStJohn/cyoa) (Rust + Axum + MongoDB / JSON branching engine)

---

## 1. Executive Summary & Vision

`medotnet` serves as the **working record of directing AI agents to build production software**. 

Adding an interactive **Choose Your Own Adventure (CYOA) engine** as a dedicated showcase page (`/adventure` or `/cyoa`) transforms the portfolio from a static reading experience into an **immersive, playable demonstration of full-stack engineering, animation craft, and systems storytelling.**

The engine leverages rich, line-by-line formatting, dynamic state machines, and switchable visual presentation themes (from retro CRT terminals to cinematic narrative storybooks).

---

## 2. Visual Front-End Concepts ("The Flashing Front End")

The CYOA schema provides granular, line-by-line formatting. The UI can bring this to life with distinct visual modes that players can toggle:

### A. The "Cyberpunk / Retro CRT Terminal" Mode
* **Aesthetic:** Green phosphor / Amber glow on deep matte black, subtle scanlines, CRT barrel curvature, flickering static on dramatic moments.
* **Typing Physics:** Typewriter text streaming with configurable pacing, audible mechanical key click SFX (toggleable), blinking block cursor.
* **Line-by-Line Formatting Tricks:**
  - `[glitch]`: Text scrambles into random Unicode/hex before resolving.
  - `[system]`: Fixed-width, subdued cyan bracketed telemetry (`[KERNEL]: Memory pressure detected`).
  - `[alert]`: Flashing red reverse-video blocks (`>>> CRITICAL FAILURE <<<`).
  - `[speaker: "Monolith"]`: Avatar icon or terminal prompt prefix (`monolith@home:~$`).

### B. The "Cinematic Narrative / Graphic Storybook" Mode
* **Aesthetic:** Clean, dark-mode luxury layout using MudBlazor cards with subtle backdrop blur, atmospheric ambient gradient shifts matching scene tension.
* **Visual Story Beats:**
  - Animated speech bubbles for dialogue vs. italicized narrative prose.
  - Ambient color grading (e.g., cold slate blue in the server room, amber warm glow in the lounge, neon red during alerts).
  - Branching decision cards with hover-lift micro-interactions, countdown timers for urgent choices, and sound effects on route selection.

### C. The "Branch Graph / Story Map" Overlay (The Metagame)
* An interactive minimap / DAG tree showing the player's journey through the story nodes.
* Unexplored branches rendered as mystery silhouettes; dead ends marked with skull icons; winning endings highlighted in gold.

---

## 3. The Line-by-Line Rich Story Schema

Expanding the base CYOA data model into a rich, expressive format that remains clean in JSON:

```json
{
  "id": "node_server_outage_01",
  "title": "2:00 AM on Monolith",
  "ambient_theme": "alert_red",
  "bg_music": "ambient_drone_low.mp3",
  "lines": [
    {
      "speaker": "System",
      "text": "ALERT: Memory ceiling exceeded on Mac Mini 2012 (15.8 / 16.0 GB).",
      "style": "telemetry_warning",
      "delay_ms": 200
    },
    {
      "speaker": "Narrator",
      "text": "The hum of the server fan kicks into high gear. You stand in the dark holding a lukewarm Manhattan.",
      "style": "narrative_prose",
      "delay_ms": 400
    },
    {
      "speaker": "Claude",
      "text": "Jason, the SQL Server container is hungry. We need to shed memory or the SignalR circuit will die.",
      "style": "dialogue_agent",
      "avatar": "claude_avatar.png",
      "glitch": false
    }
  ],
  "choices": [
    {
      "id": "choice_kill_sql",
      "text": "Force-stop the SQL Server container (Risky - lose uncommitted migrations)",
      "target_node": "node_sql_killed",
      "required_flags": [],
      "gain_flags": ["sql_down"]
    },
    {
      "id": "choice_stream_agents",
      "text": "Direct Gemini & Claude to prune unused caches",
      "target_node": "node_cache_pruned",
      "required_flags": ["has_seltzer"]
    },
    {
      "id": "choice_sip_drink",
      "text": "Take a slow sip of the Manhattan and let Docker DNAT figure it out",
      "target_node": "node_disaster_ending"
    }
  ]
}
```

---

## 4. The Killer App: Gamifying ClaudePress Site Creation ("Site-Creator RPG")

Instead of purely fictional stories, we can **gamify the actual operation of the 5-department `site-creator` pipeline**. The player takes the role of the *Director of AI Operations* on `monolith`.

```
[Incoming Client Brief]
         │
         ▼
[Department 1: Intake & Scope Strategy] ──► (Tradeoffs: Budget vs Features vs Performance)
         │
         ▼
[Department 2: Data Architect] ──────────► (PocketBase Schema & Invariant Decisions)
         │
         ▼
[Department 3: Frontend Dev] ────────────► (ClaudePress Shadowing, Nuxt Layout, Tokens)
         │
         ▼
[Department 4: Site Auditor] ────────────► (A11y, Mobile Responsiveness, SEO Pass)
         │
         ▼
[Department 5: Ops Engineer] ────────────► (Port Allocation, Compose Overrides, NPM WS)
         │
         ▼
[DUAL-OUTPUT VICTORY SCREEN]
 ├── In-Game Scorecard ("Launch Grade: A+ | RAM Used: 420MB")
 └── Real Artifact Export (Valid `site-creator` specification JSON/Markdown)
```

### Stage-by-Stage Gameplay Mechanics

1. **Intake & Scope Negotiation:**
   * *Scenario:* Client demands uncompressed 4K video backgrounds with auto-play sound on mobile.
   * *Choices:* Push back for a clean CSS hero (+Performance bonus, +Budget leftover), cave to client (-20% Performance penalty), or let an unhinged agent generate dynamic video (Hilarious OOM crash ending!).

2. **Data Architect (Schema Design):**
   * *Scenario:* Modeling complex parent-child relationships (e.g. dogs + health results, or products + options).
   * *Choices:* Clean relational collections with cascading deletes vs single giant unstructured JSON blob (accumulates "Tech Debt" flags that bite in the Auditor stage).

3. **Frontend Theming & Shadows:**
   * *Choices:* Select design tokens, dark/light contrast rules, and component overrides.

4. **Ops & Port Assignment:**
   * *Scenario:* Allocating host ports on `monolith` and setting up reverse proxy ingress.
   * *Choices:* Verify `port-registry.md`, bind `0.0.0.0` with `compose.local.yml`, toggle WebSockets in NPM.
   * *Failure Trap:* Deploying from Portainer UI without a compose file (leads to the "Ghost Container with Empty Labels" curse).

### Real-World Dual-Output
At the end of a winning playthrough, the game doesn't just show a high score:
* **The "Export Blueprint" Button:** Serializes the player's choices into a real, valid `site-creator` pipeline brief (`Context.md` + schema specs).
* **Portfolio Showcase Value:** Demonstrates to visitors, hiring managers, and prospective clients exactly how rigorous, deliberate, and fun your architectural process is.

---

## 5. Architectural Integration with MeDotNet (Blazor Server)

Because `medotnet` runs on **.NET 9 + Blazor Server (MudBlazor)**, we have native real-time bidirectional communication over SignalR out of the box:

```
[Browser Client]
       │
       ▼  (SignalR Circuit / DOM Events)
[Blazor Server Component: CyoaPlayer.razor]
       │
       ├── State Machine (CurrentNode, VisitedHistory, InventoryFlags, SoundState)
       ├── Story Parser & Line Typewriter Engine (async Task streaming)
       │
       ▼
[Story Provider Service]
       ├── Mode A: Direct JSON loader (from local embedded resources or DB)
       └── Mode B: HTTP Client pulling from Rust CYOA Backend (Axum / Mongo)
```

### Key Technical Advantages of Blazor Server for this:
1. **Zero Client-Side Logic Exposure:** Secret story branches, hidden easter eggs, and victory conditions stay on the server until unlocked.
2. **Smooth Asynchronous Streaming:** Easy `await Task.Delay()` line-by-line rendering without complex JavaScript timer management.
3. **Session Persistence:** Save games directly to user cookies or database state so users can resume an adventure later.
4. **Keyboard Accessibility:** Direct C# event handling for number keys (`1`, `2`, `3`), spacebar (skip typewriter animation), and `Tab` navigation.

---

## 6. Next Steps & Implementation Roadmap

- [ ] **Phase 1:** Build `CyoaPlayer.razor` component with core state machine & line-by-line parser in `medotnet`.
- [ ] **Phase 2:** Implement CRT / Retro Terminal CSS styling & typewriter pacing.
- [ ] **Phase 3:** Create the first playable scenario (`stories/site-creator-quest.json` + `stories/monolith-outage.json`).
- [ ] **Phase 4:** Add sound effects (subtle mechanical keystrokes, alert beeps via lightweight JS interop).
- [ ] **Phase 5:** Mount on `jasonobject.work/adventure`.
