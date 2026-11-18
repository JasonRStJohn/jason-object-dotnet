# Editable Page Copy Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Move every piece of prose on `/work` and `/projects` into the database and make it editable in the admin, with both pages looking identical on day one.

**Architecture:** Three new EF entities (`ProjectCategory`, three fixed rows; `Project`; `PageText`, named blocks) are seeded by one migration from a static `PageCopySeed` holding today's copy. A `PageCopyService` reads them for the public pages (through `PersistentLoader`) and writes them from four new `[Authorize]` admin pages. All Markdown goes through `MarkdownRenderer` (sanitized), with a new inline mode for text that continues a bold label.

**Tech Stack:** .NET 9 Blazor Server, MudBlazor 9.6, EF Core 9 (SQL Server; in-memory provider in tests), Markdig + HtmlSanitizer, xUnit + FluentAssertions, Playwright in Docker for screenshots.

**Spec:** `docs/superpowers/specs/2026-11-18-editable-page-copy-design.md`

## Global Constraints

- Branch `feature/editable-copy`, created from `feature/design-identity` after merging `main` into it (picks up the sanitize hotfix, PR #4).
- Never rebuild or restart `medotnet-app-1` (`:5000`, live). Never run this branch against database `MeDotNet`. The preview uses `MeDotNet_Preview` only.
- Seed copy is the copy at the head of `feature/design-identity`, verbatim. No em-dash (U+2014) anywhere in seeded text or new UI copy.
- Plain-text fields render Razor-encoded. Markdown renders only via `MarkdownRenderer.ToHtml` / `ToInlineHtml`. `MarkupStringGuardTests` must keep passing: the only `MarkupString` casts are of `MarkdownRenderer.` output.
- Categories are exactly `ai-systems`, `platforms`, `claudepress` (sort 1, 2, 3); accent colour stays in code (`GroupAccent()`).
- Follow existing patterns: `PostService` (`IDbContextFactory<AppDbContext>`), `PostServiceTests` (EF in-memory), `PersistentLoader`, `Admin/Posts.razor` and `Admin/PostEdit.razor`.
- Commit after every task, staging only that task's files. Never commit `.env`, keys, backups or the untracked `jasonrstjohn-gmail.com-uid-*` folder.

## Review Focus

1. **An admin enters an unsafe link or attribute in any Markdown field** → it renders without script. Task 3 adds inline-mode injection tests; block mode is covered by the hotfix tests.
2. **The project being edited was deleted in another tab** → saving shows "This project no longer exists." instead of throwing. Task 5: `SaveProjectAsync_MissingId_ReturnsError`.
3. **Moving the first project up or the last project down** → nothing changes and the order stays contiguous. Task 5: `MoveProjectAsync_AtEdges_IsNoOp`.
4. **`/projects?category=claudepress` when every claudepress project is hidden** → the page shows all groups, as an unknown category does today, and `/work` hides that panel's "See the..." button. Task 4: `GetAsync_CategoryWithAllProjectsHidden_HasVisibleProjectsFalse`; Task 6/7 page logic.
5. **A whitespace-only required field, or tags with blank lines and stray spaces** → required fields are rejected, and tags are trimmed with empty ones dropped. Task 5: `Validate_WhitespaceOnlyRequired_Fails`, `TagList_TrimsAndDropsEmpty`.

---

### Task 1: Branch, preview database and screenshot tooling

**Files:**
- Create: `scripts/preview.sh`, `scripts/visual/snap.mjs`, `scripts/visual/snap.sh`
- Modify: `.gitignore` (add `dataprotection-keys-preview/`)

**Interfaces:**
- Produces: `scripts/preview.sh [--refresh-db]`, which builds the working tree and runs container `medotnet-preview` on `:5055` against `MeDotNet_Preview`. `scripts/visual/snap.sh <outdir> <paths,> <dark,light> <d,m> [full]`, which writes `<outdir>/<path>-<mode>-<vp>.png`, with `/` named `home` and `/` in paths as `_`.

- [ ] **Step 1:** `git switch feature/design-identity && git merge main` (no conflicts are expected; the hotfix touches only the renderer, csproj and tests). Then `git switch -c feature/editable-copy`. Run `dotnet test`: all pass.
- [ ] **Step 2: Write `scripts/preview.sh`.**
  - Read `SA_PASSWORD` from `.env` without echoing it. Use `set -a; . ./.env; set +a` and pass it as `SQLCMDPASSWORD` to `docker exec`.
  - On `--refresh-db`, or when `MeDotNet_Preview` is missing:
    - `BACKUP DATABASE [MeDotNet] ... WITH COPY_ONLY, INIT` to `/var/opt/mssql/data/preview-src.bak`.
    - `RESTORE DATABASE [MeDotNet_Preview] ... WITH REPLACE`, with `MOVE` of the logical files to `MeDotNet_Preview.mdf` / `MeDotNet_Preview_log.ldf`. Get the logical names from `RESTORE FILELISTONLY`.
  - Then build image `medotnet-preview:dev` from the repo.
  - Replace the container: `docker rm -f medotnet-preview`, then `docker run -d --name medotnet-preview --network medotnet_internal -p 5055:8080`.
    - Set the same env as the compose `app` service, but with `Database=MeDotNet_Preview`. Leave `ADMIN_*` unset.
    - Mount `./dataprotection-keys-preview:/keys`.
- [ ] **Step 3: Write the snapshot tooling.** `snap.sh` runs `mcr.microsoft.com/playwright:v1.65.1-noble` with `--network host`, using `playwright-core@1.65.1` (install it into `scripts/visual/node_modules`, which is gitignored).
  - `snap.mjs`:
    - Sets cookie `theme=<mode>` on `http://127.0.0.1:5055` and uses `reducedMotion: 'reduce'`.
    - Desktop viewport is `1440x900`. Phone is `390x844` with `isMobile`, `deviceScaleFactor: 2` and `hasTouch`.
    - Waits for `networkidle`.
- [ ] **Step 4: Verify isolation.**
  - Run `scripts/preview.sh --refresh-db`.
  - `docker inspect medotnet-preview` shows `Database=MeDotNet_Preview`.
  - `curl -s -o /dev/null -w '%{http_code}' http://127.0.0.1:5055/work` returns `200`.
  - `curl` on `:5000` also returns `200`.
- [ ] **Step 5: Capture the baseline.** The preview is still running design-identity code at this point. Run `scripts/visual/snap.sh ~/.cache/medotnet-visual/before "/work,/projects" dark,light d,m full`.
- [ ] **Step 6: Commit** the scripts and `.gitignore`: `chore(dev): preview on its own database; screenshot tooling`.

### Task 2: Entities, seed data and migration

**Files:**
- Create: `src/MeDotNet/Models/ProjectCategory.cs`, `Models/Project.cs`, `Models/PageText.cs`, `Models/PageTextKeys.cs`, `src/MeDotNet/Data/Seed/PageCopySeed.cs`, `src/MeDotNet/Data/Migrations/<ts>_EditablePageCopy.cs` (+ Designer, snapshot)
- Modify: `src/MeDotNet/Data/AppDbContext.cs`
- Test: `tests/MeDotNet.Tests/Data/PageCopySeedTests.cs`

**Interfaces:**
- Produces:
  - `ProjectCategory { string Key (PK); int SortOrder; string WorkTitle, WorkTag, WorkProblem, WorkApproach, WorkLinkText, InquiryLabel, ProjectsShortLabel, ProjectsTitle, ProjectsSubtitle }`
  - `Project { int Id; string CategoryKey (FK → ProjectCategory.Key, Restrict); int SortOrder; string Title, Badge, Meta, Body, Tags; string? LinkText, LinkHref, RecordProject; bool IsEmployer; bool IsVisible = true; [NotMapped] IReadOnlyList<string> TagList }`
    - `Tags` is newline-separated. `TagList` splits on newlines, trims each tag and drops empty ones.
  - `PageText { string Key (PK); string Value }`
  - `PageTextKeys` constants:
    - `WorkIntro = "work.intro"`
    - `WorkTogetherContract = "work.together.contract"`
    - `WorkTogetherFullTime = "work.together.fulltime"`
    - `WorkInquiryIntro = "work.inquiry.intro"`
    - `ProjectsIntro = "projects.intro"`
    - `ProjectsEmployerNote = "projects.employerNote"`
  - `PageTextKeys.All`: an `IReadOnlyList<PageTextDefinition>`, where `record PageTextDefinition(string Key, string Label, string WhereShown, PageTextFormat Format)` and `enum PageTextFormat { Plain, Inline, Block }`.
    - Inline: the two `work.together.*` keys.
    - Plain: `ProjectsEmployerNote`.
    - Block: the rest.
  - `AppDbContext.ProjectCategories`, `.Projects`, `.PageTexts`.
  - `PageCopySeed.Categories` (`IReadOnlyList<ProjectCategory>`), `PageCopySeed.Projects` (`IReadOnlyList<Project>`, with Ids 1..16 set explicitly), `PageCopySeed.Texts` (`IReadOnlyList<PageText>`), and `PageCopySeed.Apply(MigrationBuilder)`.

- [ ] **Step 1: Write the failing tests** in `PageCopySeedTests`:
  - `Seed_HasThreeCategoriesInOrder`: keys are `["ai-systems","platforms","claudepress"]`, and SortOrder is 1, 2, 3.
  - `Seed_ProjectCountsPerCategory`: ai-systems 4, platforms 8, claudepress 4 (16 total). Ids are unique, and SortOrder within each category is 1..n in today's order.
  - `Seed_HasEveryPageTextKey`: the set of `Texts` keys equals the set of `PageTextKeys.All` keys.
  - `Seed_ContainsNoEmDash`: no string property of any seeded row contains `'—'`.
  - `Seed_SpotChecks`:
    - The ai-systems `WorkTag` is `"Architecture / Sprint"`.
    - The platforms `InquiryLabel` is `"Custom Platform / .NET / Vue Build"`.
    - The project titled `"Agentic Job Search"` has `LinkHref` `"https://github.com/JasonRStJohn/agentic-job-search"` and `IsEmployer` false.
    - `"jasonobject.work (this site)"` has `RecordProject` `"JasonObject"`.
    - `"Job Management Platform 2.0"` has `IsEmployer` true.
  - `Seed_ProblemFieldsHaveNoLabelAndNoBlankLines`: no `WorkProblem` starts with `"The problem"`, and none contains `"\n\n"`.
  - `Seed_LinkPairsAreBothOrNeither`.
  - `TagList_TrimsAndDropsEmpty`: `Tags = " a \n\n b\n"` gives `TagList` `["a","b"]`.
- [ ] **Step 2:** Run `dotnet test --filter PageCopySeedTests`. Expected: build FAIL (the types don't exist yet).
- [ ] **Step 3: Implement the entities, `PageTextKeys` and `PageCopySeed`.**
  - Copy every string verbatim from `Work.razor` and `Projects.razor` at the branch head.
  - Convert inline HTML to Markdown (`<strong>X</strong>` becomes `**X**`). Store the inquiry intro's email as `[jasonrstjohn@gmail.com](mailto:jasonrstjohn@gmail.com)`.
  - Drop the `"The problem:"`, `"Contract & Architecture:"` and `"Full-Time Leadership:"` labels; they stay in code.
  - `PageCopySeed.Apply` calls `migrationBuilder.InsertData` for each row. Add a header comment: "Frozen once merged: later copy changes happen in the admin, never here."
- [ ] **Step 4: Configure `AppDbContext`.**
  - `Project → ProjectCategory`: FK `CategoryKey`, `OnDelete(Restrict)`.
  - Index on `(CategoryKey, SortOrder)`.
- [ ] **Step 5: Generate the migration.**
  - With the preview SQL reachable (the dev override exposes `1433`), set `ConnectionStrings__DefaultConnection` to the **`MeDotNet_Preview`** database and run `dotnet ef migrations add EditablePageCopy -p src/MeDotNet`.
  - Append `PageCopySeed.Apply(migrationBuilder);` to the end of `Up`.
- [ ] **Step 6:** Run `dotnet test`. Expected: PASS.
- [ ] **Step 7: Commit:** `feat(copy): page copy entities, seed and migration`.

### Task 3: Inline Markdown rendering

**Files:**
- Modify: `src/MeDotNet/Services/Content/MarkdownRenderer.cs`
- Test: `tests/MeDotNet.Tests/Services/Content/MarkdownInlineTests.cs`

**Interfaces:**
- Produces: `public static string MarkdownRenderer.ToInlineHtml(string? markdown)`. It returns the inline content of the paragraphs with no `<p>` wrapper. Multiple paragraphs are joined with a single space. The output is sanitized through the same `Sanitizer` and the same link-scheme filter as `ToHtml`.

- [ ] **Step 1: Write the failing tests:**
  - `ToInlineHtml_Bold_NoParagraphWrapper`: `"a **b**"` gives exactly `"a <strong>b</strong>"`.
  - `ToInlineHtml_Link_Kept`: `"[x](https://example.com)"` contains `href="https://example.com"`.
  - `ToInlineHtml_MultipleParagraphs_JoinedWithSpace`: `"a\n\nb"` gives `"a b"`.
  - `ToInlineHtml_Empty_ReturnsEmpty`: both `null` and `"  "` give `""`.
  - `ToInlineHtml_UnsafeInput_Neutralised`, a theory: `"[x](javascript:alert(1))"`, `"x {onclick=alert(1)}"` and `"<img src=x onerror=alert(1)>"`. The output contains neither `javascript:` nor `onerror=` / `onclick=` inside a tag, and no `<img`.
- [ ] **Step 2:** Run them. Expected: FAIL (the method doesn't exist).
- [ ] **Step 3: Implement.**
  - Parse with the shared pipeline and apply the same link loop.
  - Render each top-level `ParagraphBlock`'s inlines with an `HtmlRenderer` over a `StringWriter`, joined with `" "`.
  - Sanitize the result.
  - Factor the link loop into a private `Neutralise(MarkdownDocument)` so both methods share it.
- [ ] **Step 4:** Run `dotnet test`. Expected: PASS.
- [ ] **Step 5: Commit:** `feat(content): inline Markdown rendering for labelled copy`.

### Task 4: PageCopyService (read side)

**Files:**
- Create: `src/MeDotNet/Services/PageCopy/PageCopyService.cs`, `Services/PageCopy/PageCopy.cs`
- Modify: `src/MeDotNet/Program.cs` (`AddScoped<PageCopyService>()`), `Components/_Imports.razor` (`@using MeDotNet.Services.PageCopy`)
- Test: `tests/MeDotNet.Tests/Services/PageCopy/PageCopyServiceReadTests.cs`

**Interfaces:**
- Consumes: Task 2 entities.
- Produces:
  - `record CategoryCopy(ProjectCategory Category, List<Project> Projects)` with `bool HasVisibleProjects => Projects.Count > 0`.
  - `record PageCopy(List<CategoryCopy> Categories, Dictionary<string,string> Texts)` with `string Text(string key)` (returns `""` when the key is missing).
  - `PageCopyService(IDbContextFactory<AppDbContext>)` with `Task<PageCopy> GetAsync()`.
    - Categories are ordered by SortOrder.
    - `Projects` holds visible projects only, ordered by SortOrder.
    - All three categories are always present.

- [ ] **Step 1: Write the failing tests.** Use the `PostServiceTests` in-memory setup and seed rows with `PageCopySeed` collections.
  - `GetAsync_ReturnsCategoriesInSortOrder`.
  - `GetAsync_ExcludesHiddenProjects_AndOrdersBySortOrder`.
  - `GetAsync_CategoryWithAllProjectsHidden_HasVisibleProjectsFalse`: the category is still in the list.
  - `Text_MissingKey_ReturnsEmpty`.
  - `GetAsync_RoundTripsThroughJson`: `JsonSerializer` serialize then deserialize equals the original counts and texts (this is what `PersistentLoader` relies on).
- [ ] **Step 2:** Run them. Expected: FAIL.
- [ ] **Step 3: Implement** with `AsNoTracking`.
- [ ] **Step 4:** Run them. Expected: PASS.
- [ ] **Step 5: Commit:** `feat(copy): page copy read service`.

### Task 5: PageCopyService (admin writes and validation)

**Files:**
- Create: `src/MeDotNet/Services/PageCopy/PageCopyValidator.cs`
- Modify: `Services/PageCopy/PageCopyService.cs`
- Test: `tests/MeDotNet.Tests/Services/PageCopy/PageCopyServiceWriteTests.cs`, `PageCopyValidatorTests.cs`

**Interfaces:**
- Produces, on `PageCopyService`:
  - `Task<List<Project>> GetAllProjectsAsync()`: ordered by category SortOrder, then project SortOrder, including hidden projects.
  - `Task<Project?> GetProjectAsync(int id)`.
  - `Task<IReadOnlyList<string>> SaveProjectAsync(Project p)`: returns errors, empty on success. `Id == 0` creates the project at the end of its category. A missing Id returns `["This project no longer exists."]`.
  - `Task DeleteProjectAsync(int id)`.
  - `Task MoveProjectAsync(int id, int direction)`: `-1` up, `+1` down. It swaps SortOrder with the neighbour in the same category and is a no-op at the edges.
  - `Task SetProjectVisibleAsync(int id, bool visible)`.
  - `Task<ProjectCategory?> GetCategoryAsync(string key)`.
  - `Task<IReadOnlyList<string>> SaveCategoryAsync(ProjectCategory c)`: updates only, never inserts.
  - `Task<IReadOnlyList<string>> SaveTextsAsync(IReadOnlyDictionary<string,string> values)`: upserts known keys only and ignores unknown ones.
- Also produces `static class PageCopyValidator` with:
  - `IReadOnlyList<string> Validate(Project)`: Title and Body are required (not whitespace); the link pair is both-or-neither; `LinkHref` must start with `http://` or `https://`; `CategoryKey` must be one of the three.
  - `Validate(ProjectCategory)`: every field is required; `WorkProblem` must not contain a blank line ("The problem must be one paragraph.").
  - `ValidateTexts(dict)`: Inline-format keys must not contain a blank line.

- [ ] **Step 1: Write the failing tests:**
  - `Validate_WhitespaceOnlyRequired_Fails`.
  - `Validate_LinkTextWithoutHref_Fails`.
  - `Validate_JavascriptHref_Fails`.
  - `Validate_UnknownCategory_Fails`.
  - `ValidateCategory_BlankLineInProblem_Fails`.
  - `ValidateTexts_BlankLineInInlineKey_Fails`.
  - `SaveProjectAsync_New_AppendsToEndOfCategory`.
  - `SaveProjectAsync_MissingId_ReturnsError`.
  - `SaveProjectAsync_Invalid_DoesNotPersist`.
  - `MoveProjectAsync_SwapsWithNeighbour`.
  - `MoveProjectAsync_AtEdges_IsNoOp`.
  - `SetProjectVisibleAsync_HidesFromGetAsync`.
  - `SaveCategoryAsync_UnknownKey_ReturnsError`.
  - `SaveTextsAsync_IgnoresUnknownKeys`.
- [ ] **Step 2:** Run them. Expected: FAIL.
- [ ] **Step 3: Implement.**
- [ ] **Step 4:** Run `dotnet test`. Expected: PASS.
- [ ] **Step 5: Commit:** `feat(copy): admin write operations and validation`.

### Task 6: /projects reads from the database

**Files:**
- Modify: `src/MeDotNet/Components/Pages/Projects.razor`, `src/MeDotNet/wwwroot/app.css` (add `.fm-copy`)

**Interfaces:**
- Consumes: `PageCopyService.GetAsync()`, `PageCopy.Text`, `CategoryCopy`, `Project.TagList`, `MarkdownRenderer.ToHtml`.

- [ ] **Step 1: Load the copy.** Use `PersistentLoader` (key `"page-copy"`) in `OnInitializedAsync`; implement `IDisposable` like `RecordEntry.razor`. Remove the `ProjectEntry` and `ProjectGroup` records and the `_groups` data.
- [ ] **Step 2: Render from it.**
  - Groups are the categories where `HasVisibleProjects`.
  - Chips use `ProjectsShortLabel`. The group head uses `ProjectsTitle`, and the count tag is `Projects.Count`.
  - The subtitle is plain text.
  - The project body is `<div class="mud-typography mud-typography-body1 fm-measure fm-copy">@((MarkupString)MarkdownRenderer.ToHtml(p.Body))</div>`.
  - The intro is block Markdown in the same wrapper with the `mb-4` class.
  - The employer caption is `copy.Text(PageTextKeys.ProjectsEmployerNote)`.
  - `?category=` falls back to all groups when the key is unknown or the category has no visible projects.
  - Keep the `GroupAccent()` mapping.
- [ ] **Step 3: Add `.fm-copy` CSS.**
  - `p { margin: 0 0 0.75em }`, and `:last-child` has margin 0.
  - Links: `color: var(--mud-palette-primary); text-underline-offset: 0.2em`, matching `MudLink`.
- [ ] **Step 4: Run `dotnet test`** (includes the MarkupString guard): PASS.
- [ ] **Step 5: Compare against the baseline.**
  - Run `scripts/preview.sh --refresh-db`. The migration seeds `MeDotNet_Preview` only.
  - Run `snap.sh ~/.cache/medotnet-visual/after "/projects" dark,light d,m full`.
  - Compare against `before`: there should be no visible difference. A page-height difference of up to 2px is acceptable.
- [ ] **Step 6: Commit:** `feat(copy): /projects renders from the database`.

### Task 7: /work reads from the database

**Files:**
- Modify: `src/MeDotNet/Components/Pages/Work.razor`

**Interfaces:**
- Consumes: as in Task 6, plus `MarkdownRenderer.ToInlineHtml`.

- [ ] **Step 1: Load the copy** as in Task 6 (same persistence key).
- [ ] **Step 2: Render the three track panels in one `@foreach` over `copy.Categories`.**
  - The heading is `WorkTitle`, the tag is `WorkTag`, and the accent comes from the key (the same switch as Projects; move it into a shared `static class CategoryAccent { string For(string key) }` in `Components/` and use it from both pages).
  - The problem line is `<strong>The problem:</strong> @((MarkupString)MarkdownRenderer.ToInlineHtml(c.WorkProblem))`.
  - The approach is block Markdown inside the existing `details`.
  - The "See the..." button (text `WorkLinkText`, href `/projects?category={Key}`) renders only when `HasVisibleProjects`.
  - The last panel uses `mb-6` instead of `mb-4`, as today.
- [ ] **Step 3: Render the rest from page text.**
  - The intro: block, `fm-measure`, `mb-6`.
  - The together panel: inline after the existing bold labels.
  - The inquiry intro: block, with the `mud-typography-body2 mud-text-secondary mb-4 fm-copy` classes.
- [ ] **Step 4: Build the inquiry chips.**
  - The chips are the categories' `InquiryLabel`s plus the constant `LeadershipTrack = "Full-Time / Fractional Leadership"`.
  - "Ask about this track" selects `c.Category.InquiryLabel`. The leadership button selects `LeadershipTrack`.
  - The default selection stays the first label (the planned clean-up revisits this).
- [ ] **Step 5:** Run `dotnet test`: PASS. Then refresh the preview and snap `/work` into `after`.
  - Compare with `before`: no visible difference.
  - Then click a "How I approach it" disclosure and confirm the approach text reads as before.
- [ ] **Step 6: Commit:** `feat(copy): /work renders from the database`.

### Task 8: Admin screens

**Files:**
- Create: `Components/Pages/Admin/Projects.razor` (`/admin/projects`), `Admin/ProjectEdit.razor` (`/admin/projects/new`, `/admin/projects/{Id:int}`), `Admin/PageText.razor` (`/admin/page-text`), `Admin/CategoryEdit.razor` (`/admin/categories/{Key}`)
- Modify: `Components/Layout/MainLayout.razor` (nav)

**Interfaces:**
- Consumes: the Task 5 methods, the Task 2 `PageTextKeys.All` metadata, and `PageCopyValidator` messages (shown in a `MudAlert`, as in PostEdit).

- [ ] **Step 1: Build `/admin/projects`.**
  - Use `[Authorize]`, following the `Admin/Posts.razor` layout.
  - Show one `MudTable` section per category, titled with `ProjectsTitle`.
  - Columns: Title, Badge, a Visible switch (calls `SetProjectVisibleAsync`), up/down icon buttons (`MoveProjectAsync`; disable up on the first row and down on the last), Edit, and Delete (the same confirm dialog as posts).
  - The "New project" button goes to `/admin/projects/new?category=<key>`.
- [ ] **Step 2: Build `ProjectEdit`.**
  - Fields:
    - Category select (the three).
    - Title, Badge and Meta.
    - Body, multi-line with 10 lines, helper text "Markdown".
    - Tags, multi-line with helper text "One per line".
    - Link text and Link URL.
    - "Previous employer project" switch, Record project, and Visible switch.
  - Save calls `SaveProjectAsync`; show errors, or snackbar "Saved." and navigate to `/admin/projects`.
  - Include a "View page" link to `/projects#<key>`.
- [ ] **Step 3: Build `/admin/page-text`.**
  - A list of the three categories (`WorkTitle`) linking to `/admin/categories/{Key}`.
  - Then one labelled multi-line field per `PageTextKeys.All` entry, showing `Label` with `WhereShown` as helper text and noting "one paragraph" for Inline.
  - One Save button calls `SaveTextsAsync`, and errors show in the alert.
- [ ] **Step 4: Build `CategoryEdit`.**
  - The fields are grouped under two subheadings, "On /work" and "On /projects".
  - Problem and approach are multi-line, and Problem's helper text says "One paragraph. Shown after **The problem:**".
  - Save calls `SaveCategoryAsync`, with a "View page" link.
- [ ] **Step 5: Update the nav.** In MainLayout's `Authorized` block, after "Manage Posts", add `MudNavLink`s "Manage Projects" (`admin/projects`, `Icons.Material.Filled.Work`) and "Page Text" (`admin/page-text`, `Icons.Material.Filled.TextSnippet`).
- [ ] **Step 6: Run an end-to-end smoke test on the preview.** Write `scripts/visual/admin-smoke.mjs`, reading `ADMIN_EMAIL` and `ADMIN_PASSWORD` from the environment and never printing them. Its steps:
  - Log in at `:5055/account/login`.
  - Change the "Chasma & CYOA" body, replacing `"in dark mode"` with `"in both themes"`. Save, and assert `/projects` shows the new text.
  - Hide that project, assert it's gone from `/projects`, then show it again.
  - Put a blank line in a category's problem and assert the error appears.
  - Restore the original body.

  Run it with `.env` sourced. Expected: all assertions pass.
- [ ] **Step 7:** Run `dotnet test`: PASS.
- [ ] **Step 8: Commit:** `feat(admin): edit projects, categories and page text`.

### Task 9: Record it

**Files:**
- Modify: `PRODUCT.md` (the "Wanted, not yet built" bullet becomes built: copy on `/work` and `/projects` is edited at `/admin/projects` and `/admin/page-text`; categories are fixed in code), and `README.md` if it lists admin routes.

- [ ] **Step 1:** Make both edits.
- [ ] **Step 2:** Run the full `dotnet test` and the whole before/after visual set for `/work` and `/projects`. Expected: PASS, with no visible differences.
- [ ] **Step 3: Commit:** `docs: page copy is editable in the admin`.
- [ ] **Step 4:** Report to the owner. Do not merge or deploy; deploying is the owner's call. A deploy runs the migration on the live database, so take a SQL Server backup first.
