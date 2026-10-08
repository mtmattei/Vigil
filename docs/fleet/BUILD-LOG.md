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

## 2026-10-07 11:16 — Step 5: Drugs tab + Dose sheet

Came back: `!Dose` sheet (formulary list with MVUX `Selection`, mg/kg pre-filled with the drug default, route
pre-set, computed volume at 64 px with formula and reference range), Drugs tab list + Give a dose, Pre-op
"Give premedication". Inline out-of-range confirmation (DECISIONS D4).
Numbers: Atropine 0.02 mg/kg for 18.4 kg → 0.68 mL (18.4 × 0.02 = 0.368 mg ÷ 0.54 mg/mL); 0.2 mg/kg → 6.81 mL,
flagged, confirmed, listed "Out of range, confirmed". Tests 47/47.
Surprises:
- MVUX bindable generator emitted invalid C# for a record member named `Class` (lowercased to the keyword
  `class`): ~45 CS errors. Renamed to `DrugClass`.
- The recorded `record struct` generator gotcha hit `VitalRange` once it rode inside a feed (CS0019/CS0023).
- The message dialog dismissed the flyout it was opened from (D4).
- The formula line read "… ÷ 0.54 mg/mL = 0.37 mg" (arithmetic order wrong as written); now
  "18.4 kg × 0.02 mg/kg = 0.368 mg ÷ 0.54 mg/mL".
Threads: 0.

## 2026-10-07 11:24 — Step 6: Recovery, Sign-off, read-only record, Export

Came back: Recovery tab (End anesthesia, Extubated now, Sternal now, pain 0–4, temp, notes, auto-saved),
recovery anchor at 64 px, `!SignOff` sheet (duration + readings at display scale, typed name + confirmation),
signed record read-only, Export CSV through `FileSavePicker`, Copy summary to the clipboard.
Numbers: Bella signed after 01:26 of anesthesia, 16 readings, 5 doses (1 out of range, confirmed), 1 alarm
reading. Export: 22-line CSV (header + 16 readings + 5 doses, time-ordered) through the native save dialog
(driven by `tools/Complete-SaveDialog.ps1`); notice "Saved Bella-2026-10-07-anesthesia.csv.". Copy summary
put the 12-line plain-text record on the clipboard (user clipboard backed up and restored around the test).
Tests 50/50. Lint 0.
Surprises:
- An injected `IDispatcher` in a singleton service resolved null (window-scoped): Copy summary threw
  NullReferenceException and the MVUX command swallowed it silently (no log line, no notice). Fixed with the
  window `DispatcherQueue` captured at launch (`Services/UiThread.cs`) and every hand-off now reports failures.
- UIA `ValuePattern.SetValue` on the Win32 save dialog's file-name box was refused (0x800704C7); IDOK with the
  suggested name worked. The test export landed in the user's OneDrive Documents and was moved to `artifacts/`.
- Overdue showed as "next 6:22" in the record header and Record button (absolute value of a negative remaining
  time). Now "overdue 6:22".
- `--vigil-seed=sample` clears the store by design; a case created during testing (Nova) went with it.
Threads: 0.

## 2026-10-07 11:26 — Release path gate met (step 2 closed)

CI run 37638904753 (commit 393696e): unit tests, desktop win-x64 / linux-x64 / osx-arm64, WebAssembly, Android,
iOS simulator build: all success. iOS job 14:43:49Z → 15:17:00Z (33 min) with Xcode 26.3 + workload 10.0.300.
Local Release publishes: desktop win-x64 25 s, WASM 518 s, Android APK 246 s (54.9 MB). GitHub returned HTTP 500
on every push for ~15 min (3 commits held locally), then accepted them.

## 2026-10-07 11:36 — Step 7: Settings + EN/FR + Theatre theme

Came back: Settings (theme System/Paper/Theatre via `IThemeService`, language via `ILocalizationService`
with a restart note, reduce motion, reading interval 3/5/10, load sample day, clear all with Cancel as default),
persisted `preferences.json`, full EN/FR: `tools/localize.py` gives every static XAML string an `x:Uid` and
collects `Loc.T/F` keys from C#; `tools/fr.json` holds the French; the run fails on any missing translation.
Numbers: 316 keys per language (229 XAML, 87 code). French verified at runtime after restart on Board, Record
(Monitor: FC/FR/PAM, "en retard 8:55", French decimals "18,4 kg") and Settings incl. accessible names. Interval
3 min → board overdue moved from 7:20 to 12:50 for the same last reading. Tests 50/50. Lint 0.
Surprises:
- Theatre theme: the Skia strip drew Paper colours. `ResourceDictionary.TryGetValue` on the app dictionary
  answers with the application-level theme; the resolver now reads the element-theme dictionary first and
  walks merged dictionaries last-to-first (later ones win).
- Material `ToggleSwitch` drops its `Header`: an unlabeled switch. Label row added.
- The language notice landed in the Records card (shared state); now its own line.
- The system message dialog keeps Fluent chrome (blue accent) inside the Material app: polish finding.
Threads: 0.

## 2026-10-07 11:52 — Step 8: persistence + head passes (Android emulator, WebAssembly)

Android (emulator `chefs36`, API 36, 1080×2400, Release APK 219 s, 55.7 MB): launch, fonts (R1 closed on Android),
Settings, Load sample day through the native dialog, Board with overdue band, Monitor strip on `SKCanvasElement`
(R2 closed on Android), Record reading at 11:39 → `am force-stop` → relaunch → reading present, slot back to
"next 4:42" (persistence proven). Evidence: `artifacts/shots/android-*.png`.
WebAssembly (Debug, Edge, App MCP attached): Board, Settings, New case → Create (Wasabi); a second tab (fresh
page load) listed Wasabi from browser storage (R5 closed). App MCP screenshots time out on WASM and Edge window
captures are stale (background throttling), so the strip on WASM has no pixel evidence (R2 on WASM: not captured).
Desktop: restart persistence proven earlier (15th reading after relaunch); New case → Record hand-off traced
(`Some(CaseRef)`, navigation success=True).
Findings: F-019..F-025 (phone clipping ×3 fixed, Android card washout fixed with the recorded workaround,
WASM route-in-URL leak, WASM Create does not open the record, Android clipboard toast).
Tests 50/50. Lint gating 0 (1 TEMPLATE + 1 WORKAROUND, both annotated).

## 2026-10-07 12:00 — WASM navigation fixed (D5), route diff, CI budget block

D5 (`AddressBarUpdateEnabled = false`): on WASM, New case → Create (Pistache) now opens the record and Back
returns to the Board; no route or dialog text in the URL. F-023/F-024 verified.
Atlas: `atlas extract` → 6 nodes, 4 XAML edges; built tree = spec tree, 3 code/UserControl edges explained in
`docs/atlas/ROUTE-DIFF.md`.
Blocked: CI runs 37643330218, 37644934790, 37644966575, 37647072013 never started a job: "The job was not
started because an Actions budget is preventing further use." Last executed run (23ab49a) green on all heads
it ran; run 37638904753 green on every head incl. iOS. Needs a user decision (budget, public repo, or accept).
Surprise: a stale WASM dev server (PID 41684, 11:41) kept port 5000, so a relaunch exited silently and tabs
loaded old code; `uno_app_start` did not stop it.

## 2026-10-07 12:05 — Break-it round 1 + uno-audit hygiene

Attacks (desktop unless noted): A1 double navigation → 1 Record page; A13 HR "+" ×10 → 94→114 (0 dropped),
Record ×2 back to back → 1 reading written (14→15); A10 Board↔Record ×7 with `--vigil-memlog` (full GC +
finalizers every 10 s) → managed 68.6→70.0 MB, RecordModel live 2 (plateau; the earlier 41→151 MB climb was
finalizer backlog); A6 strip drug row overprint (F-026, fixed by grouping); A4/A8 heads (F-019..F-022 fixed).
uno-audit: Uno.Sdk 6.7.30 = latest stable on NuGet, .NET 10.0.303; removed dead `ICaseStore.WatchAll`
(app reloads from the store Signal since D3); no TODO/NotImplemented/Console writes outside the DEBUG memlog;
strip renderer disposes native paints on Unloaded. Tests 50/50, lint gating 0.

## 2026-10-07 12:10 — Completeness audit + deploy matrix (Phase 3 reports)

`docs/COMPLETENESS.md`: verdict Complete with gaps (0 Blockers, 9 Majors, 12 Minors). 6/6 screens, 7/7 route
edges, 43 triggers, 26 commands, 0 simulated. Desktop R on all 7 critical workflows; Android R on 3; WASM R on 3;
iOS build-only. `docs/DEPLOY.md`: Release builds for every head (local desktop/WASM/Android, CI all six), nothing
distributed, CI not executing since 20f459e (Actions budget). Band due bar given a visible 6 px track
(gold-standard Fit & Finish). Loop stops here on a user decision (CI budget).
Totals: ~4,830 LOC app (C# + XAML), 50 unit tests, 27 findings (24 fixed/verified, 6 open), 5 decisions.

## 2026-10-08 11:00 — CI green on every head (repo public)

User made `mtmattei/Vigil` public. Run 37790804563 (3a41f89, 14:15Z): unit tests, desktop ×3, WASM, Android
green; iOS failed `CS0037` at `Controls/ThemePalette.cs:59-60` (the iOS compiler typed the palette switch
expression as `Color`, so the `null` arm did not convert; desktop accepted it). First iOS build since the
ThemePalette rewrite, which landed during the budget block. Fix 9244b45: explicit `Color?` arms. Local desktop
0 errors, tests 50/50. Run 37791335857 (9244b45): success on all 7 jobs, iOS 14:19:19Z → 14:59:14Z (40 min);
Pages job skipped (opt-in). COMPLETENESS Majors 9 → 8; DEPLOY CI table updated. Loop stop condition met.
