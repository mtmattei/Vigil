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
