# Build log — Vigil

Append-only. Single-session build: the session is the coordinator. Feeds the "how we built it" article.

## Running stats

| Stat | Value |
|---|---|
| Threads spawned (explorer / builder / breaker / climber / option) | 0 / 0 / 0 / 0 / 0 |
| Brief → first recognizable draft | — |
| Iterations to ship | — |
| Findings found / fixed / verified | 0 / 0 / 0 |
| Perf before → after (headline metric) | — |
| Options built / shipped | 0 / 0 |
| Modules / total LOC | 0 / 0 |

## Entries

## 2026-10-06 ~15:30 — Kickoff (back-filled from HANDOFF.md)

Brief: build a new Uno Platform reference app from idea to app, non-stop until completion.

Came back: `/product-thinking` Quick mode on two candidates; Vigil (veterinary anesthesia record) earned
Test first, event-rental turnaround earned Reframe/Park (`docs/VALIDATION.md`, `docs/DECISIONS.md` D1).
Numbers: 2 candidates, 1 picked.
Threads: 0 (single session).

## 2026-10-06 15:55 — Validation written (back-filled)

Came back: `docs/VALIDATION.md`. Riskiest assumption: a full vitals set recorded in < 15 s with gloves on.
Numbers: ~25 min from session start.

## 2026-10-06 16:13 — Scaffold builds (back-filled)

Came back: `dotnet new unoapp` from stable Uno.Templates 6.7.30 in an isolated hive (global templates
are 7.0-dev). MVUX, Material, regions, localization (fr).
Numbers: `net10.0-desktop` build 27 s, 0 warnings. ~43 min from session start.
Surprises: global templates were 7.0-dev; isolated hive needed to get a stable `global.json`.

## 2026-10-06 16:18 — Spec written (back-filled)

Came back: `SPEC.md` with Architecture, Design, Interaction and Spec Graph briefs, capability inventory
(6 substituted/omitted rows), spec gate 21/22 PASS (Type assets PENDING), Quality Gates.
Theme generated with DesignMd2Uno: 24/24 contrast pairs pass.
Numbers: ~48 min from session start to spec.

## 2026-10-07 10:06 — Phase 2 resumed in a rooted session

Came back: `uno_health` Healthy, `toolCount` 12, DevServer 6.7.135, Uno.Sdk 6.7.30, `Vigil.sln` auto-selected.
Numbers: discovery 584 ms.

## 2026-10-07 10:52 — Step 1: foundation + Board, first runnable (desktop)

Came back: generated theme wired (ColorPaletteOverride, Typography, Tokens), fonts downloaded as static TTFs
with `.ttf.manifest` weight maps (R1 closed on desktop), VitalsPalette (ThemeDictionaries), MotionTokens,
Uno0001 as error, `UseStudio()` gated, Domain (records, vitals ranges, schedule, dosing, CSV, sample day),
`JsonCaseStore` + formulary + clock + fault injection, Board page with Live band and case list.
Numbers: 33/33 unit tests pass (1 s). Desktop build 0 warnings (45 s cold, 11 s incremental). Lint gating counts 0.
~46 min from session resume to a verified Board. App ~1,750 LOC (C# + XAML).
Runtime (App MCP, desktop): Value, Loading (skeleton, `--vigil-slow-ms=60000`), Empty (`--vigil-seed=empty`),
Error (`--vigil-fault=store-read-once`) captured; Retry invoked by peer and recovered list + live band together;
Overdue due-state renders icon + Error ink + red sight-glass bar.
Surprises:
- `utu:VisualStateManagerExtensions.States` on the Button, then on a Grid child: no state applied (2 attempts).
  The Toolkit doc says the opposite of the `uno-toolkit` skill reference: States goes on the Control
  (here a UserControl), groups on its first child. Third attempt worked.
- `FeedView` Retry via `{Binding Refresh, ElementName=...}` over `Feed.AsyncEnumerable` → `Combine` → `AsListFeed`
  greyed out and never recovered (matches the recorded "Refresh does not recover an upstream failure" gotcha).
  Fixed with `Feed.Async(load, Store.Changed)` + a model `Retry` command raising the store signal.
- `uno_app_start` left the previous instance running → MSB3027 lock; every relaunch now sweeps `Get-Process Vigil`.
- Seeding fills the store cache, so the read fault must be driven on a run without `--vigil-seed`.
Quote: "The only 64 px number on the board is how long the patient has been asleep."
Threads: 0.

Correction (10:40): the step 1 entry above is stamped 10:52; the milestone was actually logged at 10:39.

## 2026-10-07 11:05 — Step 2 release path (CI) + Step 3 NewCase → Record shell, Pre-op, Induce

Came back: private repo `mtmattei/Vigil`, `main` pushed; CI adapted from Patina (VigilTargetFrameworks
override). NewCase sheet (`!NewCase` for a `CaseRef` result), Record page with inline Visibility tabs,
Pre-op checklist + ASA + Induce, model-owned tab state.
Numbers: CI run 37638904753: tests, desktop win/linux/osx, WASM, Android green; iOS pending at log time.
Local Release publish: desktop win-x64 exit 0 in 25 s; WASM exit 0 in 518 s. 33/33 tests, lint 0.
Runtime (App MCP, desktop): validation shows exactly the one missing field; Create saves and lists Nova;
5 checklist toggles + ASA II round-trip through `Parent.Preop` TwoWay → store → `Case` feed ("5 of 5 checks");
Induce → Anesthetized, elapsed 00:00, next 4:59; Drugs tab holds across ≥4 clock ticks; Back → Board; bands
420 (1 col) / 700 / 1200 (2 col) after detail + Back.
Surprises:
- `OneTime` SelectedIndex inside a FeedView ValueTemplate re-applies on every 1 Hz `Data` swap: a user's tab
  choice would reset each second. Moved to a model `IState<int> Tab`.
- `{utu:Responsive}` cannot attach to `UniformGridLayout` (log: "Neither DP owner ... is a FrameworkElement");
  replaced with `MaximumRowsOrColumns=2` + `MinItemWidth=360`.
- `uno_app_get_screenshot` returns a blank frame on the Record page and omits flyout popups; PrintWindow
  (`tools/Capture-Window.ps1`) shows both rendering correctly.
- `TabBarItem` has no automation peer (`peer_action select` fails): tabs need pointer clicks in automation,
  and a screen reader gets no tab semantics. Logged for break-it/a11y.
- Hot Design opened three times during the run with no matching tool action (once with the window maximized);
  a controlled Back did not reproduce it.
Threads: 0.

## 2026-10-07 11:14 — Step 4: Monitor (the five-minute strip + entry pad)

Came back: `VitalsStrip` (`SKCanvasElement`, palette snapshot on `ActualThemeChanged`, Plex Mono labels loaded
once, 200 ms new-column fade on a one-shot timer, no frame loop), `VitalStepper` pad (56/64 px buttons,
RepeatButton long-press, accessible names), `VitalsDraft` pre-filled from the last reading, range flags as text,
Table toggle bounded to the strip footprint, due slot at the next reading's column.
Numbers (riskiest assumption, measured by peer actions on desktop):
- Unchanged patient: **1 tap** (Record). HR +4: **3 taps** (+, +, Record), 0.62 s of automation time.
  Human seconds per reading are not measured: no vet tech has used it (VALIDATION.md next test).
- Vitals series contrast (R3 closed): min 4.68:1 (HR on #F3F5F2), all ≥ 4.68 light / ≥ 6.86 dark.
- Tests 47/47 (+14: draft steps/validation/flags, strip placement, summary). Lint 0.
- Local Release Android APK: exit 0 in 246 s, 54.9 MB signed APK.
Runtime: HR 45 / SYS 85 → "▼ Low" text flags, red alarm marker, MAP calc 70; Table view rows carry
"HR ▼ low · SYS ▼ low"; persistence across app restart (15th reading present after relaunch); 420 px band.
Surprises:
- Plex Mono has no ●∨∧○■: the strip legend drew tofu. Legend now draws the same shapes the strip uses.
- The due slot first sat on *now*'s column and overlapped the reading just taken; it now sits where the next
  reading lands.
- Material's default `ToggleButton` is an icon toggle that hides its text when checked ("Table" vanished):
  replaced with a Toolkit `FilterChipStyle` chip.
- Two-column pad at 420 px squeezed the value box to ~20 px ("45" showed "4"): buttons 56 px below Normal.
Threads: 0.
