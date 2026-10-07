# Findings

Written by the breaker (`uno-break-it`). Single-session build: the session is breaker, coordinator and fixer;
`verified` is set only after a re-run. Owner = page or file path, `shared` for theme/tokens/navigation shell.
Evidence in `artifacts/shots/` (screens) and `docs/fleet/findings/` (kept captures).

Severity: **P0** crash / data loss · **P1** flow blocked · **P2** wrong or visibly broken · **P3** polish.
Status: `new` → `assigned` → `fixed` → `verified` (or `wontfix` with a DECISIONS.md reference).

## Found during the build (round 0, before break-it)

| ID | Sev | Module | Attack | Repro | Evidence | Owner | Status | Found by |
|---|---|---|---|---|---|---|---|---|
| F-001 | P2 | Controls/LiveBandView | A5 | Overdue state never styled (VSM States on Button / Grid) | build log 10:39 | Controls/LiveBandView.xaml | verified | build |
| F-002 | P1 | Board | A9 | Retry over AsyncEnumerable → Combine → AsListFeed greys out, never recovers | build log 10:39 | Presentation/Board | verified | build |
| F-003 | P2 | Board | A4 | `{utu:Responsive}` on `UniformGridLayout` never applies (log: not a FrameworkElement) | app log | Presentation/Board | verified | build |
| F-004 | P2 | Record | A2 | `OneTime` SelectedIndex in a 1 Hz ValueTemplate resets the user's tab every tick | tree after Induce | Presentation/Record | verified | build |
| F-005 | P2 | Monitor strip | A11 | Legend glyphs ●∨∧○■ render as tofu in Plex Mono | monitor-1280.png | Controls/StripRenderer.cs | verified | build |
| F-006 | P2 | Monitor | A4 | 420 px: two-column pad clips "45" to "4" | monitor-420b.png | Controls/VitalStepper.xaml | verified | build |
| F-007 | P2 | Monitor | A6 | Table ToggleButton text vanishes when checked (Material icon toggle) | tree ^3k | Presentation/Record | verified | build |
| F-008 | P1 | Dose sheet | A8 | Message dialog opened from the `!Dose` flyout dismisses the flyout; entry lost (D4) | dose-4.png | Presentation/Dose | verified | build |
| F-009 | P2 | Dose sheet | A6 | Formula line "… ÷ 0.54 mg/mL = 0.37 mg" states wrong arithmetic order | dose-2.png | Domain/Dosing.cs | verified | build |
| F-010 | P1 | Recovery | A9 | Copy summary: NullReferenceException swallowed by the MVUX command (window-scoped IDispatcher in a singleton) | notice text | Services/RecordExporter.cs | verified | build |
| F-011 | P2 | Record | A6 | Overdue shown as "next 6:22" (absolute value) in header and Record button | tree ^35 | Presentation/Record | verified | build |
| F-012 | P1 | Monitor strip | A5 | Theatre theme: strip drew Paper colours (resolver used app-level theme) | monitor-dark.png | Controls/ThemePalette.cs | verified | build |
| F-013 | P3 | Settings | A6 | Material ToggleSwitch drops Header: unlabeled Reduce motion switch | settings-dark.png | Presentation/Settings | verified | build |
| F-014 | P3 | Record | A15 | `TabBarItem` exposes no automation peer: tabs not operable by peer, no tab semantics for screen readers | MCP error | Presentation/Record | new | build |
| F-015 | P3 | shared | A6 | System message dialogs (Clear all, Load sample) render Fluent chrome (blue accent) inside the Material app | clearall.png | Presentation/Settings | new | build |
| F-016 | P3 | NewCase / Dose / SignOff sheets | A4 | `!` sheets open as full-window surfaces, not the spec's right drawer (`DrawerFlyoutPresenter`) | newcase.png | shared | new | build |
| F-017 | P3 | Board | A6 | With two cases under anesthesia, the live band shows only the newest | tree ^1u | Presentation/Board | new | build |
| F-018 | P3 | Settings | A6 | Language notice rendered in the Records card | tree ^6l | Presentation/Settings | verified | build |
| F-019 | P3 | Board (Android) | A4 | Phone: empty-band help text clipped ("…with i"), no wrap | android-1.png | Presentation/Board | fixed | head pass |
| F-020 | P3 | Board (Android) | A4 | Phone: "All" chip clipped by the New case button | android-1.png | Presentation/Board | fixed | head pass |
| F-021 | P3 | Settings (Android) | A4 | Phone: theme radios wrap column-first (System, Theatre / Paper) | android-settings.png | Presentation/Settings | fixed | head pass |
| F-022 | P2 | shared (Android) | A8 | Card keeps its pressed overlay after a swipe that starts on it (recorded Toolkit gotcha) | android-settings2.png | Themes/Controls.xaml | fixed | head pass |
| F-023 | P3 | shared (WASM) | A3 | Message-dialog route and its text are pushed into the address bar (`/Settings/xxxxmessagedialogxxxx?…title=Load sample day…`); later the URL reads `/Board` while Settings shows | wasm-3.png, wasm-4.png | shared | new | head pass |
| F-024 | P2 | NewCase (WASM) | A1 | WASM: Create saves the case but the record does not open; the first tab's URL stays `/Board/NewCase`. Desktop: trace shows `Some(CaseRef)` + Record navigation success=True | wasm-3.png tab title | Presentation/Board | new | head pass |
| F-025 | P3 | Record (Android) | A6 | Framework TextBox clipboard query shows "Vigil pasted from your clipboard" toast on opening Monitor (recorded gotcha) | android-monitor.png | Controls/VitalStepper | new | head pass |

## Regression lines added by fixers

| Line | Attack | Check | Added for |
|---|---|---|---|
| A5.1 | A5 | Drive `--vigil-fault`/time to Overdue; NextText Foreground = ErrorBrush, DueIcon visible | F-001 |
| A9.1 | A9 | `--vigil-fault=store-read-once` (no seed): Retry recovers list and live band together | F-002 |
| A2.1 | A2 | Select a tab under anesthesia, wait 4 s: still selected | F-004 |
| A8.1 | A8 | Any confirmation inside a `!` sheet is inline, never a dialog | F-008 |
| A5.2 | A5 | Theatre theme: strip HR dots are #6BD37E, hairlines #2C3532 | F-012 |

## Round summaries

| Round | Date | P0 | P1 | P2 | P3 | Verified | Notes |
|---|---|---|---|---|---|---|---|
| 0 | 2026-10-07 | 0 | 4 | 9 | 5 | 14 | found and fixed during the build; F-014..F-017 open |
